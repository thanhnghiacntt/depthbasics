using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    public class ThreeNextFrame
    {
        private static int MAX_SECOND_NEXT_FRAME = 16;
        private static int T1 = 16;
        private static int T2 = 48;
        private static int MIN_POINT_LIGHT = 64;
        private Frame frame1;
        private Frame frame2;
        private Frame frame3;
        private int width;
        private int height;
        private bool flg;
        public ThreeNextFrame()
        {
            this.flg = false;
        }
        public void setWidthHeight(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public void add(byte[]pixcel)
        {
            if(frame1 == null){
                frame1 = Frame.copy(pixcel, width, height);
            }
            else
            {
                if (frame2 == null)
                {
                    frame2 = Frame.copy(pixcel, width, height);
                }
                else
                {
                    if(frame3 == null){
                        frame3 = Frame.copy(pixcel, width, height);
                    }
                    else
                    {
                        frame1 = frame2;
                        frame2 = frame3;
                        frame3 = Frame.copy(pixcel, width, height);
                    }
                }
            }
        }
        public bool isMainFrameChecks()
        {
            if (frame1 == null || frame2 == null || frame3 == null)
            {
                return false;
            }
            if (equals(frame1, frame2) && equals(frame2, frame3))
            {
                if (frame1.count < MIN_POINT_LIGHT || flg)
                {
                    return false;
                }
                flg = true;
                return true;
            }
            else
            {
                flg = false;
            }
            // Điểm tối
            if (flg && frame1.count < MIN_POINT_LIGHT)
            {
                flg = false;
            }
            return false;
        }
        public bool isMainFrameCheck()
        {
            if (frame1 == null || frame2 == null || frame3 == null)
            {
                return false;
            }
            if (equals(frame1, frame2, T1) && equals(frame2, frame3, T1) && flg == false)
            {
                //Điểm tối
                if (frame1.count < MIN_POINT_LIGHT)
                {
                    return false;
                }
                flg = true;
                return true;
            }
            else
            {
                if (!equals(frame1, frame2, T2) && !equals(frame1, frame2, T2))
                {
                    flg = false;
                }
            }
            
            return false;
        }
        public bool isSpace()
        {
            if (frame1 == null || frame2 == null || frame3 == null)
            {
                return true;
            }
            if (frame1.count < MIN_POINT_LIGHT)
            {
                return true;
            }
            return false;
        }
        private bool equals(Frame f1, Frame f2){
            return equals(f1, f2, MAX_SECOND_NEXT_FRAME);
        }
        private bool equals(Frame f1, Frame f2, int compare)
        {
            int temp = Math.Abs(f1.count - f2.count);
            if (temp < compare)
            {
                return true;
            }
            return false;
        }
        private bool equalsCorrect(Frame f1, Frame f2)
        {
            return equalsApproximately(f1, f2, 0);
        }
        private bool equalsApproximately(Frame f1, Frame f2, int count)
        {
            int temp = 0;
            int length = f1.frame.Length;
            for (int i = 0; i < length; i++)
            {
                if(f1.frame[i] != f2.frame[i]){
                    temp++;
                }
                if(temp>count){
                    return false;
                }
            }
            return true;
        }
        public Frame getMainFrame()
        {
            return this.frame1;
        }
    }
}
