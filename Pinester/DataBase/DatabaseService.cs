using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration; // Для App.config
using System.IO;
using System.Windows; // Для MessageBox (можно заменить на логирование или проброс исключений)
using System.Windows.Media.Imaging;
using Pinester.Models;


namespace Pinester.DataBase
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=pinester";

        //public DatabaseService()
        //{
        //    if (string.IsNullOrEmpty(_connectionString))
        //    {
        //        // Запасной вариант или ошибка, если строка подключения не найдена
        //        // Для простоты примера, используем жестко заданную строку, но это плохая практика для реальных приложений
        //        _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=pinester";
        //        MessageBox.Show("Connection string not found in App.config. Using default (edit DatabaseService.cs).", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        /// <summary>
        /// Загружает все изображения из базы данных, включая их бинарные данные,
        /// и создает для каждого BitmapImage.
        /// Каждому изображению присваивается AssignedName.
        /// </summary>
        /// <returns>Список объектов ImageInfo или null в случае ошибки.</returns>
        public List<ImageInfo> GetAllImages()
        {
            var images = new List<ImageInfo>();
            int counter = 1; // Для присвоения AssignedName = "Image_X"

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    // Загружаем все необходимые поля, включая image_data
                    // Сортируем по ID, чтобы AssignedName был консистентным
                    using (var cmd = new NpgsqlCommand("SELECT id, file_name, image_data FROM images ORDER BY id", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var imageInfo = new ImageInfo
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FileName = reader.GetString(reader.GetOrdinal("file_name")),
                                ImageData = (byte[])reader.GetValue(reader.GetOrdinal("image_data")),
                                AssignedName = $"Image_{counter++}" // Присваиваем имя
                            };

                            // Создаем BitmapImage из ImageData
                            if (imageInfo.ImageData != null && imageInfo.ImageData.Length > 0)
                            {
                                BitmapImage bitmap = new BitmapImage();
                                using (MemoryStream stream = new MemoryStream(imageInfo.ImageData))
                                {
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Важно для освобождения потока
                                    bitmap.StreamSource = stream;
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // Рекомендуется для BitmapImage, которые используются в коллекциях или из других потоков
                                }
                                imageInfo.ImageSource = bitmap;
                            }
                            images.Add(imageInfo);
                        }
                    }
                }
                return images;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images from database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; // или throw; или вернуть пустой список в зависимости от стратегии обработки ошибок
            }
        }

        public void InsertImage(string fileName, byte[] imageData)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO images (file_name, image_data) VALUES (@file_name, @image_data)", conn))
                    {
                        cmd.Parameters.AddWithValue("@file_name", fileName);
                        cmd.Parameters.AddWithValue("@image_data", imageData);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting image into database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public DateTime GetPictureInfoDate(ImageInfo imageInfo)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand($"SELECT uploaded_at from images where id = '{imageInfo.Id}'", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        dt = reader.GetDateTime(reader.GetOrdinal("uploaded_at"));
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error getting image info from Data Base: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }
    }
}
