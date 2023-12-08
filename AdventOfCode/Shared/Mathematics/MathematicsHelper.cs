using System;
namespace AdventOfCode.Shared.Mathematics
{
	public class MathematicsHelper
	{
        public static (double Solution1, double Soluction2) SolveQuadratic(double a, double b, double c)
        {
            // https://www.programiz.com/python-programming/examples/quadratic-roots
            var d = Math.Pow(b, 2) - (4 * a * c);

            var sol1 = (-b - Math.Sqrt(d)) / (2 * a);
            var sol2 = (-b + Math.Sqrt(d)) / (2 * a);

            return (sol1, sol2);
        }

        public static long GreatestCommonDenominator(long a, long b)
        {
            if (a == 0)
            {
                return b;
            }
                
            return GreatestCommonDenominator(b % a, a);
        }

        public static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonDenominator(a, b)) * b;
        }
    }
}

