using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinester.Models.Interfaces;
using ColorThiefDotNet;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Pinester.ViewModels
{
    internal class PictureMainColor : IPictureInfo
    {
        public static Dictionary<string, int> GetInfo(BitmapImage imageSource)
        {
            Bitmap bmp = BitmapConverter.BitmapImage2Bitmap(imageSource);
            ColorThief thief = new ColorThief();
            QuantizedColor dominantColor = thief.GetColor(bmp);

            Dictionary<string, int> color = new Dictionary<string, int>();

            color.Add("R", dominantColor.Color.R);
            color.Add("G", dominantColor.Color.G);
            color.Add("B", dominantColor.Color.B);


            return color;
        }
    }
}
