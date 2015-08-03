using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hypermint
{

    public class Hyperspin : HypermintMain
    {
        public Hyperspin()
        {

        }

        public void DroppedFileHyperspin(string[] filelist, DatabaseGame game, string selectedColumn)
        {
            int i;

            for (i = 0; i < filelist.Length; i++)
            {
                Filename = System.IO.Path.GetFileName(filelist[i]);
                ext = System.IO.Path.GetExtension(filelist[i]);
                filename3 = System.IO.Path.GetFileNameWithoutExtension(filelist[i]);
                path = null;

                if (selectedColumn == "Wheel" || selectedColumn == "Artwork1" || selectedColumn == "Artwork2"
                    || selectedColumn == "Artwork3" || selectedColumn == "Artwork4" || selectedColumn == "Video"
                    || selectedColumn == "Background")
                {
                    wheel_drop(filelist[i], filename3, game);
                }
                else if (selectedColumn == "Theme")
                {
                    theme_drop(filelist[i], filename3, game);
                }
            }

            UpdateGameBools(game);
        }

        private void wheel_drop(string file, string filenamenoExt, DatabaseGame game)
        {
            try
            {
                if (ext == ".bmp" || ext == ".gif")
                    path = FullPath + SelectedRomname + ".png";
                else
                    path = FullPath + SelectedRomname + ext;


                if (File.Exists(path))
                {
                    SendToTrash("Hyperspin", new FileInfo(path));
                }

                if (ext == ".bmp" || ext == ".gif")
                    path = FullPath + SelectedRomname + ext;

                if (file.EndsWith(".jpg") || file.EndsWith(".gif") || file.EndsWith(".jpeg")
                                 || file.EndsWith(".JPG") || file.EndsWith(".bmp"))
                {
                    using (System.Drawing.Image im = System.Drawing.Image.FromFile(file))
                    {
                        im.Save(FullPath + SelectedRomname + ".png", ImageFormat.Png);
                        UpdateGameBools(game);
                    }
                    return;
                }
                else if (file.EndsWith(".PNG") || file.EndsWith(".png") || file.EndsWith(".mp4"))
                {
                    if (!Directory.Exists(FullPath))
                        Directory.CreateDirectory(FullPath);
                    System.IO.File.Copy(file, path);
                }
            }
            catch (Exception)
            {

            }

            UpdateGameBools(game);

        }
        private void theme_drop(string file, string filenamenoExt, DatabaseGame game)
        {

            // if extension is a zip just rename & copy over for now
            if (ext == ".zip")
            {
                path = FullPath + SelectedRomname + ext;

                if (File.Exists(path))
                    SendToTrash("Hyperspin", new FileInfo(path));

                if (!Directory.Exists(FullPath))
                    Directory.CreateDirectory(FullPath);

                System.IO.File.Copy(file, path);
                game.HaveTheme = true;
                return;
            }

            // Quick theme file creator
            // 
            // If image format is dragged into theme folder then create a basic theme with image
            // renamed to Background.png, info.txt & Theme.xml
            // romname + ".zip"
            using (System.Drawing.Image im = System.Drawing.Image.FromFile(file))
            {
                System.Drawing.Image im2 = System.Drawing.Image.FromFile(file);
                im2 = ImageEdits.ResizeImageEdit(im, new System.Drawing.Size(1024, 768));

                im2.Save("Background.png", ImageFormat.Png);
            }

            try
            {
                if (!File.Exists("Theme.xml"))
                    return;

                using (ZipFile zip = new ZipFile())
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
                    zip.AddFile("Background.png");
                    zip.AddFile("Theme.xml");
                    zip.AddFile("Info.txt");
                    zip.Save(FullPath + "\\" + SelectedRomname + ".zip");
                    System.IO.File.Delete("Background.png");
                }
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// Return the full path to selected cell
        /// </summary>
        /// <param name="columnHeader"></param>
        /// <param name="game_click"></param>
        /// <param name="hsPath"></param>
        /// <returns></returns>
        public string getFullPath(string columnHeader, DatabaseGame game_click, string hsPath)
        {
            //SystemName = systemName;
            switch (columnHeader)
            {
                case "Name":
                    FullPath = hsPath + "\\Media\\" + base.SystemName + "\\Images\\Wheel\\";
                    isHyperspin = true;
                    break;
                case "Description":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    isHyperspin = true;
                    break;
                case "Manuafacturer":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    isHyperspin = true;
                    break;
                case "Artwork1":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork1\\";
                    isHyperspin = true;
                    break;
                case "Artwork2":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork2\\";
                    isHyperspin = true;
                    break;
                case "Artwork3":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork3\\";
                    isHyperspin = true;
                    break;
                case "Artwork4":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork4\\";
                    isHyperspin = true;
                    break;
                case "Theme":
                    isHyperspin = true;
                    WheelSource = null;
                    InfoMessage = HyperSpinMedia.GetThemeInfoText(FullPath + SelectedRomname + ".zip");
                    break;
                case "Video":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Video\\";
                    isHyperspin = true;
                    break;
                case "Background":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Backgrounds\\";
                    isHyperspin = true;
                    break;
                case "Wheel":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    isHyperspin = true;
                    break;
                case "Letters":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Letters\\";
                    isHyperspin = true;
                    break;
                case "Special":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Special\\";
                    isHyperspin = true;
                    break;
                case "GenreBG":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Genre\\Backgrounds\\";
                    isHyperspin = true;
                    break;
                case "GenreWheel":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Genre\\Wheel\\";
                    isHyperspin = true;
                    break;
                case "Pointer":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Other\\";
                    isHyperspin = true;
                    break;

                //RocketLaunch Folders
                case "Artwork":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath);
                    isHyperspin = false;
                    break;
                case "Backgrounds":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Bezels":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Cards":
                    FullPath = setRLMediaPath("Bezels", game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Controller":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Fade":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Guides":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Manuals":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "MultiGame":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Music":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Saves":
                    FullPath = setRLMediaPath("Saved Games", game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                case "Videos":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); isHyperspin = false;
                    break;
                default:
                    break;
            }

            return FullPath;
        }

        private string setRLMediaPath(string columnName, string romname, string systemname, string MediaPath)
        {
            return FullPath = MediaPath + columnName + "\\" + systemname + "\\" + romname;
        }


        internal List<FileCollection> GetFileCollection()
        {
            ext = ".png";
            ViewerFilename = FullPath + SelectedRomname + ext;

            if (SelectedColumn == "Special" || SelectedColumn == "Letters" || SelectedColumn == "GenreBG"
                || SelectedColumn == "GenreWheel")
            {
                if (doesDirectoryContainFiles(FullPath, "*.png"))
                {
                    DirectoryInfo di = new DirectoryInfo(FullPath);
                    FileInfo[] fi = di.GetFiles("*.png");
                    foreach (FileInfo item in fi)
                    {
                        fc.Add(new FileCollection(item.Name, item.Extension, FullPath));
                    }
                }

            }
            else if (checkHSMediaFiles(ViewerFilename))
            {
                fc.Add(new FileCollection(SelectedRomname, ext, ViewerFilename));
                //filesBox.Items.Refresh();
                WheelSource = ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath(ViewerFilename)));
                InfoMessage = ImageInfo.getImageInfo(ViewerFilename);
            }

            return fc;
        }

        public ImageSource DisplaySelectedImage(DatabaseGame SelectedGameObject)
        {
            object im = null;
            if (SelectedGameObject == null)
                return (ImageSource)im;

            string ext = ".png";
            ViewerFilename = FullPath + SelectedGameObject.RomName + ext;
            string name = System.IO.Path.GetFileNameWithoutExtension(ViewerFilename);
            return ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath(ViewerFilename)));
            //WheelSource = ImageEdits.BitmapFromUri(new Uri(fi[0].FullName));
            // InfoMessage = ImageInfo.getImageInfo(fi[0].FullName);
            //Console.WriteLine("Image Type : " + format.ToString());
            //Console.WriteLine("Image width : " + img.Width);
            //Console.WriteLine("Image height : " + img.Height);
            //Console.WriteLine("Image resolution : " + (img.VerticalResolution * img.HorizontalResolution));

            //Console.WriteLine("Image Pixel depth : " + Image.GetPixelFormatSize(img.PixelFormat));
            //Console.WriteLine("Image Creation Date : " + creation.ToString("yyyy-MM-dd"));
            //Console.WriteLine("Image Creation Time : " + creation.ToString("hh:mm:ss"));
            //Console.WriteLine("Image Modification Date : " + modify.ToString("yyyy-MM-dd"));
            //Console.WriteLine("Image Modification Time : " + modify.ToString("hh:mm:ss"));

        }

        public override void UpdateGameBools(DatabaseGame game = null)
        {
            base.UpdateGameBools(game);
        }
    }



}
