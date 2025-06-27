using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Pinester.DataBase;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        public ICommand UploadImageCommand => new RelayCommand(_ => UploadImage());
        public ICommand LoadImagesCommand => new RelayCommand(_ => LoadImages());
        public ObservableCollection<ImageInfo> ImageCollection { get; } = new ObservableCollection<ImageInfo>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ImageContainer.ItemsSource = ImageCollection;
        }

        private void UploadImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    byte[] imageData = File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);

                    var dbService = new DatabaseService();
                    dbService.InsertImage(fileName, imageData);

                    // Create ImageInfo object
                    var imageInfo = new ImageInfo
                    {
                        FileName = fileName,
                        ImageSource = new BitmapImage(new Uri(filePath))
                    };

                    ImageCollection.Add(imageInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadImages()
        {
            var dbService = new DatabaseService();
            var allImages = dbService.GetAllImages();

            if (allImages != null)
            {
                foreach (var image in allImages)
                {
                    ImageCollection.Add(image);
                }
            }
        }

        private void ImageBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is ImageInfo imageInfo)
            {
                var viewer = new ImageViewer(imageInfo.ImageSource, imageInfo.FileName);
                viewer.Show();
            }
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
    }
}