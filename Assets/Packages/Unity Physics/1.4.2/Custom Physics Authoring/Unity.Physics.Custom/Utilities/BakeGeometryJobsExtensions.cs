using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    public static partial class PhysicsShapeExtensions
    {
        public static class BakeBoxJobExtension
        {
            internal static float4x4 GetBakeToShape(PhysicsShapeAuthoring shape, float3 center, EulerAngles orientation)
            {
                Transform transform = shape.transform;
                var localToWorld = (float4x4)transform.localToWorldMatrix;
                float4x4 shapeToWorld = shape.GetShapeToWorldMatrix();
                quaternion tmpOri = orientation;
                return Math.GetBakeToShape(localToWorld, shapeToWorld, ref center, ref tmpOri);
            }
        }

        public static class BakeCapsuleJobExtension
        {
            internal static float4x4 GetBakeToShape(PhysicsShapeAuthoring shape, float3 center, EulerAngles orientation)
            {
                Transform transform = shape.transform;
                var localToWorld = (float4x4)transform.localToWorldMatrix;
                float4x4 shapeToWorld = shape.GetShapeToWorldMatrix();
                quaternion tmpOri = orientation;
                return Math.GetBakeToShape(localToWorld, shapeToWorld, ref center, ref tmpOri, bakeUniformScale: true, makeZAxisPrimaryBasis: true);
            }
        }

        public static void SetBakedCapsuleSize(this PhysicsShapeAuthoring shape, float height, float radius)
        {
            CapsuleGeometryAuthoring capsule = shape.GetCapsuleProperties();
            float3 center = capsule.Center;

            float4x4 bakeToShape = BakeCapsuleJobExtension.GetBakeToShape(shape, center, capsule.OrientationEuler);
            float3 scale = bakeToShape.DecomposeScale();

            float newRadius = radius / math.cmax(scale.xy);
            if (math.abs(capsule.Radius - newRadius) > kMinimumChange)
            {
                capsule.Radius = newRadius;
            }

            height /= scale.z;

            if (math.abs(math.length(capsule.Height - height)) > kMinimumChange)
            {
                capsule.Height = height;
            }

            shape.SetCapsule(capsule);
        }

        internal static CapsuleGeometryAuthoring BakeToBodySpace(this CapsuleGeometryAuthoring capsule, float4x4 localToWorld, float4x4 shapeToWorld, bool bakeUniformScale = true)
        {
            using (var geometry = new NativeArray<CapsuleGeometryAuthoring>(1, Allocator.TempJob) { [0] = capsule })
            {
                var job = new BakeCapsuleJob
                {
                    Capsule = geometry,
                    LocalToWorld = localToWorld,
                    ShapeToWorld = shapeToWorld,
                    BakeUniformScale = bakeUniformScale,
                };
                job.Run();
                return geometry[0];
            }
        }

        public static class BakeCylinderJobExtension
        {
            internal static float4x4 GetBakeToShape(PhysicsShapeAuthoring shape, float3 center, EulerAngles orientation)
            {
                Transform transform = shape.transform;
                var localToWorld = (float4x4)transform.localToWorldMatrix;
                float4x4 shapeToWorld = shape.GetShapeToWorldMatrix();
                quaternion tmpOri = orientation;
                return Math.GetBakeToShape(localToWorld, shapeToWorld, ref center, ref tmpOri, bakeUniformScale: true, makeZAxisPrimaryBasis: true);
            }
        }

        public static CylinderGeometry GetBakedCylinderProperties(this PhysicsShapeAuthoring shape)
        {
            CylinderGeometry cylinder = shape.GetCylinderProperties(out EulerAngles orientation);
            return cylinder.BakeToBodySpace(shape.transform.localToWorldMatrix, shape.GetShapeToWorldMatrix(), orientation);
        }

        public static void SetBakedSphereRadius(this PhysicsShapeAuthoring shape, float radius)
        {
            SphereGeometry sphere = shape.GetSphereProperties(out EulerAngles eulerAngles);
            quaternion orientation = eulerAngles;
            float3 center = sphere.Center;
            radius = math.abs(radius);

            float4x4 basisToWorld = Math.GetBasisToWorldMatrix(shape.transform.localToWorldMatrix, center, orientation, 1f);
            int3 basisPriority = basisToWorld.HasShear() ? Math.GetBasisAxisPriority(basisToWorld) : Math.Constants.DefaultAxisPriority;
            float4x4 bakeToShape = Math.GetPrimitiveBakeToShapeMatrix(shape.transform.localToWorldMatrix, shape.GetShapeToWorldMatrix(), ref center, ref orientation, basisPriority);

            float scale = math.cmax(bakeToShape.DecomposeScale());

            float newRadius = radius / scale;
            sphere.Radius = newRadius;
            shape.SetSphere(sphere);
        }

        public static void SetBakedPlaneSize(this PhysicsShapeAuthoring shape, float2 size)
        {
            shape.GetPlaneProperties(out float3 center, out float2 planeSize, out EulerAngles orientation);

            float2 prevSize = math.abs(planeSize);
            size = math.abs(size);

            if (math.abs(size[0] - prevSize[0]) < kMinimumChange)
            {
                size[0] = prevSize[0];
            }

            if (math.abs(size[1] - prevSize[1]) < kMinimumChange)
            {
                size[1] = prevSize[1];
            }

            planeSize = size;

            shape.SetPlane(center, planeSize, orientation);
        }
    }
}
