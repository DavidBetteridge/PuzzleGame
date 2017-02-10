using System;
using System.Collections.Generic;

namespace Puzzle
{
    /// <summary>
    /// Represents all the cells on the board
    /// </summary>
    public class Board
    {
        // Event handlers for sound effects
        public EventHandler OnSweetEaten { get; set; }
        public EventHandler OnWallsMoved { get; set; }
        public EventHandler OnMonsterKilled { get; set; }
        public EventHandler OnMonsterTeleported { get; set; }
        public EventHandler OnPlayerKilled { get; set; }
        public EventHandler OnPlayerTeleported { get; set; }
        public EventHandler OnBlockPushed { get; set; }
        public EventHandler OnBlockTeleported { get; set; }

        // The player?
        public Player Player { get; private set; }

        //What monsters
        private List<Monster> Monsters = new List<Monster>();

        //What blocks
        private List<SlidingBlock> SlidingBlocks = new List<SlidingBlock>();


        public int Height { get; private set; }
        public int Width { get; private set; }
        public int SweetsRemaining { get; private set; }
        public int TimeRemaining { get; private set; }

        private int TimeInSecondsTillWallsMove = 10;

        private DateTime started;

        public void AddFurniture(int x, int y)
        {
            this.ClearCell(x, y);
            this.Cells[x, y].Contents = new Furniture() { X = x, Y = y };
        }

        public void AddExit(int x, int y)
        {
            this.ClearCell(x, y);
            this.Cells[x, y].Contents = new Exit() { X = x, Y = y };
        }


        public void AddSlidingBlock(int x, int y, Direction direction)
        {
            this.ClearCell(x, y);
            var slidingBlock = new SlidingBlock();
            slidingBlock.X = x;
            slidingBlock.Y = y;
            slidingBlock.Direction = direction;
            this.Cells[x, y].Contents = slidingBlock;
            this.SlidingBlocks.Add(slidingBlock);
        }

        public Cell[,] Cells { get; set; }  //x,y



        public Board(int boardWidth, int boardHeight)
        {
            this.Width = boardWidth;
            this.Height = boardHeight;

            this.Cells = new Cell[this.Width, this.Height];

            // Create all cells
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    this.Cells[x, y] = new Cell();
                    this.Cells[x, y].X = x;
                    this.Cells[x, y].Y = y;
                }
            }

            // Add the walls around the edge of the world
            for (int x = 0; x < this.Width; x++)
            {
                this.Cells[x, 0].HasTopWall = true;
                this.Cells[x, this.Height - 1].HasBottomWall = true;
            }
            for (int y = 0; y < this.Height; y++)
            {
                this.Cells[0, y].HasLeftWall = true;
                this.Cells[this.Width - 1, y].HasRightWall = true;
            }
        }

        public void AddSweet(int x, int y)
        {
            this.ClearCell(x, y);
            this.Cells[x, y].Contents = new Sweet() { X = x, Y = y };
            this.SweetsRemaining++;

        }


        public void AddTeleport(int x1, int y1, int x2, int y2)
        {
            this.ClearCell(x1, y1);
            this.ClearCell(x2, y2);
            var t1 = new Teleport() { X = x1, Y = y1 };
            var t2 = new Teleport() { X = x2, Y = y2 }; ;
            this.Cells[x1, y1].Contents = t1;
            this.Cells[x2, y2].Contents = t2;
            t1.Pair = t2;
            t2.Pair = t1;

        }

        internal void KillMonster(Monster monster)
        {
            this.Monsters.Remove(monster);
        }

        //Set the location for the player
        public void SetPlayerLocation(int x, int y)
        {
            if (Player != null)
            {
                this.Cells[Player.X, Player.Y].Contents = null;
            }
            else
            {
                this.Player = new Player();
                this.Player.OnPlayerTeleported += NotifyOnPlayerTeleported;
            }

            this.ClearCell(x, y);
            this.Player.X = x;
            this.Player.Y = y;
            this.Cells[x, y].Contents = this.Player;
        }

        private void NotifyOnPlayerTeleported(object sender, EventArgs e)
        {
            this.OnPlayerTeleported?.Invoke(sender, e);
        }

        internal void SweetEaten()
        {
            this.SweetsRemaining--;
            this.OnSweetEaten?.Invoke(this, null);
        }

        public void ClearCell(int x, int y)
        {
            if (this.Cells[x, y].Contents as Sweet != null) this.SweetEaten();
            if (this.Cells[x, y].Contents as Monster != null) this.KillMonster((Monster)this.Cells[x, y].Contents);

            this.Cells[x, y].Contents = null;
        }

        /// <summary>
        /// Adds a monster to the board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddMonster(int x, int y)
        {
            this.ClearCell(x, y);
            var monster = new Monster();
            monster.X = x;
            monster.Y = y;
            this.Cells[x, y].Contents = monster;
            this.Monsters.Add(monster);
        }

        public void StartGame()
        {
            this.started = DateTime.Now;
            this.TimeRemaining = 300;
        }

        /// <summary>
        /// Fires every 0.1 seconds
        /// </summary>
        /// <returns>Returns False once the game is over.</returns>
        public GameState Tick()
        {
            var result = GameState.InPlay;

            this.TimeRemaining = 30 - (int)DateTime.Now.Subtract(this.started).TotalSeconds;
            if (this.TimeRemaining <= 0)
            {
                this.TimeRemaining = 0;
                return GameState.Timeout;
            }

            TimeInSecondsTillWallsMove--;
            if (TimeInSecondsTillWallsMove == 0)
            {
                // result = MoveWalls();
                TimeInSecondsTillWallsMove = 10;
                if (result != GameState.InPlay) return result;
            }

            result = CalculateBlocks();
            if (result != GameState.InPlay) return result;

            return CalculateMonsters();
        }

        private GameState MoveWalls()
        {
            // Store where the walls currently are
            var tempLeft = new bool[Width, Height];
            var tempRight = new bool[Width, Height];
            var tempTop = new bool[Width, Height];
            var tempBottom = new bool[Width, Height];

            for (int x = 1; x < this.Width - 1; x++)
            {
                for (int y = 1; y < this.Height - 1; y++)
                {
                    tempLeft[x, y] = this.Cells[x, y].HasLeftWall;
                    tempRight[x, y] = this.Cells[x, y].HasRightWall;
                    tempTop[x, y] = this.Cells[x, y].HasTopWall;
                    tempBottom[x, y] = this.Cells[x, y].HasBottomWall;
                }
            }

            // Move the vertical walls down by wall,  and the horizontal walls right by one
            for (int x = 1; x < this.Width - 1; x++)
            {
                for (int y = 1; y < this.Height - 1; y++)
                {
                    this.Cells[x, (y + 1) % this.Height].HasLeftWall = tempLeft[x, y];
                    this.Cells[x, (y + 1) % this.Height].HasRightWall = tempRight[x, y];
                    this.Cells[(x + 1) % this.Width, y].HasTopWall = tempTop[x, y];
                    this.Cells[(x + 1) % this.Width, y].HasBottomWall = tempBottom[x, y];
                }
            }

            return GameState.InPlay;
        }

        public Cell GetAdjacentCell(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (x > 0)
                        return this.Cells[x - 1, y];
                    else
                        return null;

                case Direction.Right:
                    if (x < this.Width - 1)
                        return this.Cells[x + 1, y];
                    else
                        return null;

                case Direction.Up:
                    if (y > 0)
                        return this.Cells[x, y - 1];
                    else
                        return null;

                case Direction.Down:
                    if (y < this.Height - 1)
                        return this.Cells[x, y + 1];
                    else
                        return null;

                default:
                    return null;
            }
        }

        public bool BlockedByWall(Cell cell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return cell.HasLeftWall;
                case Direction.Right:
                    return cell.HasRightWall;
                case Direction.Up:
                    return cell.HasTopWall;
                case Direction.Down:
                    return cell.HasBottomWall;
                default:
                    return false;
            }
        }

        private GameState CalculateBlocks()
        {
            var result = GameState.InPlay;

            foreach (var block in this.SlidingBlocks)
            {
                var currentLocation = this.Cells[block.X, block.Y];
                var nextCell = GetAdjacentCell(block.X, block.Y, block.Direction);

                if (!BlockedByWall(currentLocation, block.Direction) && block.CanEnterCell(this, nextCell, block.Direction))
                {
                    currentLocation.Contents = null;
                    result = block.MoveIntoCell(this, nextCell, block.Direction);
                    if (result != GameState.InPlay) return result;
                    block.OnTheMove = true;
                }
                else
                {
                    block.OnTheMove = false;
                }

            }

            return result;
        }

        private GameState CalculateMonsters()
        {
            var result = GameState.InPlay;

            foreach (var monster in this.Monsters)
            {
                var currentLocation = this.Cells[monster.X, monster.Y];
                var xDiff = monster.X - this.Player.X;
                var yDiff = monster.Y - this.Player.Y;
                var direction = Direction.None;

                if (xDiff > 0) direction = Direction.Left;
                if (xDiff < 0) direction = Direction.Right;
                if (direction != Direction.None)
                {
                    if (!this.BlockedByWall(currentLocation, direction))
                    {
                        var newLocation = this.GetAdjacentCell(monster.X, monster.Y, direction);
                        if (monster.CanEnterCell(this, newLocation, direction))
                        {
                            currentLocation.Contents = null;
                            result = monster.MoveIntoCell(this, newLocation, direction);
                            if (result != GameState.InPlay) return result;
                            continue;
                        }
                    }
                }

                if (yDiff > 0) direction = Direction.Up;
                if (yDiff < 0) direction = Direction.Down;
                if (direction != Direction.None)
                {
                    if (!this.BlockedByWall(currentLocation, direction))
                    {
                        var newLocation = this.GetAdjacentCell(monster.X, monster.Y, direction);
                        if (monster.CanEnterCell(this, newLocation, direction))
                        {
                            currentLocation.Contents = null;
                            result = monster.MoveIntoCell(this, newLocation, direction);
                            if (result != GameState.InPlay) return result;
                            continue;
                        }
                    }
                }
            }

            return result;
        }

        public GameState MovePlayer(Direction direction)
        {
            // Get the cell where the player currently is.
            var currentCell = this.Cells[Player.X, Player.Y];

            // Can they go that way?
            if (!BlockedByWall(currentCell, direction))
            {
                var nextCell = GetAdjacentCell(Player.X, Player.Y, direction);
                if (Player.CanEnterCell(this, nextCell, direction))
                {
                    currentCell.Contents = null;
                    return Player.MoveIntoCell(this, nextCell, direction);
                }
            }

            return GameState.InPlay;
        }


    }
}
