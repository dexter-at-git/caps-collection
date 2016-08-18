using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class ThumbnailService : IThumbnailService
    {
        public byte[] Generate(byte[] imageBytes, int thumbWidth, int thumbHeight)
        {
            if (imageBytes == null || imageBytes.Length == 0 || thumbWidth == 0 || thumbHeight == 0)
            {
                return new byte[] { };
            }

            Image orignalImage = null;
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                orignalImage = Image.FromStream(ms);
            }

            // Rotating image 360 degrees to discart internal thumbnail image
            orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

            int newHeight = orignalImage.Height * thumbWidth / orignalImage.Width;
            int newWidth = thumbWidth;

            // New height is greater than our thumbHeight so we need to keep height fixed and calculate the width accordingly
            if (newHeight > thumbHeight)
            {
                newWidth = orignalImage.Width * thumbHeight / orignalImage.Height;
                newHeight = thumbHeight;
            }

            var thumbImage = ResizeImageFile(imageBytes, new Size(newWidth, newHeight), ImageFormat.Png);

            return thumbImage;
        }

        private static Task<byte[]> GenerateThumbnailAsync(byte[] imageBytes, int thumbWidth, int thumbHeight)
        {
            return Task.Run(() =>
            {
                if (imageBytes == null)
                    return null;

                Stream stream = new MemoryStream(imageBytes);
                Image orignalImage = Image.FromStream(stream);

                // Rotating image 360 degrees to discart internal thumbnail image
                orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                int newHeight = orignalImage.Height * thumbWidth / orignalImage.Width;
                int newWidth = thumbWidth;

                // New height is greater than our thumbHeight so we need to keep height fixed and calculate the width accordingly
                if (newHeight > thumbHeight)
                {
                    newWidth = orignalImage.Width * thumbHeight / orignalImage.Height;
                    newHeight = thumbHeight;
                }

                var thumbImage = ResizeImageFile(imageBytes, new Size(newWidth, newHeight), ImageFormat.Png);

                return thumbImage;
            });
        }

        private static byte[] ResizeImageFile(byte[] imageFile, Size targetSize, ImageFormat imageFormat)
        {
            using (Image oldImage = Image.FromStream(new MemoryStream(imageFile)))
            {
                // Save resized picture
                var qualityEncoder = Encoder.Quality;
                var quality = (long)50;
                var ratio = new EncoderParameter(qualityEncoder, quality);
                var codecParams = new EncoderParameters(1);
                codecParams.Param[0] = ratio;

                var codecInfo = GetEncoder(imageFormat);

                Size newSize = targetSize;
                using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics canvas = Graphics.FromImage(newImage))
                    {
                        canvas.SmoothingMode = SmoothingMode.AntiAlias;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.PixelOffsetMode = PixelOffsetMode.Half;
                        canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
                        MemoryStream m = new MemoryStream();
                        newImage.Save(m, codecInfo, codecParams);
                        return m.GetBuffer();
                    }
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
