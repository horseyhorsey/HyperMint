using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint
{
    /// <summary>
    /// Image editing/Converting
    /// </summary>
    public static class ImageEdits
    {
        /// <summary>
        /// Get imagesource from URI file link
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImageSource BitmapFromUri(Uri source)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = source;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {

            int sourceWidth = (int)imgToResize.Width;
            int sourceHeight = (int)imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            // Image img3 = Image.FromFile("temp.png");
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
            }

            using (TextureBrush brush = new TextureBrush(b, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.FillRectangle(brush, 0, 0, 1920, 1080);
                g.Dispose();
            }



            return (System.Drawing.Image)b;
        }
        /// <summary>
        /// Return a tiled Image
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static System.Drawing.Image TileImage(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            return null;
        }
        public static System.Drawing.Image ResizeImageTile(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            int destWidth = (int)(size.Width);
            int destHeight = (int)(size.Height);

            Bitmap b = new Bitmap(destWidth, destHeight);

            using (TextureBrush brush = new TextureBrush(imgToResize, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.FillRectangle(brush, 0, 0, destWidth, destHeight);
                g.Dispose();
            }
            return (System.Drawing.Image)b;
        }
        public static System.Drawing.Image ResizeImageEdit(System.Drawing.Image imgToResize, System.Drawing.Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            int destWidth = (int)(size.Width);
            int destHeight = (int)(size.Height);

            Bitmap b = new Bitmap(destWidth, destHeight);


            using (TextureBrush brush = new TextureBrush(imgToResize, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(b))
            {
                // Do your painting in here
                //g.FillRectangle(brush, 0, 0, destWidth, destHeight);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                // Draw string to screen.
                g.Dispose();
            }

            return (System.Drawing.Image)b;
        }
    }
    public static class ImageInfo
    {
        /// <summary>
        /// Get properties info for image
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string getImageInfo(string fileName)
       {
            if (!File.Exists(fileName))
                return "";
            string result;
            long len = new FileInfo(fileName).Length;

            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
                ImageFormat format = img.RawFormat;
                result = "Width : " + img.Width + " - Image height : " + img.Height + " Size: " + convertBytesToReadable(len);

                // Dispose of the image because it stays  locked!
                img.Dispose();
                return result;
            }
            catch (Exception)
            {

                return null;
            }


        }
        /// <summary>
        /// Convert the images bytes to a readable string
        /// </summary>
        /// <param name="TotalFilesLength"></param>
        /// <returns></returns>
        private static string convertBytesToReadable(long TotalFilesLength)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (TotalFilesLength >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                TotalFilesLength = TotalFilesLength / 1024;
            }

            string result = String.Format("{0:0.##} {1}", TotalFilesLength, sizes[order]);
            return result;
        }
    }

    public class ImageCreate : HyperMintBaseClass
    {
        public string InputImage { get; set; }
        public string OutputFileName { get; set; }
        public string romName { get; set; }
        public int InWidth { get; set; }
        public int InHeight { get; set; }
        public int OutWidth { get; set; }
        public int OutHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string FolderName { get; set; }
        public string CellFolder { get; set; }
        public string NewCellFolder { get; set; }
        public string FadeLayerName { get; set; }
        public string Prefix { get; set; }
        public string Ratio { get; set; }
        public string Author { get; set; }
        public bool Resize { get; set; }
        public bool Stretch { get; set; }
        public bool Tile { get; set; }
        public bool Preview { get; set; }
        public bool MyProperty { get; set; }
        public bool JPEG { get; set; }
        public bool FlipL { get; set; }
        public bool FlipR { get; set; }        
        public string SystemName { get; set; }
        public string Rom { get; set; }
        private string newext;
        private   int w,h;

        public ImageCreate()
        {
           // picbox.Source = BitmapFromUri(new Uri(System.IO.Path.GetFullPath(imgLocation)));
        }

        /// <summary>
        /// Set image parameters for renaming to Rocketlauncher names
        /// and set new size
        /// </summary>
        /// <param name="Preview"></param>
        public void SetImageParameter(bool Preview)
        {
            int c = 1;
            bool flag = true;

            //ImageEdits.BitmapFromUri(imgSource);

            if (FadeLayerName == "Background")
            {
                NewCellFolder = "Backgrounds";
                OutputFileName = "Background - " + Ratio + " (" + Author + ")";
            }
            else if(FadeLayerName == "Bezel Background")
            {
                NewCellFolder = "Bezels";
                OutputFileName = "Background - " + Ratio + " (" + Author + ")";
            }
            else if (FadeLayerName.Contains("Layer"))
            {
                NewCellFolder = "Fade";
                OutputFileName = FadeLayerName + " - " + Ratio + " " + Prefix + " (" + Author + ")"; 
            }
            else if (FadeLayerName.Contains("Background HS"))
            {
                NewCellFolder = "Backgrounds";
                OutputFileName = romName;
            }

            if (Resize)
            {
                w = OutWidth;
                h = OutHeight;
            }
            else
            {
                w = InWidth;
                h = InHeight;
            }

            //string OriginalOutputFilename = OutputFilename;
            if (JPEG)
                newext = ".jpg";
            else
                newext = ".png";

            string newRocketLaunchPath = Path.Combine(RLMediaPath, NewCellFolder, SystemName, Rom);

            #region FileChecking
            while (flag)
            {
                if (!FadeLayerName.Contains("Background HS"))
                {
                    if (File.Exists(newRocketLaunchPath + "\\" + OutputFileName + ".png") || File.Exists(newRocketLaunchPath + "\\" + OutputFileName + ".jpg"))
                    {
                        if (FadeLayerName.Contains("Layer"))
                        {
                            OutputFileName = FadeLayerName + " - " + Ratio + " " + Prefix + "(" + c + ") " + "(" + Author + ")";
                        }
                        else
                            OutputFileName = FadeLayerName + Prefix + "(" + c + ") " + "(" + Author + ")";

                        if (FadeLayerName == "Background" || FadeLayerName == "Bezel Background")
                        {
                            OutputFileName = "Background - " + Ratio + " (" + c + ") " + "(" + Author + ")";
                        }

                        c++;
                    }
                    else
                        flag = false;
                }
                else
                    flag = false;
            }
	#endregion           
            if (Preview)
                createImage(true);
            else
               createImage(false);
            }

        /// <summary>
        /// Create image 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string createImage(bool p)
        {
 	         string newPath;
             string newRocketLaunchPath = Path.Combine(RLMediaPath, NewCellFolder, SystemName, Rom);

             if (FadeLayerName.Contains("Background HS"))
             {
                 newRocketLaunchPath = Path.Combine(Properties.Settings.Default.hsPath, "Media\\" + SystemName + "\\Images\\Backgrounds\\", OutputFileName);
                 newPath = newRocketLaunchPath + newext;
             }
             else
             {
                 if (p)
                     newPath = "Preview" + newext;
                 else
                 {
                     newPath = newRocketLaunchPath + "\\" + OutputFileName + newext;
                     string filePathNew = System.IO.Path.GetDirectoryName(newPath);
                     if (!Directory.Exists(filePathNew))
                         Directory.CreateDirectory(filePathNew);
                 }
             }

             System.Drawing.Image img = System.Drawing.Image.FromFile(InputImage);
             System.Drawing.Image img2 = img;


             if (Tile)
            { 
                int tw = TileWidth;
                int th = TileHeight;
               
                img2 = ImageEdits.ResizeImage(img, new System.Drawing.Size(tw, th));
                if (FlipL)
                    img2.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else if (FlipR)
                    img2.RotateFlip(RotateFlipType.Rotate270FlipNone);

                img = ImageEdits.ResizeImageTile(img2, new System.Drawing.Size(w, h));
            }
              else if (Stretch)
            {
                img = ImageEdits.ResizeImageEdit(img, new System.Drawing.Size(w, h));
            }

             else if (Resize)
             {
                 img = ImageEdits.ResizeImage(img, new System.Drawing.Size(w, h));
             }

             else
             {
                 img = ImageEdits.ResizeImageEdit(img, new System.Drawing.Size(w, h));
             }

             if (JPEG)
                 img.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
             else
                 img.Save(newPath, System.Drawing.Imaging.ImageFormat.Png);


             if (Preview)
                  return System.IO.Path.GetFullPath(newPath);
             else
             {
                 return string.Empty;
             }
        }
    }

    }



