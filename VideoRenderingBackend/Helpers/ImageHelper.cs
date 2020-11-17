
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoRenderingBackend.Helpers
{
    public class ImageHelper
    {
        public static byte[] GetScaledCenteredJpgImageByte(Image image, int width, int height)
        {
            return GetScaledCenteredJpgImageByte( ImageToByte(image),width,height);
        }
        public static byte[] GetScaledCenteredJpgImageByte(byte[] byteImage, int width, int height)
        {
            if (byteImage != null)
            {
                using (var ms2 = new MemoryStream(byteImage))
                {
                    var image = ResizeImage(Image.FromStream(ms2), width, height, false, true);
                    return ImageToByte(image);
                }
            }
            return null;
        }

        public static Image ResizeImage(Image image, int maxWidth, int maxHeight, bool whiteBackground = false, bool center = false)
        {
            int newWidth;
            int newHeight;

            int originalWidth = image.Width;
            int originalHeight = image.Height;

            if (originalWidth > maxWidth || originalHeight > maxHeight)
            {
                // To preserve the aspect ratio  
                float ratioX = (float)maxWidth / (float)originalWidth;
                float ratioY = (float)maxHeight / (float)originalHeight;
                float ratio = System.Math.Min(ratioX, ratioY);
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);
            }
            else
            {
                //its okay already
                if (center || whiteBackground)
                {
                    if (whiteBackground)
                        center = true; //otherwise doesnt make sense

                    newWidth = (int)originalWidth;
                    newHeight = (int)originalHeight;
                }
                else
                {
                    //return as it is!!
                    return image;
                }


            }
            var startX = 0;
            var startY = 0;

            var destImage = new Bitmap(newWidth, newHeight);

            //center it in bitmap
            if (center)
            {
                startX = (maxWidth - newWidth) / 2;
                startY = (maxHeight - newHeight) / 2;
                destImage = new Bitmap(maxWidth, maxHeight);
            }

            var destRect = new Rectangle(startX, startY, newWidth, newHeight);


            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;


                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    if (whiteBackground)
                        graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, maxWidth, maxHeight);
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }


            return destImage;
        }
        public static byte[] ImageToByte(Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            };
        }

        public static Image ByteToImage(byte[] binaryImage)
        {
            using (var ms = new MemoryStream(binaryImage))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
