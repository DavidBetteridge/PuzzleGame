using System.Collections.Generic;
using System.IO;

namespace Puzzle
{
    public class LevelManager
    {
        public Board LoadLevel(string filename)
        {
            var board = new Board(30, 30);

            using (var fs = File.OpenRead(filename))
            {
                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {
                        ReadCellFromFile(board, fs, board.Cells[x, y]);
                    }
                }
            }

            return board;
        }

        public void SaveLevel(Board board, string filename)
        {
            using (var fs = File.Create(filename))
            {
                for (int x = 0; x < board.Width; x++)
                {
                    for (int y = 0; y < board.Height; y++)
                    {
                        StoreCellInFile(board.Cells[x, y], fs);
                    }
                }
                fs.Close();
            }
        }

        private void ReadCellFromFile(Board board, FileStream fs, Cell cell)
        {
            byte wallAndContent = (byte)fs.ReadByte();
            var walls = (byte)(wallAndContent >> 4); ;
            var content = (byte)(wallAndContent & 15);

            cell.HasTopWall = (walls & 1) == 1;
            cell.HasBottomWall = (walls & 2) == 2;
            cell.HasLeftWall = (walls & 4) == 4;
            cell.HasRightWall = (walls & 8) == 8;

            switch (content)
            {
                case 1:
                    board.AddExit(cell.X, cell.Y);
                    break;
                case 2:
                    board.AddFurniture(cell.X, cell.Y);
                    break;
                case 3:
                    board.AddMonster(cell.X, cell.Y);
                    break;
                case 4:
                    board.SetPlayerLocation(cell.X, cell.Y);
                    break;
                case 5:
                    var pairX = (byte)fs.ReadByte();
                    var pairY = (byte)fs.ReadByte();
                    if (cell.X < pairX || (cell.X == pairX && cell.Y < pairY))
                        board.AddTeleport(cell.X, cell.Y, pairX, pairY);
                    break;
                case 6:
                    var dir = ByteToDirection((byte)fs.ReadByte());
                    board.AddSlidingBlock(cell.X, cell.Y, dir);
                    break;
                case 7:
                    board.AddSweet(cell.X, cell.Y);
                    break;
                case 8:
                    board.AddDeath(cell.X, cell.Y);
                    break;
                default:
                    break;
            }

        }

        private void StoreCellInFile(Cell cell, FileStream fs)
        {
            var teleport = cell.Contents as Teleport;
            var slidingBlock = cell.Contents as SlidingBlock;

            byte walls = 0;
            if (cell.HasTopWall) walls += 1;
            if (cell.HasBottomWall) walls += 2;
            if (cell.HasLeftWall) walls += 4;
            if (cell.HasRightWall) walls += 8;

            byte content = 0;
            if (cell.Contents as Exit != null) content = 1;
            if (cell.Contents as Furniture != null) content = 2;
            if (cell.Contents as Monster != null) content = 3;
            if (cell.Contents as Player != null) content = 4;
            if (teleport != null) content = 5;
            if (slidingBlock != null) content = 6;
            if (cell.Contents as Sweet != null) content = 7;
            if (cell.Contents as Death != null) content = 8;

            fs.WriteByte((byte)((walls << 4) | content));

            if (teleport != null)
            {
                fs.WriteByte((byte)teleport.Pair.X);
                fs.WriteByte((byte)teleport.Pair.Y);
            }

            if (slidingBlock != null)
            {
                fs.WriteByte(DirectionToByte(slidingBlock.Direction));
            }
        }

        private byte DirectionToByte(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return 0;
                case Direction.Right:
                    return 1;
                case Direction.Up:
                    return 2;
                case Direction.Down:
                    return 3;
                default:
                    return 4;
            }
        }

        private Direction ByteToDirection(byte b)
        {
            switch (b)
            {
                case 0:
                    return Direction.Left;
                case 1:
                    return Direction.Right;
                case 2:
                    return Direction.Up;
                case 3:
                    return Direction.Down;

                default:
                    return Direction.None;
            }
        }
    }
}
