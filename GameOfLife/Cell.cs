namespace GameOfLife
{
    public class Cell
    {
        public Cell()
        {
            State = State.Dead;
        }

        public State State;
    }

    public enum State
    {
        Dead,
        Alive
    }
}
