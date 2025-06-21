using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinester.Models.Interfaces;
using ColorThiefDotNet;
using System.Drawing;

namespace Pinester.Models
{
    internal class PictureMainColor: IPictureInfo
    {
        public static Dictionary<string, int> GetInfo(Bitmap picture)
        {
            ColorThief thief = new ColorThief();
            QuantizedColor dominantColor = thief.GetColor(picture);

            Dictionary<string, int> color = new Dictionary<string, int>();

            color.Add("R", dominantColor.Color.R);
            color.Add("G", dominantColor.Color.G);
            color.Add("B", dominantColor.Color.B);
            

            return color;
        }
    }
}
