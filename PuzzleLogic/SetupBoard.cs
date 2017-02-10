namespace Puzzle
{
    public class SetupBoard
    {
        public Board NewGame()
        {
            const int boardWidth = 30;
            const int boardHeight = 30;

            var board = new Board(boardWidth, boardHeight);

            // Some test data.
            board.SetPlayerLocation(1, 1);
            board.AddMonster(9, 9);
            board.AddSlidingBlock(2, 2, Direction.Right);
            board.AddSweet(3, 2);
            board.AddTeleport(3, 3, 8, 8);
            board.AddTeleport(3, 4, 12, 12);
            board.AddExit(10,10);
            board.AddFurniture(5, 5);

            //for (int x = 0; x < boardWidth; x++)
            //{
            //    for (int y = 0; y < boardHeight; y++)
            //    {
            //        board.AddSweet(x, y);
            //    }
            //}

            board.Cells[2, 3].HasRightWall = true;
            board.Cells[2, 4].HasRightWall = true;
            board.Cells[2, 5].HasRightWall = true;
            board.Cells[2, 6].HasRightWall = true;

            board.Cells[3, 3].HasLeftWall = true;
            board.Cells[3, 4].HasLeftWall = true;
            board.Cells[3, 5].HasLeftWall = true;
            board.Cells[3, 6].HasLeftWall = true;

            board.Cells[3, 2].HasBottomWall = true;
            board.Cells[4, 2].HasBottomWall = true;
            board.Cells[5, 2].HasBottomWall = true;
            board.Cells[6, 2].HasBottomWall = true;

            board.Cells[3, 3].HasTopWall = true;
            board.Cells[4, 3].HasTopWall = true;
            board.Cells[5, 3].HasTopWall = true;
            board.Cells[6, 3].HasTopWall = true;

            return board;
        }
    }
}
