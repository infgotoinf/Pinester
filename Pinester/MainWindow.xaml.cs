using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Pinester.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Pinester
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _dbService;
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };

        public ICommand AddFilesCommand { get; }
        public ICommand AddFolderCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            // Привязываем команды к методам-обработчикам
            AddFilesCommand = new RelayCommand(ExecuteAddFiles);
            AddFolderCommand = new RelayCommand(ExecuteAddFolder);

            // Устанавливаем DataContext на само окно, чтобы XAML-привязки команд работали
            this.DataContext = this;

            // Загружаем изображения из БД при запуске приложения
            LoadImagesFromDb();
        }

        /// <summary>
        /// Загружает все изображения из базы данных и отображает их.
        /// </summary>
        private void LoadImagesFromDb()
        {
            try
            {
                var images = _dbService.GetAllImages();
                if (images != null)
                {
                    ImageContainer.ItemsSource = images;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load images: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик команды для добавления нескольких файлов.
        /// </summary>
        private void ExecuteAddFiles(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображения",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|All files (*.*)|*.*",
                Multiselect = true // Разрешаем выбор нескольких файлов
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

        /// <summary>
        /// Обработчик команды для добавления изображений из папки.
        /// </summary>
        private void ExecuteAddFolder(object parameter)
        {
            // Используем современный диалог выбора папки
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Выберите папку с изображениями"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folderPath = dialog.FileName;

                // Ищем все файлы в папке, которые являются изображениями
                var filePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                         .Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

                var imageInfos = new List<ImageInfo>();
                foreach (var filePath in filePaths)
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
                else
                {
                    MessageBox.Show("В выбранной папке не найдено подходящих изображений.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Общий метод для сохранения списка изображений в БД и обновления UI.
        /// </summary>
        private void SaveChangesToDb(List<ImageInfo> images)
        {
            try
            {
                _dbService.InsertMultipleImages(images);
                MessageBox.Show($"Успешно добавлено {images.Count} изображений.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Перезагружаем изображения, чтобы отобразить новые
                LoadImagesFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении изображений в базу данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
