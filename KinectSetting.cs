using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class KinectSetting
    {

        public float marginLeftPerc { get; set; }
        public float marginRightPerc { get; set; }
        public float marginTopPerc { get; set; }
        public float marginBotPerc { get; set; }
        public float nearSpacePerc { get; set; }
        public int absoluteSpace { get; set; }
        public int findCenterContourJump { get; set; }
        public int findCenterInsideJump { get; set; }
        public double fingertipFindJumpPerc { get; set; }

        public int findFingertipsJump { get; set; }
        public double theta { get; set; }
        public int k { get; set; }

        public int maxTrackedHands { get; set; }
        public int smoothingIterations { get; set; }

        public KinectSetting()
        {
            setDefault();
        }
        public int ScreenSize
        {
            get
            {
                return -1;
            }
        }

        public int ScreenWidth
        {
            get
            {
                return -1;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return -1;
            }
        }

        private void setDefault()
        {

            smoothingIterations = 1;
            nearSpacePerc = 0.1f;
            absoluteSpace = 15; // 700 optimal
            // Margins applied to the original size image (Percentaje)
            marginLeftPerc = 0;
            marginRightPerc = 0;
            marginTopPerc = 0;
            marginBotPerc =0;
            // jump to find a center point
            findCenterContourJump = 10;//8
            findCenterInsideJump = 10;
            // jump ti find a fingertips
            findFingertipsJump = 2;//2
            fingertipFindJumpPerc = 0.1f;//0.1
            // angle to determine fingertip
            theta = 55 * (Math.PI / 180);
            // to  create angle from i
            k = 20;
            //number hands tracking
            maxTrackedHands = 1;
        }
    }
}
