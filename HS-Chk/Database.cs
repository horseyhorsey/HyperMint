using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Collections;

namespace Hypermint
{
    /// <summary>
    /// Databases for Hyperspin - Base class
    /// </summary>
    /// 

    public class Database : HyperMintBaseClass, INotifyPropertyChanged, IHyperminSelected2, IHyperspinSystem
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        [XmlIgnore]
        public ObservableCollection<DatabaseGame> DatabaseGameCollection = new ObservableCollection<DatabaseGame>();

        private string systemName;
        public string SystemName
        {
            get
            {
                return systemName;
            }
            set
            {
                systemName = value;
            }
        }

        public Database()
        {
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;           
        }

        [XmlIgnore]
        public string HyperspinDatabaseXml { get; set; }

        private int gameCount;
        [XmlIgnore]
        public int GameCount
        {
            get { return gameCount; }
            set { gameCount = value; }
        }

        private string fullPath;
        [XmlIgnore]
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value;
            }
        }
        /// <summary>
        /// Is the cell Hyperspin or Rocketlaunch
        /// </summary>
        bool isHyperspin;
        [XmlIgnore]
        public bool IsHyperspin
        {
            get
            {
                return isHyperspin;
            }
            set
            {
                isHyperspin = value;
            }
        }

        /// <summary>
        /// Set the systems xml path and count the number of games.
        /// </summary>
        /// <returns></returns>
        public string SetDatabaseFileAndCount()
        {
            // Load the system xml from directory
            XmlDocument xdoc = new XmlDocument();
            HyperspinDatabaseXml = HSPath + @"\Databases\" + SystemName + "\\" + SystemName + ".xml";
            try
            {
                try
                {
                    xdoc.Load(HyperspinDatabaseXml);
                }
                catch (Exception)
                {
                    return string.Format("No xml found for this system : {0}", HyperspinDatabaseXml);
                }

                GameCount = xdoc.SelectNodes("menu/game").Count;

                //progressBar.Maximum = HM.GameCount;
                //DisableGUIControls();

                //bw.WorkerReportsProgress = true;
                //bw.WorkerSupportsCancellation = true;


                //bw.DoWork -= bw_DoWorkMultiSystem;
                //bw.RunWorkerCompleted -= bw_RunWorkerMultiSystemCompleted;
                //bw.DoWork -= bw_DoWorkGetAllFavories;
                //bw.RunWorkerCompleted -= bw_RunWorkerFaveScanSystemCompleted;

                //bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                //bw.ProgressChanged += bw_ProgressChanged;
                //bw.DoWork += bw_DoWork;


                //if (!bw.IsBusy)
                //{
                //    progressBar.IsActive = true;
                //    HM.InfoMessage = "Building list from:  " + HM.XMLPath;
                //    bw.RunWorkerAsync();
                //}

            }
            catch (Exception)
            {
                //EnableGUIControls();
            }

            return "";
        }       
        static internal void SerializeToXML(List<DatabaseGame> databaselist, string systemname, string dbName, string HSPath = "")
        {            
            if (dbName != "Favorites")
                databaselist.RemoveAt(0);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //  Add lib namespace with empty prefix
            ns.Add("", "");
            XmlRootAttribute root = new XmlRootAttribute("menu");
            //XmlSerializer serializer = new XmlSerializer(typeof(BindingList<Table>), root);
            XmlSerializer serializer = new XmlSerializer(typeof(List<DatabaseGame>), root);

            if (!Directory.Exists(HSPath + @"\Databases\" + systemname))
                Directory.CreateDirectory(HSPath + @"\Databases\" + systemname);

            dbName = dbName + ".xml";
            string newPath = Path.Combine(HSPath + "\\Databases\\" + systemname + "\\", dbName);
            TextWriter textWriter = new StreamWriter(newPath);
            serializer.Serialize(textWriter, (List<DatabaseGame>)databaselist, ns);
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

        /// <summary>
        /// Serialize Main Menu objects to to xml. Provide menuXmlName no ".xml" extension
        /// </summary>
        /// <param name="menuXml"></param>
        public void SerializeMainMenuItems(string menuXmlName, List<DatabaseMenu> listToSerialize, string pathToSave)
        {
            if (!System.IO.Directory.Exists(pathToSave))
                System.IO.Directory.CreateDirectory(pathToSave);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlRootAttribute root = new XmlRootAttribute("menu");
            XmlSerializer serializer = new XmlSerializer(typeof(List<DatabaseMenu>), root);
            System.IO.TextWriter textWriter = new System.IO.StreamWriter(pathToSave + "\\" + menuXmlName + ".xml");

            serializer.Serialize(textWriter, listToSerialize, ns);
            textWriter.Close();
        }
        public void SerializeGenreItems(List<DatabaseMenu> listToSerialize, string pathToSave)
        {
            if (!System.IO.Directory.Exists(pathToSave))
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));

            }

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlRootAttribute root = new XmlRootAttribute("menu");
            XmlSerializer serializer = new XmlSerializer(typeof(List<DatabaseMenu>), root);
            System.IO.TextWriter textWriter = new System.IO.StreamWriter(pathToSave);

            serializer.Serialize(textWriter, listToSerialize, ns);
            textWriter.Close();
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="dbname"></param>
        public void OpenDatabase(string dbname)
        {

        }
        /// <summary>
        /// Get the systems from a HS main menu.xml - Add Main Menu to list
        /// </summary>
        /// <returns></returns>
        public string[] getSystems()
        {
            string[] sysname;
            // Using a using block to dispose of the text reader.
            //
            using (XmlTextReader reader = new XmlTextReader(HyperspinDatabaseXml))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(HyperspinDatabaseXml);
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
        /// <summary>
        /// Grab all the Main Menu databases inside Hyperspin/Databases
        /// </summary>
        /// <returns>string[] of main menu xmls</returns>
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
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    IsHyperspin = true;
                    break;
                case "Description":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    IsHyperspin = true;
                    break;
                case "Manufacturer":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    IsHyperspin = true;
                    break;
                case "Artwork1":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork1\\";
                    IsHyperspin = true;
                    break;
                case "Artwork2":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork2\\";
                    IsHyperspin = true;
                    break;
                case "Artwork3":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork3\\";
                    IsHyperspin = true;
                    break;
                case "Artwork4":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Artwork4\\";
                    IsHyperspin = true;
                    break;
                case "Theme":
                    IsHyperspin = true;
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Themes\\";
                    break;
                case "Video":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Video\\";
                    IsHyperspin = true;
                    break;
                case "Background":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Backgrounds\\";
                    IsHyperspin = true;
                    break;
                case "Wheel":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Images\\Wheel\\";
                    IsHyperspin = true;
                    break;
                case "Letters":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Letters\\";
                    IsHyperspin = true;
                    break;
                case "Special":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Special\\";
                    IsHyperspin = true;
                    break;
                case "GenreBG":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Genre\\Backgrounds\\";
                    IsHyperspin = true;
                    break;
                case "GenreWheel":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Genre\\Wheel\\";
                    IsHyperspin = true;
                    break;
                case "Pointer":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Images\\Other\\";
                    IsHyperspin = true;
                    break;
                case "Snds-Wheel":
                    FullPath = hsPath + "\\Media\\" + game_click.RomName + "\\Sound\\Wheel Sounds\\";
                    IsHyperspin = true;
                    break;
                case "BG-Music":
                    FullPath = hsPath + "\\Media\\" + SystemName + "\\Sound\\Background Music\\";
                    IsHyperspin = true;
                    break;

                //RocketLaunch Folders
                case "Artwork":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath);
                    IsHyperspin = false;
                    break;
                case "Backgrounds":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Bezels":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Cards":
                    FullPath = setRLMediaPath("Bezels", game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Controller":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Fade":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Guides":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Manuals":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "MultiGame":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Music":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Saves":
                    FullPath = setRLMediaPath("Saved Games", game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                case "Videos":
                    FullPath = setRLMediaPath(columnHeader, game_click.RomName, SystemName, hsPath); IsHyperspin = false;
                    break;
                default:
                    break;
            }

            return FullPath;
        }
        /// <summary>
        /// Set the rocketlauncher path
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="romname"></param>
        /// <param name="systemname"></param>
        /// <param name="MediaPath"></param>
        /// <returns></returns>
        private string setRLMediaPath(string columnName, string romname, string systemname, string MediaPath)
        {
            return FullPath = MediaPath + columnName + "\\" + SystemName + "\\" + romname;
        }

        /// <summary>
        /// Using background worker to build collection of game objects from XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            
            XmlDocument xdoc = new XmlDocument();            
            xdoc.Load(HyperspinDatabaseXml);
            DatabaseGameCollection = new ObservableCollection<DatabaseGame>();
            string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                manu = string.Empty, genre = string.Empty, rating = string.Empty;
            int enabled = 0;
            int year = 0;
            string index = string.Empty;
            int i = 0;
            string lastRom = string.Empty;

            DatabaseGameCollection.Add(new DatabaseGame("_Default", "_Default"));
            foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                name = node.SelectSingleNode("@name").InnerText;

                char s = name[0];
                char t;

                if (lastRom != string.Empty)
                {
                    t = lastRom[0];
                    if (char.ToLower(s) == char.ToLower(t))
                    {
                        index = string.Empty;
                        image = string.Empty;
                    }
                    else
                    {
                        index = "true";
                        image = char.ToLower(s).ToString();
                    }
                }

                if (SystemName != "Main Menu")
                {
                    if (node.SelectSingleNode("@enabled") != null)
                    {
                        if (node.SelectSingleNode("@enabled").InnerText != null)
                        {
                            enabled = Convert.ToInt32(node.SelectSingleNode("@enabled").InnerText);
                        }
                    }
                    else
                        enabled = 1;


                    desc = node.SelectSingleNode("description").InnerText;

                    if (node.SelectSingleNode("cloneof") != null)
                        cloneof = node.SelectSingleNode("cloneof").InnerText;
                    if (node.SelectSingleNode("crc") != null)
                        crc = node.SelectSingleNode("crc").InnerText;
                    if (node.SelectSingleNode("manufacturer") != null)
                        manu = node.SelectSingleNode("manufacturer").InnerText;
                    if (node.SelectSingleNode("year") != null)
                        if (!string.IsNullOrEmpty(node.SelectSingleNode("year").InnerText))
                            Int32.TryParse(node.SelectSingleNode("year").InnerText, out year);

                    if (node.SelectSingleNode("genre") != null)
                        genre = node.SelectSingleNode("genre").InnerText;
                    if (node.SelectSingleNode("rating") != null)
                        rating = node.SelectSingleNode("rating").InnerText;

                }

                DatabaseGameCollection.Add(new DatabaseGame(name, index, image, desc, cloneof, crc, manu, year, genre, rating, enabled));

                lastRom = name;
                i++;
            }

            /// Scan the favorites text, set the hspath and systemname 
            Favorites f = new Favorites();
            f.HSPath = HSPath;
            f.SystemName = SystemName;
            Favorites.dbPath = HSPath + "\\Databases\\";
            List<string> faveText = Favorites.GetFavoritesFromText(f.SystemName);
            Favorites.updateGameListFavorites(DatabaseGameCollection.ToList(), faveText);

            // Return the new collection to the backgroundworker
            e.Result = DatabaseGameCollection;
        }
       
    }

    [XmlType(TypeName = "game")]
    public class DatabaseMenu : Database
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        private int enabled;
        [XmlAttribute("enabled")]
        public int Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                this.NotifyPropertyChanged("enabled");
            }
        }
        private Uri sysIcon;
        [XmlIgnore]
        public Uri SysIcon
        {
            get { return sysIcon; }
            set
            {
                sysIcon = value;
                NotifyPropertyChanged("SysIcon");
            }
        }
        private bool xmlExists;
        [XmlIgnore]
        public bool XmlExists
        {
            get { return xmlExists; }
            set { xmlExists = value; }
        }
        private bool genreExists;
        [XmlIgnore]
        public bool GenreExists
        {
            get { return genreExists; }
            set { genreExists = value; }
        }

        public DatabaseMenu()
        { }
        public DatabaseMenu(string _name)
        {
            name = _name;

        }
        public DatabaseMenu(string _name, int _enabled = 1)
        {
            name = _name;
            this.Enabled = _enabled;
        }
        public DatabaseMenu(string _name, Uri pathToIcon, int _enabled = 1)
        {
            this.name = _name;
            Enabled = _enabled;
            if (pathToIcon != null)
                this.sysIcon = pathToIcon;
        }

        public List<DatabaseMenu> GetMainMenuItemsFromXml(string xmlPath, string pathToIcons = "")
        {
            if (!System.IO.File.Exists(xmlPath))
                return null;

            List<DatabaseMenu> tempMenuList = new List<DatabaseMenu>();

            using (XmlTextReader reader = new XmlTextReader(xmlPath))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(xmlPath);

                int sysCount = xdoc.SelectNodes("menu/game").Count;
                Uri img;

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            string name = reader.GetAttribute("name");
                            int enabled = Convert.ToInt32(reader.GetAttribute("enabled"));
                            string icon = pathToIcons + "\\" + name + ".png";

                            if (!System.IO.File.Exists(icon))
                                tempMenuList.Add(new DatabaseMenu(name, enabled));
                            else
                            {
                                img = new Uri(icon);
                                tempMenuList.Add(new DatabaseMenu(name, img, enabled));
                            }
                        }
                }
            }

            return tempMenuList;
        }

    }

    [XmlType(TypeName = "game")]
    public partial class DatabaseGame : Database, IHyperspinGame, IRocketLaunchStats, IHyperspinFavorite
    {
        public DatabaseGame() 
        {

        }
        /// <summary>
        /// standard romname & desc contructor
        /// </summary>
        /// <param name="Gamename"></param>
        /// <param name="Description"></param>
        public DatabaseGame(string Gamename, string Description)
        {
            this.RomName = Gamename;
            this.Description = Description;
        }
        /// <summary>
        /// To Construct game object from XML
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="image"></param>
        /// <param name="desc"></param>
        /// <param name="cloneof"></param>
        /// <param name="crc"></param>
        /// <param name="manu"></param>
        /// <param name="year"></param>
        /// <param name="genre"></param>
        /// <param name="rating"></param>
        /// <param name="enabled"></param>
        public DatabaseGame(string name, string index, string image, string desc, string cloneof,
            string crc, string manu, int year, string genre, string rating, int enabled)
        {
            this.RomName = name;
            this.Index = index;
            this.Image = image;
            this.Description = desc;
            this.CloneOf = cloneof;
            this.Crc = crc;
            this.Manufacturer = manu;
            this.Year = year;
            this.Genre = genre;
            this.Rating = rating;
            this.Enabled = enabled;

        }

        public DatabaseGame(string name, string index, string image, string desc, string cloneof,
            string crc, string manu, int year, string genre, string rating, int enabled, string system)
        {
            RomName = name;
            this.Index = index;
            this.Image = image;
            this.Description = desc;
            this.CloneOf = cloneof;
            this.Crc = crc;
            this.Manufacturer = manu;
            this.Year = year;
            this.Genre = genre;
            this.Rating = rating;
            this.Enabled = enabled;
            this.name = system;

        }
        public string name { get; set; }
        internal DatabaseGame(DatabaseGame game)
        {
            this.RomName = game.RomName;
            this.Description = game.Description;
            this.Genre = game.Genre;
            this.Year = game.Year;
            this.name = game.name;
            this.Rating = game.Rating;
            this.Enabled = game.Enabled;
            this.CloneOf = game.CloneOf;
            this.Index = game.Index;
            this.Image = game.Image;
            this.Manufacturer = game.Manufacturer;
            this.Crc = game.Crc;
        }

        public virtual void UpdateGameBools(string column, string fullPath,DatabaseGame game = null)
        {
            FullPath = fullPath;
            switch (column)
            {
                case "Wheel":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveWheel = true;
                    else
                        game.HaveWheel = false;
                    break;
                case "Background":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveBackgroundsHS = true;
                    else
                        game.HaveBackgroundsHS = false;
                    break;
                case "Artwork1":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveArt1 = true;
                    else
                        game.HaveArt1 = false;
                    break;
                case "Artwork2":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveArt2 = true;
                    else
                        game.HaveArt2 = false;
                    break;
                case "Artwork3":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveArt3 = true;
                    else
                        game.HaveArt3 = false;
                    break;
                case "Artwork4":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
                        game.HaveArt4 = true;
                    else
                        game.HaveArt4 = false;
                    break;
                case "Theme":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".zip"))
                        game.HaveTheme = true;
                    else
                        game.HaveTheme = false;
                    break;
                case "Video":
                    if (FileManagement.CheckForFile(fullPath + game.RomName + ".mp4"))
                        game.HaveVideo = true;
                    else if (FileManagement.CheckForFile(fullPath + game.RomName + ".flv"))
                        game.HaveVideo = true;
                    else if (FileManagement.CheckForFile(fullPath + game.RomName + ".png"))
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
                    break;
                default:
                    break;
            }
        }

        public DatabaseGame checkGameInXml(string system, List<string> systemFaves = null)
        {
            string mainMenuXml = Path.Combine(HSPath + "\\Databases\\", system + "\\" + system + ".xml");
            XDocument xdoc = null;
            using (XmlReader xr = XmlReader.Create(mainMenuXml))
            {
                xdoc = XDocument.Load(xr);
            }

            List<string> lis = new List<string>();

            foreach (var rom in systemFaves)
            {
                var gameVars =
                    from item in xdoc.Descendants("game")
                    where item.Attribute("name").Value.Contains(rom)
                    select new
                    {
                        RomName = (string)item.Attribute("name").Value,
                        Enabled = 1,
                        Description = item.Element("description").Value,
                        CloneOf = item.Element("cloneof").Value,
                        CRC = item.Element("crc").Value,
                        Manufacturer = item.Element("manufacturer").Value,
                        Genre = item.Element("genre").Value,
                        Year = item.Element("year").Value,
                        Rating = item.Element("rating").Value,

                    };

                lis.Add(gameVars.ElementAt(0).RomName);
            }

            return new DatabaseGame();


        }

        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlDocument xdoc = new XmlDocument();            
            xdoc.Load(HyperspinDatabaseXml);
            DatabaseGameCollection = new ObservableCollection<DatabaseGame>();
            string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                manu = string.Empty, genre = string.Empty, rating = string.Empty;
            int enabled = 0;
            int year = 0;
            string index = string.Empty;
            int i = 0;
            string lastRom = string.Empty;

            DatabaseGameCollection.Add(new DatabaseGame("_Default", "_Default"));
            foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                name = node.SelectSingleNode("@name").InnerText;

                char s = name[0];
                char t;

                if (lastRom != string.Empty)
                {
                    t = lastRom[0];
                    if (char.ToLower(s) == char.ToLower(t))
                    {
                        index = string.Empty;
                        image = string.Empty;
                    }
                    else
                    {
                        index = "true";
                        image = char.ToLower(s).ToString();
                    }
                }

                if (SystemName != "Main Menu")
                {
                    if (node.SelectSingleNode("@enabled") != null)
                    {
                        if (node.SelectSingleNode("@enabled").InnerText != null)
                        {
                            enabled = Convert.ToInt32(node.SelectSingleNode("@enabled").InnerText);
                        }
                    }
                    else
                        enabled = 1;


                    desc = node.SelectSingleNode("description").InnerText;

                    if (node.SelectSingleNode("cloneof") != null)
                        cloneof = node.SelectSingleNode("cloneof").InnerText;
                    if (node.SelectSingleNode("crc") != null)
                        crc = node.SelectSingleNode("crc").InnerText;
                    if (node.SelectSingleNode("manufacturer") != null)
                        manu = node.SelectSingleNode("manufacturer").InnerText;
                    if (node.SelectSingleNode("year") != null)
                        if (!string.IsNullOrEmpty(node.SelectSingleNode("year").InnerText))
                            Int32.TryParse(node.SelectSingleNode("year").InnerText, out year);

                    if (node.SelectSingleNode("genre") != null)
                        genre = node.SelectSingleNode("genre").InnerText;
                    if (node.SelectSingleNode("rating") != null)
                        rating = node.SelectSingleNode("rating").InnerText;

                }

                DatabaseGameCollection.Add(new DatabaseGame(name, index, image, desc, cloneof, crc, manu, year, genre, rating, enabled));

                lastRom = name;
                i++;

            }

            e.Result = DatabaseGameCollection;
            //List<DatabaseGame> gamelist = new List<DatabaseGame>(DatabaseGame);
            //HM.buildFavorites(gamelist);

        }
        public override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        
        private string romName;
        [XmlAttribute("name")]
        public string RomName
        {
            get { return romName; }

            set
            {
                romName = value;
                this.NotifyPropertyChanged("romName");
            }

        }
        private int enabled;
        [XmlAttribute("enabled")]
        public int Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                this.NotifyPropertyChanged("enabled");
            }
        }
        #region Main XML Variables
        [XmlIgnore]
        public List<string> GameStats { get; set; }
        private string index;
        [XmlAttribute("index")]
        public string Index
        {
            get { return index; }

            set
            {
                index = value;
            }

        }
        private string image;
        [XmlAttribute("image")]
        public string Image
        {
            get { return image; }

            set
            {
                image = value;
            }

        }
        private string description;
        [XmlElement("description")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("description");
            }
        }
        private string cloneof;
        [XmlElement("cloneof")]
        public string CloneOf
        {
            get { return cloneof; }
            set
            {
                cloneof = value;
                this.NotifyPropertyChanged("cloneof");
            }
        }
        private string crc;
        [XmlElement("crc")]
        public string Crc
        {
            get { return crc; }
            set
            {
                crc = value;
                this.NotifyPropertyChanged("crc");
            }
        }
        private string manufacturer;
        [XmlElement("manufacturer")]
        public string Manufacturer
        {
            get { return manufacturer; }
            set
            {
                manufacturer = value;
                this.NotifyPropertyChanged("manufacturer");
            }
        }
        private string genre;
        [XmlElement("genre")]
        public string Genre
        {
            get { return genre; }
            set
            {
                genre = value;
                this.NotifyPropertyChanged("genre");
            }
        }
        private int year;
        [XmlElement("year")]
        public int Year
        {
            get { return year; }
            set
            {
                year = value;
                this.NotifyPropertyChanged("year");
            }
        }
        private string rating;
        [XmlElement("rating")]
        public string Rating
        {
            get { return rating; }
            set
            {
                rating = value;
                this.NotifyPropertyChanged("rating");
            }
        }
        #endregion      
        #region Properties Accessors for HS
        private bool isFavorite;
        [XmlIgnore]
        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                isFavorite = value;
                this.NotifyPropertyChanged("isFavorite");
            }
        }
        private bool haveWheel;
        [XmlIgnore]
        public bool HaveWheel
        {
            get { return haveWheel; }
            set
            {
                haveWheel = value;
                this.NotifyPropertyChanged("haveWheel");
            }
        }
        private bool haveTheme;
        [XmlIgnore]
        public bool HaveTheme
        {
            get { return haveTheme; }
            set
            {
                haveTheme = value;
                this.NotifyPropertyChanged("haveTheme");
            }
        }
        private bool haveArt1;
        [XmlIgnore]
        public bool HaveArt1
        {
            get { return haveArt1; }
            set
            {
                haveArt1 = value;
                this.NotifyPropertyChanged("haveArt1");
            }
        }
        private bool haveArt2;
        [XmlIgnore]
        public bool HaveArt2
        {
            get { return haveArt2; }
            set
            {
                haveArt2 = value;
                this.NotifyPropertyChanged("haveArt2");
            }
        }
        private bool haveArt3;
        [XmlIgnore]
        public bool HaveArt3
        {
            get { return haveArt3; }
            set
            {
                haveArt3 = value;
                this.NotifyPropertyChanged("haveArt3");
            }
        }
        private bool haveArt4;
        [XmlIgnore]
        public bool HaveArt4
        {
            get { return haveArt4; }
            set
            {
                haveArt4 = value;
                this.NotifyPropertyChanged("haveArt4");
            }
        }
        private bool haveBackgroundsHS;
        [XmlIgnore]
        public bool HaveBackgroundsHS
        {
            get { return haveBackgroundsHS; }
            set
            {
                haveBackgroundsHS = value;
                this.NotifyPropertyChanged("haveBackgroundsHS");
            }
        }
        private bool haveBGMusic;
        [XmlIgnore]
        public bool HaveBGMusic
        {
            get { return haveBGMusic; }
            set
            {
                haveBGMusic = value;
                this.NotifyPropertyChanged("haveBGMusic");
            }
        }
        private bool haveVideo;
        [XmlIgnore]
        public bool HaveVideo
        {
            get { return haveVideo; }
            set
            {
                haveVideo = value;
                this.NotifyPropertyChanged("haveVideo");
            }
        }
        private bool haveGenreWheel;
        [XmlIgnore]
        public bool HaveGenreWheel
        {
            get { return haveGenreWheel; }
            set
            {
                haveGenreWheel = value;
                this.NotifyPropertyChanged("haveGenreArt");
            }
        }
        private bool haveLetters;
        [XmlIgnore]
        public bool HaveLetters
        {
            get { return haveLetters; }
            set
            {
                haveLetters = value;
                this.NotifyPropertyChanged("haveLetters");
            }
        }
        private bool haveSpecial;
        [XmlIgnore]
        public bool HaveSpecial
        {
            get { return haveSpecial; }
            set
            {
                haveSpecial = value;
                this.NotifyPropertyChanged("haveSpecial");
            }
        }
        private bool haveGenreBG;
        [XmlIgnore]
        public bool HaveGenreBG
        {
            get { return haveGenreBG; }
            set
            {
                haveGenreBG = value;
                this.NotifyPropertyChanged("haveGenreBG");
            }
        }
        private bool havePointer;
        [XmlIgnore]
        public bool HavePointer
        {
            get { return havePointer; }
            set
            {
                havePointer = value;
                this.NotifyPropertyChanged("havePointer");
            }
        }

        private bool haveS_Wheel;
        [XmlIgnore]
        public bool HaveS_Wheel
        {
            get { return haveS_Wheel; }
            set
            {
                haveS_Wheel = value;
                this.NotifyPropertyChanged("haveS_Wheel");
            }
        }
        private bool haveS_Start;
        [XmlIgnore]
        public bool HaveS_Start
        {
            get { return haveS_Start; }
            set
            {
                haveS_Start = value;
                this.NotifyPropertyChanged("haveS_Start");
            }
        }

        private bool haveS_Exit;
        [XmlIgnore]
        public bool HaveS_Exit
        {
            get { return haveS_Exit; }
            set
            {
                haveS_Exit = value;
                this.NotifyPropertyChanged("haveS_Exit");
            }
        }

        private bool haveS_Click;
        [XmlIgnore]
        public bool HaveS_Click
        {
            get { return haveS_Click; }
            set
            {
                haveS_Click = value;
                this.NotifyPropertyChanged("haveS_Click");
            }
        }

        #endregion
        #region Properties Accessors for Xtra Media
        private bool haveArtwork;
        [XmlIgnore]
        public bool HaveArtwork
        {
            get { return haveArtwork; }
            set
            {
                haveArtwork = value;
                this.NotifyPropertyChanged("haveArtwork");
            }
        }
        private bool haveBackgrounds;
        [XmlIgnore]
        public bool HaveBackgrounds
        {
            get { return haveBackgrounds; }
            set
            {
                haveBackgrounds = value;
                this.NotifyPropertyChanged("haveBackgrounds");
            }
        }
        private bool haveBezels;
        [XmlIgnore]
        public bool HaveBezels
        {
            get { return haveBezels; }
            set
            {
                haveBezels = value;
                this.NotifyPropertyChanged("haveBezels");
            }
        }
        private bool haveCards;
        [XmlIgnore]
        public bool HaveCards
        {
            get { return haveCards; }
            set
            {
                haveCards = value;
                this.NotifyPropertyChanged("haveCards");
            }
        }
        private bool haveController;
        [XmlIgnore]
        public bool HaveController
        {
            get { return haveController; }
            set
            {
                haveController = value;
                this.NotifyPropertyChanged("haveController");
            }
        }
        private bool haveFade;
        [XmlIgnore]
        public bool HaveFade
        {
            get { return haveFade; }
            set
            {
                haveFade = value;
                this.NotifyPropertyChanged("haveFade");
            }
        }
        private bool haveGuide;
        [XmlIgnore]
        public bool HaveGuide
        {
            get { return haveGuide; }
            set
            {
                haveGuide = value;
                this.NotifyPropertyChanged("haveGuide");
            }
        }
        private bool haveManual;
        [XmlIgnore]
        public bool HaveManual
        {
            get { return haveManual; }
            set
            {
                haveManual = value;
                this.NotifyPropertyChanged("haveManual");
            }
        }
        private bool haveMultiGame;
        [XmlIgnore]
        public bool HaveMultiGame
        {
            get { return haveMultiGame; }
            set
            {
                haveMultiGame = value;
                this.NotifyPropertyChanged("haveMultiGame");
            }
        }
        private bool haveMusic;
        [XmlIgnore]
        public bool HaveMusic
        {
            get { return haveMusic; }
            set
            {
                haveMusic = value;
                this.NotifyPropertyChanged("haveMusic");
            }
        }
        private bool haveSaves;
        [XmlIgnore]
        public bool HaveSaves
        {
            get { return haveSaves; }
            set
            {
                haveSaves = value;
                this.NotifyPropertyChanged("haveSaves");
            }
        }
        private bool haveVideoXT;
        [XmlIgnore]
        public bool HaveVideoXT
        {
            get { return haveVideoXT; }
            set
            {
                haveVideoXT = value;
                this.NotifyPropertyChanged("haveVideoXT");
            }
        }
        #endregion

        public bool ShouldSerializeSystemName()
        {
            if (System == string.Empty)
                return false;
            else
                return true;
        }
        private string system;
        [XmlElement]
        public string System
        {
            get { return system; }
            set
            {
                system = value;
                this.NotifyPropertyChanged("system");
            }
        }

        private int timesPlayed;
        [XmlIgnore]
        public int TimesPlayed
        {
            get
            {
                return timesPlayed;
            }
            set
            {
                timesPlayed = value;
            }
        }
        [XmlIgnore]
        public DateTime LastTimePlayed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        [XmlIgnore]
        public TimeSpan AvgTimePlayed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        [XmlIgnore]
        public TimeSpan TotalTimePlayed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        [XmlIgnore]
        public TimeSpan TotalOverallTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the 4 values from the ini for the game sent to this
        /// times played, last time played , average, total time
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="romName"></param>
        /// <returns></returns>
        public void GetSingleGameStats(DatabaseGame game)
        {
            IniFile ini = new IniFile();
            ini.Load(RLPath + "\\Data\\Statistics\\" + SystemName + ".ini");
            var i = ini.GetSection(RomName);
            if (i == null)
                return;

            try { TimesPlayed = Convert.ToInt32(ini.GetKeyValue(RomName, "Number_of_Times_Played")); }
            catch (Exception) { }

            try { LastTimePlayed = Convert.ToDateTime(ini.GetKeyValue(RomName, "Last_Time_Played")); }
            catch (Exception) { }

            try
            {
                var avgTime = TimeSpan.Parse(ini.GetKeyValue(RomName, "Average_Time_Played")).Days;
                AvgTimePlayed = new TimeSpan(0, 0, avgTime);
            }
            catch (Exception) { }

            try
            {
                var TotalTime = TimeSpan.Parse(ini.GetKeyValue(RomName, "Total_Time_Played")).Days;
                TotalTimePlayed = new TimeSpan(0, 0, TotalTime);
                TotalOverallTime = TotalOverallTime + TotalTimePlayed;
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Build game variables from the database
        /// </summary>
        /// <returns></returns>
        public static DatabaseGame GetAttributesFromGame()
        {
            DatabaseGame game = new DatabaseGame();
            return game;
        }

    }

    public sealed class DatabaseSearch : DatabaseGame
    {
        public List<DatabaseGame> SearchForGame(string searchTerm)
        {
            List<DatabaseGame> searchResult = new List<DatabaseGame>();

            foreach (var item in getSystems())
            {
                if (item != "Main Menu")
                {
                    foreach (var gameItem in SearchXmlForGame(searchTerm,item))                    
                        searchResult.Add(gameItem);                                        
                }
            }
           
            return searchResult;
        }

        /// <summary>
        /// Get Game values for game search
        /// </summary>
        /// <param name="romName"></param>
        /// <param name="system"></param>
        public List<DatabaseGame> SearchXmlForGame(string romName, string system)
        {
            string systemXml = Path.Combine(HSPath + "\\Databases\\", system + "\\" + system + ".xml");
            XDocument xdoc = null;
            List<DatabaseGame> outList = new List<DatabaseGame>();

            using (XmlReader xr = XmlReader.Create(systemXml))
            {
                try
                {
                    xdoc = XDocument.Load(xr);
                }
                catch (XmlException e)
                {

                    MessageBox.Show(e.Message);
                }
            }

            try
            {
                var gameVars =
               from item in xdoc.Descendants("game")
               //where (item.Attribute("name").Value.Contains(romName) && (item.Element("cloneof").Value == ""))
               where (item.Element("description").Value.Contains(romName) && (item.Element("cloneof").Value == ""))

               select new
               {
                   RomName = item.Attribute("name").Value,
                   Enabled = 1,
                   Description = item.Element("description").Value,
                   CloneOf = item.Element("cloneof").Value,
                   CRC = item.Element("crc").Value,
                   Manufacturer = item.Element("manufacturer").Value,
                   Genre = item.Element("genre").Value,
                   Year = Convert.ToInt32(item.Element("year").Value),
                   Rating = item.Element("rating").Value,
                   System = system
               };

                try
                {
                    foreach (var item in gameVars)
                    {
                        if (item != null)
                        {
                            outList.Add(new DatabaseGame(item.RomName, "", "", item.Description, item.CloneOf, item.CRC, item.Manufacturer, item.Year,
                                item.Genre, item.Rating, item.Enabled, system));
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                }
            }
            catch (Exception)
            {

            }

            return outList;
        }

    }

    public class MuliSystemCreator : Database
    {

        public bool CreateSymlinks { get; set; }
        public bool defaultTheme { get; set; }
        public bool CreateRomMap { get; set; }
        public bool CreateGenres { get; set; }
        public string SettingsFile { get; set; }
        public string CurrentMenu { get; set; }
        public string multiSystemName { get; set; }

        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string lastRom = string.Empty;
            List<object> menuandgames = (List<object>)e.Argument;
            List<DatabaseGame> m_System = (List < DatabaseGame >) menuandgames.ElementAt(0);
            List<DatabaseMenu> mm = (List<DatabaseMenu>)menuandgames.ElementAt(1);

            m_System.Sort((x, y) => String.Compare(x.RomName, y.RomName));
            string tempSymlinkFile = string.Empty;
            string FileToLink = string.Empty;
            int i = 0;
            DatabaseMenu m = new DatabaseMenu();
            try
            {
                var a = AppDomain.CurrentDomain.BaseDirectory;
                List<DatabaseGame> removeGameList = new List<DatabaseGame>();

                foreach (var item in m_System)
                {
                    //bool exists = m_System.Exists(x => x.RomName == item.RomName);

                    string name = item.RomName;
                    char s = name[0];
                    char t;

                    if (lastRom != string.Empty)
                    {
                        t = lastRom[0];
                        if (char.ToLower(s) == char.ToLower(t))
                        {
                            m_System.ElementAt(i).Index = string.Empty;
                            m_System.ElementAt(i).Image = string.Empty;
                        }
                        else
                        {
                            m_System.ElementAt(i).Index = "true";
                            m_System.ElementAt(i).Image = char.ToLower(s).ToString();
                        }

                    }
                    else
                    {
                        m_System.ElementAt(i).Index = "true";
                        m_System.ElementAt(i).Image = char.ToLower(s).ToString();
                    }

                    // Remove the rom if it existed
                    if (name == lastRom)
                    {
                        removeGameList.Add(item);
                    }
                    else
                    {
                        //+++++++++++++++++++++++
                        // Symbolic link creation
                        //+++++++++++++++++++++++

                        if (CreateSymlinks)
                        {
                            // Assign variables then Check wheels
                            tempSymlinkFile = HSPath + "\\Media\\" + multiSystemName + @"\Images\Wheel\";
                            SymLink.CreateDirectory(tempSymlinkFile);
                            tempSymlinkFile = System.IO.Path.Combine(a, tempSymlinkFile + item.RomName + ".png");
                            FileToLink = HSPath + "\\Media\\" + item.System + "\\Images\\Wheel\\" + item.RomName + ".png";
                            SymLink.CheckThenCreate(FileToLink, tempSymlinkFile);

                            // Checkin videos and creating links
                            tempSymlinkFile = HSPath + "\\Media\\" + multiSystemName + @"\Video\";
                            SymLink.CreateDirectory(tempSymlinkFile);
                            tempSymlinkFile = System.IO.Path.Combine(a, tempSymlinkFile + item.RomName + ".mp4");
                            FileToLink = HSPath + "\\Media\\" + item.System + "\\Video\\" + item.RomName + ".mp4";
                            SymLink.CheckThenCreate(FileToLink, tempSymlinkFile);

                            // Checkin videos and creating links
                            tempSymlinkFile = HSPath + "\\Media\\" + multiSystemName + @"\Themes\";
                            SymLink.CreateDirectory(tempSymlinkFile);
                            tempSymlinkFile = System.IO.Path.Combine(a, tempSymlinkFile + item.RomName + ".zip");
                            FileToLink = HSPath + "\\Media\\" + item.System + "\\Themes\\" + item.RomName + ".zip";

                            //If include default theme
                            // If a theme doesn't exist copy over the default theme for this system name and rename to the romname
                            if (defaultTheme)
                            {
                                if (!File.Exists(FileToLink))
                                    FileToLink = HSPath + "\\Media\\" + item.System + "\\Themes\\" + "default.zip";
                            }
                            SymLink.CheckThenCreate(FileToLink, tempSymlinkFile);

                            for (int ii = 1; ii < 5; ii++)
                            {
                                // Checking artwork folders and creating links
                                tempSymlinkFile = HSPath + "\\Media\\" + multiSystemName + @"\Images\Artwork" + ii + "\\";
                                SymLink.CreateDirectory(tempSymlinkFile);
                                tempSymlinkFile = System.IO.Path.Combine(a, tempSymlinkFile + item.RomName + ".png");
                                FileToLink = HSPath + "\\Media\\" + item.System + "\\Images\\Artwork" + ii + "\\" + item.RomName + ".png";
                                SymLink.CheckThenCreate(FileToLink, tempSymlinkFile);
                            }

                        }
                        //+++++++++++++++++++++++
                        // End Symbolic link creation
                        //+++++++++++++++++++++++
                    }
                    lastRom = name;
                    i++;
                }

                foreach (var item in removeGameList)
                {
                    m_System.Remove(item);

                }

                // Create Hyperspin settings
                if (File.Exists(SettingsFile))
                {
                    //Directory.CreateDirectory(@"Exports\Hyperspin\Settings\" + multiSystemName);
                    File.Copy(SettingsFile, HSPath + @"\Settings\" + multiSystemName + ".ini", true);
                }

                Database.SerializeToXML(m_System, multiSystemName, multiSystemName, HSPath);

                try
                {
                    // Stop this loop adding the system to the main menu if you already have it
                    bool flag = false;
                    foreach (var itemName in mm)
                    {
                        if (itemName.name == multiSystemName)
                        {
                            flag = true;
                        }
                    }

                    // Add system because it doesn't exist
                    if (!flag)
                        mm.Add(new DatabaseMenu(multiSystemName, 1));

                    string Xml = HSPath + @"\Databases\" + multiSystemName + "\\" + multiSystemName + ".xml";
                    //Create rom mapping for RocketLaunch
                    if (CreateRomMap)
                    {
                        //var koof = System.IO.Path.GetDirectoryName(RLPath);
                        string gamesiniPath = RLPath + "\\Settings\\" + multiSystemName;


                        if (!Directory.Exists(gamesiniPath))
                            Directory.CreateDirectory(gamesiniPath);

                        RomMapper.CreateGamesIni(Xml, gamesiniPath);
                    }

                    //=====================================
                    // GENRE GENERATING
                    //=====================================
                    // 

                    if (CreateGenres)
                    {
                        List<string> genreList = new List<string>();
                        foreach (DatabaseGame itemGenre in m_System)
                        {
                            itemGenre.Genre = itemGenre.Genre.Replace("/", "-");

                            bool exists = genreList.Exists(element => element == itemGenre.Genre);
                            if (!exists)
                            {
                                genreList.Add(itemGenre.Genre);
                                //  System.Windows.MessageBox.Show(item.Genre);
                            }
                        }

                        genreList.Sort();
                        List<DatabaseMenu> mss = new List<DatabaseMenu>();

                        //Create the main Genre.xml
                        foreach (var itemGenre in genreList)
                        {
                            if (itemGenre != string.Empty)
                            {
                                mss.Add(new DatabaseMenu(itemGenre,1));
                            }
                        }


                        m.SerializeGenreItems(mss, HSPath + @"\Databases\" + multiSystemName + "\\Genre.xml");

                        // loop through the genremenu list
                        foreach (DatabaseMenu menuItem in mss)
                        {
                            List<DatabaseGame> gg = new List<DatabaseGame>();
                            foreach (DatabaseGame items in m_System)
                            {
                                if (items.Genre == menuItem.name)
                                    gg.Add(new DatabaseGame(items));
                            }

                            SerializeToGenreXML(gg, multiSystemName, menuItem.name);

                        }

                    }
                }
                catch (Exception)
                {


                }


                //BuildMainMenuItems();

            }

            catch (Exception)
            {


            }
            finally
            {
                //mm.RemoveAt(0);
                m.SerializeMainMenuItems(CurrentMenu, mm, HSPath + "\\Databases\\Main Menu");
               // mm.Insert(0, new DatabaseMenu("Main Menu"));
            }
        
        }
        public override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Complete");
        }

    }

    struct MultiSystem
    {
        public MultiSystem(DatabaseGame gameObject)
        {
            this.gameObject = gameObject;
        }
        private DatabaseGame gameObject;
        public DatabaseGame GameObject
        {
            get { return gameObject; }
            set { gameObject = value; }
        }

    }

    public class Favorites : Database
    {
        // public static Game game { get; set; }
        public static string hsPath { get; set; }
        public static string dbPath { get; set; }
        public string name;
        /// <summary>
        /// Output List set for return favorite DatabaseGame objects
        /// </summary>
        public static List<DatabaseGame> FavesOutList = new List<DatabaseGame>();

        internal Favorites() { }
        internal Favorites(string sys, string HSPath)
        {
            name = sys;
            hsPath = HSPath;
            dbPath = hsPath + "\\Databases\\";
        }

        #region UsedMethods
        /// <summary>
        /// Returns a list of favorites from a Hyperspin favorites text file
        /// </summary>
        /// <param name="system"></param>
        /// <param name="hsPath"></param>
        /// <returns></returns>
        public static List<string> GetFavoritesFromText(string system)
        {
            // Creating empty list
            List<string> FavouritesList = new List<string>();
            //Check if the file and path exits
            if (File.Exists(dbPath + system + "\\" + "favorites.txt"))
            {
                // add each line using a streamreader
                using (StreamReader reader = new StreamReader(dbPath + system + "\\" + "favorites.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FavouritesList.Add(line); // Add to list.
                    }

                    //Sort favorites
                    FavouritesList.Sort();
                }
                return FavouritesList;
            }
            else
                return FavouritesList;
        }
        /// <summary>
        /// Takes a list of games and updates its IsFavorite  boolean
        /// </summary>
        /// <param name="gamelist"></param>
        public static void updateGameListFavorites(List<DatabaseGame> gamelist, List<string> FavouritesList)
        {
            foreach (var item in gamelist)
            {
                bool exist = FavouritesList.Exists(x => x == item.RomName);
                if (exist)
                {
                    item.IsFavorite = true;
                }
            }
        }
        /// <summary>
        /// Create an xml from all the games tagged with IsFavorite
        /// </summary>
        /// <param name="gamelist"></param>
        public static void ConvertGameFavoritesToXml(List<DatabaseGame> gamelist, string systemName, bool saveToGenre)
        {
            FavesOutList.Clear();
            foreach (DatabaseGame game in gamelist)
            {
                if (game.IsFavorite)
                    FavesOutList.Add(game);
            }

            //Serialize xml
            SerializeToXML(FavesOutList, systemName, "Favorites", hsPath);

            if (saveToGenre)
            {
                List<string> genreList = AddFavoritesToGenre(hsPath + "\\Databases\\" + systemName + "\\Genre.xml");
                DatabaseMenu dbMenu = new DatabaseMenu();
                List<DatabaseMenu> genreMenuList = new List<DatabaseMenu>();
                //Create the main Genre.xml
                string lastGenre = string.Empty;
                foreach (var itemGenre in genreList)
                {
                    if (itemGenre != string.Empty)
                    {
                        if (lastGenre != itemGenre)
                            genreMenuList.Add(new DatabaseMenu(itemGenre,1));

                        lastGenre = itemGenre;
                    }
                }

                dbMenu.SerializeGenreItems(genreMenuList, hsPath + @"\Databases\" + systemName + "\\Genre.xml");
            }

        }        
        #endregion

        public override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Taking in menu items bw argument from Main UI
            List<DatabaseMenu> menuItems = (List <DatabaseMenu>)e.Argument;

            // Build string array of items removing an entry for the Main Menu
            string[] syss = new string[menuItems.Count - 1];

            int i = 0;
            try
            {
                // Add each name from a menu item into the array
                // Making sure it is not the main menu
                foreach (DatabaseMenu item in menuItems)
                {
                    if (item.name != "Main Menu")
                    {
                        syss[i] = item.name;
                        i++;
                    }
                }

                List<DatabaseGame> g = new List<DatabaseGame>();
                Favorites f = new Favorites("", HSPath);

                for (int ii = 0; ii < syss.Length; ii++)
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    List<string> ls = GetFavoritesFromText(syss[ii]);

                    if (ls.Count != 0)
                    {
                        foreach (var items in ls)
                        {
                            if (bw.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            //item.ToString();
                            f.QueryFavorite(items, syss[ii]);
                        }
                        ii++;
                    }

                }

                e.Result = (List<DatabaseGame>)f.getCurrentGameList();
               
            }
            catch (Exception)
            {

                e.Cancel = true;
            }
        }
        public override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Favorite  class worker complete");
        }

        /// <summary>
        /// Scans all systems and returns a Database game list of favorites.
        /// </summary>
        /// <param name="systems"></param>
        /// <returns></returns>
        public static List<DatabaseGame> GetAllFavorites(string[] systems)
        {
            FavesOutList.Clear();
            List<string> FavouritesList = new List<string>();
                        
            foreach (var sysItems in systems)
            {                
                if (File.Exists(dbPath + sysItems + "\\" + "favorites.txt"))
                {
                    // Using method inside this class to get from favorites text
                    FavouritesList = GetFavoritesFromText(sysItems);

                    string mainMenuXml = Path.Combine(dbPath, sysItems, sysItems + ".xml");
                    // Creat null Xdocument 
                    XDocument xdoc = null;
                    // Using XmlReader to be able to load into xdoc
                    foreach (var rom in FavouritesList)
                    {
                        // Query with LINQ
                        //var systems =
                        //    from item in xdoc.Descendants("game")
                        //    select item.Attribute("name").Value;
                        using (XmlReader xr = XmlReader.Create(mainMenuXml))
                        {
                            xdoc = XDocument.Load(xr);
                        }

                        try
                        {
                           var gameVars =
                           from item in xdoc.Descendants("game")
                           where item.Attribute("name").Value.Contains(rom)
                           select new
                           {
                               RomName = (string)item.Attribute("name").Value,
                               // Enabled = 1,
                               Description = item.Element("description").Value,
                               // CloneOf = item.Element("cloneof").Value,
                               //  CRC = item.Element("crc").Value,
                               //  Manufacturer = item.Element("manufacturer").Value,
                               // Genre = item.Element("genre").Value,
                               //Year = item.Element("year").Value,
                               //   Rating = item.Element("rating").Value,
                           };

                            foreach (var item in gameVars)
                            {
                                FavesOutList.Add(new DatabaseGame(item.RomName, "", "", item.Description,
                                    "", "", "", 0, "",
                                    "", 1));
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            return FavesOutList;
        }
        /// <summary>
        /// Get string list of favorites for a given system
        /// </summary>
        /// <param name="system"></param>
        /// <param name="hsPath"></param>
        /// <returns></returns>
        internal List<string> getFaves(string system, string hsPath)
        {
            List<string> FavouritesList = new List<string>();

            if (File.Exists(dbPath + system + "\\" + "favorites.txt"))
            {
                using (StreamReader reader = new StreamReader(dbPath + system + "\\" + "favorites.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FavouritesList.Add(line); // Add to list.
                    }
                }

                return FavouritesList;
            }
            else
                return FavouritesList;
        }
        /// <summary>
        /// Get Game values for given favorite
        /// </summary>
        /// <param name="romName"></param>
        /// <param name="system"></param>
        internal void QueryFavorite(string romName, string system)
        {
            string mainMenuXml = Path.Combine(dbPath, system + "\\" + system + ".xml");
            XDocument xdoc = null;
            DatabaseGame g = new DatabaseGame();

            if (system == "Future Pinball")
            {
                Console.WriteLine("");
            }
            using (XmlReader xr = XmlReader.Create(mainMenuXml))
            {
                try
                {
                    xdoc = XDocument.Load(xr);
                }
                catch (XmlException e)
                {

                    MessageBox.Show(e.Message);
                }
            }

            try
            {
                var gameVars =
               from item in xdoc.Descendants("game")
               where (item.Attribute("name").Value.Contains(romName) && (item.Element("cloneof").Value == ""))

               select new
               {
                   RomName = item.Attribute("name").Value,
                   Enabled = 1,
                   Description = item.Element("description").Value,
                   CloneOf = item.Element("cloneof").Value,
                   CRC = item.Element("crc").Value,
                   Manufacturer = item.Element("manufacturer").Value,
                   Genre = item.Element("genre").Value,
                   Year = Convert.ToInt32(item.Element("year").Value),
                   Rating = item.Element("rating").Value,
                   System = system
               };

                try
                {

                    foreach (var item in gameVars)
                    {
                        if (item != null)
                        {
                            FavesOutList.Add(new DatabaseGame(item.RomName, "", "", item.Description, item.CloneOf, item.CRC, item.Manufacturer, item.Year,
                                item.Genre, item.Rating, item.Enabled, system));
                        }
                    }
                }
                catch (NullReferenceException e)
                {


                }


            }
            catch (Exception)
            {

            }


        }
        /// <summary>
        /// Convert a list of game objects that are favorites into an xml
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="saveToGenre"></param>
        /// <param name="hsPath"></param>
        public void FavoriteToXml(string systemName, bool saveToGenre, string hsPath)
        {
            Favorites f = new Favorites(systemName, hsPath);
            List<DatabaseGame> g = new List<DatabaseGame>();

            g = (List<DatabaseGame>)f.getCurrentGameList();

            if (g.Count == 0)
            {
                MessageBox.Show("Try saving the xml first to get updated enabled tags.");
            }

            SerializeToXML(g, systemName, "Favorites", hsPath);


            if (saveToGenre)
            {
                List<string> genreList = AddFavoritesToGenre(HSPath + "\\Databases\\" + systemName + "\\Genre.xml");
                DatabaseMenu m = new DatabaseMenu();
                List<DatabaseMenu> mss = new List<DatabaseMenu>();
                //Create the main Genre.xml
                string lastGenre = string.Empty;
                foreach (var itemGenre in genreList)
                {
                    if (itemGenre != string.Empty)
                    {
                        if (lastGenre != itemGenre)
                            mss.Add(new DatabaseMenu(itemGenre));

                        lastGenre = itemGenre;
                    }
                }

                m.SerializeGenreItems(mss, hsPath + @"\Databases\" + systemName + "\\Genre.xml");
            }
        }
        /// <summary>
        /// Not sure this is needed anymore, use base class
        /// </summary>
        /// <param name="games"></param>
        /// <param name="systemname"></param>
        /// <param name="dbName"></param>
        public void SerializeToXML(List<DatabaseGame> games, string systemname, string dbName)
        {
            Database.SerializeToXML(games, systemname, dbName);
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

        internal IEnumerable getCurrentGameList()
        {
            return FavesOutList;
        }
    }

}
