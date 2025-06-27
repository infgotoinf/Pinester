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
        private readonly Random rand = new Random();
        public ICommand ImageTest => new RelayCommand(_ => AddPinterestImage());
        public ICommand UploadImageCommand => new RelayCommand(_ => UploadImage());
        public ObservableCollection<BitmapImage> ImageCollection { get; } = new ObservableCollection<BitmapImage>();

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

                    MessageBox.Show("Image uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddPinterestImage()
        {
            var dbService = new DatabaseService();
            var allImages = dbService.GetAllImages();

            if (allImages != null && allImages.Count > 0)
            {
                var randomImageInfo = allImages[rand.Next(allImages.Count)];
                if (randomImageInfo.ImageSource != null)
                {
                    ImageCollection.Add(randomImageInfo.ImageSource);
                }
                else
                {
                    MessageBox.Show("Selected image from database has no visual content.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No images found in the database.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}