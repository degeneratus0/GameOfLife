using System.Drawing;

namespace GameOfLife
{
    public static class Game
    {
        private static int FieldSize;
        private static int Generation;
        private static Cell[,] Field;
        private static Cell[,] PreviousField;
        private static HashSet<Point> CellsToCheck;

        public static void Start()
        {
            Generation = 0;
            CellsToCheck = new HashSet<Point>();
            FieldSize = GetFieldSize();
            HashSet<Point> LivingCells = GetInput();

            Console.SetWindowSize(
                FieldSize * 2 + 10 > Console.LargestWindowWidth ? Console.LargestWindowWidth : FieldSize * 2 + 10,
                FieldSize + 10 > Console.LargestWindowHeight ? Console.LargestWindowHeight : FieldSize + 10);

            Field = InitField();
            PreviousField = InitField();

            Field = InitAliveCells(LivingCells, Field);
            FillCellsToCheck(LivingCells, Field);

            Console.Clear();
            PrintField(Field);
            while (true)
            {
                Console.CursorVisible = false;
                ModifyField(Field, PreviousField);

                Console.WriteLine($"Generation: {Generation}");
                Console.WriteLine($"Press Z to start over");
                Console.WriteLine($"Press X to reprint the field");
                Console.WriteLine($"Press any other key for next generation");
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

                Console.CursorVisible = true;
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

        private static HashSet<Point> GetInput()
        {
            Console.WriteLine($"Field size: {FieldSize}");
            Console.WriteLine("Enter alive cells coords separated with space, type '-' to end");
            Console.WriteLine("Enter G for glider preset");
            Console.WriteLine("Enter GG for Gosper glider gun preset");
            Console.WriteLine("Enter R for R-Pentomino preset");
            HashSet<Point> inputs = new HashSet<Point>();
            while (true)
            {
                string[]? input = Console.ReadLine()?.Split(' ');
                switch (input?.First().ToLower())
                {
                    case "-":
                        return inputs;
                    case "g":
                        return Configs.GetGlider(FieldSize).ToHashSet();
                    case "gg":
                        return Configs.GosperGun.ToHashSet();
                    case "r":
                        return Configs.GetRPentomino(FieldSize).ToHashSet();
                }
                if (int.TryParse(input?.First(), out int first) && int.TryParse(input?.Last(), out int last))
                {
                    inputs.Add(new Point(first - 1, last - 1));
                }
                else
                {
                    Console.WriteLine("Wrong format!");
                }
            }
        }

        private static Cell[,] InitField()
        {
            Cell[,] cells = new Cell[FieldSize, FieldSize];
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    cells[i, j] = new Cell(i, j);
                }
            }
            return cells;
        }

        private static Cell[,] InitAliveCells(HashSet<Point> aliveCells, Cell[,] field)
        {
            foreach (Point coords in aliveCells)
            {
                field[coords.X, coords.Y].State = State.Alive;
            }
            return field;
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
            DetermineConsoleColor(cell);
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

        private static void DetermineConsoleColor(Cell cell)
        {
            if (cell.State == State.Dead)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            }
            if (cell.Coords.X == 0)
            {
                if (cell.Coords.Y % 2 == 1)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
            }
            else if (cell.Coords.Y == 0)
            {
                if (cell.Coords.X % 2 == 1)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
            }
            if (cell.State == State.Alive)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
            }
        }

        private static Cell[,] CalculateNextGen(Cell[,] currentField)
        {
            HashSet<Point> cellsToCheck = new HashSet<Point>(CellsToCheck);
            CellsToCheck.Clear();

            Cell[,] nextField = InitField();

            foreach (Point coords in cellsToCheck)
            {
                nextField[coords.X, coords.Y].State = currentField[coords.X, coords.Y].State;
                Cell currentModifiedCell = nextField[coords.X, coords.Y];
                Cell currentCell = currentField[coords.X, coords.Y];

                int aliveCellsAround = CheckNearCells(coords, currentField);
                currentModifiedCell.State = DetermineCellState(currentCell.State, aliveCellsAround);
            }
            FillCellsToCheck(cellsToCheck, nextField);
            return nextField;
        }

        private static State DetermineCellState(State state, int aliveCellsAround)
        {
            if (state == State.Dead && aliveCellsAround == 3)
            {
                return State.Alive;
            }
            else if (state == State.Alive && !(aliveCellsAround == 2 || aliveCellsAround == 3))
            {
                return State.Dead;
            }
            else
            {
                return state;
            }
        }

        private static int CheckNearCells(Point cellCoords, Cell[,] field)
        {
            int aliveCellsAround = 0;
            IterateThroughSurroundingCells(cellCoords, (p) =>
            {
                if (!(p.X == cellCoords.X && p.Y == cellCoords.Y) && field[p.X, p.Y].State == State.Alive)
                {
                    aliveCellsAround++;
                }
            });
            return aliveCellsAround;
        }

        private static void FillCellsToCheck(HashSet<Point> previousCellsToCheck, Cell[,] field)
        {
            foreach (Point coords in previousCellsToCheck)
            {
                if (field[coords.X, coords.Y].State == State.Alive)
                {
                    IterateThroughSurroundingCells(coords, (p) => CellsToCheck.Add(p));
                }
            }
        }

        private static void IterateThroughSurroundingCells(Point coords, Action<Point> action)
        {
            for (int k = coords.X - 1; k <= coords.X + 1; k++)
            {
                for (int m = coords.Y - 1; m <= coords.Y + 1; m++)
                {
                    if (k >= 0 && m >= 0 && k < FieldSize && m < FieldSize)
                    {
                        action(new Point(k, m));
                    }
                }
            }
        }
    }
}
