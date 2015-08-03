using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Hypermint
{
    public class Media : HyperMintBaseClass
    {
        public List<DatabaseGame> DatabaseGame;
        public string systemName;
        public string FullPath;
        public string FileName { get; set; }

        public Media()
        {
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        public void RunScan()
        {
            //bw.RunWorkerAsync();
        }

        public virtual void DroppedFile(string FullPath, string[] filelist)
        {

        }
        public virtual void DroppedFile(string[] filelist, DatabaseGame game, string selectedColumn)
        {

        }

    }

    public class Videos : Media
    {
        public static string FileToSaveAs { get; set; }
        public static string DownloadLink { get; set; }

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
        /// <summary>
        /// Download youtube video supplying URl & format size
        /// </summary>
        public static void DownloadVideo(string videoUrl,int videoSize)
        {
            StringBuilder newURL = new StringBuilder(videoUrl);
            newURL.Replace(@"https://", "");
            DownloadLink = newURL.ToString();

            if (DownloadLink.Contains("www.youtube.com/watch"))
            {
                string Exe = @"Tools\YT_Horse.exe";
                ProcessStartInfo YoutubeDownload = new ProcessStartInfo();
                YoutubeDownload.FileName = Exe;
                YoutubeDownload.Arguments = DownloadLink + " " + "\"" + FileToSaveAs + "\"" + " " + videoSize;
                Process.Start(YoutubeDownload);
            }
            else
            {
                System.Windows.MessageBox.Show("No video selected.");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public static Uri YoutubeSearch(DatabaseGame game, string systemName)
        {
            if (game == null)
                return new Uri("");

            string GameName = game.Description;
            string YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName;
            YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName + "+" + systemName;

            return GetUriForYoutube(YoutubeSearchLink);
        }

        //Youtube D
        /// <summary>
        /// Return a youtube URi
        /// </summary>
        /// <param name="searchLink"></param>
        /// <param name="FileToSaveAs"></param>
        /// <returns></returns>
        /// 
        private static Uri GetUriForYoutube(string searchLink)
        {
            Uri uri = new Uri(searchLink);
            return uri;
        }
    }

    /// <summary>
    /// Class to manage Hyperspin media
    /// </summary>
    public class HyperSpinMedia : Media
    {
        public HyperSpinMedia()
        {           
        }
        public HyperSpinMedia(List<DatabaseGame> games)
        {
            DatabaseGame = games;
            bw.DoWork += bw_DoWork;
        }

        /// <summary>
        /// Scan a hyperspin media path against a list of games to show media not used.
        /// </summary>
        /// <param name="games"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<FileInfo> ScanMediaPathForUnused(List<DatabaseGame> games, string path)
        {
            //Get all files from media path to a FIleInfo array
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fi;
            fi = di.GetFiles("*.*");

            // itterate through the files against the list of games using linq
            // Return all unused
            List<FileInfo> UnusedFiles = new List<FileInfo>();
            foreach (FileInfo file in fi)
            {
                bool exist = games.Exists(x => x.RomName == System.IO.Path.GetFileNameWithoutExtension(file.Name));
                if (!exist)
                {
                    UnusedFiles.Add(file);
                }
            }

            return UnusedFiles;
        }

        public override void DroppedFile(string[] filelist, DatabaseGame game, string selectedColumn)
        {
            int i;
            string Filename, ext, filename3;
            
            for (i = 0; i < filelist.Length; i++)
            {
                Filename = System.IO.Path.GetFileName(filelist[i]);
                ext = System.IO.Path.GetExtension(filelist[i]);
                filename3 = System.IO.Path.GetFileNameWithoutExtension(filelist[i]);

                if (selectedColumn == "Wheel" || selectedColumn == "Artwork1" || selectedColumn == "Artwork2"
                    || selectedColumn == "Artwork3" || selectedColumn == "Artwork4" || selectedColumn == "Video"
                    || selectedColumn == "Background")
                {
                    wheel_drop(filelist[i], filename3, game, selectedColumn);
                }
                else if (selectedColumn == "Theme")
                {
                    theme_drop(filelist[i], filename3, game, selectedColumn);
                }
                else if (selectedColumn =="BG-Music")
                {
                    audio_drop(filelist[i], filename3, game, selectedColumn);
                }
            }
        }
        private void wheel_drop(string file, string filenamenoExt, DatabaseGame game, string selectedColumn)
        {
            string ext = Path.GetExtension(file);
            string path = Path.GetDirectoryName(file);
            string endPath = string.Empty;

            //try { File.Copy(file, file + "_temp"); }
            //catch { }

            try
            {
                if (ext == ".bmp" || ext == ".gif")
                    endPath = Path.Combine(path, game.RomName + ".png");
                else
                    endPath = Path.Combine(path, game.RomName + ext);

                if (File.Exists(endPath))
                {
                    FileManagement.SendToTrash("Hyperspin", new FileInfo(endPath), systemName, selectedColumn, game.RomName);
                }

                if (ext == ".bmp" || ext == ".gif")
                    path = FullPath + game.RomName + ext;

                if (file.EndsWith(".jpg") || file.EndsWith(".gif") || file.EndsWith(".jpeg")
                                 || file.EndsWith(".JPG") || file.EndsWith(".bmp"))
                {
                    using (System.Drawing.Image im = System.Drawing.Image.FromFile(file))
                    {
                        im.Save(Path.Combine(path, game.RomName + ".png"), ImageFormat.Png);
                        //UpdateGameBools(game);
                    }
                    return;
                }
                else if (file.EndsWith(".PNG") || file.EndsWith(".png") || file.EndsWith(".mp4"))
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    System.IO.File.Copy(file, endPath);
                    //File.Delete(file + "_temp");
                }
            }
            catch (Exception)
            {

            }

            //UpdateGameBools(game);

        }
        private void theme_drop(string file, string filenamenoExt, DatabaseGame game, string selectedColumn)
        {
            string ext = Path.GetExtension(file);
            string path;
            // if extension is a zip just rename & copy over for now
            if (ext == ".zip")
            {
                path = FullPath + game.RomName + ext;

                if (File.Exists(path))
                {
                    FileManagement.SendToTrash("Hyperspin", new FileInfo(path), systemName, selectedColumn, game.RomName);
                }

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
                    zip.Save(FullPath + "\\" + game.RomName + ".zip");
                    System.IO.File.Delete("Background.png");
                }
            }
            catch (Exception)
            {


            }

        }
        private void audio_drop(string file, string filenamenoExt, DatabaseGame game, string selectedColumn)
        {
            string ext = Path.GetExtension(file);
            string path;
            FullPath = Path.GetDirectoryName(file);
            // if extension is a zip just rename & copy over for now
            if (ext == ".mp3")
            {
                path = FullPath + "\\" + game.RomName + ext;

                if (File.Exists(path))
                {
                    FileManagement.SendToTrash("Hyperspin", new FileInfo(path), systemName, selectedColumn, game.RomName);
                }

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(FullPath);

                System.IO.File.Copy(file, path);
                game.HaveBGMusic = true;
                return;
            }

        }

        /// <summary>
        /// Grab from info.text inside of theme.zip
        /// </summary>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public static string GetThemeInfoText(string zipPath)
        {
            if (File.Exists(zipPath))
            {
                try
                {
                    string themeInfo = string.Empty;
                    using (ZipFile zip1 = ZipFile.Read(zipPath))
                    {
                        foreach (ZipEntry e in zip1)
                        {
                            if (e.FileName == "Info.txt")
                            {
                                System.IO.File.Delete("Info.txt");
                                e.Extract();
                                using (StreamReader sr = System.IO.File.OpenText("Info.txt"))
                                {
                                    themeInfo = sr.ReadLine();
                                }
                            }
                        }
                    }
                    return themeInfo;
                }
                catch (Exception)
                {
                    return "No theme";
                }

            }
            else
                return string.Format("File doesn't exist : {0}", zipPath);
        }

        /// <summary>
        /// Scan the game list for hyperspin artwork and set bools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                manu = string.Empty, genre = string.Empty, rating = string.Empty, enabled = string.Empty;

            int i = 0;

            foreach (DatabaseGame node in DatabaseGame)
            {

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (systemName != "_Default")
                {
                    if (systemName == "Main Menu")
                    {
                        // Scan the extra Hyperspin Art folders
                        // If the folder doesn't exist for the system create it before scan
                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Images\\Letters";
                        DatabaseGame.ElementAt(i).HaveLetters = FileManagement.CheckMediaFolderFiles(FullPath, "*.png");
                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Images\\Special";
                        DatabaseGame.ElementAt(i).HaveSpecial = FileManagement.CheckMediaFolderFiles(FullPath, "*.png");
                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Images\\Genre\\Wheel";
                        DatabaseGame.ElementAt(i).HaveGenreWheel = FileManagement.CheckMediaFolderFiles(FullPath, "*.png");
                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Images\\Genre\\Backgrounds\\";
                        DatabaseGame.ElementAt(i).HaveGenreBG = FileManagement.CheckMediaFolderFiles(FullPath, "*.png");
                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Images\\Other";
                        DatabaseGame.ElementAt(i).HavePointer = FileManagement.CheckMediaFolderFiles(FullPath, "*.png");

                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Sound\\";
                        DatabaseGame.ElementAt(i).HaveS_Click = FileManagement.CheckMediaFolderFiles(FullPath, "Wheel Click.mp3");

                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Sound\\Wheel Sounds\\";
                        DatabaseGame.ElementAt(i).HaveS_Wheel = FileManagement.CheckMediaFolderFiles(FullPath, "*.mp3");

                        FullPath = HSPath + "\\Media\\" + node.RomName + "\\Sound\\Background Music\\" + node.RomName + ".mp3";
                        DatabaseGame.ElementAt(i).HaveBGMusic = FileManagement.CheckForFile(FullPath);

                    }
                    else
                    {
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Artwork1\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                        DatabaseGame.ElementAt(i).HaveArt1 = FileManagement.CheckForFile(FullPath);
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Artwork2\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                        DatabaseGame.ElementAt(i).HaveArt2 = FileManagement.CheckForFile(FullPath);
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Artwork3\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                        DatabaseGame.ElementAt(i).HaveArt3 = FileManagement.CheckForFile(FullPath);
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Artwork4\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                        DatabaseGame.ElementAt(i).HaveArt4 = FileManagement.CheckForFile(FullPath);
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Backgrounds\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                        DatabaseGame.ElementAt(i).HaveBackgroundsHS = FileManagement.CheckForFile(FullPath);

                        FullPath = HSPath + "\\Media\\" + systemName + "\\Sound\\Background Music\\" + DatabaseGame.ElementAt(i).RomName + ".mp3";
                        DatabaseGame.ElementAt(i).HaveBGMusic = FileManagement.CheckForFile(FullPath);

                        FullPath = HSPath + "\\Media\\" + systemName + "\\Sound\\System Start\\";
                        DatabaseGame.ElementAt(i).HaveS_Start = FileManagement.CheckMediaFolderFiles(FullPath, "*.mp3");
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Sound\\System Exit\\";
                        DatabaseGame.ElementAt(i).HaveS_Exit = FileManagement.CheckMediaFolderFiles(FullPath, "*.mp3");


                    }

                    FullPath = HSPath + "\\Media\\" + systemName + "\\Images\\Wheel\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                    DatabaseGame.ElementAt(i).HaveWheel = FileManagement.CheckForFile(FullPath);
                    FullPath = HSPath + "\\Media\\" + systemName + "\\Themes\\" + DatabaseGame.ElementAt(i).RomName + ".zip";
                    DatabaseGame.ElementAt(i).HaveTheme = FileManagement.CheckForFile(FullPath);

                    //Video slightly different, where you have flvs & pngs
                    FullPath = HSPath + "\\Media\\" + systemName + "\\Video\\" + DatabaseGame.ElementAt(i).RomName + ".mp4";
                    if (!FileManagement.CheckForFile(FullPath))
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Video\\" + DatabaseGame.ElementAt(i).RomName + ".flv";
                    if (!FileManagement.CheckForFile(FullPath))
                        FullPath = HSPath + "\\Media\\" + systemName + "\\Video\\" + DatabaseGame.ElementAt(i).RomName + ".png";
                    if (!FileManagement.CheckForFile(FullPath))
                        DatabaseGame.ElementAt(i).HaveVideo = false;
                    else
                        DatabaseGame.ElementAt(i).HaveVideo = true;

                }
                (sender as BackgroundWorker).ReportProgress(i);
                i++;
            }
        }
        public override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //base.bw_RunWorkerCompleted(sender, e);

        }
    }

    /// <summary>
    /// Class to manage rocketlauncher media
    /// 
    /// </summary>
    public class RocketlaunchMedia : Media
    {
        public string Author { get; set; }
        public string Description { get; set; }
        public bool Resize { get; set; }
        public bool Stretch { get; set; }
        public enum RLMediaType
        {
            Fade1,
            Fade2,
            Fade3,
            FadeExit,
            PauseBG,
            BezelBG
        };

        public RocketlaunchMedia()
        {
        }

        /// <summary>
        /// Initilzi with a list of games
        /// </summary>
        /// <param name="games"></param>
        public RocketlaunchMedia(List<DatabaseGame> games, string rlmedia,string SystemName)
        {
            DatabaseGame = games;
            RLMediaPath = rlmedia;
            systemName = SystemName;
            bw.DoWork +=bw_DoWork;
        }

        private string systemName;

        /// <summary>
        /// Deal with dropped media files
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="filelist"></param>
        public override void DroppedFile(string FullPath, string[] filelist)
        {
            int i;
            string Filename, ext, filename3, path;

            if (!Directory.Exists(FullPath))
                Directory.CreateDirectory(FullPath);

            for (i = 0; i < filelist.Length; i++)
            {
                Filename = System.IO.Path.GetFileName(filelist[i]);
                ext = System.IO.Path.GetExtension(filelist[i]);
                filename3 = System.IO.Path.GetFileNameWithoutExtension(filelist[i]);
                path = null;

                path = FullPath + "\\" + Filename;
                System.IO.File.Copy(filelist[i], path);
            }
        }

        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
               manu = string.Empty, genre = string.Empty, rating = string.Empty, enabled = string.Empty;
            int i = 0;

            // Scan the extra Hyperspin Art folders
            // If the folder doesn't exist for the system create it before scan
            FullPath = RLMediaPath + "\\Artwork\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveArtwork = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Backgrounds\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveBackgrounds = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Bezels\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveBezels = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            DatabaseGame.ElementAt(i).HaveCards = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Controller\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveController = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveFade = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Guides\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveGuide = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Manuals\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveManual = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\MultiGame\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveMultiGame = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Music\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveMusic = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Saved Games\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveSaves = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");
            FullPath = RLMediaPath + "\\Videos\\" + systemName + "\\_Default" + "\\";
            DatabaseGame.ElementAt(i).HaveVideoXT = FileManagement.CheckMediaFolderFiles(FullPath, "*.*");

            foreach (DatabaseGame node in DatabaseGame)
            {

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                FullPath = RLMediaPath + "\\Artwork\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveArtwork = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Backgrounds\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveBackgrounds = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Bezels\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveBezels = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                DatabaseGame.ElementAt(i).HaveCards = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Controller\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveController = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Fade\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveFade = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Guides\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveGuide = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Manuals\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveManual = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\MultiGame\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveMultiGame = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Music\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveMusic = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Saved Games\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveSaves = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");
                FullPath = RLMediaPath + "\\Videos\\" + systemName + "\\" + node.RomName + "\\";
                DatabaseGame.ElementAt(i).HaveVideoXT = FileManagement.doesDirectoryContainFiles(FullPath, "*.*");

                i++;
            }
        }
        public override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (!e.Cancelled)
            //{
            //    HM.InfoMessage = "RL media scan complete:              " + HM.XMLPath;
            //    rectRLScan.Fill = System.Windows.Media.Brushes.Green;
            //}
            //else
            //{
            //    HM.InfoMessage = "RL media scan cancelled";
            //    rectRLScan.Fill = System.Windows.Media.Brushes.Red;
            //}

            //progressBar.IsActive = false;
        }

    }
    /// <summary>
    /// Class for the bezel editing for images, backgrounds and ini
    /// </summary>
    class BezelEdit : RocketLaunch
    {
        public int pX, pY, X, Y;
        public string SelectedFilenamePath { get; set; }
        public string SelectedScreenNamePath { get; set; }
        public bool draggedOver { get; set; }
        public string folderPath { get; set; }
        string ratio;

        public string Filename { get; set; }

        public BezelEdit()
        {

        }

        public BezelEdit(string filename)
        {
            this.folderPath = filename;
        }

        /// <summary>
        /// Supply the bezels ini file and return a string array with the points
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public string[] GetValueFromINI(string Filename)
        {
            string[] bezelInipoints = new string[4];
            IniFileReader ini = new IniFileReader(Filename);
            bezelInipoints[0] = ini.IniReadValue("General", "Bezel Screen Top Left X Coordinate");
            bezelInipoints[1] = ini.IniReadValue("General", "Bezel Screen Top Left Y Coordinate");
            bezelInipoints[2] = ini.IniReadValue("General", "Bezel Screen Bottom Right X Coordinate");
            bezelInipoints[3] = ini.IniReadValue("General", "Bezel Screen Bottom Right Y Coordinate");
            return bezelInipoints;
        }

        /// <summary>
        /// Saves the ini with new naming convention
        /// </summary>
        /// <param name="Inipoints"></param>
        /// <param name="ratio"></param>
        /// <param name="desc"></param>
        /// <param name="author"></param>
        public void saveIni(string[] Inipoints, string ratio, string desc, string author)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(SelectedFilenamePath);
            string pathDirectoryName = System.IO.Path.GetDirectoryName(SelectedFilenamePath);
            string ext = System.IO.Path.GetExtension(SelectedFilenamePath);
            string fullpath = pathDirectoryName + "\\" + filename + ".ini";
            string newBezelName = "Bezel - " + ratio + " " + desc + @" (" + author + @")";
            string tempBezelName = newBezelName;
            int i = 1;

            if (!File.Exists(pathDirectoryName + "\\" + newBezelName + ext))
            {
                File.Copy(SelectedFilenamePath, pathDirectoryName + "\\" + newBezelName + ext);
                fullpath = pathDirectoryName + "\\" + newBezelName + ".ini";
            }
            else
            {
                while (File.Exists(pathDirectoryName + tempBezelName + ext))
                {
                    tempBezelName = "Bezel - " + ratio + " " + desc + " (" + i + ") " + "(" + author + ")";
                    i++;
                }

                try
                {
                    File.Copy(SelectedFilenamePath, pathDirectoryName + "\\" + tempBezelName + ext);
                }
                catch (Exception)
                {
                                       
                }
                
                fullpath = pathDirectoryName + "\\" + tempBezelName + ".ini";
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullpath))
            {
                sw.WriteLine("[General]");
                sw.WriteLine("Bezel Screen Top Left X Coordinate=" + Inipoints[0]);
                sw.WriteLine("Bezel Screen Top Left Y Coordinate=" + Inipoints[1]);
                sw.WriteLine("Bezel Screen Bottom Right X Coordinate=" + Inipoints[2]);
                sw.WriteLine("Bezel Screen Bottom Right Y Coordinate=" + Inipoints[3]);
                sw.Close();
            }


        }

        // OTHER SHIT NEEDED THAT DOESNT BELONG HERE
        // Enabled the crosshair in the gui window
        // this.Cursor = Cursors.Cross;
        // authorText.Text = Properties.Settings.Default.Author;


        //private void imageCtrl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    Point locationInControl = e.GetPosition(bezelImage);
        //    LeftClickX.Text = locationInControl.X.ToString();
        //}
    }
    public class InstructionCard : RocketlaunchMedia
    {
        /// <summary>
        /// Position of where the card should be placed on the screen
        /// </summary>
        public string Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool JPEG { get; set; }
        public string FullPath { get; set; }

        public InstructionCard()
        {

        }

        public InstructionCard(string file, string[] elements, string fullPath)
        {
            FileName = file;
            Author = elements[0];
            Description = elements[1];
            Position = elements[2];
            Width = Convert.ToInt32(elements[3]);
            Height = Convert.ToInt32(elements[4]);
            Resize = Convert.ToBoolean(elements[5]);
            Stretch = Convert.ToBoolean(elements[6]);
            FullPath = fullPath;
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

    public class OtherImages : RocketlaunchMedia
    {
        public OtherImages()
        {

        }
    }

}
