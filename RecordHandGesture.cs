using Accord.MachineLearning.VectorMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class RecordHandGesture
    {
        public String[] result = { "kxd", "A", "B", "C", "D", "Đ", "E", "G", "H", "I", "K",
                                     "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "X", "Y" };
        public MulticlassSupportVectorMachine ksvm;
        public RecordHandGesture()
        {
            ksvm = MulticlassSupportVectorMachine.Load(Properties.Resources.PathTraining);
        }
        public int getOutput(double[] input)
        {
            // output classifier
            return ksvm.Compute(input, MulticlassComputeMethod.Voting);
        }
        public int getOutput()
        {
            double[] input = new double[12];
            return getOutput(input);
        }
        public String getCharector()
        {
            int output = getOutput();
            return result[output];
        }
        public String getCharector(byte[] input)
        {
          //  Calculation.generateValidMatrix();
            int output = getOutput();
            return result[output];
        }

        public String getCharector(List<Hand> hands)
        {
            //  Calculation.generateValidMatrix();
            int output = getOutput();
            return result[output];
        }

        public string getCharector(Frame frame)
        {
            // Chỉ xử lý tại đây và trả về một ký tự.
            bool [][] bo = Calculation.generateValidMatrix(frame.width,frame.height,frame.frame);
            List<Hand> hands = Calculation.localizeHands(bo);
            String rs = Calculation.getOutput(hands, result, ksvm);
            return rs;
        }
    }
}
