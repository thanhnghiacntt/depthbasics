using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    public class Hand
    {
        // define a hand
        public Point palm { get; set; }
        public List<Point> fingertips { get; set; }
        public List<Point> contour { get; set; }
        public List<Point> hole { get; set; }
        public List<Point> inside { get; set; }
        public int leftUpperCornerX { get; set; }
        public int leftUpperCornerY { get; set; }
        public int righDownCornerX { get; set; }
        public int righDownCornerY { get; set; }
        public float r { get; set; }

        public Hand()
        {
            palm = new Point(-1, -1);

            fingertips = new List<Point>(5);
            contour = new List<Point>();
            inside = new List<Point>();
            hole = new List<Point>();
            leftUpperCornerX = leftUpperCornerY = int.MaxValue;
            righDownCornerX = righDownCornerY = int.MinValue;
        }

        //point inside box
        public bool isInsideContainerBox(Point p)
        {
            if (p.X < righDownCornerX && p.X > leftUpperCornerX
                && p.Y > leftUpperCornerY && p.Y < righDownCornerY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // calculate a box
        public bool calculateContainerBox()
        {
            if (contour != null && contour.Count > 0)
            {
                for (int j = 0; j < contour.Count; ++j)
                {
                    if (leftUpperCornerX > contour[j].X)
                        leftUpperCornerX = contour[j].X;

                    if (righDownCornerX < contour[j].X)
                        righDownCornerX = contour[j].X;

                    if (leftUpperCornerY > contour[j].Y)
                        leftUpperCornerY = contour[j].Y;

                    if (righDownCornerY < contour[j].Y)
                        righDownCornerY = contour[j].Y;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
