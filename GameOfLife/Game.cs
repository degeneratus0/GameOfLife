using System.Drawing;

namespace GameOfLife
{
    public static class Game
    {
        private static int FieldSize;
        private static int Generation;
        private static Cell[,] Field;
        private static Cell[,] PreviousField;
        private static List<Point> CellsToCheck = new List<Point>();

        public static void Start()
        {
            Generation = 0;

            FieldSize = GetFieldSize();
            List<Point> LivingCells = GetInput();

            Console.SetWindowSize(
                FieldSize * 2 + 10 > Console.LargestWindowWidth ? Console.LargestWindowWidth : FieldSize * 2 + 10,
                FieldSize + 10 > Console.LargestWindowHeight ? Console.LargestWindowHeight : FieldSize + 10);

            InitField(ref Field);
            InitField(ref PreviousField);
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
                Console.WriteLine($"Press any other key to continue");
                ConsoleKey key = Console.ReadKey(true).Key;
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

        private static int GetFieldSize()
        {
            while (true)
            {
                Console.WriteLine("Enter desired field size (Press Enter for default 40)");
                string? input = Console.ReadLine();
                Console.Clear();
                if (input == string.Empty)
                {
                    return 40;
                }
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                else 
                { 
                    Console.WriteLine("Wrong format!");
                }
            }
        }

        private static List<Point> GetInput()
        {
            Console.WriteLine($"Field size: {FieldSize}");
            Console.WriteLine("Enter alive cells coords separated with space, type '-' to end");
            Console.WriteLine("Enter G for glider preset");
            Console.WriteLine("Enter GG for Gosper glider gun preset");
            Console.WriteLine("Enter R for R-Pentomino preset");
            List<Point> inputs = new List<Point>();
            while (true)
            {
                string[]? input = Console.ReadLine()?.Split(' ');
                switch (input?.First().ToLower())
                {
                    case "-":
                        return inputs;
                    case "g":
                        return Configs.Glider;
                    case "gg":
                        return Configs.GosperGun;
                    case "r":
                        return Configs.GetRPentomino(FieldSize);
                }
                if (int.TryParse(input?.First(), out int first) && int.TryParse(input?.Last(), out int last))
                {
                    inputs.Add(new Point(first, last));
                }
                else
                {
                    Console.WriteLine("Wrong format!");
                }
            }
        }

        private static void InitField(ref Cell[,] cells)
        {
            cells = new Cell[FieldSize, FieldSize];
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    cells[i, j] = new Cell(i, j);
                }
            }
        }

        private static Cell[,] InitAliveCells(List<Point> aliveCells, Cell[,] field)
        {
            foreach (Point coords in aliveCells)
            {
                field[coords.X, coords.Y].State = State.Alive;
            }
            return field;
        }

        private static void InitCellsToCheck(List<Point> aliveCells, Cell[,] currentfield)
        {
            foreach (Point coords in aliveCells)
            {
                CheckNearCells(coords, currentfield, true);
            }
        }

        private static void ModifyField(Cell[,] field, Cell[,] prevField)
        {
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    if (field[i, j].State != prevField[i, j].State)
                    {
                        Console.SetCursorPosition(j * 2, i);
                        PrintWithGrid(field[i, j]);
                    }
                }
            }
            Console.SetCursorPosition(0, FieldSize);
        }

        private static void PrintField(Cell[,] field)
        {
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    PrintWithGrid(field[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void PrintWithGrid(Cell cell)
        {
            if (cell.State == State.Dead)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
            }
            if (cell.Coords.X == 0)
            {
                Console.Write($"{cell.Coords.Y + 1, 2}");
            }
            else if (cell.Coords.Y == 0)
            {
                Console.Write($"{cell.Coords.X + 1, 2}");
            }
            else
            {
                Console.Write("  ");
            }
            Console.ResetColor();
        }

        private static Cell[,] CalculateNextGen(Cell[,] currentField)
        {
            List<Point> cellsToCheck = CellsToCheck.OrderBy(x => x.X).ThenBy(x => x.Y).Select(x => x).ToList();
            Cell[,] modifiedField = new Cell[FieldSize, FieldSize];
            InitField(ref modifiedField);
            foreach (Point coords in cellsToCheck)
            {
                modifiedField[coords.X, coords.Y].State = currentField[coords.X, coords.Y].State;
                Cell currentModifiedCell = modifiedField[coords.X, coords.Y];
                Cell currentCell = currentField[coords.X, coords.Y];
                int aliveCellsAround = CheckNearCells(coords, currentField, false);
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
            foreach (Point coords in cellsToCheck)
            {
                CheckNearCells(coords, modifiedField, true);
            }
            return modifiedField;
        }

        private static int CheckNearCells(Point cellCoords, Cell[,] currentField, bool AddCellToCheck)
        {
            int aliveCellsAround = 0;
            for (int k = cellCoords.X - 1; k <= cellCoords.X + 1; k++)
            {
                for (int m = cellCoords.Y - 1; m <= cellCoords.Y + 1; m++)
                {                    
                    if (k >= 0 && m >= 0 && k < FieldSize && m < FieldSize)
                    {
                        if (AddCellToCheck && currentField[cellCoords.X, cellCoords.Y].State == State.Alive)
                        {
                            AddCellToCheckIfNotContains(new Point(k, m));
                        }
                        if (!(k == cellCoords.X && m == cellCoords.Y) && currentField[k, m].State == State.Alive)
                        {
                            aliveCellsAround++;
                        }
                    }
                }
            }
            return aliveCellsAround;
        }

        private static void AddCellToCheckIfNotContains(Point coords)
        {
            if (!CellsToCheck.Contains(coords))
            {
                CellsToCheck.Add(coords);
            }
        }
    }
}
