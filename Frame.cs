using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    public class Frame
    {
        public byte[] frame { get; set; }
        public int count { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        private Frame(byte[] frame, int count)
        {
            this.frame = frame;
            this.count = count;
        }
        private Frame(byte[] frame, int width, int height)
        {
            this.frame = frame;
            this.count = Calculation.countPixcel(frame);
            this.width = width;
            this.height = height;
        }
        public static Frame copy(byte[]frame, int width, int height){
            byte[]fra = UtilConvert.copy(frame);
            Frame fr = new Frame(fra, width, height);
            return fr;
        }
    }
}
