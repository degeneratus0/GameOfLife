using System.Drawing;

namespace GameOfLife
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            State = State.Dead;
            Coords.X = x;
            Coords.Y = y;
        }

        public State State;
        public Point Coords;
    }

    public enum State
    {
        Dead,
        Alive
    }
}
