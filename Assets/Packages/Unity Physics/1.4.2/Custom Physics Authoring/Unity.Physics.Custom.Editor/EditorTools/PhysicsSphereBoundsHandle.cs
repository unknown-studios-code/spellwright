using Unity.Mathematics;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.Physics.Editor
{
    internal class PhysicsSphereBoundsHandle : SphereBoundsHandle
    {
        protected override void DrawWireframe()
        {
            bool x = IsAxisEnabled(Axes.X);
            bool y = IsAxisEnabled(Axes.Y);
            bool z = IsAxisEnabled(Axes.Z);

            if (x && !y && !z)
            {
                Handles.DrawLine(Vector3.right * radius, Vector3.left * radius);
            }

            if (!x && y && !z)
            {
                Handles.DrawLine(Vector3.up * radius, Vector3.down * radius);
            }

            if (!x && !y && z)
            {
                Handles.DrawLine(Vector3.forward * radius, Vector3.back * radius);
            }

            const float kEpsilon = 0.000001F;

            if (radius > 0)
            {
                Color frontfacedColor = Handles.color;
                Color backfacedColor = Handles.color * new Color(1f, 1f, 1f, PhysicsBoundsHandleUtility.kBackfaceAlphaMultiplier);
                bool[] discVisible = new bool[] { y && z, x && z, x && y };
                var discOrientations = new float3[] { Vector3.right, Vector3.up, Vector3.forward };

                // Since the geometry is transformed by Handles.matrix during rendering, we transform the camera position
                // by the inverse matrix so that the two-shaded wireframe will have the proper orientation.
                Matrix4x4 invMatrix = Handles.inverseMatrix;

                Vector3 cameraCenter = Camera.current == null ? Vector3.zero : Camera.current.transform.position;
                Vector3 cameraToCenter = center - invMatrix.MultiplyPoint(cameraCenter); // vector from camera to center
                float sqrDistCameraToCenter = cameraToCenter.sqrMagnitude;
                float sqrRadius = radius * radius; // squared radius
                bool isCameraOrthographic = Camera.current == null || Camera.current.orthographic;
                float sqrOffset = isCameraOrthographic ? 0 : (sqrRadius * sqrRadius / sqrDistCameraToCenter); // squared distance from actual center to drawn disc center
                float insideAmount = sqrOffset / sqrRadius;
                if (insideAmount < 1)
                {
                    if (math.abs(sqrDistCameraToCenter) >= kEpsilon)
                    {
                        using (new Handles.DrawingScope(frontfacedColor))
                        {
                            if (isCameraOrthographic)
                            {
                                float horizonRadius = radius;
                                Vector3 horizonCenter = center;
                                Handles.DrawWireDisc(horizonCenter, cameraToCenter, horizonRadius);
                            }
                            else
                            {
                                float horizonRadius = math.sqrt(sqrRadius - sqrOffset);
                                Vector3 horizonCenter = center - (sqrRadius * cameraToCenter / sqrDistCameraToCenter);
                                Handles.DrawWireDisc(horizonCenter, cameraToCenter, horizonRadius);
                            }
                        }

                        Vector3 planeNormal = cameraToCenter.normalized;
                        for (int i = 0; i < 3; i++)
                        {
                            if (!discVisible[i])
                            {
                                continue;
                            }

                            float3 discOrientation = discOrientations[i];

                            float angleBetweenDiscAndNormal = math.acos(math.dot(discOrientation, planeNormal));
                            angleBetweenDiscAndNormal = (math.PI * 0.5f) - math.min(angleBetweenDiscAndNormal, math.PI - angleBetweenDiscAndNormal);

                            float f = math.tan(angleBetweenDiscAndNormal);
                            float g = math.sqrt(sqrOffset + (f * f * sqrOffset)) / radius;
                            if (g < 1)
                            {
                                float angleToHorizon = math.degrees(math.asin(g));
                                float3 discTangent = math.cross(discOrientation, planeNormal);
                                Vector3 vectorToPointOnHorizon = Quaternion.AngleAxis(angleToHorizon, discOrientation) * discTangent;
                                float horizonArcLength = (90 - angleToHorizon) * 2.0f;

                                using (new Handles.DrawingScope(frontfacedColor))
                                {
                                    Handles.DrawWireArc(center, discOrientation, vectorToPointOnHorizon, horizonArcLength, radius);
                                }

                                using (new Handles.DrawingScope(backfacedColor))
                                {
                                    Handles.DrawWireArc(center, discOrientation, vectorToPointOnHorizon, horizonArcLength - 360, radius);
                                }
                            }
                            else
                            {
                                using (new Handles.DrawingScope(backfacedColor))
                                {
                                    Handles.DrawWireDisc(center, discOrientation, radius);
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (new Handles.DrawingScope(backfacedColor))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            float3 discOrientation = discOrientations[i];
                            Handles.DrawWireDisc(center, discOrientation, radius);
                        }
                    }
                }
            }
        }
    }
}
