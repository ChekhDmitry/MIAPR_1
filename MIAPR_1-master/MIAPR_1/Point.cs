using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIAPR_1
{
    internal class Point
    {
        private System.Drawing.Point point;
        public int X
        {
            get
            {
                return point.X;
            }
            set
            {
                point.X = value;
            }
        }
        public int Y
        {
            get
            {
                return point.Y;
            }
            set
            {
                point.Y = value;
            }
        }

        public int classNum;
    }
}
