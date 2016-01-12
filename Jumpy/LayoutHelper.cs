using System;
using System.Collections.Generic;
using System.Linq;

namespace Jumpy
{
    public static class KeyboardLayoutHelper
    {
        // TODO: maybe use Key enum?
        private static readonly char[] LeftHandChars =
        {
            'f', 'd', 's', 'a', 'g', // middle row
            'r', 'e', 'w', 'q', 't', // upper row
            'v', 'c', 'x', 'z', 'b', // lower row
        };

        private static readonly char[] RightHandChars =
        {
            'j', 'k', 'l', ';', 'h', // middle row
            'u', 'i', 'o', 'p', 'y', // upper row
            'm', 'n', ',', '.', '/', // lower row
        };

        public static IEnumerable<Tuple<char, char>> EnumeratePairs()
        {
            // 15*15*4 = 900 combinations
            return new[]
                {
                    Permutate(LeftHandChars, RightHandChars),
                    Permutate(RightHandChars, LeftHandChars),
                    Permutate(LeftHandChars, LeftHandChars),
                    Permutate(RightHandChars, RightHandChars),
                }
                .SelectMany(_ => _)
                .OrderBy(GetPriority)
                .ThenBy(t => t.Item1)
                .Select(tuple => Tuple.Create(tuple.Item1, tuple.Item2));
        }

        private static int GetPriority(Tuple<char, char, int, int> tuple)
        {
            // TODO: elaborate: prioritize different hands and fingers
            // TODO: group by lines? (can be faster to think of "row", then column)
            // sum of distances from main row/finger
            var sum = tuple.Item3 + tuple.Item4;
            // penalize same letter
            if (tuple.Item1 == tuple.Item2)
                sum += 100;
            return sum;
        }

        private static IEnumerable<Tuple<char, char, int, int>> Permutate(char[] firstChars, char[] secondChars)
        {
            return firstChars.SelectMany((first, i) => secondChars.Select((second, j) => Tuple.Create(first, second, i, j)));
        }
    }
}
