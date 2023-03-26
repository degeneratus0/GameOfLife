using System.Drawing;

namespace GameOfLife
{
    public class Configs
    {
        public static readonly List<Point> GosperGun = new List<Point>
        {
            new Point (1, 5),
            new Point (1, 6),
            new Point (2, 5),
            new Point (2, 6),

            new Point (11, 5),
            new Point (11, 6),
            new Point (11, 7),
            new Point (12, 4),
            new Point (12, 8),
            new Point (13, 3),
            new Point (13, 9),
            new Point (14, 3),
            new Point (14, 9),
            new Point (15, 6),
            new Point (16, 4),
            new Point (16, 8),
            new Point (17, 5),
            new Point (17, 6),
            new Point (17, 7),
            new Point (18, 6),

            new Point (21, 3),
            new Point (21, 4),
            new Point (21, 5),
            new Point (22, 3),
            new Point (22, 4),
            new Point (22, 5),
            new Point (23, 2),
            new Point (23, 6),
            new Point (25, 1),
            new Point (25, 2),
            new Point (25, 6),
            new Point (25, 7),

            new Point (35, 4),
            new Point (35, 5),
            new Point (36, 4),
            new Point (36, 5)
        };

        public static List<Point> GetGlider(int n)
        {
            int halfN = n / 2;
            return new List<Point> {
                new Point (halfN, halfN),
                new Point (halfN, halfN + 1),
                new Point (halfN, halfN + 2),
                new Point (halfN - 1, halfN + 2),
                new Point (halfN - 2, halfN + 1)
        };
        }

        public static List<Point> GetRPentomino(int n)
        {

            int halfN = n / 2;
            return new List<Point> {
                new Point (halfN, halfN),
                new Point (halfN, halfN + 1),
                new Point (halfN - 1, halfN),
                new Point (halfN - 2, halfN),
                new Point (halfN - 1, halfN - 1)
            };
        }

        public static List<Point> GetSpaceship(int n)
        {

            int halfN = n / 2;
            return new List<Point> {
                new Point (halfN, halfN),
                new Point (halfN, halfN + 2),
                new Point (halfN + 1, halfN + 3),
                new Point (halfN + 2, halfN + 3),
                new Point (halfN + 3, halfN + 3),
                new Point (halfN + 4, halfN + 3),
                new Point (halfN + 4, halfN + 2),
                new Point (halfN + 4, halfN + 1),
                new Point (halfN + 3, halfN)
            };
        }
    }
}
