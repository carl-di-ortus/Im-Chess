using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreUtils
{
    public static class Extensions
    {
        public static bool IsEqual(this double number, double value)
        {
            return Math.Abs(number - value) < Double.Epsilon;
        }

        public static bool IsEqual(this int number, double value)
        {
            return Math.Abs(number - value) < Double.Epsilon;
        }

        public static IEnumerable<int> To(this int from, int to)
        {
            if (from < to)
            {
                while (from <= to)
                {
                    yield return from++;
                }
            }
            else
            {
                while (from >= to)
                {
                    yield return from--;
                }
            }
        }

        public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int step)
        {
            if (step == 0)
            {
                throw new ArgumentOutOfRangeException("step", "Param cannot be zero.");
            }

            return source.Where((x, i) => (i % step) == 0);
        }
    }
}
