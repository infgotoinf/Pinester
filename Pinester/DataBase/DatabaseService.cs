using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Pinester.Models;


namespace Pinester.DataBase
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=zxc;Database=postgres";

        /// <summary>
        /// Загружает все изображения из базы данных.
        /// </summary>
        public List<ImageInfo> GetAllImages()
        {
            var images = new List<ImageInfo>();
            int counter = 1;

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
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
                                AssignedName = $"Image_{counter++}"
                            };

                            if (imageInfo.ImageData != null && imageInfo.ImageData.Length > 0)
                            {
                                BitmapImage bitmap = new BitmapImage();
                                using (MemoryStream stream = new MemoryStream(imageInfo.ImageData))
                                {
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmap.StreamSource = stream;
                                    bitmap.EndInit();
                                    bitmap.Freeze();
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
                return null;
            }
        }

        /// <summary>
        /// Вставляет одно изображение.
        /// </summary>
        public void InsertImage(string fileName, byte[] imageData)
        {
            InsertMultipleImages(new List<ImageInfo>
            {
                new ImageInfo { FileName = fileName, ImageData = imageData }
            });
        }

        /// <summary>
        /// Вставляет несколько изображений в базу данных в одной транзакции.
        /// </summary>
        /// <param name="imagesToInsert">Коллекция объектов для вставки.</param>
        public void InsertMultipleImages(IEnumerable<ImageInfo> imagesToInsert)
        {
            if (!imagesToInsert.Any())
            {
                return;
            }

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO images (file_name, image_data) VALUES (@file_name, @image_data)", conn, transaction))
                        {
                            cmd.Parameters.Add("@file_name", NpgsqlTypes.NpgsqlDbType.Text);
                            cmd.Parameters.Add("@image_data", NpgsqlTypes.NpgsqlDbType.Bytea);

                            foreach (var image in imagesToInsert)
                            {
                                cmd.Parameters["@file_name"].Value = image.FileName;
                                cmd.Parameters["@image_data"].Value = image.ImageData;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error inserting multiple images: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw;
                    }
                }
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

