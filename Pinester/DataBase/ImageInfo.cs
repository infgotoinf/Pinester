using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging; // Для BitmapImage

namespace Pinester.DataBase
{
    public class ImageInfo
    {
        public int Id { get; set; }                 // ID из базы данных
        public string FileName { get; set; }        // Оригинальное имя файла
        public string AssignedName { get; set; }    // Программно присвоенное имя (например, "Image_1")
        public byte[] ImageData { get; set; }       // Бинарные данные изображения (загружаем сразу)
        public BitmapImage ImageSource { get; set; } // Готовый BitmapImage для отображения в UI
    }
}

