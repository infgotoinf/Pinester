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
using System.Windows.Shapes;

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
            double maxWidth = SystemParameters.WorkArea.Width * 0.9;
            double maxHeight = SystemParameters.WorkArea.Height * 0.9;

            Width = Math.Min(image.PixelWidth + padding, maxWidth);
            Height = Math.Min(image.PixelHeight + padding + 30, maxHeight); // +30 for title bar

            // Center the image in the window
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}