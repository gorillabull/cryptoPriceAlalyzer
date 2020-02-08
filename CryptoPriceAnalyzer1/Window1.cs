using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using System.Drawing.Imaging;
namespace CryptoPriceAnalyzer1
{

    public partial class Window1 : Form
    {
        public Stream file;
        Bitmap image;
        public Window1()
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.DefaultExt = ".jpg";
            openFileDialog1.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            InitializeComponent();
        }

        private void Window1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void update1MinPrices(string price)
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text += price;
            });
            

        }

        public void clear1MinPrices()
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text = "";
            });
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult  = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                file = openFileDialog1.OpenFile();
                image = new Bitmap(file);
                pictureBox1.Image = image;
                trackBar1.Value = 0;
            }
            
        }

        private unsafe void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int blurval = trackBar1.Value;
            DateTime start = DateTime.Now;

            if (radioButton1.Checked==true)
            {
                image = gaussianFilter(image, blurval);
            }
            else
            {
                if (radioButton2.Checked==true)
                {
                    image = Sobel(image);
                }
                else
                {
                    if (radioButton3.Checked == true)
                    {

                    }
                }
            }
           

            pictureBox1.Image = image;
            TimeSpan duration = DateTime.Now - start;


            richTextBox1.Text = "Finished in " + duration.ToString() + " milliseconds." ;
        }


        private static double[] Create1DGaussianKernel(double sd)
        {
            // Normally software will compute the size of the kernel required to adequately capture the standard deviation of the gaussian. 2.5 x sd radius is plenty.
            int radius = (int)Math.Ceiling(sd * 2.5);

            double[] kernel = new double[radius * 2 + 1];
            int kernelPosition = 0;

            // You can see the formula I've used for the Gaussian function at http://en.wikipedia.org/wiki/Normal_distribution.
            double norm = 1 / Math.Sqrt(2 * Math.PI * sd * sd);

            for (int u = -radius; u <= radius; u++)
            {
                kernel[kernelPosition++] = norm * Math.Exp(-(u * u) / (2 * sd * sd));
            }

            return kernel;
        }

        private static unsafe Bitmap gaussianFilter(Bitmap image, int blurval)
        {
            //change the pixel format. 
            //image = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb);

            // This simple code is meant for 32bpp images, it could be adapted to handle other image types.
            // 32ARGB and 32RGB are the same, except in the latter the alpha byte is simply padding.
            if (image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppRgb)
            {
                return null;
            }

            int width = image.Width;
            int height = image.Height;

            // Gaussian blur is best done in two passes, so we need an intermediate image.
            // This doesn't have to be an actual System.Bitmap, any array will do.
            uint[] intermediateBuffer = new uint[width * height];

            // System.Bitmap includes SetPixel and GetPixel methods. These are very slow, so it's better to lock
            // the image and access the data directly. Locking prevents C# memory management moving it around.
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.PixelFormat);

            uint* ptr = (uint*)imageData.Scan0.ToPointer(); // An unsigned int pointer. This points to the image data in memory, each uint is one pixel ARGB
            int stride = imageData.Stride / 4; // Stride is the width of one pixel row, including any padding. In bytes, /4 converts to 4 byte pixels 

            double[] kernel = Create1DGaussianKernel(blurval); // Kernel to be convolved

            int kernelRadius = kernel.Length / 2; // Integer division, be careful!
            int kernelSize = kernel.Length;

            byte r, g, b;

            // First pass - horizontal gaussian blur
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Totals for the convolution
                    double rT = 0.0, gT = 0.0, bT = 0.0, kT = 0.0;

                    for (int u = 0; u < kernelSize; u++)
                    {
                        // Current position is cX, y
                        int cX = x + u - kernelRadius;

                        // We must make sure that the position isn't off either end of the image
                        if (cX < 0 || cX > width - 1)
                        {
                            continue;
                        }

                        // Obtain current pixel data
                        uint pixel = *(ptr + y * stride + cX); // Standard formula from the video, px = y * stride + x

                        // Split channels - this uses bit shifting and bitwise AND
                        r = (byte)((0x00FF0000 & pixel) >> 16);
                        g = (byte)((0x0000FF00 & pixel) >> 8);
                        b = (byte)((0x000000FF & pixel));

                        // Add to convolve total
                        rT += r * kernel[u];
                        gT += g * kernel[u];
                        bT += b * kernel[u];
                        kT += kernel[u];
                    }

                    // Compute nearest possible byte values
                    r = (byte)(rT / kT + 0.5);
                    g = (byte)(gT / kT + 0.5);
                    b = (byte)(bT / kT + 0.5);

                    // Combine channels into temporary array -  More bit shifting and bitwise OR.
                    intermediateBuffer[(y * width) + x] = (0xFF000000 | (uint)(r << 16) | (uint)(g << 8) | b);
                }
            }

            // Second pass - vertical gaussian blur
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Totals for the convolution
                    double rT = 0.0, gT = 0.0, bT = 0.0, kT = 0.0;

                    for (int v = 0; v < kernelSize; v++)
                    {
                        // Current position is x, cY
                        int cY = y + v - kernelRadius;

                        // We must make sure that the position isn't off either end of the image
                        if (cY < 0 || cY > height - 1)
                        {
                            continue;
                        }

                        // Obtain current pixel data
                        uint pixel = intermediateBuffer[cY * width + x];

                        // Split channels
                        r = (byte)((0x00FF0000 & pixel) >> 16);
                        g = (byte)((0x0000FF00 & pixel) >> 8);
                        b = (byte)((0x000000FF & pixel));

                        // Add to convolve total
                        rT += r * kernel[v];
                        gT += g * kernel[v];
                        bT += b * kernel[v];
                        kT += kernel[v];
                    }

                    // Compute nearest possible byte values
                    r = (byte)(rT / kT + 0.5);
                    g = (byte)(gT / kT + 0.5);
                    b = (byte)(bT / kT + 0.5);

                    // 0xFF000000 here is alpha, not transparent
                    *(ptr + y * stride + x) = (0xFF000000 | (uint)(r << 16) | (uint)(g << 8) | b);
                }
            }

            // Finish with image and save
            image.UnlockBits(imageData);
            return image;
        }

        private static unsafe Bitmap Sobel (Bitmap image){

            //change the pixel format. 
            //image = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb);


            // This simple code is meant for 32bpp images, it could be adapted to handle other image types.
            // 32ARGB and 32RGB are the same, except in the latter the alpha byte is simply padding. We ignore alpha in this code anyway.
            if (image.PixelFormat != PixelFormat.Format32bppArgb
                && image.PixelFormat != PixelFormat.Format32bppRgb)
            {
                return null ;
            }

            // Obtain grayscale conversion of the image
            byte[] grayData = ConvertTo8bpp(image);

            int width = image.Width;
            int height = image.Height;

            // Buffers
            byte[] buffer = new byte[9];
            double[] magnitude = new double[width * height]; // Stores the magnitude of the edge response
            double[] orientation = new double[width * height]; // Stores the angle of the edge at that location

            // First pass - convolve sobel operator and calculate orientation. We're using the byte array now, since it's easier.
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // Unlike the other Kernel operations, where the radius etc. might change this one is simple so we can hard code
                    // the kernel operations in. Pointer arithmetic would make this slightly faster, but we won't worry about it.
                    int index = y * width + x;

                    // 3x3 window around (x,y)
                    buffer[0] = grayData[index - width - 1];
                    buffer[1] = grayData[index - width];
                    buffer[2] = grayData[index - width + 1];
                    buffer[3] = grayData[index - 1];
                    buffer[4] = grayData[index];
                    buffer[5] = grayData[index + 1];
                    buffer[6] = grayData[index + width - 1];
                    buffer[7] = grayData[index + width];
                    buffer[8] = grayData[index + width + 1];

                    // Sobel horizontal and vertical response
                    double dx = buffer[2] + 2 * buffer[5] + buffer[8] - buffer[0] - 2 * buffer[3] - buffer[6];
                    double dy = buffer[6] + 2 * buffer[7] + buffer[8] - buffer[0] - 2 * buffer[1] - buffer[2];

                    magnitude[index] = Math.Sqrt(dx * dx + dy * dy) / 1141; // 1141 is approximately the max sobel response, we will normalise later anyway

                    // Directional orientation
                    orientation[index] = Math.Atan2(dy, dx) + Math.PI; // Angle is in radians, now from 0 - 2PI. 
                }
            }

            /* Now that we have the magnitude and orientation, we want to combine these into a coloured image for output.
             * The HSV colour model would work well here, hue is the angle of the colour, saturation we keep constant at 1,
             * and value is the magnitude. This should produce an image that is coloured based on edge angle, and whose brightness
             * reflects edge intensity. */

            // System.Bitmap includes SetPixel and GetPixel methods. These are very slow, so it's better to lock
            // the image and access the data directly. Locking prevents C# memory management moving it around.
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, image.PixelFormat);

            uint* ptr = (uint*)imageData.Scan0.ToPointer(); // An unsigned int pointer. This points to the image data in memory, each uint is one pixel ARGB
            int stride = imageData.Stride / 4; // Stride is the width of one pixel row, including any padding. In bytes, /4 converts to 4 byte pixels 

            byte r, g, b;

            // We want to scale magnitude from 0 - 1, because it's unlikely any magnitude would reach the theoretical maximum value
            // C#, like other high level languages, contains various list extension methods for ease of use.
            double magnitudeMax = magnitude.Max();

            // Combine 
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    // C# functions can make use of an out parameter. This works much like C/C++ address pointer (&), except it is compiler enforced
                    // Here we can use it to essentially return three values
                    double hue = orientation[index];
                    double val = magnitude[index] / magnitudeMax; // This will still highlight mostly very bright edges, to highlight more try Math.Sqrt(magnitude[index] / magnitudeMax)

                    HSVtoRGB(hue, 1, val, out r, out g, out b); // Using a saturation of 0 will make the image grayscale i.e. regular sobel.

                    // Combine rgb back into the output image
                    *(ptr + y * stride + x) = (0xFF000000 | (uint)(r << 16) | (uint)(g << 8) | b);
                }
            }

            // Finish with image and save
            image.UnlockBits(imageData);

            return image;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = true;
            radioButton3.Checked = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = true;
        }

        unsafe public static byte[] ConvertTo8bpp(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            byte[] grayData = new byte[width * height];

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, image.PixelFormat);

            uint* ptr = (uint*)imageData.Scan0.ToPointer();
            int inputStride = imageData.Stride / 4;

            byte r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Obtain current pixel data
                    uint pixel = *(ptr + y * inputStride + x); // Standard formula from the video, px = y * stride + x

                    // Split channels - this uses bit shifting and bitwise AND
                    r = (byte)((0x00FF0000 & pixel) >> 16);
                    g = (byte)((0x0000FF00 & pixel) >> 8);
                    b = (byte)((0x000000FF & pixel));

                    byte gray = (byte)(0.2126 * r + 0.7152 * g + 0.0722 * b);
                    grayData[y * width + x] = gray;
                }
            }

            image.UnlockBits(imageData);

            return grayData;
        }

        private static void HSVtoRGB(double h, double s, double v, out byte r, out byte g, out byte b)
        {
            // h_ 0-6
            double h_ = h / (2 * Math.PI) * 6;

            double c = s * v;
            double x = c * (1 - Math.Abs((h_ % 2) - 1));
            double r_, g_, b_;
            if (h_ < 1)
            {
                r_ = c;
                g_ = x;
                b_ = 0;
            }
            else if (h_ < 2)
            {
                r_ = x;
                g_ = c;
                b_ = 0;
            }
            else if (h_ < 3)
            {
                r_ = 0;
                g_ = c;
                b_ = x;
            }
            else if (h_ < 4)
            {
                r_ = 0;
                g_ = x;
                b_ = c;
            }
            else if (h_ < 5)
            {
                r_ = x;
                g_ = 0;
                b_ = c;
            }
            else
            {
                r_ = c;
                g_ = 0;
                b_ = x;
            }

            double m = v - c;

            r_ += m;
            g_ += m;
            b_ += m;

            r = (byte)(r_ * 255);
            g = (byte)(g_ * 255);
            b = (byte)(b_ * 255);
        }
    }
}
