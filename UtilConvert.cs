using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class UtilConvert
    {
        public UtilConvert()
        {

        }
   
        public static unsafe int[][] arrayToTable(ushort* array, int width, int height)
        {
            int[][] result = new int[width][];
            for (int i = 0; i < width; i++)
            {
                result[i] = new int[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i][j] = array[i * height + j];
                }
            }
            return result;
        }
        public static byte[] tableToArray(byte[][]by, int width, int height)
        {
            byte[] result = new byte[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i * height + j] = by[i][j];
                }
            }
            return result;
        }
        public static int[] tableToArray(int[][] matrix, int width, int height)
        {
            int[] result = new int[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i * height + j] = matrix[i][j];
                }
            }
            return result;
        }
        public static byte[] convertArrayIntToByte(int[] array)
        {
            byte[] result = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (byte)array[i];
            }
            return result;
        }
        public static int[] convertArrayIntToByte(byte[] array)
        {
            int[] result = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }
            return result;
        }
        /**
         * f là ma trân 2 chiều kích thước phải lớn hơn 3x3, h là ma trận kích thướt 3x3 
         */
        public static byte[][] imFilter(byte[][]f, byte[][]h)
        {
            int width = f.Length;
            int height = f[0].Length;
            byte[][] result = copyMatrix(f, width, height);
            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < height-1; j++)
                {
                    result[i][j] =(byte) (f[i][j] - f[i-1][j-1]*h[0][0]-f[i-1][j]*h[0][1]-f[i-1][j+1]*h[0][2]
                            - f[i][j - 1] * h[1][0] - f[i][j] * h[1][1] - f[i][j + 1] * h[1][2]
                            - f[i+1][j - 1] * h[2][0] - f[i+1][j] * h[2][1] - f[i+1][j + 1] * h[2][2]);
                }
            }

            return result;
        }
        public static int[][] imFilter(int[][] f, int[][] h)
        {
            int width = f.Length;
            int height = f[0].Length;
            int[][] result = new int[f.Length][];
            for (int i = 0; i < width; i++)
            {
                result[i] = new int[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i > 0 && j > 0 && i < width - 1 && j < height - 1)
                    {
                        result[i][j] = (f[i][j] - f[i - 1][j - 1] * h[0][0] - f[i - 1][j] * h[0][1] - f[i - 1][j + 1] * h[0][2]
                            - f[i][j - 1] * h[1][0] - f[i][j] * h[1][1] - f[i][j + 1] * h[1][2]
                            - f[i + 1][j - 1] * h[2][0] - f[i + 1][j] * h[2][1] - f[i + 1][j + 1] * h[2][2]);
                    }
                    else
                    {
                        result[i][j] = f[i][j];
                    }

                }
            }

            return result;
        }
        public static ushort[][] imFilter(ushort[][] f, ushort[][] h)
        {
            int width = f.Length;
            int height = f[0].Length;
            ushort[][] result = new ushort[f.Length][];
            for (ushort i = 0; i < width; i++)
            {
                result[i] = new ushort[height];
            }
            for (ushort i = 0; i < width; i++)
            {
                for (ushort j = 0; j < height; j++)
                {
                    if (i > 0 && j > 0 && i < width - 1 && j < height - 1)
                    {
                        result[i][j] = (ushort)(f[i][j] - f[i - 1][j - 1] * h[0][0] - f[i - 1][j] * h[0][1] - f[i - 1][j + 1] * h[0][2]
                            - f[i][j - 1] * h[1][0] - f[i][j] * h[1][1] - f[i][j + 1] * h[1][2]
                            - f[i + 1][j - 1] * h[2][0] - f[i + 1][j] * h[2][1] - f[i + 1][j + 1] * h[2][2]);
                    }
                    else
                    {
                        result[i][j] = f[i][j];
                    }

                }
            }

            return result;
        }
        public static byte[][] filterMedian(byte[][] imgs, int width, int height)
        {
            byte[][] img = copyMatrix(imgs, width, height);
            byte[] mask = new byte[9];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        mask[0] = img[i - 1][j - 1];
                    }
                    else
                    {
                        mask[0] = 0;
                    }

                    if (j - 1 >= 0 && i + 1 < width)
                    {
                        mask[1] = img[i + 1][j - 1];
                    }
                    else
                        mask[1] = 0;

                    if (j - 1 >= 0)
                    {
                        mask[2] = img[i][j - 1];
                    }
                    else
                        mask[2] = 0;

                    if (i + 1 < width)
                    {
                        mask[3] = img[i + 1][j];
                    }
                    else
                        mask[3] = 0;

                    if (i - 1 >= 0)
                    {
                        mask[4] = img[i - 1][j];
                    }
                    else
                        mask[4] = 0;

                    if (i - 1 >= 0 && j + 1 < height)
                    {
                        mask[5] = img[i - 1][j + 1];
                    }
                    else
                        mask[5] = 0;

                    if (j + 1 < height)
                    {
                        mask[6] = img[i][j + 1];
                    }
                    else
                        mask[6] = 0;


                    if (i + 1 < width && j + 1 < height)
                    {
                        mask[7] = img[i + 1][j + 1];
                    }
                    else
                        mask[7] = 0;
                    Array.Sort(mask);
                    byte mid = mask[4];
                    img[i][j] = mid;
                }
            }
            return img;
        }
        /**
         * h=[1,1,1]^-1 
         **/
        public static int[][] imErorion(int[][]img)
        {
            int width = img.Length;
            int height = img[0].Length;
            int[][] result = copyMatrix(img, width, height);
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (img[i - 1][j] > 0 && img[i][j] > 0 && img[i + 1][j] > 0)
                    {
                        continue;
                    }
                    else
                    {
                        result[i][j] = 0;
                    }
                }
            }
            return result;
        }
        /**   1, 1, 1
        * h=[ 1, 1, 1 ]
         *    1, 1, 1
        **/
        public static int[][] imErorion3X3(int[][] img)
        {
            int width = img.Length;
            int height = img[0].Length;
            int[][] result = copyMatrix(img, width, height);
            bool flg = true;
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height -1; j++)
                {
                    flg = false;
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            if(img[i+1-m][j+1-n] == 0){
                                flg = true;
                                result[i][j] = 0;
                                break;
                            }
                        }
                        if(flg){
                            break;
                        }
                    }
                }
            }
            return result;
        }
        public static int[][] imHitMiss(int[][] img, int[][] h)
        {
            int width = img.Length;
            int height = img[0].Length;
            int[][] result = copyMatrix(img, width, height);
            bool hit = true;
            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < height-1; j++)
                {
                    hit = true;
                    if(img[i][j] > 0){
                        if (h[0][0] > 0)
                        {
                            if (hit && img[i - 1][j - 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i - 1][j - 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[0][1] > 0)
                        {
                            if (hit && img[i - 1][j] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i - 1][j] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[0][2] > 0)
                        {
                            if (hit && img[i - 1][j + 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i - 1][j + 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[1][0] > 0)
                        {
                            if (hit && img[i][j - 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i][j - 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[1][1] > 0)
                        {
                            if (hit && img[i][j] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i][j] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[1][2] > 0)
                        {
                            if (hit && img[i][j + 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i][j + 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[2][0] > 0)
                        {
                            if (hit && img[i + 1][j - 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i + 1][j - 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[2][1] > 0)
                        {
                            if (hit && img[i + 1][j] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i + 1][j] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        if (h[2][2] > 0)
                        {
                            if (hit && img[i + 1][j + 1] > 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (hit && img[i + 1][j + 1] == 0)
                            {
                                hit = true;
                            }
                            else
                            {
                                result[i][j] = 0;
                                continue;
                            }
                        }
                        
                    }
                  
                }
            }
            return result;
        }
        public static unsafe ushort[] filterMedian(ushort* img, int width, int height)
        {
            ushort[] imgs = copyArray(img,width,height);
            ushort[] mask = new ushort[9];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        mask[0] = imgs[(i - 1)*height+j - 1];
                    }
                    else
                    {
                        mask[0] = 0;
                    }

                    if (j - 1 >= 0 && i + 1 < width)
                    {
                        mask[1] = imgs[(i + 1)*height+j - 1];
                    }
                    else
                        mask[1] = 0;

                    if (j - 1 >= 0)
                    {
                        mask[2] = imgs[i*height+j - 1];
                    }
                    else
                        mask[2] = 0;

                    if (i + 1 < width)
                    {
                        mask[3] = imgs[(i + 1)*height+j];
                    }
                    else
                        mask[3] = 0;

                    if (i - 1 >= 0)
                    {
                        mask[4] = imgs[(i - 1)*height+j];
                    }
                    else
                        mask[4] = 0;

                    if (i - 1 >= 0 && j + 1 < height)
                    {
                        mask[5] = imgs[(i - 1)*height+j + 1];
                    }
                    else
                        mask[5] = 0;

                    if (j + 1 < height)
                    {
                        mask[6] = imgs[i*height+j + 1];
                    }
                    else
                        mask[6] = 0;


                    if (i + 1 < width && j + 1 < height)
                    {
                        mask[7] = imgs[(i + 1)*height+j + 1];
                    }
                    else
                        mask[7] = 0;
                    Array.Sort(mask);
                    ushort mid = mask[4];
                    imgs[i * height + j] = mid;
                }
            }
            return imgs;
        }
      
        public static int[][] copyMatrix(int[][] matrix, int width, int height)
        {
            int[][] result = new int[width][];
            for (int i = 0; i < width; i++)
            {
                result[i] = new int[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i][j] = matrix[i][j];
                }
            }
            return result;
        }
        public static byte[][] copyMatrix(byte[][] matrix, int width, int height)
        {
            byte[][] result = new byte[width][];
            for (int i = 0; i < width; i++)
            {
                result[i] = new byte[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i][j] = matrix[i][j];
                }
            }
            return result;
        }
        public static ushort[][] copyMatrix(ushort[][] matrix, int width, int height)
        {
            ushort[][] result = new ushort[width][];
            for (int i = 0; i < width; i++)
            {
                result[i] = new ushort[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i][j] = matrix[i][j];
                }
            }
            return result;
        }
        public static unsafe ushort[] copyArray(ushort* matrix, int width, int height)
        {
            ushort[] array = new ushort[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    array[i * height + j] = matrix[i * height + j]; 
                }
            }
            return array;
        }
        public static byte[] copyArray(byte[]input,int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = input[i];
            }
            return result;
        }
        public static int[][] createMatrix()
        {
            int[][] h = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                h[i] = new int[3];
            }
            h[0][0] = -1;
            h[0][1] = -1;
            h[0][2] = -1;
            h[1][0] = -1;
            h[1][1] = 8;
            h[1][2] = -1;
            h[2][0] = -1;
            h[2][1] = -1;
            h[2][2] = -1;
            return h;
        }
        public static int[][] createMatrixHitMiss()
        {
            int[][] h = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                h[i] = new int[3];
            }
            h[0][0] = 0;
            h[0][1] = 1;
            h[0][2] = 0;
            h[1][0] = 0;
            h[1][1] = 1;
            h[1][2] = 0;
            h[2][0] = 0;
            h[2][1] = 0;
            h[2][2] = 0;
            return h;
        }
        public static byte[] copy(byte[]by)
        {
            return (byte[])by.Clone();
        }
        public static byte[] processNhieuHatTieu(byte[]input, int width, int height)
        {
            byte[] output = copy(input);
            for (int i = 1; i < width-1; i++)
            {
                for (int j = 1; j < height-1; j++)
                {
                    int len1 = getLenght(i-1, j-1, width, height);
                    int len2 = getLenght(i-1, j, width, height);
                    int len3 = getLenght(i-1, j+1, width, height);
                    int len4 = getLenght(i, j-1, width, height);
                    int len5 = getLenght(i, j, width, height);
                    int len6 = getLenght(i, j+1, width, height);
                    int len7 = getLenght(i+1, j-1, width, height);
                    int len8 = getLenght(i+1, j, width, height);
                    int len9 = getLenght(i+1, j+1, width, height);
                    byte[] by = { input[len1], input[len2], input[len3],
                                input[len4],input[len5],input[len6],
                                input[len7],input[len8],input[len9]};
                    Array.Sort(by);
                    output[len5] = by[4];
                }
            }
            return output;
        }
        private static int getLenght(int x, int y, int width, int height)
        {
            return x * height + y;
        }
    }
}
