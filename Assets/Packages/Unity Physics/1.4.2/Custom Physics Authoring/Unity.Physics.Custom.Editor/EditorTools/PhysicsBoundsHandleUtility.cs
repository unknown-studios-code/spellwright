using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

namespace Unity.Physics.Editor
{
    internal static class PhysicsBoundsHandleUtility
    {
        internal const float kBackfaceAlphaMultiplier = 0.2f;
        private const float kDegreeEpsilon = 0.001f;
        public const float kDistanceEpsilon = 0.0001f;

        public static bool IsBackfaced(float3 localPos, float3 localTangent, float3 localBinormal, Axes axes, bool isCameraInsideBox)
        {
            // if inside the box then ignore back facing alpha multiplier (otherwise all handles will look disabled)
            if (isCameraInsideBox || axes != Axes.All)
            {
                return false;
            }

            // use tangent and binormal to calculate normal in case handle matrix is skewed
            float3 worldTangent = math.normalize(Handles.matrix.MultiplyVector(localTangent));
            float3 worldBinormal = math.normalize(Handles.matrix.MultiplyVector(localBinormal));
            float3 worldDir = math.normalize(math.cross(worldTangent, worldBinormal));

            // adjust color if handle is back facing
            float cosV;

            Camera currentCamera = Camera.current;
            if (currentCamera != null && !currentCamera.orthographic)
            {
                float3 cameraPos = currentCamera.transform.position;
                float3 worldPos = Handles.matrix.MultiplyPoint(localPos);
                cosV = math.dot(math.normalize(cameraPos - worldPos), worldDir);
            }
            else
            {
                float3 cameraForward = currentCamera == null ? Vector3.forward : currentCamera.transform.forward;
                cosV = math.dot(-cameraForward, worldDir);
            }

            return cosV < -0.0001f;
        }

        public static Color GetStateColor(bool isBackfaced)
        {
            float alphaMultiplier = isBackfaced ? kBackfaceAlphaMultiplier : 1f;
            return Handles.color * new Color(1f, 1f, 1f, alphaMultiplier);
        }

        private static void AdjustMidpointHandleColor(bool isBackfaced)
        {
            Handles.color = GetStateColor(isBackfaced);
        }

        private static readonly Vector3[] s_FacePoints = new Vector3[8];
        private static readonly Vector3[] s_LinePoints = new Vector3[2];

        private static readonly int[] k_NextAxis = { 1, 2, 0 };

        public static void DrawFace(float3 center, float3 size, float cornerRadius, int normalAxis, Axes axes, bool isCameraInsideBox)
        {
            // 0 = 0 1 2
            // 1 = 1 2 0
            // 2 = 2 0 1

            int a = normalAxis;
            int b = k_NextAxis[a];
            int c = k_NextAxis[b];

            cornerRadius = math.abs(cornerRadius);
            size *= 0.5f;
            var normal = new float3 { [a] = size[a] };
            float3 ctr = center + normal;
            size -= new float3(cornerRadius);

            // check if our face is a point
            if (math.abs(size[c]) < kDistanceEpsilon && math.abs(size[b]) < kDistanceEpsilon)
            {
                return;
            }

            Vector3[] points;
            // check if our face is a line or not
            if (math.abs(size[c]) >= kDistanceEpsilon && math.abs(size[b]) >= kDistanceEpsilon)
            {
                int i = 0;
                s_FacePoints[i++] = ctr + new float3 { [b] = size[b], [c] = size[c] };
                s_FacePoints[i++] = ctr + new float3 { [b] = -size[b], [c] = size[c] };

                s_FacePoints[i++] = ctr + new float3 { [b] = -size[b], [c] = size[c] };
                s_FacePoints[i++] = ctr + new float3 { [b] = -size[b], [c] = -size[c] };

                s_FacePoints[i++] = ctr + new float3 { [b] = -size[b], [c] = -size[c] };
                s_FacePoints[i++] = ctr + new float3 { [b] = size[b], [c] = -size[c] };

                s_FacePoints[i++] = ctr + new float3 { [b] = size[b], [c] = -size[c] };
                s_FacePoints[i++] = ctr + new float3 { [b] = size[b], [c] = size[c] };
                points = s_FacePoints;
            }
            else if (math.abs(size[c]) >= kDistanceEpsilon)
            {
                int i = 0;
                s_LinePoints[i++] = ctr + new float3 { [b] = size[b], [c] = size[c] };
                s_LinePoints[i++] = ctr + new float3 { [b] = size[b], [c] = -size[c] };
                points = s_LinePoints;
            }
            else
            {
                int i = 0;
                s_LinePoints[i++] = ctr + new float3 { [c] = size[c], [b] = size[b] };
                s_LinePoints[i++] = ctr + new float3 { [c] = size[c], [b] = -size[b] };
                points = s_LinePoints;
            }

            float3 tangent,
                biNormal;
            if (size[c] > 0)
            {
                tangent = math.cross(normal, math.normalizesafe(new float3 { [c] = size[c] }));
                biNormal = math.cross(normal, tangent);
            }
            else
            {
                tangent = math.cross(normal, math.normalizesafe(new float3 { [b] = size[b] }));
                biNormal = math.cross(normal, tangent);
            }

            using (new Handles.DrawingScope(Handles.color))
            {
                AdjustMidpointHandleColor(IsBackfaced(ctr, tangent, biNormal, axes, isCameraInsideBox));
                Handles.DrawLines(points);
            }
        }

        public struct Corner
        {
            public float3 angle;
            public float3x2 intersections;
            public float3x2 points;
            public float3x3 axes;
            public float3x3 normals;
            public bool3x2 splitAxis;
            public int splitCount;

            public float3 position;
            public float radius;
            public bool isBackFaced;

            public float3 cameraForward;
        }

        public static void CalculateCornerHorizon(float3 cornerPosition, quaternion orientation, float3 cameraCenter, float3 cameraForward, bool cameraOrtho, float radius, out Corner corner)
        {
            var axisx = new float3(1f, 0f, 0f);
            var axisy = new float3(0f, 1f, 0f);
            var axisz = new float3(0f, 0f, 1f);

            // a vector pointing away from the center of the corner
            float3 cornerNormal = math.normalize(math.mul(orientation, new float3(1f, 1f, 1f)));

            float3x3 axes = math.mul(new float3x3(orientation), new float3x3(axisx, axisy, axisz));
            CalculateCornerHorizon(cornerPosition, axes, cornerNormal, cameraCenter, cameraForward, cameraOrtho, radius, out corner);
        }

        public static void CalculateCornerHorizon(
            float3 cornerPosition,
            float3x3 axes,
            float3 cornerNormal,
            float3 cameraCenter,
            float3 cameraForward,
            bool cameraOrtho,
            float radius,
            out Corner corner
        )
        {
            float3 cameraToCenter = cornerPosition - cameraCenter; // vector from camera to center
            float sqrRadius = radius * radius;
            float sqrDistCameraToCenter = math.lengthsq(cameraToCenter);
            float sqrOffset = sqrRadius * sqrRadius / sqrDistCameraToCenter; // squared distance from actual center to drawn disc center

            if (!cameraOrtho)
            {
                cameraForward = cameraToCenter;
            }

            var normals = new float3x3
            {
                c0 = math.normalize(math.cross(axes[1], axes[2])),
                c1 = math.normalize(math.cross(axes[2], axes[0])),
                c2 = math.normalize(math.cross(axes[0], axes[1])),
            };

            corner = new Corner
            {
                angle = new float3(Vector3.Angle(axes[0], axes[1]), Vector3.Angle(axes[1], axes[2]), Vector3.Angle(axes[2], axes[0])),
                intersections = default,
                points = default,
                splitAxis = default,

                axes = axes,
                normals = normals,

                position = cornerPosition,
                radius = radius,
                cameraForward = cameraForward,
                isBackFaced = math.dot(cornerNormal, cameraForward) > 0,
                splitCount = 0,
            };

            if (math.abs(sqrDistCameraToCenter) <= sqrRadius)
            {
                return;
            }

            for (int n = 0, sign = -1; n < 2; n++, sign += 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    float3 axis1 = normals[i] * sign;
                    float3 axis2 = axes[(i + 1) % 3] * sign;
                    float3 axis3 = axes[(i + 2) % 3] * sign;

                    float Q = Vector3.Angle(cameraForward, axis1);
                    float f = math.tan(math.radians(90 - math.min(Q, 180 - Q)));
                    float g = sqrOffset + (f * f * sqrOffset);
                    if (g >= sqrRadius)
                    {
                        continue;
                    }

                    float e = math.degrees(math.asin(math.sqrt(g) / radius));
                    Vector3 vectorToPointOnHorizon = Quaternion.AngleAxis(e, axis1) * math.normalize(math.cross(axis1, cameraForward));

                    vectorToPointOnHorizon = math.normalize(Vector3.ProjectOnPlane(vectorToPointOnHorizon, axis1));

                    Vector3 intersectionDirection = vectorToPointOnHorizon;
                    float angle1 = Vector3.SignedAngle(axis2, intersectionDirection, axis1);
                    float angle2 = Vector3.SignedAngle(axis3, intersectionDirection, axis1);

                    if (angle1 <= 0 || angle2 >= 0)
                    {
                        continue;
                    }

                    float3 point = corner.position + (float3)(intersectionDirection * radius);

                    if (corner.splitCount < 2)
                    {
                        corner.splitAxis[corner.splitCount][i] = true;
                        corner.intersections[corner.splitCount] = intersectionDirection;
                        corner.points[corner.splitCount] = point;

                        corner.splitCount++;
                    }
                }
            }

            if (!math.any(corner.splitAxis[0]) && !math.any(corner.splitAxis[1]))
            {
                corner.splitCount = 0;
                corner.splitAxis[0] = false;
                corner.splitAxis[1] = false;
            }
        }

        public static void DrawCorner(Corner corner, bool3 showAxis)
        {
            Color color = Handles.color;
            float3x3 axes = corner.axes;
            float3x2 intersections = corner.intersections;
            float3x3 normals = corner.normals;
            float3 origin = corner.position;
            float radius = corner.radius;

            if (corner.splitCount <= 1)
            {
                AdjustMidpointHandleColor(corner.isBackFaced);
                if (showAxis[0])
                {
                    Handles.DrawWireArc(origin, normals[0], axes[1], corner.angle[1], radius);
                }

                if (showAxis[1])
                {
                    Handles.DrawWireArc(origin, normals[1], axes[2], corner.angle[2], radius);
                }

                if (showAxis[2])
                {
                    Handles.DrawWireArc(origin, normals[2], axes[0], corner.angle[0], radius);
                }
            }
            else
            {
                float angleLength = Vector3.SignedAngle(
                    Vector3.ProjectOnPlane(intersections[0], corner.cameraForward),
                    Vector3.ProjectOnPlane(intersections[1], corner.cameraForward),
                    corner.cameraForward
                );
                bool reversePolarity = angleLength < 0;
                if (reversePolarity)
                {
                    Handles.DrawWireArc(origin, corner.cameraForward, corner.points[1] - origin, -angleLength, radius);
                }
                else
                {
                    Handles.DrawWireArc(origin, corner.cameraForward, corner.points[0] - origin, angleLength, radius);
                }

                Color backfacedColor = GetStateColor(true);

                var axesBackfaced = new bool3(
                    math.length(intersections[0] - axes[0]) < kDistanceEpsilon || math.length(intersections[1] - axes[0]) < kDistanceEpsilon,
                    math.length(intersections[0] - axes[1]) < kDistanceEpsilon || math.length(intersections[1] - axes[1]) < kDistanceEpsilon,
                    math.length(intersections[0] - axes[2]) < kDistanceEpsilon || math.length(intersections[1] - axes[2]) < kDistanceEpsilon
                );

                Color color1 = reversePolarity ? color : backfacedColor;
                Color color2 = reversePolarity ? backfacedColor : color;

                for (int A = 1, B = 2, C = 0; C < 3; A = B, B = C, C++)
                {
                    if (corner.splitAxis[0][C] == corner.splitAxis[1][C])
                    {
                        if (!axesBackfaced[A])
                        {
                            angleLength = Vector3.Angle(intersections[0], axes[A]);
                            axesBackfaced[A] = angleLength < kDegreeEpsilon || angleLength > corner.angle[C] - kDegreeEpsilon;
                        }
                        if (!axesBackfaced[B])
                        {
                            angleLength = Vector3.Angle(intersections[1], axes[A]);
                            axesBackfaced[B] = angleLength < kDegreeEpsilon || angleLength > corner.angle[C] - kDegreeEpsilon;
                        }
                    }
                    else if (corner.splitAxis[0][C])
                    {
                        if (showAxis[C])
                        {
                            angleLength = Vector3.Angle(intersections[0], axes[A]);
                            Handles.color = color1;
                            Handles.DrawWireArc(origin, normals[C], intersections[0], -angleLength, radius);
                            Handles.color = color2;
                            Handles.DrawWireArc(origin, normals[C], intersections[0], corner.angle[A] - angleLength, radius);
                        }
                        axesBackfaced[A] = true;
                    }
                    else
                    //if (corner.splitAxis[1][C])
                    {
                        if (showAxis[C])
                        {
                            angleLength = Vector3.Angle(intersections[1], axes[A]);
                            Handles.color = color2;
                            Handles.DrawWireArc(origin, normals[C], intersections[1], -angleLength, radius);
                            Handles.color = color1;
                            Handles.DrawWireArc(origin, normals[C], intersections[1], corner.angle[A] - angleLength, radius);
                        }
                        axesBackfaced[B] = true;
                    }
                }

                // check for singularity
                if (math.all(axesBackfaced))
                {
                    axesBackfaced = corner.isBackFaced;
                }

                for (int A = 1, B = 2, C = 0; C < 3; A = B, B = C, C++)
                {
                    if (!showAxis[C])
                    {
                        continue;
                    }

                    if (corner.splitAxis[0][C] == corner.splitAxis[1][C])
                    {
                        Handles.color = (axesBackfaced[B] && axesBackfaced[A]) ? color1 : color2;
                        Handles.DrawWireArc(origin, normals[C], axes[A], corner.angle[A], radius);
                    }
                }
            }
            Handles.color = color;
        }
    }
}
