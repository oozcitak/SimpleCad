using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleCAD
{
    [Flags]
    public enum ExtendType
    {
        None = 0,
        This = 1,
        Other = 2,
        Both = This | Other,
    }

    public abstract class Curve : Drawable
    {
        public const int MinCurveSegments = 4;
        public const int MaxCurveSegments = 200;

        [Browsable(false)]
        public abstract float StartParam { get; }
        [Browsable(false)]
        public abstract float EndParam { get; }

        public virtual Point2D StartPoint => GetPointAtParam(StartParam);
        public virtual Point2D EndPoint => GetPointAtParam(EndParam);

        public abstract float Area { get; }
        public virtual float Length => GetDistAtParam(EndParam);
        public virtual bool Closed => false;

        public abstract float GetDistAtParam(float param);
        public abstract Point2D GetPointAtParam(float param);
        public abstract Vector2D GetNormalAtParam(float param);

        public virtual float GetParamAtDist(float dist)
        {
            if (dist < 0) return StartParam;

            float lastDist = 0;
            float currDist = 0;
            float dt = (EndParam - StartParam) / (float)MaxCurveSegments;
            float t = dt;
            Point2D lastPt = GetPointAtParam(StartParam);
            for (int i = 1; i < MaxCurveSegments; i++)
            {
                Point2D pt = GetPointAtParam(t);
                lastDist = currDist;
                currDist += (pt - lastPt).Length;
                if (dist > lastDist && dist < currDist)
                {
                    return (dist - lastDist) / (currDist - lastDist) * dt + t;
                }
                lastPt = pt;
                t += dt;
            }

            return EndParam;
        }

        public virtual float GetParamAtPoint(Point2D pt)
        {
            float minDist = float.MaxValue;
            float minDist1 = 0;
            float minDist2 = 0;
            float mint = 0;
            bool found = false;

            float dt = (EndParam - StartParam) / (float)MaxCurveSegments;
            float t = dt;
            Point2D lastPt = GetPointAtParam(StartParam);
            for (int i = 1; i < MaxCurveSegments; i++)
            {
                Point2D currPt = GetPointAtParam(t);
                float d1 = (pt - lastPt).Length;
                float d2 = (pt - currPt).Length;
                float d = d1 + d2;
                if (d < minDist)
                {
                    minDist = d;
                    minDist1 = d1;
                    minDist2 = d2;
                    mint = t - dt;
                    found = true;
                }
                lastPt = pt;
                t += dt;
            }

            if (found)
            {
                return minDist1 / minDist * dt + mint;
            }
            else
            {
                if ((pt - StartPoint).Length < (pt - EndPoint).Length)
                    return StartParam;
                else
                    return EndParam;
            }
        }

        public virtual void GetPointAtDist(float dist) => GetPointAtParam(GetParamAtDist(dist));
        public virtual void GetDistAtPoint(Point2D pt) => GetDistAtParam(GetParamAtPoint(pt));

        public abstract void Reverse();
        public virtual bool Split(float[] @params, out Curve[] subCurves)
        {
            subCurves = new Curve[0];
            return false;
        }

        public virtual bool IntersectWith(Curve other, ExtendType extend, out Point2D[] points)
        {
            points = new Point2D[0];
            return false;
        }

        protected float[] ValidateParams(float[] @params)
        {
            Array.Sort(@params);
            List<float> validParams = new List<float>();
            for (int i = 0; i < @params.Length; i++)
            {
                if (MathF.IsBetween(@params[i], StartParam, EndParam, false))
                {
                    if (i != 0 && MathF.IsEqual(@params[i], @params[i - 1]))
                        continue;
                    validParams.Add(@params[i]);
                }
            }
            return validParams.ToArray();
        }
    }
}
