using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinester.Models.Interfaces
{
    internal interface IPictureInfo
    {
        static abstract Dictionary<string, int> GetInfo(Bitmap picture);
    }
}
