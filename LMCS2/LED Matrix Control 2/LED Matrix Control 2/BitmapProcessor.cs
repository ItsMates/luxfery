using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace LED_Matrix_Control_2
{
    public class BitmapProcessor
    {
        public byte[] ProcessImage(Bitmap image, int width, int height, InterpolationMode mode, Rectangle dimensions)
        {
            Bitmap workingBitmap = CropImage(image, dimensions);
            Bitmap DownSampBit = DownsampleBitmap(workingBitmap, width, height, mode);

            int[] bitmapRGB = new int[DownSampBit.Width * DownSampBit.Height];
            DownSampBit.getRGB(0, 0, DownSampBit.Width, DownSampBit.Height, bitmapRGB, 0, DownSampBit.Width);

            DownSampBit.Dispose();
            workingBitmap.Dispose();

            byte[] bitmapData = new byte[bitmapRGB.Length * 3];
            int index = 0;

            //Converting the int (32 bit) RGB encoded array to multiple R,G,B bytes
            for (int i = 0; i < bitmapRGB.Length; i++)
            {
                for (int j = 0; j < 3; j++) //for R,G,B
                {
                    //The R,G,B data are encoded in single int
                    //with this code you can decode it to 3 separate R,G,B bytes
                    bitmapData[index++] = (byte)(bitmapRGB[i] >> (j * 8));
                }              
            }

            return bitmapData;
        }


        Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
               Rectangle dest = new Rectangle(0, 0, cropArea.Width, cropArea.Height);
               g.DrawImage(img, dest, cropArea, GraphicsUnit.Pixel);
            }
            
            return bmp;
        }


        private Bitmap DownsampleBitmap(Bitmap b, int width, int height, InterpolationMode mode)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                //g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = mode;
                g.DrawImage(b, 0, 0, width, height);
            }
            return result;
        }


        public Bitmap ImagePreviewHandler(Bitmap b, int x, int y, int endX, int endY)
        {
            Bitmap processedBitmap = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.DontCare);
            
            using (Graphics g = Graphics.FromImage(processedBitmap))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.DrawRectangle(new Pen(Brushes.HotPink, (b.Width/150)), new Rectangle(x, y, endX - x, endY - y));
            }
            return processedBitmap;
        }


        public Bitmap ScaleToFitPB(Bitmap source, int pbWidth, int pbHeight)
        {
            Bitmap workingBit = source.Clone(new Rectangle(0, 0, source.Width, source.Height), PixelFormat.DontCare);
            Bitmap returnBit;
            if ((float)source.Width / (float)source.Height < (float)pbWidth / (float)pbHeight)
                returnBit = new Bitmap(workingBit, (int)(((float)source.Width / (float)source.Height) * pbHeight), pbHeight);

            else if ((float)source.Width / (float)source.Height > (float)pbWidth / (float)pbHeight)
                returnBit = new Bitmap(workingBit, pbWidth, (int)(((float)source.Height / (float)source.Width) * pbWidth));

            else
                returnBit = new Bitmap(workingBit, pbWidth, pbHeight);

            workingBit.Dispose();
            return returnBit;
        }


        public Bitmap ScreenToBitmap(Rectangle captureArea)
        {
            Bitmap screenBitmap = new Bitmap(captureArea.Width, captureArea.Height, PixelFormat.Format24bppRgb);

                using (Graphics captureGraphics = Graphics.FromImage(screenBitmap))
                {
                    captureGraphics.CopyFromScreen(captureArea.Left, captureArea.Top, 0, 0, captureArea.Size);
                }

            return screenBitmap;
        }

     
    }

   

}
