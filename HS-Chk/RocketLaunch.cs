using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Hypermint
{
    /// <summary>
    /// Class for managing Rocketlauncher
    /// </summary>
    public class RocketLaunch : HyperMintBaseClass, IRocketLaunch
    {

        /// <summary>
        /// Launch game with systemname Romname
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="rom"></param>
        public void RocketLaunchGameSTD(string sys, string rom)
        {
            if (Directory.Exists(RLPath))
            {
                try
                {
                    System.Diagnostics.Process.Start(RLPath + "\\Rocketlauncher.exe", "-s " + "\"" + sys + "\"" + " -r " + "\"" + rom + "\"" + " -p hyperspin");
                }
                catch (Exception)
                {
                }
            }
            else
                System.Windows.MessageBox.Show("Cannot find launcher.");

        }
        /// <summary>
        /// Launch rocketlauncher into one of its modes for testing
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="sys"></param>
        /// <param name="rom"></param>
        public void RocketLaunchMode(string mode,string sys, string rom)
        {
            if (Directory.Exists(RLPath))
            {
                try
                {
                    System.Diagnostics.Process.Start(RLPath + "\\Rocketlauncher.exe", "-s " + "\"" + sys + "\"" + " -r " + "\"" + rom + "\""
                         + " -m " + mode + " -p hyperspin");
                }
                catch (Exception)
                {
                }
            }
            else
                System.Windows.MessageBox.Show("Cannot find launcher.");
        }

    }

    class InstructionCards : RocketLaunch
    {
        public string FileName { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public bool Resize { get; set; }
        public bool Stretch { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool JPEG { get; set; }
        public string FullPath { get; set; }

        public InstructionCards()
        {

        }

        public InstructionCards(string file, string[] elements, string fullPath)
        {
            this.FileName = file;
            this.Author = elements[0];
            this.Description = elements[1];
            this.Position = elements[2];
            this.Width = Convert.ToInt32(elements[3]);
            this.Height = Convert.ToInt32(elements[4]);
            this.Resize = Convert.ToBoolean(elements[5]);
            this.Stretch = Convert.ToBoolean(elements[6]);
            this.FullPath = fullPath;
        }

        /// <summary>
        /// Save presets for an instruction card
        /// </summary>
        /// <param name="presetName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="description"></param>
        /// <param name="author"></param>
        /// <param name="pos"></param>
        /// <param name="resize"></param>
        /// <param name="stretch"></param>
        public void saveXML_Preset(string presetName, string width, string height, string description, string author, string pos, bool resize, bool stretch)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            if (Directory.Exists("presets\\cards"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\\cards\\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("author", author);
                    write.WriteElementString("description", description);
                    write.WriteElementString("position", pos);

                    write.WriteElementString("width", width);
                    write.WriteElementString("height", height);

                    write.WriteElementString("resize", resize.ToString());
                    write.WriteElementString("stretch", stretch.ToString());
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }

            }
        }

        /// <summary>
        /// Refresh presets in "presets\\cards"
        /// </summary>
        /// <returns>string array of presets</returns>
        public string[] presetRefresh()
        {
            DirectoryInfo dir = new DirectoryInfo("presets\\cards");
            FileInfo[] Files;
            Files = dir.GetFiles("*.xml");
            string[] presetFiles = new string[Files.Length];
            int i = 0;
            foreach (var item in Files)
            {
                presetFiles[i] = item.Name;
                i++;
            }

            return presetFiles;

        }

        /// <summary>
        /// Converts an instruction card
        /// </summary>
        public void ConvertCard()
        {
            int c = 1;
            int w, h;
            bool flag = true;
            string OutputFilename = "Instruction Card - " + Description + " - " + Position;
            string OriginalOutputFilename = OutputFilename;
            string newExt = string.Empty;

            System.Drawing.Image img = System.Drawing.Image.FromFile(FileName);

            if (Resize)
            {
                w = Width;
                h = Height;
            }
            else
            {
                w = img.Width;
                h = img.Height;
            }

            if (JPEG)
                newExt = ".jpg";
            else
                newExt = ".png";

            while (flag)
            {
                if (File.Exists(FullPath + "\\" + OutputFilename + ".png") || File.Exists(FullPath + "\\" + OutputFilename + ".jpg"))
                {
                    OutputFilename = "Instruction Card - " + Description + " " + c + " - " + Position;
                    c++;
                }
                else
                {
                    flag = false;
                }
            }
            createCard(newExt, OutputFilename, w, h);
        }

        /// <summary>
        /// Creates the instruction card
        /// </summary>
        /// <param name="newext"></param>
        /// <param name="OutputFilename"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        private void createCard(string newext, string OutputFilename, int w, int h)
        {
            string newPath;

            newPath = FullPath + "\\" + OutputFilename + newext;
            string filePathNew = System.IO.Path.GetDirectoryName(newPath);
            if (!Directory.Exists(filePathNew))
                Directory.CreateDirectory(filePathNew);

            //MessageBox.Show("Output Path" + newPath);
            System.Drawing.Image img = System.Drawing.Image.FromFile(FileName);

            if (Resize)
                img = ImageEdits.ResizeImage(img, new System.Drawing.Size(w, h));
            else
                img = ImageEdits.ResizeImageEdit(img, new System.Drawing.Size(w, h));

            if (JPEG)
                img.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            else
                img.Save(newPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
    public static class MouseUtilities
    {
        public static Point CorrectGetPosition(Visual relativeTo)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
    }
}
