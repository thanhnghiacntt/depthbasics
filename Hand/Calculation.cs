using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    public class Calculation
    {
        private static int PLAYER_INDEX_BITMASK_WIDTH = 3;
        private static KinectSetting Setting = new KinectSetting();
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
        public static int countPixcel(bool [][] image, int width, int height)
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
        private bool[][] generateValidMatrix(short[][] frame, int[] distance)
        {
            int width = frame.Length;
            int height = frame[0].Length;
            // Create the matrix. The size depends on the margins
            var x1 = (int)(width * Setting.MarginLeftPerc / 100.0f);
            var x2 = (int)(width - x1);
            var y1 = (int)(height * Setting.MarginTopPerc / 100.0f);
            var y2 = (int)(height - y1);
            var near = new bool[y2 - y1][];
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
            var margin = (int)(min + Setting.NearSpacePerc * (max - min));
            int index = 0;
            if (Setting.AbsoluteSpace != -1) margin = min + Setting.AbsoluteSpace;
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
            if (Setting.SmoothingIterations > 0)
            {
                near = dilate(near, Setting.SmoothingIterations);
                near = erode(near, Setting.SmoothingIterations);
            }

            // Mark as not valid the borders of the matrix to improve the efficiency in some methods
            // First row
            for (int j = 0; j < near[0].Length; ++j)
                near[0][j] = false;

            // Last row
            var m = near.Length - 1;
            for (int j = 0; j < near[0].Length; ++j)
                near[m][j] = false;

            // First column
            for (int i = 0; i < near.Length; ++i)
                near[i][0] = false;

            // Last column
            m = near[0].Length - 1;
            foreach (bool[] t in near)
                t[m] = false;

            return near;
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

        /*
       * This function calcute the border of a closed figure starting in one of the contour points.
       * The turtle algorithm is used.
       */

        private static List<Point> calculateFrontier(ref bool[][] valid, Point start, ref bool[][] contour)
        {
            var list = new List<Point>();
            var last = new Point(-1, -1);
            var current = new Point(start);
            int dir = 0;

            do
            {
                if (valid[current.X][current.Y])
                {
                    dir = (dir + 1) % 4;
                    if (current != last)
                    {
                        list.Add(new Point(current.X, current.Y));
                        last = new Point(current);
                        contour[current.X][current.Y] = false;
                    }
                }
                else
                {
                    dir = (dir + 4 - 1) % 4;
                }

                switch (dir)
                {
                    case 0:
                        current.X += 1;
                        break; // Down
                    case 1:
                        current.Y += 1;
                        break; // Right
                    case 2:
                        current.X -= 1;
                        break; // Up
                    case 3:
                        current.Y -= 1;
                        break; // Left
                }
            } while (current != start);

            return list;
        }
        private static List<Hand> localizeHands(bool[][] valid)
        {
            int i, j, k;

            var hands = new List<Hand>();

            var insidePoints = new List<Point>();
            var contourPoints = new List<Point>();

            var contour = new bool[valid.Length][];
            for (i = 0; i < valid.Length; ++i)
            {
                contour[i] = new bool[valid[0].Length];
            }

            // Divide points in contour and inside points
            for (i = 1; i < valid.Length - 1; ++i)
            {
                for (j = 1; j < valid[i].Length - 1; ++j)
                {
                    if (valid[i][j])
                    {
                        // Count the number of valid adjacent points
                        var count = numValidPixelAdjacent(ref i, ref j, ref valid);

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

            // Create the sorted contour list, using the turtle algorithm
            for (i = 0; i < contourPoints.Count; ++i)
            {
                var hand = new Hand();

                // If it is a possible start point
                if (contour[contourPoints[i].X][contourPoints[i].Y])
                {
                    // Calculate the contour
                    hand.Contour = calculateFrontier(ref valid, contourPoints[i], ref contour);

                    // Check if the contour is big enough to be a hand
                    if (hand.Contour.Count / (contourPoints.Count * 1.0f) > 0.20f && hand.Contour.Count > Setting.K)
                    {
                        // Calculate the container box
                        hand.calculateContainerBox(Setting.ContainerBoxReduction);

                        // Add the hand to the list
                        hands.Add(hand);
                    }

                    // Don't look for more hands, if we reach the limit
                    if (hands.Count >= Setting.MaxTrackedHands)
                    {
                        break;
                    }
                }
            }

            // Allocate the inside points to the correct hand using its container box

            //List<int> belongingHands = new List<int>();
            for (i = 0; i < insidePoints.Count; ++i)
            {
                for (j = 0; j < hands.Count; ++j)
                {
                    if (hands[j].isPointInsideContainerBox(insidePoints[i]))
                    {
                        hands[j].Inside.Add(insidePoints[i]);
                        //belongingHands.Add(j);
                    }
                }

                // A point can only belong to one hand, if not we don't take that point into account
                /*if (belongingHands.Count == 1)
                {
                    hands[belongingHands.ElementAt(0)].inside.Add(insidePoints[i]);
                }
                belongingHands.Clear();*/
            }

            // Find the center of the palm
            float max;

            for (i = 0; i < hands.Count; ++i)
            {
                max = float.MinValue;
                for (j = 0; j < hands[i].Inside.Count; j += Setting.FindCenterInsideJump)
                {
                    var min = float.MaxValue;
                    for (k = 0; k < hands[i].Contour.Count; k += Setting.FindCenterInsideJump)
                    {
                        var distance = Point.distanceEuclidean(hands[i].Inside[j], hands[i].Contour[k]);

                        if (!hands[i].isCircleInsideContainerBox(hands[i].Inside[j], distance))
                            continue;

                        if (distance < min)
                            min = distance;

                        if (min < max)
                            break;
                    }

                    if (max < min && min != float.MaxValue)
                    {
                        max = min;
                        hands[i].Palm = hands[i].Inside[j];
                    }
                }
            }

            // Find the fingertips
            Point p1, p2, p3, pAux, r1, r2;
            int size;
            double angle;
            int jump;

            for (i = 0; i < hands.Count; ++i)
            {
                // Check if there is a point at the beginning to avoid checking the last ones of the list
                max = hands[i].Contour.Count;
                size = hands[i].Contour.Count;
                jump = (int)(size * Setting.FingertipFindJumpPerc);
                for (j = 0; j < Setting.K; j += 1)
                {
                    p1 = hands[i].Contour[(j - Setting.K + size) % size];
                    p2 = hands[i].Contour[j];
                    p3 = hands[i].Contour[(j + Setting.K) % size];
                    r1 = p1 - p2;
                    r2 = p3 - p2;

                    angle = Point.angle(r1, r2);

                    if (angle > 0 && angle < Setting.Theta)
                    {
                        pAux = p3 + ((p1 - p3) / 2);
                        if (Point.distanceEuclideanSquared(pAux, hands[i].Palm) >
                            Point.distanceEuclideanSquared(hands[i].Contour[j], hands[i].Palm))
                            continue;

                        hands[i].Fingertips.Add(hands[i].Contour[j]);
                        max = hands[i].Contour.Count + j - jump;
                        max = Math.Min(max, hands[i].Contour.Count);
                        j += jump;
                        break;
                    }
                }

                // Continue with the rest of the points
                for (; j < max; j += Setting.FindFingertipsJump)
                {
                    p1 = hands[i].Contour[(j - Setting.K + size) % size];
                    p2 = hands[i].Contour[j];
                    p3 = hands[i].Contour[(j + Setting.K) % size];
                    r1 = p1 - p2;
                    r2 = p3 - p2;

                    angle = Point.angle(r1, r2);

                    if (angle > 0 && angle < Setting.Theta)
                    {
                        pAux = p3 + ((p1 - p3) / 2);
                        if (Point.distanceEuclideanSquared(pAux, hands[i].Palm) >
                            Point.distanceEuclideanSquared(hands[i].Contour[j], hands[i].Palm))
                            continue;

                        hands[i].Fingertips.Add(hands[i].Contour[j]);
                        j += jump;
                    }
                }
            }

            return hands;
        }
              /*
         * This function calcute the border of a closed figure starting in one of the contour points.
         * The turtle algorithm is used.
         */

        private List<Point> CalculateFrontier(ref bool[][] valid, Point start, ref bool[][] contour)
        {
            var list = new List<Point>();
            var last = new Point(-1, -1);
            var current = new Point(start);
            int dir = 0;

            do
            {
                if (valid[current.X][current.Y])
                {
                    dir = (dir + 1) % 4;
                    if (current != last)
                    {
                        list.Add(new Point(current.X, current.Y));
                        last = new Point(current);
                        contour[current.X][current.Y] = false;
                    }
                }
                else
                {
                    dir = (dir + 4 - 1) % 4;
                }

                switch (dir)
                {
                    case 0:
                        current.X += 1;
                        break; // Down
                    case 1:
                        current.Y += 1;
                        break; // Right
                    case 2:
                        current.X -= 1;
                        break; // Up
                    case 3:
                        current.Y -= 1;
                        break; // Left
                }
            } while (current != start);

            return list;
        }
    }
}
