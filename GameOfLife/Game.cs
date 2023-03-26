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

        private static bool IsOpen = false;

        public static void Start()
        {
            Generation = 0;
            CellsToCheck = new HashSet<Point>();
            FieldSize = GetFieldSize();
            HashSet<Point> livingCells = GetInput();

            Console.SetWindowSize(
                FieldSize * 2 + 10 > Console.LargestWindowWidth ? Console.LargestWindowWidth : FieldSize * 2 + 10,
                FieldSize + 10 > Console.LargestWindowHeight ? Console.LargestWindowHeight : FieldSize + 10);

            Field = InitField();
            PreviousField = InitField();

            Field = InitAliveCells(livingCells, Field);
            FillCellsToCheck(livingCells, Field);

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
            Console.WriteLine($"Field type: {(IsOpen ? "open" : "closed")}");
            Console.WriteLine("Enter F to change field type");
            Console.WriteLine("Enter alive cells coordinates separated with space, type '-' to end");
            Console.WriteLine("Enter G for glider preset");
            Console.WriteLine("Enter GG for Gosper glider gun preset");
            Console.WriteLine("Enter R for R-Pentomino preset");
            Console.WriteLine("Enter L for Spaceship preset");
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
                    case "l":
                        return Configs.GetSpaceship(FieldSize).ToHashSet();
                    case "f":
                        IsOpen = !IsOpen;
                        Console.WriteLine($"Field type changed to: {(IsOpen ? "open" : "closed")}");
                        continue;
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
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    cells[x, y] = new Cell(x, y);
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
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    if (field[x, y].State != prevField[x, y].State)
                    {
                        Console.SetCursorPosition(x * 2, y);
                        PrintWithGrid(field[x, y]);
                    }
                }
            }
            Console.SetCursorPosition(0, FieldSize);
        }

        private static void PrintField(Cell[,] field)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    PrintWithGrid(field[x, y]);
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
            for (int x = coords.X - 1; x <= coords.X + 1; x++)
            {
                for (int y = coords.Y - 1; y <= coords.Y + 1; y++)
                {
                    Point point = new Point(x, y);
                    if (IsOpen)
                    {
                        if (x < 0)
                        {
                            point.X = FieldSize - 1;
                        }
                        else if (x >= FieldSize)
                        {
                            point.X = 0;
                        }
                        if (y < 0)
                        {
                            point.Y = FieldSize - 1;
                        }
                        else if (y >= FieldSize)
                        {
                            point.Y = 0;
                        }
                    }
                    else if (x < 0 || y < 0 || x >= FieldSize || y >= FieldSize)
                    {
                        continue;
                    }
                    action(point);
                }
            }
        }
    }
}
