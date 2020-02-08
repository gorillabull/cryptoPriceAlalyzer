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
            }
            
        }

        private unsafe void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int blurval = trackBar1.Value;
            DateTime start = DateTime.Now;

            //change the pixel format. 
            //image = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb);

            // This simple code is meant for 32bpp images, it could be adapted to handle other image types.
            // 32ARGB and 32RGB are the same, except in the latter the alpha byte is simply padding.
            if (image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppRgb)
            {
                return;
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
    }
}
