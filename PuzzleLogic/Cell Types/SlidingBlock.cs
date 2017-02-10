using System;

namespace Puzzle
{
    public class SlidingBlock : CellBase
    {
        /// <summary>
        /// Which why does this block slide?
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// Once the block starts moving this gets set to TRUE,  when the block comes to rest we reset this back to False
        /// The player can only be killed when the block is on the move.
        /// </summary>
        public bool OnTheMove { get; set; }

        /// <summary>
        /// Can the block enter this new cell?
        /// </summary>
        /// <param name="board"></param>
        /// <param name="newLocation"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanEnterCell(Board board, Cell newLocation, Direction direction)
        {
            if (newLocation.Contents == null)
            {
                // Cell is empty
                return true;
            }

            if (newLocation.Contents as Player != null && this.OnTheMove)
            {
                // Cell contains the player - player loses
                return true;
            }

            if (newLocation.Contents as Monster != null)
            {
                // Cell contains the monster - monster dies
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
        /// Slide the cell into this new location.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cell"></param>
        /// <param name="direction"></param>
        internal GameState MoveIntoCell(Board board, Cell newLocation, Direction direction)
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
                // Cell contains the player - player dies
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;

                return GameState.KilledByBlock;
            }

            var monster = newLocation.Contents as Monster;
            if (monster != null)
            {
                // Cell contains the player - kill the monster
                this.X = newLocation.X;
                this.Y = newLocation.Y;
                newLocation.Contents = this;
                board.KillMonster(monster);
                return GameState.InPlay;
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
