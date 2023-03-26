﻿using System.Drawing;

namespace GameOfLife
{
    public class Configs
    {
        public static readonly List<Point> GosperGun = new List<Point>
        {
            new Point (5, 1),
            new Point (6, 1),
            new Point (5, 2),
            new Point (6, 2),

            new Point (5, 11),
            new Point (6, 11),
            new Point (7, 11),
            new Point (4, 12),
            new Point (8, 12),
            new Point (3, 13),
            new Point (9, 13),
            new Point (3, 14),
            new Point (9, 14),
            new Point (6, 15),
            new Point (4, 16),
            new Point (8, 16),
            new Point (5, 17),
            new Point (6, 17),
            new Point (7, 17),
            new Point (6, 18),

            new Point (3, 21),
            new Point (4, 21),
            new Point (5, 21),
            new Point (3, 22),
            new Point (4, 22),
            new Point (5, 22),
            new Point (2, 23),
            new Point (6, 23),
            new Point (1, 25),
            new Point (2, 25),
            new Point (6, 25),
            new Point (7, 25),

            new Point (4, 35),
            new Point (5, 35),
            new Point (4, 36),
            new Point (5, 36)
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
                new Point (halfN + 1, halfN),
                new Point (halfN, halfN - 1),
                new Point (halfN, halfN - 2),
                new Point (halfN - 1, halfN - 1)
            };
        }
    }
}
