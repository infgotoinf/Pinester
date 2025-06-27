using System.Windows.Media.Imaging;
using System.Windows;

namespace Pinester
{
    public partial class ImageViewer : Window
    {
        public ImageViewer(BitmapImage image, string fileName = "", string additionalInfo = "")
        {
            InitializeComponent();
            FullImage.Source = image;

            // Combine file name and additional info
            FileNameText.Text = $"{fileName}\n{additionalInfo}";

            InitializeComponent();
            FullImage.Source = image;
            FileNameText.Text = fileName;

            // Size window to fit image with proper padding
            double padding = 40;
            double maxWidth = SystemParameters.WorkArea.Width;
            double maxHeight = SystemParameters.WorkArea.Height;

            Width = Math.Min(image.PixelWidth + padding, maxWidth);
            Height = Math.Min(image.PixelHeight + padding + 30, maxHeight); // +30 for title bar

            // Center the image in the window
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}