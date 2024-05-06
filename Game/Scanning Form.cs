using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using ZXing.QrCode.Internal;
using ZXing;
using System.Drawing.Drawing2D;

namespace Game
{
    public partial class Scanning_Form : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;

        public Scanning_Form()
        {
            InitializeComponent();
            this.Load += Scanning_Form_Load;
        }

        private void start_Click(object sender, EventArgs e)
        {
          
               

                captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
             
                //captureDevice.SetCameraProperty(
                //      CameraControlProperty.Zoom,
                //      5,
                //      CameraControlFlags.Manual);

                captureDevice.NewFrame += CaptureDevice_NewFrame;
                captureDevice.Start();
                timer1.Start();

            
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //pictureBox1.Size = new Size(1600, 1900);

            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

            int newWidth = frame.Width;
            int newHeight = frame.Height;  
            // Create a new bitmap with the zoomed out dimensions
            Bitmap zoomedOutFrame = new Bitmap(newWidth, newHeight);
            //MessageBox.Show("Width : " + newWidth + "Height : " + newHeight);
            using (Graphics graphics = Graphics.FromImage(zoomedOutFrame))
            {
                // Set the interpolation mode to achieve better image quality
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Draw the zoomed out frame

                graphics.DrawImage(frame, new Rectangle(0, 0, newWidth, newHeight));
            }

            zoomedOutFrame.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = zoomedOutFrame;
        }

        private void Scanning_Form_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);

            cboDevice.SelectedIndex = 0;



        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(1600, 1900);

            if (pictureBox1.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                
                barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    txt_qr.Text = result.ToString();

                    if (captureDevice.IsRunning)
                    {
                        captureDevice.Stop();
                        captureDevice.SignalToStop();
                        captureDevice.WaitForStop();
                        captureDevice = null;
                    }
                    timer1.Stop();
                    pictureBox1.Image = null;
                    captureDevice = null;
                    
                }
            }
            }
    }
}
