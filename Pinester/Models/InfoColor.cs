using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinester.Models
{
    internal struct InfoColor
    {
        public int R {  get;}
        public int G {  get;}
        public int B {  get;}

        public InfoColor (int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

    }
}
