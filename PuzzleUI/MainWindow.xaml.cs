using System;
using System.Globalization;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Puzzle;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Board Board;

        const int ViewPortWidth = 20;
        const int ViewPortHeight = 14;

        // How do we offset the viewport as the player moves around
        int XOffset = 0;
        int YOffset = 0;

        const int cellWidth = 64;
        const int cellHeight = 64;
        const int wallWidth = 10;
        const int wallHeight = 10;

        //Assets
        private readonly BitmapImage sweetImg;
        private readonly BitmapImage wallImg;
        private readonly BitmapImage manImg;
        private readonly BitmapImage evilImg;
        private readonly BitmapImage teleportImg;
        private readonly BitmapImage blockLeftImg;
        private readonly BitmapImage blockRightImg;
        private readonly BitmapImage blockUpImg;
        private readonly BitmapImage blockDownImg;
        private readonly BitmapImage exitImg;
        private readonly BitmapImage furnitureImg;
        private readonly BitmapImage BackgroundImg;

        // For playing sounds
        private readonly SoundManager SoundManager;

        private readonly System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public MainWindow()
        {
            InitializeComponent();

            // Load Assets
            this.sweetImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\sweet.bmp", UriKind.Absolute));
            this.wallImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\wall.bmp", UriKind.Absolute));
            this.manImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\man.jpg", UriKind.Absolute));
            this.evilImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\evil.jpg", UriKind.Absolute));
            this.teleportImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\teleport.jpeg", UriKind.Absolute));
            this.blockLeftImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\LeftBlock.bmp", UriKind.Absolute));
            this.blockRightImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\RightBlock.bmp", UriKind.Absolute));
            this.blockUpImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\UpBlock.bmp", UriKind.Absolute));
            this.blockDownImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\DownBlock.bmp", UriKind.Absolute));
            this.exitImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\exit.bmp", UriKind.Absolute));
            this.furnitureImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\assets\FixedBlock.bmp", UriKind.Absolute));

            this.BackgroundImg = new BitmapImage(new Uri(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\Background.jpg", UriKind.Absolute));

            // Full screen mode
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            // Setup the sounds
            this.SoundManager = new SoundManager();

            var levelManager = new LevelManager();
            var tempBoard = new SetupBoard().NewGame();
         //   levelManager.SaveLevel(tempBoard, @"c:\temp\level.bin");
            this.Board = levelManager.LoadLevel(@"c:\temp\level.bin");

            //Setup a new game
            //  this.Board = new SetupBoard().NewGame();
            this.Board.OnSweetEaten += OnSweetEaten;
            this.Board.OnPlayerTeleported += OnTeleport;
            //   this.Board.OnPlayerKilled += OnGameOver;

            this.dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            this.KeyDown += MainWindow_KeyDown;

            this.Board.StartGame(); //start the clock



        }

        private void OnTeleport(object sender, EventArgs e)
        {
            this.SoundManager.PlaySound("TELEPORT");
        }

        private void OnSweetEaten(object sender, EventArgs e)
        {
            this.SoundManager.PlaySound("SWEET_EATEN");
        }

        private void OnGameOver(object sender, EventArgs e)
        {
            this.SoundManager.PlaySound("GAME_VER");
        }
        private Direction KeyToDirection(Key key)
        {
            if (key == Key.Z) return Direction.Left;
            if (key == Key.X) return Direction.Right;
            if (key == Key.K) return Direction.Up;
            if (key == Key.M) return Direction.Down;
            return Direction.None;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var direction = KeyToDirection(e.Key);
            if (direction == Direction.None) return;

            var gameState = this.Board.MovePlayer(direction);
            this.InvalidateVisual();

            if (gameState != GameState.InPlay)
            {
                // Game over
                GameOver(gameState);
                return;
            }

            if (gameState == GameState.InPlay)
            {
                switch (direction)
                {
                    case Direction.Left:
                        // If the player has moved left into the first visible row,  then shift the view point left
                        var leftRow = XOffset;
                        if (this.Board.Player.X == leftRow && XOffset > 0)
                            XOffset--;
                        break;

                    case Direction.Right:
                        // If the player has moved right into the final visible row,  then shift the view point right
                        var rightRow = ViewPortWidth + XOffset;
                        if (this.Board.Player.X == rightRow && XOffset + ViewPortWidth < this.Board.Width)
                            XOffset++;
                        break;

                    case Direction.Up:
                        // If the player has moved down into the first visible row,  then shift the view point up
                        var firstVisibleRow = YOffset;
                        if (this.Board.Player.Y == firstVisibleRow && YOffset > 0)
                            YOffset--;
                        break;

                    case Direction.Down:
                        // If the player has moved down into the final visible row,  then shift the view point down
                        var finalVisibleRow = ViewPortHeight + YOffset;
                        if (this.Board.Player.Y == finalVisibleRow - 1 && YOffset + ViewPortHeight < this.Board.Height)
                            YOffset++;
                        break;

                    case Direction.None:
                        break;

                    default:
                        break;
                }
            }

        }

        private void GameOver(GameState gameState)
        {
            //Kill the timer
            dispatcherTimer.IsEnabled = false;

            OnGameOver(this, null);

            switch (gameState)
            {
                case GameState.InPlay:
                    break;
                case GameState.KilledByMonster:
                    MessageBox.Show("You were killed by a monster", "Game Over");//, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case GameState.KilledByBlock:
                    MessageBox.Show("You were killed by a sliding block", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case GameState.Won:
                    MessageBox.Show("You won", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case GameState.Timeout:
                    MessageBox.Show("You ran out of time.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    break;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var result = this.Board.Tick();
            this.InvalidateVisual();

            if (result != GameState.InPlay)
            {
                GameOver(result);
            }
        }

        static Pen pen = new Pen(Brushes.Black, 5.0);

        private void DrawText(DrawingContext drawingContext, string text, int x, int y)
        {
            var formattedText = new FormattedText(
                    text,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    24,
                    Brushes.Black);
            drawingContext.DrawText(formattedText, new Point(this.Width - 350 - x, y));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Total width is the Board.Width * cellWidth + wallWidth
            var totalWidth = Board.Width * (cellWidth + wallWidth);
            var totalHeight = Board.Height * (cellHeight + wallHeight);
            var backX = -XOffset * (cellWidth + wallWidth);
            var backY = -YOffset * (cellHeight + wallHeight);
            var r = ViewPortWidth * (cellWidth + wallWidth);

            // Background
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(RenderSize));
            drawingContext.DrawImage(BackgroundImg, new Rect(backX, backY, totalWidth, totalHeight));
            //MyBackgroundBrush.Viewbox = new Rect(this.Left, this.Top, this.Width - 400, this.Height);
            // Details in the RHS
            drawingContext.DrawRectangle(Brushes.AliceBlue, null, new Rect(r, 0, this.Width - r, this.Height));
            DrawText(drawingContext, $"Level: One", 0, 100);
            DrawText(drawingContext, $"Sweets Remaining: {Board.SweetsRemaining}", 0, 150);
            DrawText(drawingContext, $"Time Remaining: {Board.TimeRemaining}s:", 0, 200);

            // Border
            
            drawingContext.DrawLine(pen, new Point(r, 0), new Point(r, this.Height)); //rhs
            drawingContext.DrawLine(pen, new Point(0, this.Height - 10), new Point(r, this.Height - 10)); //bottom

            for (int x = 0; x < ViewPortWidth; x++)
            {
                var xb = x + XOffset;
                for (int y = 0; y < ViewPortHeight; y++)
                {
                    var yb = y + YOffset;

                    // The cell to draw
                    var cellToDraw = this.Board.Cells[xb, yb];

                    //Corners of the cell
                    var topLeft = new Point(x * (cellWidth + wallWidth), y * (cellWidth + wallWidth));
                    var bottomLeft = new Point(x * (cellWidth + wallWidth), (y + 1) * (cellWidth + wallWidth));
                    var topRight = new Point((x + 1) * (cellWidth + wallWidth), y * (cellWidth + wallWidth));
                    var bottomRight = new Point((x + 1) * (cellWidth + wallWidth), (y + 1) * (cellWidth + wallWidth));

                    var innerTopLeft = new Point(x * (cellWidth + wallWidth) + (wallWidth / 2), y * (cellWidth + wallWidth) + (wallHeight / 2));
                    var innerBottomRight = new Point((x + 1) * (cellWidth + wallWidth) - (wallWidth / 2), (y + 1) * (cellWidth + wallWidth) - (wallHeight / 2));

                    if (cellToDraw.HasTopWall)
                    {
                        var topLeftOfWall = new Point(topLeft.X, topLeft.Y - (wallHeight / 2));
                        var bottomRightOfWall = new Point(topRight.X, topRight.Y + (wallHeight / 2));
                        drawingContext.DrawImage(wallImg, new Rect(topLeftOfWall, bottomRightOfWall));
                    }

                    if (cellToDraw.HasBottomWall)
                    {
                        var topLeftOfWall = new Point(bottomLeft.X, bottomLeft.Y + (wallHeight / 2));
                        var bottomRightOfWall = new Point(bottomRight.X, bottomRight.Y - (wallHeight / 2));
                        drawingContext.DrawImage(wallImg, new Rect(topLeftOfWall, bottomRightOfWall));
                    }

                    if (cellToDraw.HasLeftWall)
                    {
                        var topLeftOfWall = new Point(topLeft.X - (wallWidth / 2), topLeft.Y);
                        var bottomRightOfWall = new Point(bottomLeft.X + (wallWidth / 2), bottomLeft.Y);
                        drawingContext.DrawImage(wallImg, new Rect(topLeftOfWall, bottomRightOfWall));
                    }

                    if (cellToDraw.HasRightWall)
                    {
                        var topLeftOfWall = new Point(topRight.X + (wallWidth / 2), topRight.Y);
                        var bottomRightOfWall = new Point(bottomRight.X - (wallWidth / 2), bottomRight.Y);
                        drawingContext.DrawImage(wallImg, new Rect(topLeftOfWall, bottomRightOfWall));
                    }

                    //What is in the cell?
                    if (cellToDraw.Contents as Player != null)
                    {
                        drawingContext.DrawImage(manImg, new Rect(innerTopLeft, innerBottomRight));
                    }

                    if (cellToDraw.Contents as Monster != null)
                    {
                        drawingContext.DrawImage(evilImg, new Rect(innerTopLeft, innerBottomRight));
                    }

                    if (cellToDraw.Contents as Sweet != null)
                    {
                        drawingContext.DrawImage(sweetImg, new Rect(innerTopLeft, innerBottomRight));
                    }

                    if (cellToDraw.Contents as SlidingBlock != null)
                    {
                        switch (((SlidingBlock)cellToDraw.Contents).Direction)
                        {
                            case Direction.Down:
                                drawingContext.DrawImage(blockDownImg, new Rect(innerTopLeft, innerBottomRight));
                                break;

                            case Direction.Up:
                                drawingContext.DrawImage(blockUpImg, new Rect(innerTopLeft, innerBottomRight));
                                break;

                            case Direction.Left:
                                drawingContext.DrawImage(blockLeftImg, new Rect(innerTopLeft, innerBottomRight));
                                break;

                            case Direction.Right:
                                drawingContext.DrawImage(blockRightImg, new Rect(innerTopLeft, innerBottomRight));
                                break;
                            default:
                                break;
                        }
                    }

                    if (cellToDraw.Contents as Teleport != null)
                    {
                        drawingContext.DrawImage(teleportImg, new Rect(innerTopLeft, innerBottomRight));
                    }

                    if (cellToDraw.Contents as Exit != null)
                    {
                        drawingContext.DrawImage(exitImg, new Rect(innerTopLeft, innerBottomRight));
                    }

                    if (cellToDraw.Contents as Furniture != null)
                    {
                        drawingContext.DrawImage(furnitureImg, new Rect(innerTopLeft, innerBottomRight));
                    }
                }
            }

            //for (int x = 0; x < ViewPortWidth; x++)
            //{
            //    var xb = x + XOffset;

            //    var previousPoint = new Point(x * (cellWidth + wallWidth), 1);
            //    var newPoint = new Point(x * (cellWidth + wallWidth), this.Height);
            //    drawingContext.DrawLine(pen, previousPoint, newPoint);
            //}


            //for (int y = 0; y < ViewPortHeight; y++)
            //{
            //    var previousPoint = new Point(1, y * (cellHeight + wallHeight));
            //    var newPoint = new Point(this.Width, y * (cellHeight + wallHeight));
            //    drawingContext.DrawLine(pen, previousPoint, newPoint);
            //}


        }


    }
}
