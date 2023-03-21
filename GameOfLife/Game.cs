namespace GameOfLife
{
    public static class Game
    {
        private const int n = 50;
        private static int Generation;
        private static Cell[,] Field = new Cell[n, n];
        private static Cell[,] PreviousField = new Cell[n, n];
        private static List<(int, int)> CellsToCheck = new List<(int, int)>();

        public static void Start()
        {
            Generation = 0;
            Console.SetWindowSize(
                n * 2 + 10 > Console.LargestWindowWidth ? Console.LargestWindowWidth : n * 2 + 10, 
                n + 10 > Console.LargestWindowHeight ? Console.LargestWindowHeight : n + 10);
            
            InitField(Field);
            InitField(PreviousField);
            List<(int, int)> LivingCells = GetInput();
            Field = InitAliveCells(LivingCells, Field);
            InitCellsToCheck(LivingCells, Field);

            Console.Clear();
            PrintField(Field);
            while (true)
            {
                ModifyField(Field, PreviousField);

                Console.WriteLine($"Generation: {Generation}");
                Console.WriteLine($"Press Z to start over");
                Console.WriteLine($"Press X to reprint the field");
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.Z)
                {
                    Console.Clear();
                    break;
                }
                switch (key)
                {
                    case ConsoleKey.X:
                        Console.Clear();
                        PrintField(Field);
                        continue;
                }

                PreviousField = Field;
                Field = CalculateNextGen(Field);
                Generation++;
            }
        }

        private static void InitField(Cell[,] cells)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    cells[i, j] = new Cell();
                }
            }
        }

        private static List<(int first, int last)> GetInput()
        {
            Console.WriteLine("Enter alive cells coords separated with space, type '-' to end");
            Console.WriteLine("Enter G for glider preset");
            Console.WriteLine("Enter GG for Gosper glider gun preset");
            Console.WriteLine("Enter R for R-Pentomino preset");
            List<(int, int)> inputs = new List<(int, int)>();
            while (true)
            {
                string[] input = Console.ReadLine().Split(' ');
                switch (input.First().ToLower())
                {
                    case "-":
                        return inputs;
                    case "g":
                        return Configs.Glider;
                    case "gg":
                        return Configs.GosperGun;
                    case "r":
                        return Configs.GetRPentomino(n);
                }
                if (int.TryParse(input.First(), out int first) && int.TryParse(input.Last(), out int last))
                {
                    inputs.Add((first, last));
                }
                else
                {
                    Console.WriteLine("Wrong format!");
                }
            }
        }

        private static Cell[,] InitAliveCells(List<(int, int)> aliveCells, Cell[,] field)
        {
            foreach ((int first, int last) coords in aliveCells)
            {
                field[coords.first, coords.last].State = State.Alive;
            }
            return field;
        }

        private static void InitCellsToCheck(List<(int, int)> aliveCells, Cell[,] currentfield)
        {
            foreach ((int first, int last) coords in aliveCells)
            {
                CheckNearCells(coords.first, coords.last, currentfield, true);
            }
        }

        private static void ModifyField(Cell[,] field, Cell[,] prevField)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (field[i, j].State != prevField[i, j].State)
                    {
                        Console.SetCursorPosition(j * 2, i);
                        if (field[i, j].State == State.Dead)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        Console.Write("  ");
                        Console.ResetColor();
                    }
                }
            }
            Console.SetCursorPosition(0, n);
        }

        private static void PrintField(Cell[,] field)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (field[i, j].State == State.Dead)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    }
                    if (i == 0)
                    {
                        Console.Write($"{j,2}");
                    }
                    else if (j == 0)
                    {
                        Console.Write($"{i,2}");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private static Cell[,] CalculateNextGen(Cell[,] currentField)
        {
            List<(int, int)> cellsToCheck = CellsToCheck.OrderBy(x => x.Item1).ThenBy(x => x.Item2).Select(x => x).ToList();
            Cell[,] modifiedField = new Cell[n, n];
            InitField(modifiedField);
            foreach ((int first, int last) coords in cellsToCheck)
            {
                modifiedField[coords.first, coords.last].State = currentField[coords.first, coords.last].State;
                Cell currentModifiedCell = modifiedField[coords.first, coords.last];
                Cell currentCell = currentField[coords.first, coords.last];
                int aliveCellsAround = CheckNearCells(coords.first, coords.last, currentField, false);
                if (currentCell.State == State.Dead && aliveCellsAround == 3)
                {
                    currentModifiedCell.State = State.Alive;
                }
                else if (currentCell.State == State.Alive && !(aliveCellsAround == 2 || aliveCellsAround == 3))
                {
                    currentModifiedCell.State = State.Dead;
                }
            }
            CellsToCheck.Clear();
            foreach ((int first, int last) coords in cellsToCheck)
            {
                CheckNearCells(coords.first, coords.last, modifiedField, true);
            }
            return modifiedField;
        }

        private static int CheckNearCells(int firstCellCoord, int lastCellCoord, Cell[,] currentField, bool AddCellToCheck)
        {
            int aliveCellsAround = 0;
            for (int k = firstCellCoord - 1; k <= firstCellCoord + 1; k++)
            {
                for (int m = lastCellCoord - 1; m <= lastCellCoord + 1; m++)
                {
                    if (k >= 0 && m >= 0 && k < n && m < n)
                    {
                        if (AddCellToCheck && currentField[firstCellCoord, lastCellCoord].State == State.Alive)
                        {
                            AddCellToCheckIfNotContains(k, m);
                        }
                        if (!(k == firstCellCoord && m == lastCellCoord) && currentField[k, m].State == State.Alive)
                        {
                            aliveCellsAround++;
                        }
                    }
                }
            }
            return aliveCellsAround;
        }

        private static void AddCellToCheckIfNotContains(int x, int y)
        {
            if (!CellsToCheck.Contains((x, y)))
            {
                CellsToCheck.Add((x, y));
            }
        }
    }
}
