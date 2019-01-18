using Accord.MachineLearning.VectorMachines;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    public class Calculation
    {
        private static int PLAYER_INDEX_BITMASK_WIDTH = 3;
        private static KinectSetting settings = new KinectSetting();

        private const double Rad2Deg = 180.0 / Math.PI;
        private const double Deg2Rad = Math.PI / 180.0;
        public static int[] generateDistances(short[] depth)
        {
            // Calculate the real distance
            int[] distance = new int[depth.Length];
            for (int i = 0; i < distance.Length; ++i)
            {
                distance[i] = depth[i] >> PLAYER_INDEX_BITMASK_WIDTH;
            }

            return distance;
        }
        public static int countPixcel(bool[][] image, int width, int height)
        {
            int result = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (image[i][j])
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public static int countPixcel(byte[][] image, int width, int height)
        {
            int result = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (image[i][j] > 0)
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public static int countPixcel(byte[] pix)
        {
            int result = 0;
            int length = pix.Length;
            for (int i = 0; i < length; i++)
            {
                if (pix[i] > 0)
                {
                    result++;
                }
            }
            return result;
        }
        public static bool[][] erode(bool[][] image, int it)
        {
            // Matrix to store the dilated image
            var erodeImage = new bool[image.Length][];
            for (int i = 0; i < image.Length; ++i)
            {
                erodeImage[i] = new bool[image[i].Length];
            }

            // Distances matrix
            int[][] distance = manhattanDistanceMatrix(image, false);

            // Dilate the image
            for (int i = 0; i < image.Length; i++)
            {
                for (int j = 0; j < image[i].Length; j++)
                {
                    erodeImage[i][j] = ((distance[i][j] > it) ? true : false);
                }
            }

            return erodeImage;
        }
        public static bool[][] dilate(bool[][] image, int it)
        {
            // Matrix to store the dilated image
            var dilateImage = new bool[image.Length][];
            for (int i = 0; i < image.Length; ++i)
            {
                dilateImage[i] = new bool[image[i].Length];
            }

            // Distances matrix
            int[][] distance = manhattanDistanceMatrix(image, true);

            // Dilate the image
            for (int i = 0; i < image.Length; i++)
            {
                for (int j = 0; j < image[i].Length; j++)
                {
                    dilateImage[i][j] = ((distance[i][j] <= it) ? true : false);
                }
            }

            return dilateImage;
        }
        public static int[][] manhattanDistanceMatrix(bool[][] image, bool zeroDistanceValue)
        {
            var distanceMatrix = new int[image.Length][];
            for (int i = 0; i < distanceMatrix.Length; ++i)
            {
                distanceMatrix[i] = new int[image[i].Length];
            }

            // traverse from top left to bottom right
            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                for (int j = 0; j < distanceMatrix[i].Length; j++)
                {
                    if ((image[i][j] && zeroDistanceValue) || (!image[i][j] && !zeroDistanceValue))
                    {
                        // first pass and pixel was on, it gets a zero
                        distanceMatrix[i][j] = 0;
                    }
                    else
                    {
                        // pixel was off
                        // It is at most the sum of the lengths of the array
                        // away from a pixel that is on
                        distanceMatrix[i][j] = image.Length + image[i].Length;
                        // or one more than the pixel to the north
                        if (i > 0) distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i - 1][j] + 1);
                        // or one more than the pixel to the west
                        if (j > 0) distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i][j - 1] + 1);
                    }
                }
            }
            // traverse from bottom right to top left
            for (int i = distanceMatrix.Length - 1; i >= 0; i--)
            {
                for (int j = distanceMatrix[i].Length - 1; j >= 0; j--)
                {
                    // either what we had on the first pass
                    // or one more than the pixel to the south
                    if (i + 1 < distanceMatrix.Length)
                        distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i + 1][j] + 1);
                    // or one more than the pixel to the east
                    if (j + 1 < distanceMatrix[i].Length)
                        distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i][j + 1] + 1);
                }
            }

            return distanceMatrix;
        }
        // Generate a representable image of the valid matrix
        public static byte[] generateDepthImage(bool[][] near)
        {
            // Image pixels
            var pixels = new byte[near.Length * near[0].Length * 4];
            int width = near[0].Length;

            for (int i = 1; i < near.Length - 1; ++i)
            {
                for (int j = 1; j < near[i].Length - 1; ++j)
                {
                    if (near[i][j])
                    {
                        if (!near[i + 1][j] || !near[i - 1][j]
                            || !near[i][j + 1] || !near[i][j - 1]) // Is border
                        {
                            pixels[(i * width + j) * 4 + 0] = 255;
                            pixels[(i * width + j) * 4 + 1] = 0;
                            pixels[(i * width + j) * 4 + 2] = 0;
                            pixels[(i * width + j) * 4 + 3] = 0;
                        }
                        else
                        {
                            pixels[(i * width + j) * 4 + 0] = 255;
                            pixels[(i * width + j) * 4 + 1] = 255;
                            pixels[(i * width + j) * 4 + 2] = 255;
                            pixels[(i * width + j) * 4 + 3] = 0;
                        }
                    }
                }
            }

            return pixels;
        }
        public static bool[][] generateValidMatrix(int width, int height, int[] distance)
        {

            // Create the matrix. The size depends on the margins
            int x1 = (int)(width * settings.marginLeftPerc / 100.0f);
            int x2 = (int)(width * (1 - (settings.marginRightPerc / 100.0f)));
            int y1 = (int)(height * settings.marginTopPerc / 100.0f);
            int y2 = (int)(height * (1 - (settings.marginBotPerc / 100.0f)));
            bool[][] near = new bool[y2 - y1][];
            for (int i = 0; i < near.Length; ++i)
            {
                near[i] = new bool[x2 - x1];
            }

            // Calculate max and min distance
            int max = int.MinValue, min = int.MaxValue;

            for (int k = 0; k < distance.Length; ++k)
            {
                if (distance[k] > max) max = distance[k];
                if (distance[k] < min && distance[k] != -1) min = distance[k];
            }

            // Decide if it is near or not
            int margin = (int)(min + settings.nearSpacePerc * (max - min));
            int index = 0;
            if (settings.absoluteSpace != -1) margin = min + settings.absoluteSpace;
            for (int i = 0; i < near.Length; ++i)
            {
                for (int j = 0; j < near[i].Length; ++j)
                {
                    index = width * (i + y1) + (j + x1);
                    if (distance[index] <= margin && distance[index] != -1)
                    {
                        near[i][j] = true;
                    }
                    else
                    {
                        near[i][j] = false;
                    }
                }
            }

            // Dilate and erode the image to get smoother figures
            //if (settings.smoothingIterations > 0)
            //{
            //    near = dilate(near, settings.smoothingIterations);
            //    near = erode(near, settings.smoothingIterations);
            //}

            // Mark as not valid the borders of the matrix to improve the efficiency in some methods
            int m;
            // First row
            for (int j = 0; j < near[0].Length; ++j)
                near[0][j] = false;

            // Last row
            m = near.Length - 1;
            for (int j = 0; j < near[0].Length; ++j)
                near[m][j] = false;

            // First column
            for (int i = 0; i < near.Length; ++i)
                near[i][0] = false;

            // Last column
            m = near[0].Length - 1;
            for (int i = 0; i < near.Length; ++i)
                near[i][m] = false;

            return near;
        }
   
        private static bool[][] generateValidMatrix(short[][] frame, int[] distance)
        {
            int width = frame.Length;
            int height = frame[0].Length;
            return generateValidMatrix(width, height, distance);
        }
        /*
           * Counts the number of adjacent valid points without taking into account the diagonals
        */

        private static int numValidPixelAdjacent(ref int i, ref int j, ref bool[][] valid)
        {
            int count = 0;

            if (valid[i + 1][j]) ++count;
            if (valid[i - 1][j]) ++count;
            if (valid[i][j + 1]) ++count;
            if (valid[i][j - 1]) ++count;
            //if (valid[i + 1][j + 1]) ++count;
            //if (valid[i + 1][j - 1]) ++count;
            //if (valid[i - 1][j + 1]) ++count;
            //if (valid[i - 1][j - 1]) ++count;

            return count;
        }

        public static List<Hand> localizeHands(bool[][] valid)
        {
            int i, j, k;

            List<Hand> hands = new List<Hand>();

            List<Point> insidePoints = new List<Point>();
            List<Point> contourPoints = new List<Point>();

            bool[][] contour = new bool[valid.Length][];
            for (i = 0; i < valid.Length; ++i)
            {
                contour[i] = new bool[valid[0].Length];
            }


            // Divide points in contour and inside points
            // Tính toán các điểm mà 4 lân cận có điểm đen
            // Tính toán đường biên
            // contourPoints là điểm đường biên
            int count = 0;
            for (i = 1; i < valid.Length - 1; ++i)
            {
                for (j = 1; j < valid[i].Length - 1; ++j)
                {
                    if (valid[i][j])
                    {
                        count = numValidPixelAdjacent(ref i, ref j, ref valid);

                        if (count == 4) // Inside
                        {
                            insidePoints.Add(new Point(i, j));
                        }
                        else // Contour
                        {
                            contour[i][j] = true;
                            contourPoints.Add(new Point(i, j));
                        }

                    }
                }
            }

            int numberpoint = contourPoints.Count;
            // Create the sorted contour list, using the turtle algorithm
            for (i = 0; i < numberpoint; ++i)
            {

                Hand hand = new Hand();

                // If it is a possible start point
                // Nếu nó là điểm có thể bắt đầu
                if (contour[contourPoints[i].X][contourPoints[i].Y])
                {

                    // Calculate the contour
                    // Tính toán các đường viền
                    hand.contour = CalculateFrontier(ref valid, contourPoints[i], ref contour);
                    // Check if the contour is big enough to be a hand
                    // Kiểm tra các đường viền đủ lớn để là 1 bàn tay
                    if (hand.contour.Count / (contourPoints.Count * 1.0f) > 0.20f
                        && hand.contour.Count > settings.k)
                    {

                        // Calculate the container box
                        // Tính toán nội dụng bên trong ban tay
                        hand.calculateContainerBox();
                        for (int m = 0; m < numberpoint; ++m)
                        {
                            if (hand.isInsideContainerBox(contourPoints[m]) && !compare(contourPoints[m], hand.contour))
                            {

                                List<Point> check = CalculateFrontier(ref valid, contourPoints[m], ref contour);
                                if (check.Count > 25)
                                {
                                    hand.hole = check;
                                }

                            }
                        }

                        hands.Add(hand);
                    }

                    // Don't look for more hands, if we reach the limit
                    //Đừng tìm kiếm bàn tay nhiều hơn, nếu chúng ta đạt tới giới hạn
                    if (hands.Count >= settings.maxTrackedHands)
                    {
                        break;
                    }
                }

            }


            // Allocate the inside points to the correct hand
            //Phân bổ các điểm bên trong để các tay đúng
            for (i = 0; i < insidePoints.Count; ++i)
            {
                for (j = 0; j < hands.Count; ++j)
                {
                    if (hands[j].isInsideContainerBox(insidePoints[i]))
                    {
                        hands[j].inside.Add(insidePoints[i]);
                    }
                }
            }

            // Find the center of the palm
            // Tìm trung tâm của lòng bàn tay
            float min, max, distance, d1, d2;

            for (i = 0; i < hands.Count; ++i)
            {
                max = float.MinValue;
                for (j = 0; j < hands[i].inside.Count; j += settings.findCenterInsideJump)
                {
                    min = float.MaxValue;
                    for (k = 0; k < hands[i].contour.Count; k += settings.findCenterInsideJump)
                    {
                        d1 = (hands[i].inside[j].X - hands[i].contour[k].X);
                        d2 = (hands[i].inside[j].Y - hands[i].contour[k].Y);
                        distance = d1 * d1 + d2 * d2;
                        if (distance < min) min = distance;
                        if (min < max) break;
                    }
                    if (max < min)
                    {
                        max = min;
                        double dis = min;
                        hands[i].r = (float)Math.Sqrt(min);
                        hands[i].palm = hands[i].inside[j];
                    }
                }
            }

            // Find the fingertips
            //Tìm trong tầm tay
            Point p1, p3, pAux, r1, r2;
            Size p2;
            int size;
            double angle;
            int jump;

            for (i = 0; i < hands.Count; ++i)
            {
                // Check if there is a point at the beginning to avoid checking the last ones of the list
                // Kiểm tra nếu có một điểm ngay từ đầu để tránh việc kiểm tra cuối cùng của danh sách
                max = hands[i].contour.Count;
                size = hands[i].contour.Count;
                jump = (int)(size * settings.fingertipFindJumpPerc);
                //jump = 15;
                for (j = 0; j < settings.k; j += 1)
                {
                    p1 = hands[i].contour[(j - settings.k + size) % size];
                    p2 = new Size(hands[i].contour[j]);
                    p3 = hands[i].contour[(j + settings.k) % size];
                    r1 = Point.Subtract(p1, p2);
                    r2 = Point.Subtract(p3, p2);

                    angle = Math.Acos((r1.X * r2.X + r1.Y * r2.Y) /
                        (Math.Sqrt(r1.X * r1.X + r1.Y * r1.Y) * Math.Sqrt(r2.X * r2.X + r2.Y * r2.Y)));
                    if (angle > 0 && angle < settings.theta)
                    {
                        pAux = new Point(p1.X - p3.X, p1.Y - p3.Y);
                        pAux = new Point((int)(p3.X + pAux.X / 2.0), (int)(p3.Y + pAux.Y / 2.0));
                        if (distancePowTwo(pAux, hands[i].palm) >
                            distancePowTwo(hands[i].contour[j], hands[i].palm))
                            continue;
                        //int checkfinger = 0;
                        //if (j + 6 >= hands[i].contour.Count)
                        //{
                        //    checkfinger = j + 6 - hands[i].contour.Count;
                        //}
                        //else
                        //{
                        //    checkfinger = j;
                        //}
                        hands[i].fingertips.Add(hands[i].contour[j]);
                        max = hands[i].contour.Count + j - jump;
                        max = Math.Min(max, hands[i].contour.Count);
                        j += jump;
                        break;
                        //  }

                    }
                }

                // Continue with the rest of the points
                // Tiếp tục với phần còn lại của các điểm
                for (; j < max; j += settings.findFingertipsJump)
                {
                    p1 = hands[i].contour[(j - settings.k + size) % size];
                    p2 = new Size(hands[i].contour[j]);
                    p3 = hands[i].contour[(j + settings.k) % size];
                    r1 = Point.Subtract(p1, p2);
                    r2 = Point.Subtract(p3, p2);

                    angle = Math.Acos((r1.X * r2.X + r1.Y * r2.Y) /
                        (Math.Sqrt(r1.X * r1.X + r1.Y * r1.Y) * Math.Sqrt(r2.X * r2.X + r2.Y * r2.Y)));

                    if (angle > 0 && angle < settings.theta)
                    {
                        pAux = new Point(p1.X - p3.X, p1.Y - p3.Y);
                        pAux = new Point((int)(p3.X + pAux.X / 2.0), (int)(p3.Y + pAux.Y / 2.0));
                        if (distancePowTwo(pAux, hands[i].palm) >
                            distancePowTwo(hands[i].contour[j], hands[i].palm))
                            continue;
                        hands[i].fingertips.Add(hands[i].contour[j]);
                        j += jump;
                    }
                }
            }

            return hands;
        }
        private static double radiofinger1(Point p1, Point palm)
        {
            return (Math.Sqrt(distancePowTwo(p1, palm)));
        }
        private static double ratioBox(int x1, int y1, int x2, int y2)
        {

            return ((double)(x2 - x1) / (y2 - y1));
        }
        private static bool compare(Point p, List<Point> lp)
        {
            bool bl = false;
            for (int i = 0; i < lp.Count; ++i)
            {
                if (Point.Equals(p, lp[i]))
                {
                    bl = true;
                    break;
                }
            }
            return bl;
        }
        private static int distancePowTwo(Point p1, Point p2)
        {
            int dist;
            dist = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
            return dist;
        }

        internal static bool[][] generateValidMatrix(int width, int height, byte[] distance)
        {

            // Create the matrix. The size depends on the margins
            int x1 = (int)(width * settings.marginLeftPerc / 100.0f);
            int x2 = (int)(width * (1 - (settings.marginRightPerc / 100.0f)));
            int y1 = (int)(height * settings.marginTopPerc / 100.0f);
            int y2 = (int)(height * (1 - (settings.marginBotPerc / 100.0f)));
            bool[][] near = new bool[y2 - y1][];
            for (int i = 0; i < near.Length; ++i)
            {
                near[i] = new bool[x2 - x1];
            }

            // Calculate max and min distance
            byte max = byte.MinValue, min = byte.MaxValue;

            for (int k = 0; k < distance.Length; ++k)
            {
                if (distance[k] > max) max = distance[k];
                if (distance[k] < min && distance[k] != 0) min = distance[k];
            }

            // Decide if it is near or not
            int margin = (int)(min + settings.nearSpacePerc * (max - min));
            int index = 0;
            if (settings.absoluteSpace != -1) margin = min + settings.absoluteSpace;
            for (int i = 0; i < near.Length; ++i)
            {
                for (int j = 0; j < near[i].Length; ++j)
                {
                    index = width * (i + y1) + (j + x1);
                    if (distance[index] <= margin && distance[index] != 0)
                    {
                        near[i][j] = true;
                    }
                    else
                    {
                        near[i][j] = false;
                    }
                }
            }

            // Dilate and erode the image to get smoother figures
            //if (settings.smoothingIterations > 0)
            //{
            //    near = dilate(near, settings.smoothingIterations);
            //    near = erode(near, settings.smoothingIterations);
            //}

            // Mark as not valid the borders of the matrix to improve the efficiency in some methods
            int m;
            // First row
            for (int j = 0; j < near[0].Length; ++j)
                near[0][j] = false;

            // Last row
            m = near.Length - 1;
            for (int j = 0; j < near[0].Length; ++j)
                near[m][j] = false;

            // First column
            for (int i = 0; i < near.Length; ++i)
                near[i][0] = false;

            // Last column
            m = near[0].Length - 1;
            for (int i = 0; i < near.Length; ++i)
                near[i][m] = false;

            return near;
        }

        private static List<Point> CalculateFrontier(ref bool[][] valid, Point start, ref bool[][] contour)
        {
            List<Point> list = new List<Point>();
            Point last = new Point(-1, -1);

            Point current = start;
            int dir = 0;
            do
            {
                if (valid[current.X][current.Y])
                {
                    dir = (dir + 1) % 4;
                    if (current != last)
                    {
                        list.Add(current);
                        last = current;
                        contour[current.X][current.Y] = false;
                    }
                }
                else
                {
                    dir = (dir + 4 - 1) % 4;
                }
                switch (dir)
                {
                    case 0: current.X += 1; break; // Down
                    case 1: current.Y += 1; break; // Right
                    case 2: current.X -= 1; break; // Up
                    case 3: current.Y -= 1; break; // Left
                }
            } while (current != start);

            return list;
        }

        public static String getOutput(List<Hand> hands, String[] str, MulticlassSupportVectorMachine ksvm)
        {
            double[] input = new double[12];
            int output;
            for (int i = 0; i < hands.Count; ++i)
            {
                int countfingertop = 0;
                int countfingerbot = 0;
                for (int j = 0; j < hands[i].fingertips.Count; j++)
                {
                    if (hands[i].fingertips[j].X <= hands[i].palm.X)
                    {
                        countfingertop++;
                    }
                    else
                    {
                        countfingerbot++;
                    }
                }
                if (countfingertop >= 1)
                {
                    for (int j = 0; j < hands[i].fingertips.Count; j++)
                    {
                        if (quarter(hands[i], hands[i].fingertips[j]) == 3)
                        {
                            hands[i].fingertips.RemoveAt(j);
                        }
                    }
                }
                countfingertop = 0;
                countfingerbot = 0;
                for (int j = 0; j < hands[i].fingertips.Count; j++)
                {
                    if (hands[i].fingertips[j].X <= hands[i].palm.X)
                    {
                        countfingertop++;
                    }
                    else
                    {
                        countfingerbot++;
                    }
                }
                if (countfingerbot == 2)
                {
                    for (int j = 0; j < hands[i].fingertips.Count; j++)
                    {
                        if (hands[i].fingertips[j].X <= hands[i].palm.X)
                        {
                            hands[i].fingertips.RemoveAt(j);
                        }
                    }
                }
                //input data to recognize
                if (ratioBox(hands[i].leftUpperCornerX, hands[i].leftUpperCornerY, hands[i].righDownCornerX, hands[i].righDownCornerY) > 1)
                {
                    input[0] = 0;
                }
                else
                {
                    input[0] = 1;
                }
                input[1] = hands[i].fingertips.Count;
                if (input[1] == 0)
                {
                    input[2] = 0;
                    input[3] = 0;
                    input[4] = 0;
                    input[5] = 0;
                    input[6] = 0;
                    input[10] = 0;
                    input[11] = 0;
                }
                else
                {
                    input[2] = angle2Points(hands[i].fingertips[0], hands[i].palm);
                    input[3] = quarter(hands[i], hands[i].fingertips[0]);
                    if (input[1] == 1)
                    {
                        input[4] = 0;
                        input[5] = 0;
                        input[6] = 0;
                        input[11] = 0;
                        for (int k = 0; k < hands[i].contour.Count; ++k)
                        {
                            if (hands[i].contour[k].Equals(hands[i].fingertips[0]))
                            {
                                int pf, pl;
                                if (i - settings.k < 0)
                                {
                                    pf = hands[i].contour.Count - 1 + i - settings.k;
                                }
                                else
                                {
                                    pf = i - settings.k;
                                }
                                if (i + settings.k > hands[i].contour.Count - 1)
                                {
                                    pl = i + settings.k - hands[i].contour.Count + 1;
                                }
                                else
                                {
                                    pl = i + settings.k;
                                }
                                Point directPoint = new Point((hands[i].contour[pf].X + hands[i].contour[pl].X) / 2, (hands[i].contour[pf].Y + hands[i].contour[pl].Y) / 2);
                                input[10] = angle2Points(hands[i].contour[k], directPoint);
                            }
                        }
                    }
                    else
                    {

                        for (int k = 0; k < hands[i].contour.Count; ++k)
                        {
                            if (hands[i].contour[k].Equals(hands[i].fingertips[0]))
                            {
                                int pf, pl;
                                if (i - settings.k < 0)
                                {
                                    pf = hands[i].contour.Count - 1 + i - settings.k;
                                }
                                else
                                {
                                    pf = i - settings.k;
                                }
                                if (i + settings.k > hands[i].contour.Count - 1)
                                {
                                    pl = i + settings.k - hands[i].contour.Count + 1;
                                }
                                else
                                {
                                    pl = i + settings.k;
                                }
                                Point directPoint = new Point((hands[i].contour[pf].X + hands[i].contour[pl].X) / 2, (hands[i].contour[pf].Y + hands[i].contour[pl].Y) / 2);
                                input[10] = angle2Points(hands[i].contour[k], directPoint);
                            }

                            if (hands[i].contour[k].Equals(hands[i].fingertips[1]))
                            {
                                int pf, pl;
                                if (i - settings.k < 0)
                                {
                                    pf = hands[i].contour.Count - 1 + i - settings.k;
                                }
                                else
                                {
                                    pf = i - settings.k;
                                }
                                if (i + settings.k > hands[i].contour.Count - 1)
                                {
                                    pl = i + settings.k - hands[i].contour.Count + 1;
                                }
                                else
                                {
                                    pl = i + settings.k;
                                }
                                Point directPoint = new Point((hands[i].contour[pf].X + hands[i].contour[pl].X) / 2, (hands[i].contour[pf].Y + hands[i].contour[pl].Y) / 2);
                                input[11] = angle2Points(hands[i].contour[k], directPoint);
                            }
                        }
                        input[4] = angle2Points(hands[i].fingertips[1], hands[i].palm);
                        input[5] = quarter(hands[i], hands[i].fingertips[1]);
                        input[6] = angle3Points(hands[i].fingertips[0], hands[i].fingertips[1], hands[i].palm);
                    }
                }
                Point pt1 = new Point(hands[i].palm.X, hands[i].leftUpperCornerY);

                Point pt2 = new Point(hands[i].leftUpperCornerX, hands[i].palm.Y);

                Point pt3 = new Point(hands[i].righDownCornerX, hands[i].palm.Y);
                input[7] = radiofinger1(pt1, hands[i].palm);
                input[8] = radiofinger1(pt2, hands[i].palm);
                input[9] = radiofinger1(pt3, hands[i].palm);
                if (hands[i].hole.Count > 30)
                {
                    input[9] = 1;
                }
                else
                {
                    input[9] = 0;
                }
            }
            output = ksvm.Compute(input, MulticlassComputeMethod.Voting);
            return str[output];
        }

        private static int quarter(Hand hand, Point finger)
        {
            int check = 0;
            Point pUp = new Point(hand.leftUpperCornerX, hand.leftUpperCornerY);
            Point pDown = new Point(hand.righDownCornerX, hand.righDownCornerY);
            if (insidequarter(pUp, hand.palm, finger))
            {
                check = 1;
            }
            if (insidequarter(hand.palm, pDown, finger))
            {
                check = 3;
            }
            Point pUpRight = new Point(hand.palm.X, hand.leftUpperCornerY);
            Point pDownRight = new Point(hand.righDownCornerX, hand.palm.Y);

            if (insidequarter(pUpRight, pDownRight, finger))
            {
                check = 2;
            }

            Point pUpLeft = new Point(hand.leftUpperCornerX, hand.palm.Y);
            Point pDownLeft = new Point(hand.palm.X, hand.righDownCornerY);

            if (insidequarter(pUpLeft, pDownLeft, finger))
            {
                check = 4;
            }
            return check;
            //if
        }
        private static bool insidequarter(Point pUp, Point pDown, Point p)
        {
            bool check = false;
            if (pUp.X <= p.X && pUp.Y <= p.Y && pDown.X >= p.X && pDown.Y >= p.Y)
            {
                check = true;
            }
            return check;
        }
        private static double angle2Points(Point first, Point mid)
        {
            return Math.Atan2(-first.X + mid.X, -first.Y + mid.Y) * Rad2Deg;
        }
        private static double angle3Points(Point first, Point second, Point mid)
        {
            Size p1 = new Size(mid);
            Point rr1 = Point.Subtract(first, p1);
            Point rr2 = Point.Subtract(second, p1);
            double angle3 = Math.Acos((rr1.X * rr2.X + rr1.Y * rr2.Y) /
           (Math.Sqrt(rr1.X * rr1.X + rr1.Y * rr1.Y) * Math.Sqrt(rr2.X * rr2.X + rr2.Y * rr2.Y))) * Rad2Deg;
            return angle3;
        }


    }
}
