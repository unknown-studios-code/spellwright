using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.Physics.Authoring
{
    internal class PhysicsShapeBaker : BaseColliderBaker<PhysicsShapeAuthoring>
    {
        public static List<PhysicsShapeAuthoring> physicsShapeComponents = new();
        public static List<UnityEngine.Collider> colliderComponents = new();

        private bool ShouldConvertShape(PhysicsShapeAuthoring authoring)
        {
            return authoring.enabled;
        }

        private GameObject GetPrimaryBody(GameObject shape, out bool hasBodyComponent, out bool isStaticBody)
        {
            GameObject pb = FindFirstEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_PhysicsBodiesBuffer);
            GameObject rb = FindFirstEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_RigidbodiesBuffer);
            hasBodyComponent = pb != null || rb != null;
            isStaticBody = false;

            if (pb != null)
            {
                return rb == null ? pb.gameObject
                    : pb.transform.IsChildOf(rb.transform) ? pb.gameObject
                    : rb.gameObject;
            }

            if (rb != null)
            {
                return rb.gameObject;
            }

            // for implicit static shape, first see if it is part of static optimized hierarchy
            isStaticBody = FindTopmostStaticEnabledAncestor(shape, out GameObject topStatic);
            if (topStatic != null)
            {
                return topStatic;
            }

            // otherwise, find topmost enabled Collider or PhysicsShapeAuthoring
            GameObject topCollider = FindTopmostEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_CollidersBuffer);
            GameObject topShape = FindTopmostEnabledAncestor(shape, PhysicsShapeExtensions_NonBursted.s_ShapesBuffer);

            return topCollider == null
                ? topShape == null
                    ? shape.gameObject
                    : topShape
                : topShape == null
                    ? topCollider
                    : topShape.transform.IsChildOf(topCollider.transform)
                        ? topCollider
                        : topShape;
        }

        private ShapeComputationDataBaking GetInputDataFromAuthoringComponent(PhysicsShapeAuthoring shape, Entity colliderEntity)
        {
            GameObject shapeGameObject = shape.gameObject;
            GameObject body = GetPrimaryBody(shapeGameObject, out bool hasBodyComponent, out bool isStaticBody);
            GameObject child = shapeGameObject;
            int shapeInstanceID = shape.GetInstanceID();

            Entity bodyEntity = GetEntity(body, TransformUsageFlags.Dynamic);

            // prepare the static root
            if (isStaticBody)
            {
                Entity staticRootMarker = CreateAdditionalEntity(TransformUsageFlags.Dynamic, true, "StaticRootBakeMarker");
                AddComponent(staticRootMarker, new BakeStaticRoot() { Body = bodyEntity, ConvertedBodyInstanceID = body.transform.GetInstanceID() });
            }

            // Track dependencies to the transforms
            Transform shapeTransform = GetComponent<Transform>(shape);
            Transform bodyTransform = GetComponent<Transform>(body);
            var instance = new ColliderInstanceBaking
            {
                AuthoringComponentId = shapeInstanceID,
                BodyEntity = bodyEntity,
                ShapeEntity = GetEntity(shapeGameObject, TransformUsageFlags.Dynamic),
                ChildEntity = GetEntity(child, TransformUsageFlags.Dynamic),
                BodyFromShape = ColliderInstanceBaking.GetCompoundFromChild(shapeTransform, bodyTransform),
            };

            ForceUniqueColliderAuthoring forceUniqueComponent = body.GetComponent<ForceUniqueColliderAuthoring>();
            bool isForceUniqueComponentPresent = forceUniqueComponent != null;

            ShapeComputationDataBaking data = GenerateComputationData(shape, bodyTransform, instance, colliderEntity, isForceUniqueComponentPresent);

            data.Instance.ConvertedAuthoringInstanceID = shapeInstanceID;
            data.Instance.ConvertedBodyInstanceID = bodyTransform.GetInstanceID();

            GameObject rb = FindFirstEnabledAncestor(shapeGameObject, PhysicsShapeExtensions_NonBursted.s_RigidbodiesBuffer);
            GameObject pb = FindFirstEnabledAncestor(shapeGameObject, PhysicsShapeExtensions_NonBursted.s_PhysicsBodiesBuffer);
            // The Rigidbody cannot know about the Physics Shape Component. We need to take responsibility of baking the collider.
            if (rb || (!rb && !pb && body == shapeGameObject))
            {
                GetComponents(physicsShapeComponents);
                GetComponents(colliderComponents);
                // We need to check that there are no other colliders in the same object, if so, only the first one should do this, otherwise there will be 2 bakers adding this to the entity
                // This will be needed to trigger BuildCompoundColliderBakingSystem
                // If they are legacy Colliders and PhysicsShapeAuthoring in the same object, the PhysicsShapeAuthoring will add this
                if (colliderComponents.Count == 0 && physicsShapeComponents.Count > 0 && physicsShapeComponents[0].GetInstanceID() == shapeInstanceID)
                {
                    Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                    // // Rigid Body bakes always add the PhysicsWorldIndex component and process transform
                    if (!hasBodyComponent)
                    {
                        AddSharedComponent(entity, new PhysicsWorldIndex());
                        PostProcessTransform(bodyTransform);
                    }

                    AddComponent(
                        entity,
                        new PhysicsCompoundData()
                        {
                            AssociateBlobToBody = false,
                            ConvertedBodyInstanceID = shapeInstanceID,
                            Hash = default,
                        }
                    );
                    AddComponent<PhysicsRootBaked>(entity);
                    AddComponent<PhysicsCollider>(entity);
                }
            }

            return data;
        }

        private Material ProduceMaterial(PhysicsShapeAuthoring shape)
        {
            PhysicsMaterialTemplate materialTemplate = shape.MaterialTemplate;
            if (materialTemplate != null)
            {
                DependsOn(materialTemplate);
            }

            return shape.GetMaterial();
        }

        private CollisionFilter ProduceCollisionFilter(PhysicsShapeAuthoring shape)
        {
            return shape.GetFilter();
        }

        private bool GetMeshes(PhysicsShapeAuthoring shape, out List<UnityEngine.Mesh> meshes, out List<float4x4> childrenToShape)
        {
            meshes = new List<UnityEngine.Mesh>();
            childrenToShape = new List<float4x4>();

            if (shape.CustomMesh != null)
            {
                meshes.Add(shape.CustomMesh);
                childrenToShape.Add(float4x4.identity);
            }
            else
            {
                // Try to get all the meshes in the children
                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        PhysicsShapeAuthoring shapeAuthoring = GetComponent<PhysicsShapeAuthoring>(meshFilter);
                        if (shapeAuthoring != null && shapeAuthoring != shape)
                        {
                            // Skip this case, since it will be treated independently
                            continue;
                        }

                        meshes.Add(meshFilter.sharedMesh);

                        // Don't calculate the children to shape if not needed, to avoid approximation that could prevent collider to be shared
                        if (shape.transform.localToWorldMatrix.Equals(meshFilter.transform.localToWorldMatrix))
                        {
                            childrenToShape.Add(float4x4.identity);
                        }
                        else
                        {
                            float4x4 transform = math.mul(shape.transform.worldToLocalMatrix, meshFilter.transform.localToWorldMatrix);
                            childrenToShape.Add(transform);
                        }

                        DependsOn(meshes.Last());
                    }
                }
            }

            return meshes.Count > 0;
        }

        private UnityEngine.Mesh CombineMeshes(PhysicsShapeAuthoring shape, List<UnityEngine.Mesh> meshes, List<float4x4> childrenToShape)
        {
            var instances = new List<CombineInstance>();
            int numVertices = 0;
            for (int i = 0; i < meshes.Count; ++i)
            {
                UnityEngine.Mesh currentMesh = meshes[i];
                float4x4 currentChildToShape = childrenToShape[i];
                if (!currentMesh.IsValidForConversion(shape.gameObject))
                {
                    throw new InvalidOperationException($"Mesh '{currentMesh}' assigned on {shape.name} is not readable. Ensure that you have enabled Read/Write on its import settings.");
                }

                // Combine submeshes manually
                numVertices += meshes[i].vertexCount;
                var combinedSubmeshes = new UnityEngine.Mesh();
                combinedSubmeshes.vertices = currentMesh.vertices;

                var combinedIndices = new List<int>();
                for (int indexSubMesh = 0; indexSubMesh < meshes[i].subMeshCount; ++indexSubMesh)
                {
                    combinedIndices.AddRange(currentMesh.GetIndices(indexSubMesh));
                }

                combinedSubmeshes.SetIndices(combinedIndices, MeshTopology.Triangles, 0);
                combinedSubmeshes.RecalculateNormals();
                var instance = new CombineInstance { mesh = combinedSubmeshes, transform = currentChildToShape };
                instances.Add(instance);
            }

            var mesh = new UnityEngine.Mesh();
            mesh.indexFormat = numVertices > UInt16.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.CombineMeshes(instances.ToArray());
            mesh.RecalculateBounds();

            return mesh;
        }

        private ShapeComputationDataBaking GenerateComputationData(
            PhysicsShapeAuthoring shape,
            Transform bodyTransform,
            ColliderInstanceBaking colliderInstance,
            Entity colliderEntity,
            bool isForceUniqueComponentPresent
        )
        {
            bool isUnique = isForceUniqueComponentPresent || shape.ForceUnique;
            var res = new ShapeComputationDataBaking
            {
                Instance = colliderInstance,
                Material = ProduceMaterial(shape),
                CollisionFilter = ProduceCollisionFilter(shape),
                ForceUniqueIdentifier = isUnique ? (uint)shape.GetInstanceID() : 0u,
            };

            Transform shapeTransform = shape.transform;
            var localToWorld = (float4x4)shapeTransform.localToWorldMatrix;
            var bodyLocalToWorld = (float4x4)bodyTransform.transform.localToWorldMatrix;

            // We don't bake pure uniform scales into colliders since edit-time uniform scales
            // are baked into the entity's LocalTransform.Scale property, unless the shape has non-identity scale
            // relative to its contained body. In this case we need to bake all scales into the collider geometry.
            float4x4 relativeTransform = math.mul(math.inverse(bodyLocalToWorld), localToWorld);

            bool hasNonIdentityScaleRelativeToBody = relativeTransform.HasNonIdentityScale();
            bool hasShearRelativeToBody = relativeTransform.HasShear();
            bool bakeUniformScale = hasNonIdentityScaleRelativeToBody || hasShearRelativeToBody;

            // If the body transform has purely uniform scale, and there is any scale or shear between the body and the shape,
            // then we need to extract the uniform body scale from the shape transform before baking
            // to prevent the shape from being scaled by the body's uniform scale twice. This is because pure top level body uniform scales
            // are not baked into collider geometry but represented by the body entity's LocalTransform.Scale property.
            if (bakeUniformScale)
            {
                bool bodyHasShear = bodyLocalToWorld.HasShear();
                bool bodyHasNonUniformScale = bodyLocalToWorld.HasNonUniformScale();
                if (!bodyHasShear && !bodyHasNonUniformScale)
                {
                    // extract uniform scale of body and remove it from the shape transform
                    float3 bodyScale = bodyLocalToWorld.DecomposeScale();
                    float3 bodyScaleInverse = 1 / bodyScale;
                    localToWorld = math.mul(localToWorld, float4x4.Scale(bodyScaleInverse));
                }
            }

            // bake uniform scale only if required (see above), and always bake shear and non-uniform scales into the collider geometry
            float4x4 colliderBakeMatrix = float4x4.identity;
            if (bakeUniformScale || localToWorld.HasShear() || localToWorld.HasNonUniformScale())
            {
                RigidTransform rigidBodyTransform = Math.DecomposeRigidBodyTransform(localToWorld);
                colliderBakeMatrix = math.mul(math.inverse(new float4x4(rigidBodyTransform)), localToWorld);
                // make sure we have a valid transformation matrix
                colliderBakeMatrix.c0[3] = 0;
                colliderBakeMatrix.c1[3] = 0;
                colliderBakeMatrix.c2[3] = 0;
                colliderBakeMatrix.c3[3] = 1;
            }

            float4x4 shapeToWorld = shape.GetShapeToWorldMatrix();
            EulerAngles orientation;

            res.ShapeType = shape.ShapeType;
            switch (shape.ShapeType)
            {
                case ShapeType.Box:
                {
                    res.BoxProperties = shape.GetBoxProperties(out orientation).BakeToBodySpace(localToWorld, shapeToWorld, orientation, bakeUniformScale);
                    break;
                }
                case ShapeType.Capsule:
                {
                    res.CapsuleProperties = shape.GetCapsuleProperties().BakeToBodySpace(localToWorld, shapeToWorld, bakeUniformScale).ToRuntime();
                    break;
                }
                case ShapeType.Sphere:
                {
                    res.SphereProperties = shape.GetSphereProperties(out orientation).BakeToBodySpace(localToWorld, shapeToWorld, ref orientation, bakeUniformScale);
                    break;
                }
                case ShapeType.Cylinder:
                {
                    res.CylinderProperties = shape.GetCylinderProperties(out orientation).BakeToBodySpace(localToWorld, shapeToWorld, orientation, bakeUniformScale);
                    break;
                }
                case ShapeType.Plane:
                {
                    shape.GetPlaneProperties(out float3 center, out float2 size, out orientation);
                    PhysicsShapeExtensions.BakeToBodySpace(
                        center,
                        size,
                        orientation,
                        colliderBakeMatrix,
                        out res.PlaneVertices.c0,
                        out res.PlaneVertices.c1,
                        out res.PlaneVertices.c2,
                        out res.PlaneVertices.c3
                    );
                    break;
                }
                case ShapeType.ConvexHull:
                {
                    res.ConvexHullProperties.Filter = res.CollisionFilter;
                    res.ConvexHullProperties.Material = res.Material;
                    res.ConvexHullProperties.GenerationParameters = shape.ConvexHullGenerationParameters.ToRunTime();

                    CreateMeshAuthoringData(shape, colliderBakeMatrix, colliderEntity);
                    break;
                }
                case ShapeType.Mesh:
                {
                    res.MeshProperties.Filter = res.CollisionFilter;
                    res.MeshProperties.Material = res.Material;

                    CreateMeshAuthoringData(shape, colliderBakeMatrix, colliderEntity);
                    break;
                }
            }

            return res;
        }

        private void CreateMeshAuthoringData(PhysicsShapeAuthoring shape, float4x4 colliderBakeMatrix, Entity colliderEntity)
        {
            if (GetMeshes(shape, out List<UnityEngine.Mesh> meshes, out List<float4x4> childrenToShape))
            {
                // Combine all detected meshes into a single one
                UnityEngine.Mesh mesh = CombineMeshes(shape, meshes, childrenToShape);
                if (!mesh.IsValidForConversion(shape.gameObject))
                {
                    throw new InvalidOperationException($"Mesh '{mesh}' assigned on {shape.name} is not readable. Ensure that you have enabled Read/Write on its import settings.");
                }

                var meshBakingData = new PhysicsMeshAuthoringData()
                {
                    Convex = shape.ShapeType == ShapeType.ConvexHull,
                    Mesh = mesh,
                    BakeFromShape = colliderBakeMatrix,
                    MeshBounds = mesh.bounds,
                    ChildToShape = float4x4.identity,
                };
                AddComponent(colliderEntity, meshBakingData);
            }
            else
            {
                throw new InvalidOperationException($"No {nameof(PhysicsShapeAuthoring.CustomMesh)} or {nameof(MeshFilter.sharedMesh)} assigned on {shape.name}.");
            }
        }

        public override void Bake(PhysicsShapeAuthoring authoring)
        {
            var shapeBakingData = new PhysicsColliderAuthoringData();

            // First pass
            Profiler.BeginSample("Collect Inputs from Authoring Components");

            if (ShouldConvertShape(authoring))
            {
                // We can have multiple Colliders of the same type on the same game object, so instead of adding the components to the baking entity
                // we add the components to an additional entity. These new entities will be processed by the baking system
                Entity colliderEntity = CreateAdditionalEntity(TransformUsageFlags.None, true);
                shapeBakingData.ShapeComputationalData = GetInputDataFromAuthoringComponent(authoring, colliderEntity);
                AddComponent(colliderEntity, shapeBakingData);

                // The data will be filled in by the BaseShapeBakingSystem, but we add it here so it gets reverted from the entity if the collider component is deleted
                AddComponent(
                    colliderEntity,
                    new PhysicsColliderBakedData()
                    {
                        BodyEntity = shapeBakingData.ShapeComputationalData.Instance.BodyEntity,
                        BodyFromShape = shapeBakingData.ShapeComputationalData.Instance.BodyFromShape,
                        ChildEntity = shapeBakingData.ShapeComputationalData.Instance.ChildEntity,
                        // It is a leaf if the Shape Entity equals Body Entity
                        IsLeafEntityBody = shapeBakingData.ShapeComputationalData.Instance.ShapeEntity.Equals(shapeBakingData.ShapeComputationalData.Instance.BodyEntity),
                    }
                );
            }

            Profiler.EndSample();
        }
    }
}
