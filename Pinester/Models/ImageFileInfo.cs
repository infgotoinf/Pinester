using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinester.Models
{
    internal struct ImageFileInfo
    {
        public InfoColor MainColor { get; set; }

        public DateTime Date {  get; set; }

        public int bytesFileSize { get; set; }

    }
}
