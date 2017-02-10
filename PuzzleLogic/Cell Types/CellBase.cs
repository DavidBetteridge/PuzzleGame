namespace Puzzle
{
    /// <summary>
    /// All types of objects which can be found in a cell inherit from this base call
    /// </summary>
    public abstract class CellBase
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
