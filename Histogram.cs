using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class Histogram
    {
        public static int[] getHistograme(byte[] input, int length)
        {
            int[] result = new int[256];
            for (int i = 0; i < 256; i++)
            {
                result[i] = 0;
            }
            for (int i = 0; i < length; i++)
            {
                result[input[i]]++;
            }
            return result;
        }
    }
}
