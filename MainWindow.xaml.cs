//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.DepthBasics
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Size of the RGB pixel in the bitmap
        /// </summary>
        private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MapDepthToByte = 8000 / 256;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader = null;
        /// <summary>
        /// Reader for color frames
        /// </summary>
        private ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription = null;
        /// <summary>
        /// Description of the data contained in the color frame
        /// </summary>
        private FrameDescription colorFrameDescription = null;
        /// <summary>
        /// Reader for depth/color/body index frames
        /// </summary>
        private MultiSourceFrameReader multiFrameSourceReader = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthBitmap = null;
        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthAutoBitmap = null;
        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap colorBitmap = null;

        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] depthPixels = null;
        private byte[] depthAutoPixels = null;
        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] colorPixels = null;
        private int widthDepth = 0;
        private int heightDepth = 0;
        private int widthColor = 0;
        private int heightColor = 0;
        private bool flgSpace = false;
        private int countString = 0;
        /// <summary>
        /// The size in bytes of the bitmap back buffer
        /// </summary>
        private uint colorBitmapBackBufferSize = 0;
        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
       
        private CoordinateMapper coordinateMapper = null;
        /// <summary>
        /// Intermediate storage for the color to depth mapping
        /// </summary>
        private DepthSpacePoint[] colorMappedToDepthPoints = null;
        private uint depthFrameDataSize = 0;
        private ThreeNextFrame threeNextFrame = new ThreeNextFrame();
        private RecordHandGesture recordHandGesture = new RecordHandGesture();
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            Test.test();
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();

            this.multiFrameSourceReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color | FrameSourceTypes.BodyIndex);

            this.multiFrameSourceReader.MultiSourceFrameArrived += this.MutilReaderSourceFrameArrived;
            // open the reader for the depth frames
            this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();
            // opne the reader for the color frames
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get FrameDescription from DepthFrameSource
            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
            // get FrameDescription from ColorFrameSource
            this.colorFrameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;

            this.widthColor = colorFrameDescription.Width;
            this.heightColor = colorFrameDescription.Height;
            this.widthDepth = depthFrameDescription.Width;
            this.heightDepth = depthFrameDescription.Height;
            this.threeNextFrame.setWidthHeight(this.widthDepth, this.heightDepth);
            this.colorMappedToDepthPoints = new DepthSpacePoint[widthColor * heightColor];

            // allocate space to put the pixels being received and converted
            this.depthPixels = new byte[widthDepth* heightDepth];
            this.depthAutoPixels = new byte[widthDepth * heightDepth];
            this.colorPixels = new byte[widthColor * heightColor];

            // create the bitmap to display
            this.depthBitmap = new WriteableBitmap(this.widthDepth, this.heightDepth, 96.0, 96.0, PixelFormats.Gray8, null);
            this.depthAutoBitmap = new WriteableBitmap(this.widthDepth, this.heightDepth, 96.0, 96.0, PixelFormats.Gray8, null);
            this.colorBitmap = new WriteableBitmap(this.widthColor, this.heightColor, 96.0, 96.0, PixelFormats.Bgra32, null);
            // Calculate the WriteableBitmap back buffer size
            this.colorBitmapBackBufferSize = (uint)((this.colorBitmap.BackBufferStride * (this.colorBitmap.PixelHeight - 1)) + (this.colorBitmap.PixelWidth * this.bytesPerPixel));


            // open the sensor
            this.kinectSensor.Open();


            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();
        }


        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSourceDepth
        {
            get
            {
                return this.depthBitmap;
            }
        }

        private Label label;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSourceDepthAuto
        {
            get
            {
                return this.depthAutoBitmap;
            }
        }


        public ImageSource ImageSourceColor
        {
            get
            {
                return this.colorBitmap;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.multiFrameSourceReader != null)
            {
                // MultiSourceFrameReder is IDisposable
                this.multiFrameSourceReader.Dispose();
                this.multiFrameSourceReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void DepthFrameReader(DepthFrame depthFrame)
        {
            ushort minDepths = depthFrame.DepthMinReliableDistance;
            ushort maxDepths = depthFrame.DepthMaxReliableDistance;

            if (depthFrame != null)
            {
                // the fastest way to process the body index data is to directly access 
                // the underlying buffer
                using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                {
                    // verify data and write the color data to the display bitmap
                    if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / depthFrameDescription.BytesPerPixel)) &&
                        (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                    {
                        ushort maxDepth = 1000;
                        ushort minDepth = depthFrame.DepthMinReliableDistance;
                        this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, minDepth, maxDepth);
                        this.RenderDepthPixels();
                    }
                }

            }

        }

        private void writeFrame()
        {
            DateTime localDate = DateTime.Now;
            if(count()<500){
                return;
            }
            //ScreenshotButton_Click();
            //string name = DateTime.Now.ToString("mmssfff") + (".bmp");
           // string full_path = "D:\\LuanVan\\" + name;
           // Bitmap bitMap = DepthToBitmap(depthPixels);
           // Bitmap bitMap = imageFromArray(depthPixels);
           // bitMap.Save(full_path , ImageFormat.Bmp);
          
        }
        private int count()
        {
            int count = 0;
            for (int i = 0; i < depthAutoPixels.Length; i++)
            {
                if (depthAutoPixels[i] > 1)
                {
                    count++;
                }
            }
            return count;
        }
        private void printImageDepth()
        {
            if (this.depthBitmap != null)
            {
                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new PngBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(this.depthBitmap));

                string name = DateTime.Now.ToString("mmssfff") + (".png");
                string path = "D:\\LuanVan\\Test\\" + name;

                // write the new file to disk
                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException)
                {
                    
                }
            }
        }
        private bool inNhiPhan = true;
        private void inNhiPhanHinh(string lecter)
        {
            if (inNhiPhan)
            {
                Test.SaveData("D:\\LuanVan\\Test_" + lecter+".txt",this.depthAutoPixels);
                inNhiPhan = false;
            }
            
        }

        private void printKeyFrame(String lecter)
        {
            if (this.depthAutoBitmap != null)
            {
                //inNhiPhanHinh(lecter);
                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new PngBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(this.depthBitmap));

                string name = DateTime.Now.ToString("mmssfff") + "_" + lecter + (".png");
                string path = "D:\\LuanVan\\Test\\BaiBao\\" + name;

                // write the new file to disk
                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException)
                {

                }
            }
        }
        private Bitmap imageFromArray(byte[] array)
        {

            int width = depthBitmap.PixelWidth;

            int height = depthBitmap.PixelHeight;

            Bitmap b = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, width, height),

            ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            int stride = bmData.Stride;

            System.IntPtr scan0 = bmData.Scan0;

            Marshal.Copy(array, 0, scan0, array.Length);

            b.UnlockBits(bmData);

            //Rote the image 90 degrees

            b.RotateFlip(RotateFlipType.Rotate90FlipX);

            return b;

        }
        Bitmap DepthToBitmap(byte[] frame)
        {
            Bitmap bmap = new Bitmap(depthBitmap.PixelWidth, depthBitmap.PixelHeight, 
                System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            BitmapData bmapdata = bmap.LockBits(new Rectangle(0, 0, depthBitmap.PixelWidth, depthBitmap.PixelHeight), ImageLockMode.WriteOnly, bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(frame, 0, ptr, frame.Length);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit valuel
            ushort* frameData = (ushort*)depthFrameData;
            ushort* frameDatas = (ushort*)depthFrameData;
           // ushort[]frameData = UtilConvert.filterMedian(frameDatas, depthFrameDescription.Width, depthFrameDescription.Height);
            //int[][] matrix = UtilConvert.arrayToTable(frameDatas, depthFrameDescription.Width, depthFrameDescription.Height);
           // int[][] h = UtilConvert.createMatrixHitMiss();
            //int[][] frameDataTemp = UtilConvert.imErorion3X3(matrix);
           // int[] frameData = UtilConvert.tableToArray(frameDataTemp, depthFrameDescription.Width, depthFrameDescription.Height);
           // ushort[]frameData = UtilConvert.filterMedian(frameDatas, depthFrameDescription.Width, depthFrameDescription.Height);
            this.depthFrameDataSize = depthFrameDataSize;
            int min = int.MaxValue;
            int max = int.MinValue;

            int length = (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel);
           
            for (int i = 0; i < length; ++i)
            {
                // Get the depth for this pixel
                int depth = frameData[i];
                int autoDepth = frameDatas[i];

                byte temp = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
                byte auto = (byte)(autoDepth >= minDepth && autoDepth <= maxDepth ? (autoDepth / MapDepthToByte) : 0);
            
                if(temp > max){
                    max = temp;
                }
      
                if(temp != 0 && temp < min){
                    min = temp;
                }
                //depthAutoPixels[i] = auto;
                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = temp;
            }
           // min = max - 5;
           // max = min + 5;
           // byte[][] by = UtilConvert.arrayToTable(this.depthPixels, this.depthFrameDescription.Width, this.depthFrameDescription.Height);
            //this.depthPixels = UtilConvert.processNhieuHatTieu(this.depthPixels, widthDepth, heightDepth);
            for (int i = 0; i < length; ++i)
            {
                if (depthPixels[i] < min)
                {
                    depthPixels[i] = 0;
                }
                if (max > 0)
                {
                    this.depthPixels[i] = (byte)((this.depthPixels[i] * 255) / max);
                }
            }
            //printKeyFrame("TEST");
            threeNextFrame.add(this.depthPixels);
            if (threeNextFrame.isMainFrameCheck())
            {
                depthAutoPixels = threeNextFrame.getMainFrame().frame;
                String str = recordHandGesture.getCharector(threeNextFrame.getMainFrame());
                
                //printKeyFrame(str+"chinh");
                if (str.Equals("kxd"))
                {
                    this.label.Content = this.label.Content + "_";
                }
                else
                {
                    if (countString > 40)
                    {
                        countString = 0;
                        this.label.Content = this.label.Content + "\n";
                    }
                    countString++;
                    this.label.Content = this.label.Content + str;
                    flgSpace = true;
                }

            }
            else
            {
                if (threeNextFrame.isSpace() && flgSpace)
                {
                    countString++;
                    this.label.Content = this.label.Content + "_";
                    Console.WriteLine(this.label.Content);
                    flgSpace = false;
                }
            }
        }
        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            this.label = sender as Label;
            this.label.Content = "";
        }
        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.depthPixels,
                this.depthBitmap.PixelWidth,
                0);
            this.depthAutoBitmap.WritePixels(
               new Int32Rect(0, 0, this.depthAutoBitmap.PixelWidth, this.depthAutoBitmap.PixelHeight),
               this.depthAutoPixels,
               this.depthAutoBitmap.PixelWidth,
               0);
            //writeFrame();
        }
        private void ColorFrameReader(ColorFrame colorFrame)
        {
            this.colorBitmap.Lock();
            bool isBitmapLocked = true;
            colorFrame.CopyConvertedFrameDataToIntPtr(this.colorBitmap.BackBuffer, this.colorBitmapBackBufferSize, ColorImageFormat.Bgra);
            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
            colorFrame.Dispose();
            colorFrame = null;
            if (isBitmapLocked)
            {
                this.colorBitmap.Unlock();
            }
        }
        private void MutilReaderSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            DepthFrame depthFrame = null;
            ColorFrame colorFrame = null;
            BodyIndexFrame bodyIndexFrame = null;
            bool isBitmapLocked = false;
            int depthWidth = 0;
            int depthHeight = 0;

            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            // If the Frame has expired by the time we process this event, return.
            if (multiSourceFrame == null)
            {
                return;
            }

            // We use a try/finally to ensure that we clean up before we exit the function.  
            // This includes calling Dispose on any Frame objects that we may have and unlocking the bitmap back buffer.
            try
            {
                depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame();
                colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame();
                bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame();

                // If any frame has expired by the time we process this event, return.
                // The "finally" statement will Dispose any that are not null.
                if ((depthFrame == null) || (colorFrame == null) || (bodyIndexFrame == null))
                {
                    return;
                }
                // Process Depth
                FrameDescription depthFrameDescription = depthFrame.FrameDescription;

                depthWidth = depthFrameDescription.Width;
                depthHeight = depthFrameDescription.Height;

                // Access the depth frame data directly via LockImageBuffer to avoid making a copy
                using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
                {
                    this.coordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
                        depthFrameData.UnderlyingBuffer,
                        depthFrameData.Size,
                        this.colorMappedToDepthPoints);
                }

                DepthFrameReader(depthFrame);
                ColorFrameReader(colorFrame);
            }
            finally
            {
                if (isBitmapLocked)
                {
                    this.colorBitmap.Unlock();
                }

                if (depthFrame != null)
                {
                    depthFrame.Dispose();
                }

                if (colorFrame != null)
                {
                    colorFrame.Dispose();
                }

                if (bodyIndexFrame != null)
                {
                    bodyIndexFrame.Dispose();
                }
            }
        }

    }
}
