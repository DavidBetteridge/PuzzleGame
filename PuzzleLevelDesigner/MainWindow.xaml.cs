using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Puzzle;

namespace PuzzleLevelDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private readonly BitmapImage emptyImg;

        // The board we are currently editting
        private readonly Board board;

        // The location of the board (bin) file.
        private string filename = @"c:\temp\level.bin";

        //Teleports have to be added in pairs
        private Cell teleport1;

        //Keep track of the player
        private MapCell player;

        public MapCell[,] MapCells { get; set; }  //x,y

        public MainWindow()
        {
            InitializeComponent();

            // Load Assets
            this.sweetImg = new BitmapImage(new Uri(@"assets\sweet.bmp", UriKind.Relative));
            this.wallImg = new BitmapImage(new Uri(@"assets\wall.bmp", UriKind.Relative));
            this.manImg = new BitmapImage(new Uri(@"assets\man.jpg", UriKind.Relative));
            this.evilImg = new BitmapImage(new Uri(@"assets\evil.jpg", UriKind.Relative));
            this.teleportImg = new BitmapImage(new Uri(@"assets\teleport.jpeg", UriKind.Relative));
            this.blockLeftImg = new BitmapImage(new Uri(@"assets\LeftBlock.bmp", UriKind.Relative));
            this.blockRightImg = new BitmapImage(new Uri(@"assets\RightBlock.bmp", UriKind.Relative));
            this.blockUpImg = new BitmapImage(new Uri(@"assets\UpBlock.bmp", UriKind.Relative));
            this.blockDownImg = new BitmapImage(new Uri(@"assets\DownBlock.bmp", UriKind.Relative));
            this.exitImg = new BitmapImage(new Uri(@"assets\exit.bmp", UriKind.Relative));
            this.furnitureImg = new BitmapImage(new Uri(@"assets\FixedBlock.bmp", UriKind.Relative));
            this.emptyImg = new BitmapImage(new Uri(@"assets\Empty.bmp", UriKind.Relative));

            this.board = new LevelManager().LoadLevel(this.filename);
            map.Rows = board.Width;
            map.Columns = board.Height;

            this.MapCells = new MapCell[board.Width, board.Height];

            for (int y = 0; y < map.Rows; y++)
            {
                for (int x = 0; x < map.Columns; x++)
                {
                    var cell = board.Cells[x, y];

                    var mapCell = new MapCell()
                    {
                        ImageSource = emptyImg,
                        X = x,
                        Y = y
                    };
                    this.MapCells[x, y] = mapCell;


                    if (cell.Contents as Teleport != null) mapCell.ImageSource = this.teleportImg;
                    if (cell.Contents as Player != null)
                    {
                        mapCell.ImageSource = this.manImg;
                        this.player = mapCell;
                    }

                    if (cell.Contents as Monster != null) mapCell.ImageSource = this.evilImg;
                    if (cell.Contents as Sweet != null) mapCell.ImageSource = this.sweetImg;
                    if (cell.Contents as Exit != null) mapCell.ImageSource = this.exitImg;
                    if (cell.Contents as Furniture != null) mapCell.ImageSource = this.furnitureImg;

                    var block = cell.Contents as SlidingBlock;
                    if (block != null)
                    {
                        switch (block.Direction)
                        {
                            case Direction.Left:
                                mapCell.ImageSource = this.blockLeftImg;
                                break;
                            case Direction.Right:
                                mapCell.ImageSource = this.blockRightImg;
                                break;
                            case Direction.Up:
                                mapCell.ImageSource = this.blockUpImg;
                                break;
                            case Direction.Down:
                                mapCell.ImageSource = this.blockDownImg;
                                break;
                        }
                    }

                    mapCell.TopWall = cell.HasTopWall;
                    mapCell.BottomWall = cell.HasBottomWall;
                    mapCell.RightWall = cell.HasRightWall;
                    mapCell.LeftWall = cell.HasLeftWall;


                    mapCell.Clicked += Clicked;
                    mapCell.RightClicked += RightClicked;
                    map.Children.Add(mapCell);
                }
            }
        }

        private void RightClicked(object sender, EventArgs e)
        {
            var mc = sender as MapCell;
            if (mc != null)
            {
                this.board.ClearCell(mc.X, mc.Y);
                mc.ImageSource = emptyImg;
            }
        }

        private void Clicked(object sender, EventArgs e)
        {
            var mc = sender as MapCell;
            if (mc != null)
            {
                var cell = this.board.Cells[mc.X, mc.Y];

                if (this.man.IsChecked == true)
                {
                    //If the player already exists,  then blank out his current location
                    if (player != null) player.ImageSource = emptyImg;

                    //Set and show the new location for the player
                    this.board.SetPlayerLocation(cell.X, cell.Y);
                    mc.ImageSource = manImg;
                    this.player = mc;
                }

                if (this.teleport.IsChecked == true)
                {
                    mc.ImageSource = teleportImg;
                    if (this.teleport1 == null)
                    {
                        //First part of the pair
                        this.message.Visibility = Visibility.Visible;
                        this.teleport1 = cell;

                        //Lock the pallet so that only the teleport control can be used.
                        LockPallet(true);
                        this.teleport.IsEnabled = true;
                    }
                    else
                    {
                        //Second part of the pair
                        if (this.teleport1.X == mc.X && this.teleport1.Y == mc.Y)
                        {
                            MessageBox.Show("Please place the two teleport parts in different cells.");
                            return;
                        }
                        this.board.AddTeleport(this.teleport1.X, this.teleport1.Y, mc.X, mc.Y);
                        this.teleport1 = null;
                        this.message.Visibility = Visibility.Hidden;

                        //Unlock the pallet so that all controls can now be used
                        LockPallet(false);
                    }
                }

                if (this.sweet.IsChecked == true)
                {
                    this.board.AddSweet(cell.X, cell.Y);
                    mc.ImageSource = sweetImg;
                }

                if (this.evil.IsChecked == true)
                {
                    this.board.AddMonster(cell.X, cell.Y);
                    mc.ImageSource = evilImg;
                }

                if (this.exit.IsChecked == true)
                {
                    this.board.AddExit(cell.X, cell.Y);
                    mc.ImageSource = exitImg;
                }

                if (this.downblock.IsChecked == true)
                {
                    this.board.AddSlidingBlock(cell.X, cell.Y, Direction.Down);
                    mc.ImageSource = blockDownImg;
                }

                if (this.upblock.IsChecked == true)
                {
                    this.board.AddSlidingBlock(cell.X, cell.Y, Direction.Up);
                    mc.ImageSource = blockUpImg;
                }

                if (this.leftblock.IsChecked == true)
                {
                    this.board.AddSlidingBlock(cell.X, cell.Y, Direction.Left);
                    mc.ImageSource = blockLeftImg;
                }

                if (this.rightblock.IsChecked == true)
                {
                    this.board.AddSlidingBlock(cell.X, cell.Y, Direction.Right);
                    mc.ImageSource = blockRightImg;
                }

                if (this.fixedblock.IsChecked == true)
                {
                    this.board.AddFurniture(cell.X, cell.Y);
                    mc.ImageSource = furnitureImg;
                }

                // Top Walls
                if (this.addTopWall.IsChecked == true)
                    AddTopWall(mc, cell, true);

                if (this.removeTopWall.IsChecked == true)
                    AddTopWall(mc, cell, false);

                // Left Walls
                if (this.addLeftWall.IsChecked == true)
                    AddLeftWall(mc, cell, true);

                if (this.removeLeftWall.IsChecked == true)
                    AddLeftWall(mc, cell, false);

                // Right Walls
                if (this.addRightWall.IsChecked == true)
                    AddRightWall(mc, cell, true);

                if (this.removeRightWall.IsChecked == true)
                    AddRightWall(mc, cell, false);

                // Bottom Walls
                if (this.addBottomWall.IsChecked == true)
                    AddBottomWall(mc, cell, true);

                if (this.removeBottomWall.IsChecked == true)
                    AddBottomWall(mc, cell, false);
            }
        }

        private void AddTopWall(MapCell mc, Cell cell, bool add)
        {
            if (cell.Y > 0)
            {
                cell.HasTopWall = add;
                mc.TopWall = add;

                // Add a bottom wall to the cell above
                var cell2 = this.MapCells[cell.X, cell.Y - 1];
                cell2.BottomWall = add;
                this.board.Cells[cell.X, cell.Y - 1].HasBottomWall = add;
            }
        }

        private void AddLeftWall(MapCell mc, Cell cell, bool add)
        {
            if (cell.X > 0)
            {
                cell.HasLeftWall = add;
                mc.LeftWall = add;

                var cell2 = this.MapCells[cell.X - 1, cell.Y];
                cell2.RightWall = add;
                this.board.Cells[cell.X - 1, cell.Y].HasRightWall = add;
            }
        }

        private void AddRightWall(MapCell mc, Cell cell, bool add)
        {
            if (cell.X + 1 < this.board.Width)
            {
                cell.HasRightWall = add;
                mc.RightWall = add;

                var cell2 = this.MapCells[cell.X + 1, cell.Y];
                cell2.LeftWall = add;
                this.board.Cells[cell.X + 1, cell.Y].HasLeftWall = add;
            }
        }

        private void AddBottomWall(MapCell mc, Cell cell, bool add)
        {
            if (cell.Y + 1 < this.board.Height)
            {
                cell.HasBottomWall = add;
                mc.BottomWall = add;

                var cell2 = this.MapCells[cell.X, cell.Y + 1];
                cell2.TopWall = add;
                this.board.Cells[cell.X, cell.Y + 1].HasTopWall = add;
            }
        }

        private void LockPallet(bool lockPallet)
        {
            foreach (var item in this.pallet.Children)
            {
                var rb = item as RadioButton;
                if (rb != null)
                {
                    rb.IsEnabled = !lockPallet;
                }
            }
            this.SaveButton.IsEnabled = !lockPallet;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var levelManager = new LevelManager();
            levelManager.SaveLevel(this.board, this.filename);

            MessageBox.Show("Level Saved", "Level Designer", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
