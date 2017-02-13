using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PuzzleLevelDesigner
{
    /// <summary>
    /// Interaction logic for MapCell.xaml
    /// </summary>
    public partial class MapCell : UserControl
    {
        public EventHandler Clicked;
        public EventHandler RightClicked;

        public int X { get; set; }
        public int Y { get; set; }

        public MapCell()
        {
            InitializeComponent();

        }
        public bool TopWall
        {
            get { return this.topWall.Visibility == Visibility.Hidden; }
            set { this.topWall.Visibility = value ? Visibility.Visible : Visibility.Hidden ; }
        }

        public bool BottomWall
        {
            get { return this.bottomWall.Visibility == Visibility.Hidden; }
            set { this.bottomWall.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        public bool LeftWall
        {
            get { return this.leftWall.Visibility == Visibility.Hidden; }
            set { this.leftWall.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        public bool RightWall
        {
            get { return this.rightWall.Visibility == Visibility.Hidden; }
            set { this.rightWall.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        public ImageSource ImageSource
        {
            get { return this.img.Source; }
            set { this.img.Source = value; }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Clicked?.Invoke(this, e);
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.RightClicked?.Invoke(this, e);
        }
    }
}
