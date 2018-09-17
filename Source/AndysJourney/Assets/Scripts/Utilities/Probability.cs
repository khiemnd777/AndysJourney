using UnityEngine;
using System.Linq;

public class Probability
{
    public static int[] Initialize(float[] percents)
    {
        var random = new System.Random();
        // init array with 100 elements;
        var capacity = Mathf.FloorToInt(percents.Sum());
        var arr = new int[capacity];
        for (var x = 0; x < arr.Length; x++)
        {
            var t = 0f;
            var v = 0;
            for (var y = 0; y < percents.Length; y++)
            {
                t += percents[y];
                if (x == t)
                {
                    continue;
                }
                else if (x < t)
                {
                    v = y;
                    break;
                }
            }
            arr[x] = v;
        }
        arr = arr.OrderBy(x => random.Next()).ToArray();
        return arr;
    }

    public static int[] Initialize(int[] values, float[] percents)
    {
        return Initialize<int>(values, percents);
    }

    public static float[] Initialize(float[] values, float[] percents)
    {
        return Initialize<float>(values, percents);
    }

    public static T[] Initialize<T>(T[] values, float[] percents)
    {
        var random = new System.Random();
        // init array with 100 elements;
        var capacity = Mathf.FloorToInt(percents.Sum());
        var arr = new T[capacity];
        for (var x = 0; x < arr.Length; x++)
        {
            var t = 0f;
            T v = default(T);
            for (var y = 0; y < percents.Length; y++)
            {
                t += percents[y];
                if (x == t)
                {
                    continue;
                }
                else if (x < t)
                {
                    v = values[y];
                    break;
                }
            }
            arr[x] = v;
        }
        arr = arr.OrderBy(x => random.Next()).ToArray();
        return arr;
    }

    public static T GetValueInProbability<T>(T[] probability)
    {
        if (probability == null || probability.Length == 0)
            return default(T);
        var index = Random.Range(0, probability.Length - 1);
        return probability[index];
    }
}