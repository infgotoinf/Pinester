using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        private static string resourses_directory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Resources\\";
        private Random rand = new Random();
        private string[] files = Directory.GetFiles(resourses_directory);
        public ICommand ImageTest => new RelayCommand(_ => AddPinterestImage());
        public ObservableCollection<BitmapImage> ImageCollection { get; } = new ObservableCollection<BitmapImage>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ImageContainer.ItemsSource = ImageCollection;
        }

        private string imagePath;
        private void AddPinterestImage()
        {
            // Create image source
            var bitmap = new BitmapImage();
            bitmap.BeginInit();

            // Use absolute path - replace with your actual image paths
            string imagePath = files[rand.Next(files.Length)];

            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.DecodePixelWidth = 180;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            ImageCollection.Add(bitmap);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image image && image.Parent is Grid parentGrid)
            {
                // Get current width from parent container
                double currentWidth = parentGrid.ActualWidth;

                // Create new clip matching current width
                var clip = new RectangleGeometry
                {
                    RadiusX = 10,
                    RadiusY = 10,
                    Rect = new Rect(0, 0, currentWidth, image.ActualHeight)
                };
                clip.Freeze();
                image.Clip = clip;
            }
        }
    }
}