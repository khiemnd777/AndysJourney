using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Visit http://wiki.unity3d.com/index.php?title=Mathfx to view more detail

public sealed class Mathfx
{
    //Ease in out
    #region Ease in out
    public static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }

    public static Vector2 Hermite(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value));
    }

    public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
    }
    #endregion

    //Ease out
    #region Sinerp
    public static float Sinerp(float start, float end, float value, float t = .5f)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * t));
    }

    public static Vector2 Sinerp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)));
    }

    public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.z, end.z, Mathf.Sin(value * Mathf.PI * 0.5f)));
    }
    #endregion

    //Ease in
    #region Ease in
    public static float Coserp(float start, float end, float value, float t = .5f)
    {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * t));
    }

    public static Vector2 Coserp(Vector2 start, Vector2 end, float value, float t = .5f)
    {
        return new Vector2(Coserp(start.x, end.x, value), Coserp(start.y, end.y, t));
    }

    public static Vector3 Coserp(Vector3 start, Vector3 end, float value, float t = .5f)
    {
        return new Vector3(Coserp(start.x, end.x, value, t), Coserp(start.y, end.y, value, t), Coserp(start.z, end.z, value, t));
    }
    #endregion

    //Boing
    #region Boing
    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static Vector2 Berp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Berp(start.x, end.x, value), Berp(start.y, end.y, value));
    }

    public static Vector3 Berp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Berp(start.x, end.x, value), Berp(start.y, end.y, value), Berp(start.z, end.z, value));
    }
    #endregion

    //Like lerp with ease in ease out
    #region Like lerp with ease in ease out
    public static float SmoothStep(float x, float min, float max)
    {
        x = Mathf.Clamp(x, min, max);
        float v1 = (x - min) / (max - min);
        float v2 = (x - min) / (max - min);
        return -2 * v1 * v1 * v1 + 3 * v2 * v2;
    }

    public static Vector2 SmoothStep(Vector2 vec, float min, float max)
    {
        return new Vector2(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max));
    }

    public static Vector3 SmoothStep(Vector3 vec, float min, float max)
    {
        return new Vector3(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max), SmoothStep(vec.z, min, max));
    }
    #endregion

    public static float Lerp(float start, float end, float value)
    {
        return ((1.0f - value) * start) + (value * end);
    }

    public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
        return lineStart + (closestPoint * lineDirection);
    }

    public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 fullDirection = lineEnd - lineStart;
        Vector3 lineDirection = Vector3.Normalize(fullDirection);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
        return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
    }

    //Bounce
    #region Bounce
    public static float Bounce(float x)
    {
        return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
    }

    public static Vector2 Bounce(Vector2 vec)
    {
        return new Vector2(Bounce(vec.x), Bounce(vec.y));
    }

    public static Vector3 Bounce(Vector3 vec)
    {
        return new Vector3(Bounce(vec.x), Bounce(vec.y), Bounce(vec.z));
    }
    #endregion

    // test for value that is near specified float (due to floating point inprecision)
    // all thanks to Opless for this!
    public static bool Approx(float val, float about, float range)
    {
        return ((Mathf.Abs(val - about) < range));
    }

    // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
    // compares the square of the distance to the square of the range as this 
    // avoids calculating a square root which is much slower than squaring the range
    public static bool Approx(Vector3 val, Vector3 about, float range)
    {
        return ((val - about).sqrMagnitude < range * range);
    }

    /*
      * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
      * This is useful when interpolating eulerAngles and the object
      * crosses the 0/360 boundary.  The standard Lerp function causes the object
      * to rotate in the wrong direction and looks stupid. Clerp fixes that.
      */
    public static float Clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
        float retval = 0.0f;
        float diff = 0.0f;

        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;

        // Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
        return retval;
    }

    public static Vector3 LerpByDistance(Vector3 a, Vector3 b, float x)
    {
        var p = x * Vector3.Normalize(b - a) + a;
        return p;
    }

    public static float Snap(float value, float snapDelta)
    {
        var valApartSnap = value / snapDelta;
        var valRound = value < 0 ? Mathf.Ceil(valApartSnap) : Mathf.Floor(valApartSnap);
        return valRound * snapDelta + Mathf.Round((value % snapDelta) / snapDelta) * snapDelta;
    }

    public static int RoundToInt(float value)
    {
        var valApartSnap = value / .5f;
        var valRound = value < 0 ? Mathf.Ceil(valApartSnap) : Mathf.Floor(valApartSnap);
        return (int)valRound;
    }

    public static double RoundApproximate(double dbl, int digits, double margin, System.MidpointRounding mode)
    {
        double fraction = dbl * System.Math.Pow(10, digits);
        double value = System.Math.Truncate(fraction);
        fraction = fraction - value;
        if (fraction == 0)
            return dbl;

        double tolerance = margin * dbl;
        // Determine whether this is a midpoint value.
        if ((fraction >= .5 - tolerance) & (fraction <= .5 + tolerance))
        {
            if (mode == System.MidpointRounding.AwayFromZero)
                return (value + 1) / System.Math.Pow(10, digits);
            else
               if (value % 2 != 0)
                return (value + 1) / System.Math.Pow(10, digits);
            else
                return value / System.Math.Pow(10, digits);
        }
        // Any remaining fractional value greater than .5 is not a midpoint value.
        if (fraction > .5)
            return (value + 1) / System.Math.Pow(10, digits);
        else
            return value / System.Math.Pow(10, digits);
    }
}