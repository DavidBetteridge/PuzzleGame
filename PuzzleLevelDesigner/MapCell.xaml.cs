using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            //get { return (BitmapImage)GetValue(ImageSourceProperty); }
            //set { SetValue(ImageSourceProperty, value); }

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

        //public static readonly DependencyProperty ImageSourceProperty =
        //        DependencyProperty.Register("ImageSource", typeof(BitmapImage),
        //            typeof(MapCell), new PropertyMetadata(""));
    }
}
