using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        public ICommand ImageTest { get; }
        public MainWindow()
        {
            InitializeComponent();

            ImageTest = new RelayCommand(ImageTesting);

            DataContext = this;
        }

        private void ImageTesting(object obj)
        {
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("Images/ContentImage.png", UriKind.Relative));
            button1.Background = brush;
        }
    }
}