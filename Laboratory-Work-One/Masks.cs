using System.Linq;

namespace Laboratory_Work_One
{
    class Masks
    {
        private static readonly int[][,] pointClusterMasks = new int[16][,]
        {
            new int[3,3]
            {
               { 1, 0, 1 },
               { 0, 1, 0 },
               { 0, 0, 0 }
            },
            new int[3,3]
            {
                { 0, 0, 1 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            new int[3,3]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 1, 0, 1 },
            },
            new int[3,3]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 1, 0, 0 },
            },
            new int[3,3]
            {
                { 0, 1, 0 },
                { 0, 1, 0 },
                { 0, 1, 0 },
            },
            new int[3,3]
            {
                { 0, 0, 0 },
                { 1, 1, 1 },
                { 0, 0, 0 },
            },
            new int[3,3]
            {
                { 0, 0, 1 },
                { 0, 1, 0 },
                { 1, 0, 0 },
            },
            new int[3,3]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            },
            new int[3,3]
            {
                { 0, 1, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            },
            new int[3,3]
            {
                { 0, 0, 1 },
                { 0, 1, 0 },
                { 0, 1, 0 },
            },
            new int[3,3]
            {
                { 0, 0, 0 },
                { 0, 1, 1 },
                { 1, 0, 0 },
            },
            new int[3,3]
            {
                { 0, 0, 0 },
                { 1, 1, 0 },
                { 0, 0, 1 },
            },
            new int[3,3]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 1, 0 },
            },
            new int[3,3]
            {
                { 0, 1, 0 },
                { 0, 1, 0 },
                { 1, 0, 0 },
            },
            new int[3,3]
            {
                { 0, 0, 1 },
                { 1, 1, 0 },
                { 0, 0, 0 },
            },
            new int[3,3]
            {
                { 1, 0, 0 },
                { 0, 1, 1 },
                { 0, 0, 0 },
            }
        };

        public static bool IsCorrect(int[,] matrix)
        {
            foreach (int[,] mask in pointClusterMasks)
            {
                bool isEqual = true;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (mask[x, y] == 1 && mask[x, y] != matrix[x, y])
                        {
                            isEqual = false;
                        }
                    }
                }

                if (isEqual)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
