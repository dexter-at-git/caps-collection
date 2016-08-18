using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.IO.Png;

namespace CapsCollection.Silverlight.Infrastructure.Helpers
{
    public class ThumbnailGenerator
    {
        public static byte[] Generate(byte[] image, int height, int width)
        {
            if (image == null)
            {
                throw new ArgumentNullException("Error during thumbnail generation. Image bytes cannot be null.");
            }

            Stream stream = new MemoryStream(image);

            BitmapImage bi = new BitmapImage();
            bi.SetSource(stream);
            
            var bytes = new byte[0];
            
            // create thumbnail
            WriteableBitmap wb;
            try
            {
                wb = GetImageSource(stream, height, width);

                // png
                var encoder = new PngEncoder();
                
                var img = wb.ToImage();
                using (MemoryStream stream1 = new MemoryStream())
                {
                    encoder.Encode(img, stream1);
                    bytes = stream1.ToArray();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while trying to make thumbnail" + Environment.NewLine + ex.Message);
                bytes = new byte[0];
            }
            return bytes;
        }
        
        public static WriteableBitmap GetImageSource(Stream stream, double maxWidth, double maxHeight)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(stream);

            Image img = new Image();
            img.Source = bmp;
            
            double scaleX = 1;
            double scaleY = 1;

            if (bmp.PixelHeight > maxHeight)
                scaleY = maxHeight / bmp.PixelHeight;
            if (bmp.PixelWidth > maxWidth)
                scaleX = maxWidth / bmp.PixelWidth;

            // maintain aspect ratio by picking the most severe scale
            double scale = Math.Min(scaleY, scaleX);

            int newWidth = Convert.ToInt32(bmp.PixelWidth * scale);
            int newHeight = Convert.ToInt32(bmp.PixelHeight * scale);
            WriteableBitmap result = new WriteableBitmap(newWidth, newHeight);
            //     result.Render(can, null);
            result.Render(img, new ScaleTransform { ScaleX = scale, ScaleY = scale });
            result.Invalidate();
            return result;
        }
    }
}
