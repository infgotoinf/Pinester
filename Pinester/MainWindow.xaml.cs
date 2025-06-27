using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Pinester.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _dbService;
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };

        public ICommand AddFilesCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ObservableCollection<ImageInfo> ImageCollection { get; } = new ObservableCollection<ImageInfo>();

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            AddFilesCommand = new RelayCommand(ExecuteAddFiles);
            AddFolderCommand = new RelayCommand(ExecuteAddFolder);

            this.DataContext = this;

            LoadImagesFromDb();
        }

        private void LoadImagesFromDb()
        {
            try
            {
                var images = _dbService.GetAllImages();
                ImageCollection.Clear();
                if (images != null)
                {
                    foreach (var image in images)
                    {
                        if (image.ImageSource == null && image.ImageData != null)
                        {
                            image.ImageSource = CreateBitmapImageFromData(image.ImageData);
                        }
                        ImageCollection.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений из БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddFiles(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображения",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var imageInfos = new List<ImageInfo>();
                foreach (string filePath in openFileDialog.FileNames)
                {
                    imageInfos.Add(new ImageInfo
                    {
                        FileName = Path.GetFileName(filePath),
                        ImageData = File.ReadAllBytes(filePath)
                    });
                }

                if (imageInfos.Any())
                {
                    SaveChangesToDb(imageInfos);
                }
            }
        }

        private void ExecuteAddFolder(object parameter)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Выберите папку с изображениями"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folderPath = dialog.FileName;
                var filePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                         .Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

                var imageInfos = new List<ImageInfo>();
                foreach (var filePath in filePaths)
                {
                    try
                    {
                        imageInfos.Add(new ImageInfo
                        {
                            FileName = Path.GetFileName(filePath),
                            ImageData = File.ReadAllBytes(filePath)
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось прочитать файл: {Path.GetFileName(filePath)}\nОшибка: {ex.Message}", "Ошибка чтения файла", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                if (imageInfos.Any())
                {
                    SaveChangesToDb(imageInfos);
                }
                else
                {
                    MessageBox.Show("В выбранной папке не найдено подходящих изображений.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SaveChangesToDb(List<ImageInfo> images)
        {
            try
            {
                _dbService.InsertMultipleImages(images);
                MessageBox.Show($"Успешно добавлено {images.Count} изображений.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadImagesFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении изображений в базу данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage CreateBitmapImageFromData(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void ImageBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is ImageInfo imageInfo)
            {
                if (imageInfo.ImageSource == null && imageInfo.ImageData != null)
                {
                    imageInfo.ImageSource = CreateBitmapImageFromData(imageInfo.ImageData);
                }

                if (imageInfo.ImageSource != null)
                {
                    var viewer = new ImageViewer(imageInfo.ImageSource, imageInfo.FileName);
                    viewer.Show();
                }
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
    }
}