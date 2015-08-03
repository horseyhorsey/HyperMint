using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Windows.Media;

namespace Hypermint
{
    public abstract class HypermintBase : IHypermintPaths
    {
        private string hsPath;
        /// <summary>
        /// Hyperspin Path
        /// </summary>
        public string HSPath
        {
            get
            {
                return hsPath;
            }
            set
            {
                hsPath = value;
            }
        }

        private string imPath;
        /// <summary>
        /// ImageMagick Path
        /// </summary>
        public string IMPath
        {
            get
            {
                return imPath;
            }
            set
            {
                imPath = value ;
            }
        }

        private string fEMediaPath;
        public string FEMediaPath
        {
            get
            {
                return fEMediaPath;
            }
            set
            {
                fEMediaPath = value;
            }
        }

        private string feLaunchPath;
        public string FELaunchPath
        {
            get
            {
                return feLaunchPath;
            }
            set
            {
                feLaunchPath = value;
            }
        }

        private string feParams;
        public string FEParams
        {
            get
            {
                return feParams;
            }
            set
            {
                feParams = value;
            }
        }
    }

    public class HypermintMain : HypermintBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal HypermintMain(){}

        internal List<FileCollection> fc = new List<FileCollection>();

        private ImageSource wheelSource;
        public ImageSource WheelSource
        {
            get { return wheelSource; }
            set
            {
                wheelSource = value;
                NotifyPropertyChanged("wheelSource");
            }
        }

        public virtual void UpdateGameBools(DatabaseGame game=null)
        {
            switch (SelectedColumn)
            {
                case "Wheel":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveWheel = true;
                    else
                        game.HaveWheel = false;
                    break;
                case "Background":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveBackgroundsHS = true;
                    else
                        game.HaveBackgroundsHS = false;
                    break;
                case "Artwork1":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveArt1 = true;
                    else
                        game.HaveArt1 = false;
                    break;
                case "Artwork2":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveArt2 = true;
                    else
                        game.HaveArt2 = false;
                    break;
                case "Artwork3":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveArt3 = true;
                    else
                        game.HaveArt3 = false;
                    break;
                case "Artwork4":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveArt4 = true;
                    else
                        game.HaveArt4 = false;
                    break;
                case "Theme":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".zip"))
                        game.HaveTheme = true;
                    else
                        game.HaveTheme = false;
                    break;
                case "Video":
                    if (checkHSMediaFiles(FullPath + SelectedRomname + ".mp4"))
                        game.HaveVideo = true;
                    else if (checkHSMediaFiles(FullPath + SelectedRomname + ".flv"))
                        game.HaveVideo = true;
                    else if (checkHSMediaFiles(FullPath + SelectedRomname + ".png"))
                        game.HaveVideo = true;
                    else
                        game.HaveVideo = false;
                    break;

                case "Letters":
                    FullPath = HSPath + "\\Media\\" + SystemName + "\\Images\\Letters\\";
                    //  DisplaySelectedImage(game_click);
                    break;
                case "Special":
                    FullPath = HSPath + "\\Media\\" + SystemName + "\\Images\\Special";
                    //  DisplaySelectedImage(game_click);
                    break;
                case "GenreBG":
                    FullPath = HSPath + "\\Media\\" + SystemName + "\\Images\\Genre\\Backgrounds\\";
                    //  DisplaySelectedImage(game_click);
                    break;
                case "GenreWheel":
                    FullPath = HSPath + "\\Media\\" + SystemName + "\\Images\\Genre\\Wheel";
                    //  DisplaySelectedImage(game_click);
                    break;
                case "Pointer":
                    FullPath = HSPath + "\\Media\\" + SystemName + "\\Images\\Other";
                    DisplaySelectedImage(game);
                    break;
                default:
                    break;
            }
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

        [XmlIgnore]
        public bool isHyperspin = true;

        private string infoMessage;
        /// <summary>
        /// Set the info message to display to user in GUI
        /// </summary>
        public string InfoMessage
        {
            get { return infoMessage; }
            set { infoMessage = value; this.NotifyPropertyChanged("infoMessage"); }
        }

        public string MainMenuXMLPath { get; set; }
        public string XMLPath { get; set; }
        public string SystemName { get; set; }
        [XmlIgnore]
        public int GameCount { get; set; }
        private int systemsEnabled;
        [XmlIgnore]
        public int SystemsEnabled
        {
            get { return systemsEnabled; }
            set
            {
                systemsEnabled = value;
                NotifyPropertyChanged("systemsEnabled");
            }
        }
        public string Filename { get; set; }
        public string ViewerFilename { get; set; }
        public string SelectedMediaPath { get; set; }
        public string movedFile { get; set; }
        private string selectedColumn;
        public string SelectedColumn
        {
            get { return selectedColumn; }
            set
            {
                selectedColumn = value;
                this.NotifyPropertyChanged("selectedColumn");
            }
        }
        public string LastSelectedRomname { get; set; }
        public string LastSelectedValue { get; set; }
        public string LastSelectedColumn { get; set; }
        public string[] sysname;
        private string selectedRomname;
        public string filename3;
        public string ext;
        public string path;
        public string SelectedRomname
        {
            get { return selectedRomname; }
            set
            {
                selectedRomname = value;
                this.NotifyPropertyChanged("selectedRomname");
            }
        }
        private string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;
                this.NotifyPropertyChanged("FullPath");
            }
        }

        /// <summary>
        /// Search for "Hyperspin" or "Rocketlauncher" directory on hard drive
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public string SearchForInstall(string directory)
        {
            string foundPath = string.Empty;
            DriveInfo[] drives = DriveInfo.GetDrives();

            for (int i = 0; i < drives.Count(); i++)
            {
                if (Directory.Exists(drives[i].Name + @"\" + directory))
                {
                    foundPath = drives[i].Name + directory;
                }
            }

            return foundPath;
        }
        /// <summary>
        /// Returns an array of systems from the xml set in MainMenuXMLPath
        /// </summary>
        /// <returns></returns>
        public string[] getSystems(string menuxml)
        {
            // Using a using block to dispose of the text reader.
            //
            using (XmlTextReader reader = new XmlTextReader(menuxml))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(MainMenuXMLPath);
                int sysCount = xdoc.SelectNodes("menu/game").Count + 1;
                sysname = new string[sysCount];
                int i = 0;
                sysname[i] = "Main Menu";
                i++;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            sysname[i] = reader.GetAttribute("name");
                            i++;
                        }
                }
            }
            return sysname;
        }
        public string[] getMainMenuXmls()
        {
            string xmlPath = "";
            xmlPath = HSPath + "\\Databases\\Main Menu\\";
            string[] menuXmls;
            if (Directory.Exists(xmlPath))
            {
                int i = 0;
                menuXmls = new string[Directory.GetFiles(xmlPath, "*.xml").Count()];
                foreach (var item in Directory.GetFiles(xmlPath, "*.xml"))
                {
                    menuXmls[i] = Path.GetFileNameWithoutExtension(item);
                    i++;
                }
                return menuXmls;
            }
            else
                return null;
        }

        public string searchLink { get; set; }
        public string downloadLink { get; set; }
        public string FileToSaveAs { get; set; }

        internal BackgroundWorker youtubeWorker = new BackgroundWorker();

        /// <summary>
        /// Check a folder to see if it has any files there.
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="extFilter"></param>
        /// <returns></returns>
        public bool checkHSMediaFolderFiles(string fullpath, string extFilter)
        {
            if (!Directory.Exists(fullpath))
                return false;

            string[] getFiles;
            getFiles = Directory.GetFiles(fullpath, extFilter);
            if (getFiles.Length != 0)
                return true;
            else return false;
        }
        public bool checkHSMediaFiles(string filenamePath)
        {
            if (File.Exists(filenamePath))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Search all sub directories for files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool doesDirectoryContainFiles(string path, string filter = "*")
        {

            if (!Directory.Exists(path)) return false;
            return Directory.EnumerateFiles(path, filter, SearchOption.AllDirectories).Any();
        }

        public void massBuildFavorites()
        {

        }

        /// <summary>
        /// Send selected items to RUbbish (Trash)
        /// </summary>
        /// <param name="TrashFolder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool SendToTrash(string TrashFolder, FileInfo file)
        {
            if (!file.Exists)
                return false;

            string PathToMoveTo;
            if (TrashFolder == "RocketLauncher")
                PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
                 "Rubbish\\" + TrashFolder + "\\" + SystemName + "\\" + SelectedColumn + "\\" + SelectedRomname + "\\";
            else
                PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
                                "Rubbish\\" + TrashFolder + "\\" + SystemName + "\\" + SelectedColumn + "\\";

            DirectoryInfo di = new DirectoryInfo(PathToMoveTo);
            if (!di.Exists)
                di.Create();

            string moveFilenameNoExt = Path.GetFileNameWithoutExtension(file.FullName);
            string moveFilenameNew = moveFilenameNoExt;
            int i = 1;
            while (File.Exists(PathToMoveTo + moveFilenameNew + file.Extension))
            {
                moveFilenameNew = moveFilenameNoExt + "_" + i;
                i++;
            }

            try
            {
                file.MoveTo(PathToMoveTo + moveFilenameNew + file.Extension);
                movedFile = PathToMoveTo + moveFilenameNew + file.Extension;
                return true;
            }

             

            catch (IOException e)
            {
                MessageBox.Show(
                      "{0}: Cannot send to rubbish " +
                      "because the file is in use.",
                      e.GetType().Name);
                return false;
            }



        }

        //Youtube D
        /// <summary>
        /// Return a youtube URi
        /// </summary>
        /// <param name="searchLink"></param>
        /// <param name="FileToSaveAs"></param>
        /// <returns></returns>
        /// 
        public Uri YoutubeSearch(string searchLink)
        {
            Uri uri = new Uri(searchLink);
            return uri;
        }


        //browser.Navigate(uri)
        private void youtubeWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        private void youtubeWorker_DoWork(object sender, DoWorkEventArgs e)
        {


        }
        private void youtubeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public static List<string> AddFavoritesToGenre(string genreXML)
        {
            string genreName;
            List<string> genreList = new List<string>();
            // Check to see if Genre.XML exists before populating
            if (File.Exists(genreXML))
            {
                XmlTextReader reader = new XmlTextReader(genreXML);
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                    {
                        if (reader.HasAttributes)
                        {
                            genreName = reader.GetAttribute("name");
                            StringBuilder newGenre = new StringBuilder(genreName);

                            genreList.Add(newGenre.ToString());
                        }
                    }
                }

                genreList.Add("Favorites");
                genreList.Sort();
                reader.Close();
            }
            else
            {
                genreList.Add("Favorites");

            }


            return genreList;


        }
        internal void SerializeToXML(List<DatabaseGame> games, string systemname, string dbName)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //  Add lib namespace with empty prefix
            ns.Add("", "");
            XmlRootAttribute root = new XmlRootAttribute("menu");
            //XmlSerializer serializer = new XmlSerializer(typeof(BindingList<Table>), root);
            XmlSerializer serializer = new XmlSerializer(typeof(List<DatabaseGame>), root);

            if (!Directory.Exists(HSPath + @"\Databases\" + systemname))
                Directory.CreateDirectory(HSPath + @"\Databases\" + systemname);

            dbName = dbName + ".xml";
            string newPath = System.IO.Path.Combine(HSPath + "\\Databases\\" + systemname + "\\", dbName);
            TextWriter textWriter = new StreamWriter(newPath);
            serializer.Serialize(textWriter, games, ns);
            textWriter.Close();
        }
        internal void SerializeToGenreXML(List<DatabaseGame> games, string systemname, string dbName)
        {

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //tables.RemoveAt(0);
            //  Add lib namespace with empty prefix
            ns.Add("", "");
            XmlRootAttribute root = new XmlRootAttribute("menu");
            //XmlSerializer serializer = new XmlSerializer(typeof(BindingList<Table>), root);
            XmlSerializer serializer = new XmlSerializer(typeof(List<DatabaseGame>), root);

            if (!Directory.Exists(HSPath + @"\Databases\" + systemname))
                Directory.CreateDirectory(HSPath + @"\Databases\" + systemname);

            dbName = dbName + ".xml";
            string newPath = System.IO.Path.Combine(HSPath + "\\Databases\\" + systemname + "\\", dbName);
            TextWriter textWriter = new StreamWriter(newPath);
            serializer.Serialize(textWriter, games, ns);
            textWriter.Close();
        }


    }
    
}
