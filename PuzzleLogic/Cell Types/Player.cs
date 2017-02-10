using System;

namespace Puzzle
{
    /// <summary>
    /// Our piece
    /// </summary>
    public class Player : CellBase
    {
        public EventHandler OnPlayerTeleported { get; set; }
        /// <summary>
        /// Is the player allowed to enter this new cell?
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

            if (newLocation.Contents as Exit != null)
            {
                // We can only enter the cell,  if there are no sweets remaining  TODO:
                return board.SweetsRemaining == 0; ;
            }

            if (newLocation.Contents as Monster != null)
            {
                // We can enter this cell,  but it's not going to end well!
                return true;
            }

            var slidingBlock = newLocation.Contents as SlidingBlock;
            if (slidingBlock != null && slidingBlock.Direction != direction)
            {
                // We can enter this cell but only if we can slide the block out of the cell
                return slidingBlock.CanEnterCell(board,
                                                 board.GetAdjacentCell(newLocation.Contents.X, newLocation.Y, direction),
                                                 direction);
            }

            if (newLocation.Contents as Sweet != null)
            {
                // Yes please
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

            //Furniture
            return false;
        }

        /// <summary>
        /// Moves the player into this new cell.  
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

            if (newLocation.Contents as Monster != null)
            {
                // Cell contains the monster - monster wins
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;

                return GameState.KilledByMonster;
            }

            if (newLocation.Contents as Sweet != null)
            {
                // Cell contains the sweet - eat it
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;
                board.SweetEaten();

                return GameState.InPlay;
            }

            var teleport = newLocation.Contents as Teleport;
            if (teleport != null)
            {
                // Cell contains a teleport
                var finalCell = board.GetAdjacentCell(teleport.Pair.X, teleport.Pair.Y, direction);
                OnPlayerTeleported?.Invoke(this, null);
                return MoveIntoCell(board, finalCell, direction);
            }

            var slidingBlock = newLocation.Contents as SlidingBlock;
            if (slidingBlock != null)
            {
                // Cell contains a sliding block.  Move the block into the new location and enter the cell.
                slidingBlock.MoveIntoCell(board, board.GetAdjacentCell(newLocation.X, newLocation.Y, direction), direction);
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;
                return GameState.InPlay;
            }

            if (newLocation.Contents as Exit != null)
            {
                // Cell contains the exit - we win
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;

                return GameState.Won;
            }

            // There is something already in this cell.  We cannot enter it.
            return GameState.InPlay;
        }
    }
}
