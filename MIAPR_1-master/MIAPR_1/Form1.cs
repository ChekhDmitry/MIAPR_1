using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MIAPR_1
{
    public partial class Form1 : Form
    {
        public int classCount;
        public int dotCount;

        private const int DOT_SIZE = 2;
        private const int OUTER_ELLIPSE_SIZE = 10;
        private const int INNER_ELLIPSE_SIZE = 6;
        private const int TIMER_SIZE = 10;

        public Form1()
        {
            InitializeComponent();
        }

        private void Draw(Dictionary<Point, List<Point>> classes)
        {
            BufferedGraphicsContext context = new BufferedGraphicsContext();
            BufferedGraphics graphics = context.Allocate(CreateGraphics(), ClientRectangle);

            graphics.Graphics.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);
            Color[] colors = new Color[10] {
            Color.Yellow, Color.White, Color.Green, Color.Blue, Color.Red,
            Color.Firebrick, Color.Violet, Color.Salmon, Color.Orange, Color.DarkGray };
            for (int i = 0; i < classes.Count; i++)
            {
                Point center = classes.Keys.ElementAt(i);
                Brush brush = new SolidBrush(colors[center.classNum]);
                foreach (Point point in classes[center])
                {
                    graphics.Graphics.FillRectangle(brush, new Rectangle(point.X,point.Y, DOT_SIZE, DOT_SIZE));
                }
                brush = new SolidBrush(Color.White);
                graphics.Graphics.FillEllipse(brush, center.X - OUTER_ELLIPSE_SIZE / 2, center.Y - OUTER_ELLIPSE_SIZE / 2, OUTER_ELLIPSE_SIZE, OUTER_ELLIPSE_SIZE);
                brush = new SolidBrush(Color.Black);
                graphics.Graphics.FillEllipse(brush, center.X - INNER_ELLIPSE_SIZE / 2, center.Y - INNER_ELLIPSE_SIZE / 2, INNER_ELLIPSE_SIZE, INNER_ELLIPSE_SIZE);
            }
            graphics.Render();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DotProvider.GetClasses(10000, 10, Width, Height, Draw);
        }
    }
}
