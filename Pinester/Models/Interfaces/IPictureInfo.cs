using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pinester.Models.Interfaces
{
    internal interface IPictureInfo
    {
        static abstract Dictionary<string, int> GetInfo(BitmapImage imageSource);
    }
}
