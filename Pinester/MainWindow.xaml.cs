using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
// комментарий
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
            brush.ImageSource = new BitmapImage(new Uri("Resourses/images.png", UriKind.Relative));
        }

        private void aPicture_MouseDown(object sender, MouseEventArgs e)
        {
            e.Source = new BitmapImage(new Uri(@"/Resourses/images.png", UriKind.Relative));
        }
    }
}