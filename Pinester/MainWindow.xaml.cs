using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        public ICommand ImageTest => new RelayCommand(_ => AddPinterestImage());
        public ObservableCollection<BitmapImage> ImageCollection { get; } = new ObservableCollection<BitmapImage>();
        private bool switcher = true;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ImageContainer.ItemsSource = ImageCollection; // Connect to collection
        }

        private void AddPinterestImage()
        {
            try
            {
                // Create image source
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string imagePath = switcher
            ? "pack://application:,,,/Resourses/images.png"
            : "pack://application:,,,/Resourses/images (1).png";

                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);

                // Enable variable height sizing
                bitmap.DecodePixelWidth = 180; // Fixed width
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Prevents locking files
                bitmap.EndInit();
                bitmap.Freeze(); // Makes thread-safe

                // Add to collection (will automatically appear in UI)
                ImageCollection.Add(bitmap);

                switcher = !switcher;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        private void aPicture_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}