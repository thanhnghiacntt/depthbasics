using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class Test
    {
        public static bool SaveData(string FileName, byte[] Data)
        {
            try
            {
                BinaryWriter Writer = new BinaryWriter(File.OpenWrite(FileName));       
                Writer.Write(Data);
                Writer.Flush();
                Writer.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public static void test()
        {
            // Sample data
            //   The following is simple auto association function
            //   where each input correspond to its own class. This
            //   problem should be easily solved by a Linear kernel.

            // Sample input data
            double[][] inputs =
            {
                new double[] { 0 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
            };

            // Output for each of the inputs
            int[] outputs = { 0, 3, 1, 2 };


            // Create a new Linear kernel
            IKernel kernel = new Linear();

            // Create a new Multi-class Support Vector Machine with one input,
            //  using the linear kernel and for four disjoint classes.
            var machine = new MulticlassSupportVectorMachine(inputs[0].Length, kernel, outputs.Length);

            // Create the Multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            // Run the learning algorithm
            double error = teacher.Run(); // output should be 0

            // Compute the decision output for one of the input vectors
            int decision = machine.Compute(new double[] { 3 }); // result should be 3
        }
    }
}
