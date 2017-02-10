namespace Puzzle
{
    /// <summary>
    /// An attacking monster
    /// </summary>
    public class Monster : CellBase
    {
        /// <summary>
        /// Is the monster allowed to enter this new cell?
        /// </summary>
        /// <param name="newLocation"></param>
        /// <returns></returns>
        public bool CanEnterCell(Board board, Cell newLocation, Direction direction)
        {
            if (newLocation.Contents == null)
            {
                // Cell is empty.
                return true;
            }

            if (newLocation.Contents as Player != null)
            {
                // Cell contains the player - monster wins
                return true;
            }

            var teleport = newLocation.Contents as Teleport;
            if (teleport != null)
            {
                // Cell contains a teleport - ok if the exit is clear. 
                var finalCell = board.GetAdjacentCell(teleport.Pair.X, teleport.Pair.Y, direction);
                if (CanEnterCell(board, finalCell, direction) && !board.BlockedByWall(board.Cells[teleport.Pair.X, teleport.Pair.Y], direction))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Moves the monster into this new cell.  
        /// </summary>
        /// <returns></returns>
        public GameState MoveIntoCell(Board board, Cell newLocation, Direction direction)
        {
            // What is in the new cell?
            if (newLocation.Contents == null)
            {
                // Cell is empty.
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;

                return GameState.InPlay;
            }

            if (newLocation.Contents as Player != null)
            {
                // Cell contains the player - monster wins
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;

                return GameState.KilledByMonster;
            }

            var teleport = newLocation.Contents as Teleport;
            if (teleport != null)
            {
                // Cell contains a teleport
                var finalCell = board.GetAdjacentCell(teleport.Pair.X, teleport.Pair.Y, direction);
                return MoveIntoCell(board, finalCell, direction);
            }

            // There is something already in this cell.  We cannot enter it.
            return GameState.InPlay;
        }
    }
}
