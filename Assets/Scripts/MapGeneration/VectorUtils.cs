// MADE BY GENESIS

using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class VectorUtils
    {
        public static IEnumerable<Vector2Int> GridCoordinates(int width, int height)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }
    }
}