namespace GameOfLife
{
    public class Configs
    {
        public static readonly List<(int, int)> GosperGun = new List<(int, int)>
        {
            (5, 1), (6, 1), (5, 2), (6, 2),
            (5, 11), (6, 11), (7, 11), (4, 12), (8, 12), (3, 13), (9, 13), (3, 14), (9, 14), (6, 15), (4, 16), (8, 16), (5, 17), (6, 17), (7, 17), (6, 18),
            (3, 21), (4, 21), (5, 21), (3, 22), (4, 22), (5, 22), (2, 23), (6, 23), (1, 25), (2, 25), (6, 25), (7, 25),
            (4, 35), (5, 35), (4, 36), (5, 36)
        };

        public static readonly List<(int, int)> Glider = new List<(int, int)> { (5, 5), (5, 6), (5, 7), (4, 7), (3, 6) };

        public static List<(int, int)> GetRPentomino(int n)
        {
            return new List<(int, int)> { (n / 2, n / 2), (n / 2 + 1, n / 2), (n / 2, n / 2 - 1), (n / 2, n / 2 - 2), (n / 2 - 1, n / 2 - 1) };
        }
    }
}
