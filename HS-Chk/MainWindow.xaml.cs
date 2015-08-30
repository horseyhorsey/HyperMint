using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using ExplorerTreeView.Controls;
using System.Xml.Serialization;
using GongSolutions.Wpf.DragDrop;
using Ionic.Zip;
using System.Xml.Linq;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Hypermint.HypermintTools;



namespace Hypermint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged, IHyperMintTrash, IHypermintSelected, IHyperminSelected2
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        //For launching games
        RocketLaunch RL = new RocketLaunch();

        Media fe_media = new Media();       
        Database hsdb = new Database();

        List<DatabaseMenu> masterList = new List<DatabaseMenu>();
        List<DatabaseMenu> mm = new List<DatabaseMenu>();
        ObservableCollection<DatabaseMenu> MmManagerCurrent = new ObservableCollection<DatabaseMenu>();
        ObservableCollection<DatabaseGame> DatabaseGame;

        SimpleWheel simpleWheel = new SimpleWheel();
        List<string> FavouritesList = new List<string>();
        List<FileCollection> fc = new List<FileCollection>();
        List<IntroVideos> introVideos;

        //Multi System list of games
        List<DatabaseGame> m_System = new List<DatabaseGame>();
        // Master menu objects
        ObservableCollection<DatabaseMenu> MmManagerMaster = new ObservableCollection<DatabaseMenu>();

        ExplorerTreeView.Controls.ExplorerTreeView et = new ExplorerTreeView.Controls.ExplorerTreeView();

        ObservableCollection<FileInfo> unusedFilesCollection = new ObservableCollection<FileInfo>();
        ObservableCollection<DirectoryInfo> filesBoxList = new ObservableCollection<DirectoryInfo>();
        ObservableCollection<Statistics> stats = new ObservableCollection<Statistics>();        

        public string autoScan { get; set; }
        public bool scanning { get; set; }
        public string currentBezelLoaded = string.Empty;
        public string[] positions = new string[8];

        private DatabaseGame selectedGame;
        public DatabaseGame SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                selectedGame = value;
            }
        }
        private string infoMessage;
        public string InfoMessage
        {
            get
            {
                return infoMessage;
            }
            set
            {
                infoMessage = value;
                this.NotifyPropertyChanged("InfoMessage");
            }
        }

        //Variables from the interface
        string selectedColumn;
        /// <summary>
        /// Holds the selected column name
        /// </summary>
        public string SelectedColumn
        {
            get
            {
                return selectedColumn;
            }
            set
            {
                selectedColumn = value;
            }
        }
        string lastSelectedColumn;
        /// <summary>
        /// Holds the last selected column name
        /// </summary>
        public string LastSelectedColumn
        {
            get
            {
                return lastSelectedColumn;
            }
            set
            {
                lastSelectedColumn = value;
            }
        }
        string lastSelectedRomName;
        public string LastSelectedRomName
        {
            get
            {
                return lastSelectedRomName;
            }
            set
            {
                lastSelectedRomName = value;
            }
        }
        private string columnHeader;
        public string ColumnHeader
        {
            get { return columnHeader; }
            set { columnHeader = value; this.NotifyPropertyChanged("columnHeader"); }
        }
        private string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;
                this.NotifyPropertyChanged("fullPath");
            }
        }
        string viewerFile;
        public string ViewerFile
        {
            get
            {
                return viewerFile;
            }
            set
            {
                viewerFile = value;
            }
        }

        private bool isHyperSpin;
        public bool IsHyperspin
        {
            get
            {
                return isHyperSpin;
            }
            set
            {
                isHyperSpin = value;
            }
        }

        private ImageSource wheelSource;
        /// <summary>
        /// Binding imagesource for viewer pane
        /// </summary>
        public ImageSource WheelSource
        {
            get { return wheelSource; }

            set
            {
                wheelSource = value;
                this.NotifyPropertyChanged("WheelSource");
            }
        }

        /// <summary>
        /// Some stats Variables
        /// </summary>
        private TimeSpan totalOverallTime;
        public TimeSpan TotalOverAllTime
        {
            get { return totalOverallTime; }
            set { totalOverallTime = value; this.NotifyPropertyChanged("totalOverallTime"); }
        }       
       
        private System.Windows.Media.Brushes scanButtonColor;
        public System.Windows.Media.Brushes ScanButtonColor
        {
            get { return scanButtonColor; }
            set
            {
                scanButtonColor = value;
                this.NotifyPropertyChanged("scanButtonColor");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            #region Setup & Settings
            // Load the path Variables from settings            
            hsdb.HSPath = Properties.Settings.Default.hsPath;
            hsdb.RLPath = Properties.Settings.Default.feLauncherPath;
            hsdb.RLMediaPath = Properties.Settings.Default.launchMediaPath;
            //RL.FEParams = Properties.Settings.Default.feLauncherParams;
            hsdb.IMPath = Properties.Settings.Default.IMPath;            
            
            //autoScan = Properties.Settings.Default.AutoScan;
            vidCopy.IsChecked = true;
            if (hsdb.HSPath == "")
            {
                Properties.Settings.Default.hsPath = hsdb.SearchForInstall("HyperSpin");
                hsdb.HSPath = Properties.Settings.Default.hsPath;
            }

            if (hsdb.RLPath =="")
            {
                string tempPath = hsdb.SearchForInstall("RocketLauncher");
                Properties.Settings.Default.feLauncherPath = tempPath + "\\RocketLauncher.exe";
                Properties.Settings.Default.launchMediaPath = tempPath + "\\Media\\";
                hsdb.RLPath = Properties.Settings.Default.feLauncherPath;
                hsdb.RLMediaPath = Properties.Settings.Default.launchMediaPath;
            }
            // If the HyperSpin path is found scan for systems and add to list..
                Properties.Settings.Default.Save();
                textboxHSPath.Text = hsdb.HSPath;
            try
            {
                if (hsdb.HSPath != "")
		                    {

                        _comboMainMenu.ItemsSource = hsdb.getMainMenuXmls();
                        _comboMainMenu.SelectedValue = "Main Menu";
                        _comboMenuManage.ItemsSource = hsdb.getMainMenuXmls();
                        _comboMenuManage.SelectedValue = "Main Menu";

                        hsdb.HyperspinDatabaseXml = hsdb.HSPath + @"\Databases\Main Menu\Main Menu.xml";                        
                        //Create the Master list. Create instance of Menu then use its methods
                       // Menu m = new Menu();
                        DatabaseMenu dbmenu = new DatabaseMenu();
                        masterList = dbmenu.GetMainMenuItemsFromXml(@"Master.xml", hsdb.RLMediaPath + "\\Icons");

                        BuildMainMenuItems();                        
                    } 
	        }catch (Exception)
	                {
		
	                }
                   
           
            //populate youtube resolution sizes;
            List<int> ytints = new List<int>();
            ytints.Add(1080);
            ytints.Add(720);
            ytints.Add(480);
            ytints.Add(360);
            ytints.Add(240);
            ytints.Add(144);
                      
            //populate the instruction cards positions
            positions[0] = "leftCenter";
            positions[1] = "rightCenter";
            positions[2] = "topCenter";
            positions[3] = "bottomCenter";
            positions[4] = "topLeft";
            positions[5] = "bottomLeft";
            positions[6] = "topRight";
            positions[7] = "bottomRight";

            _comboResolution.ItemsSource = ytints;
            _comboResolution.SelectedIndex = 1;

            ComboBox1.ItemsSource = positions;

            if (hsdb.RLPath != string.Empty)
                txt_launchPath.Text = hsdb.RLPath;
            if (hsdb.RLMediaPath != string.Empty)
                txt_launchMediaPath.Text = hsdb.RLMediaPath ;
            //if (RL.FEParams != string.Empty)
            //    txt_launchParams.Text = RL.FEParams;
            if (hsdb.IMPath != string.Empty)
                txt_IMPath.Text = hsdb.IMPath;

            _text_ImageAuthor.Text = Properties.Settings.Default.defaultAuthor;
            prefixTextbox.Text = "Desc";
            txt_author.Text = Properties.Settings.Default.defaultAuthor;

            try
            {
                _comboPresetFade.SelectedIndex = 0;
                presetsInMenu.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }

            #endregion

            #region GuiColorStyle
            // Get the apps saved gui style settings
            string guiColor = Properties.Settings.Default.guiColor;
            bool guiTheme = Properties.Settings.Default.guiTheme;

            foreach (var item in MahApps.Metro.ThemeManager.Accents)
            {
                comboStyles.Items.Add(item.Name);
            }


            if (guiColor != string.Empty)
            {
                changeguiColor(guiColor, guiTheme);
                comboStyles.SelectedValue = guiColor;
                guiThemeCheckbox.IsChecked = guiTheme;
            }
            #endregion

        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            presetRefresh();
            //browser.Navigate(@"https://www.youtube.com/");            

            rectScanCancel.IsEnabled = true;
            refreshCardPresets();
            multiSystemDataGrid.ItemsSource = m_System;

            populateImagePresets();
            Debug.WriteLine("Starting Application");
            authorText.Text = Properties.Settings.Default.defaultAuthor;
        }

        #region Good & Sorted
        /// <summary>
        /// Build Main menu object list 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="dark"></param>
        /// 
        private void BuildMainMenuItems()
        {
            mm.Clear();
            MmManagerCurrent.Clear();

            //Create a databaseMenu object to reference
            foreach (string item in hsdb.getSystems())
            {
                if (hsdb.RLMediaPath != string.Empty)
                {
                    Uri img = new Uri(hsdb.RLMediaPath + @"\Icons\" + item + ".png");
                    mm.Add(new DatabaseMenu(item, img));
                }
                else
                    mm.Add(new DatabaseMenu(item, new Uri("")));
            }

            // Search through the masterlist and check against the items built from the selected main menu
            // Add the disabled/Enabled int flag on
            int i = 0;
            int ii = 0;
            List<DatabaseMenu> menu = new List<DatabaseMenu>();
            menu = masterList;
            foreach (DatabaseMenu item in masterList)
            {
                DatabaseMenu foundMenuItem = mm.Find(x => x.name == item.name);

                item.XmlExists = File.Exists(hsdb.HSPath + "\\Databases\\" + item.name + "\\" + item.name + ".xml");
                item.GenreExists = File.Exists(hsdb.HSPath + "\\Databases\\" + item.name + "\\Genre.xml");
                if (foundMenuItem != null)
                {
                    item.Enabled = foundMenuItem.Enabled;
                    i++;
                }
                else
                {
                    ii++;
                }

            }
            //HM.SystemsEnabled = i;

            foreach (var item in mm)
            {
                if (item.name != "Main Menu")
                {
                    MmManagerCurrent.Add(item);
                    DatabaseMenu foundMenuItem = menu.Find(x => x.name == item.name);
                    if (foundMenuItem != null)
                    {
                        menu.Remove(item);
                    }
                }
            }

            xmlMasterMenuManage.ItemsSource = menu;


            CollectionViewSource MainMenuCollectionView;
            MainMenuCollectionView = (CollectionViewSource)(FindResource("MainMenuCollectionView"));
            MainMenuCollectionView.Source = mm;

            CollectionViewSource MainMenuManagerMasterViewSource;
            MainMenuManagerMasterViewSource = (CollectionViewSource)(FindResource("MainMenuManagerMasterViewSource"));
            MainMenuManagerMasterViewSource.Source = masterList;

            CollectionViewSource MainMenuManagerViewSource;
            MainMenuManagerViewSource = (CollectionViewSource)(FindResource("MainMenuManagerViewSource"));
            MainMenuManagerViewSource.Source = MmManagerCurrent;

            CollectionViewSource StatsCollection;
            StatsCollection = (CollectionViewSource)(FindResource("StatsCollection"));
            StatsCollection.Source = stats;

            listboxSystem.Items.Refresh();
        }
        private void ViewerGrid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // clear the image_wheel
            WheelSource = null;
            // Put dropped files into a string array, sort & send to DroppedFile
            string[] s = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            if (s == null) return;           
            Array.Sort(s);

            if (hsdb.IsHyperspin)
            {
                HyperSpinMedia feHs = new HyperSpinMedia();
                feHs.DroppedFile(s, hsDatagrid.SelectedItem as DatabaseGame, SelectedColumn);                
            }
            else
            {
                RocketlaunchMedia feRL = new RocketlaunchMedia();
                feRL.DroppedFile(FullPath,s);
                FileManagement.cellFilesRefresh(SelectedColumn, FullPath);
            }

        }

        #region Instruction Cards
        /// <summary>
        /// Save the instruction card preset Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePresetCards_Click(object sender, RoutedEventArgs e)
        {
            //Save to card preset
            CardPresets.SavePreset(presetTextBoxCards.Text, OutputfileWidthCards.Text, OutputfileHeightCards.Text, cardsTextBox.Text, authorTextBox.Text,
                ComboBox1.Text, StretchCheckBoxCards.IsChecked.Value, ResizeCheckboxCards.IsChecked.Value);

            refreshCardPresets();
        }
        /// <summary>
        /// Refresh the card presets for displaying the listbox & context menu
        /// </summary>
        private void refreshCardPresets()
        {
            presetListBoxCards.Items.Clear();
            presetsInMenu.Items.Clear();
            // Refresh the presets
            foreach (var item in CardPresets.GetPresetsFromDirectory("cards"))
            {
                presetListBoxCards.Items.Add(item);
                presetsInMenu.Items.Add(item);
            }
        }
        /// <summary>
        /// Delete an instruction card preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delPresetCards_Click(object sender, RoutedEventArgs e)
        {
            if (presetListBoxCards.SelectedItems.Count != 0)
            {
                File.Delete("presets\\cards\\" + presetListBoxCards.SelectedItem);
                presetListBoxCards.Items.Remove(presetListBoxCards.SelectedItem);
            }
        }
        /// <summary>
        /// Create an instruction card from the viewer context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createCardInMenu_Click(object sender, RoutedEventArgs e)
        {
            // Load card preset elements into a string array
            string[] presetValues = CardPresets.LoadPreset(@"presets\cards\" + presetsInMenu.Text);
            string bezelPath = hsdb.RLMediaPath + @"\Bezels\" + hsdb.SystemName + @"\" + SelectedGame.RomName;
            InstructionCard ic = new InstructionCard(ViewerFile, presetValues, bezelPath);
            ic.ConvertCard();
        }
        /// <summary>
        /// Update the values in th gui each time a preset is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void presetListBoxCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (presetListBoxCards.SelectedItems.Count == 0)
                return;
            string[] elements = CardPresets.LoadPreset(@"presets\cards\" + presetListBoxCards.SelectedItem.ToString());

            presetTextBoxCards.Text = presetListBoxCards.SelectedItem.ToString().Replace(".xml", string.Empty);
            authorTextBox.Text = elements[0];
            cardsTextBox.Text = elements[1];
            ComboBox1.SelectedItem = elements[2];
            OutputfileWidthCards.Text = elements[3];
            OutputfileHeightCards.Text = elements[4];
            StretchCheckBoxCards.IsChecked = Convert.ToBoolean(elements[5]);
            ResizeCheckboxCards.IsChecked = Convert.ToBoolean(elements[6]);
        }
        #endregion

        #endregion

        #region TheViewer
        private void ViewerGrid_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
        }
        private void ViewerGrid_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.All;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }
        #endregion

        #region STUFF TO MOVE INTO CLASSES
        private void txtToPDF_Click(object sender, RoutedEventArgs e)
        {

            txtToPDF.IsEnabled = false;
            cancelPDFWorker.IsEnabled = true;
            PDFWorker pdfwork = new PDFWorker(ViewerFile,
                                                SelectedGame.RomName, PDFWorker.PDFJobType.TextPDF);

            pdfwork.PDFBackgroundWorker.WorkerReportsProgress = true;
            pdfwork.PDFBackgroundWorker.WorkerSupportsCancellation = true;

            pdfwork.PDFBackgroundWorker.DoWork -= pdfwork.PDFBackgroundWorker_DoWork_NormalPDF;
            pdfwork.PDFBackgroundWorker.RunWorkerCompleted -= pdfwork.PDFBackgroundWorker_RunWorkerCompleted;
            pdfwork.PDFBackgroundWorker.ProgressChanged -= pdfwork.PDFBackgroundWorker_ProgressChanged;


            pdfwork.PDFBackgroundWorker.DoWork += pdfwork.PDFBackgroundWorker_DoWork_TextPDF;
            pdfwork.PDFBackgroundWorker.RunWorkerCompleted += pdfwork.PDFBackgroundWorker_RunWorkerCompleted;
            pdfwork.PDFBackgroundWorker.ProgressChanged += pdfwork.PDFBackgroundWorker_ProgressChanged;
            pdfwork.PDFBackgroundWorker.RunWorkerAsync();
            progressBarViewer.IsActive = true;

        }
        private void PDFtoIMG_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@"C:\Program Files\gs") || !Directory.Exists(@"C:\Program Files (x86)\gs"))
            {
                System.Windows.MessageBox.Show("Needs GhostScript installed.");
            }

            PDFtoIMG.IsEnabled = false;
            cancelPDFWorker.IsEnabled = true;
            PDFWorker pdfwork = new PDFWorker(ViewerFile,
            SelectedGame.RomName, PDFWorker.PDFJobType.NormalPDF);
            pdfwork.SelectedFullPath = FullPath;
            pdfwork.PDFBackgroundWorker.WorkerReportsProgress = true;
            pdfwork.PDFBackgroundWorker.WorkerSupportsCancellation = true;

            pdfwork.PDFBackgroundWorker.DoWork -= pdfwork.PDFBackgroundWorker_DoWork_TextPDF;
            pdfwork.PDFBackgroundWorker.RunWorkerCompleted -= pdfwork.PDFBackgroundWorker_RunWorkerCompleted;
            pdfwork.PDFBackgroundWorker.ProgressChanged -= pdfwork.PDFBackgroundWorker_ProgressChanged;

            pdfwork.PDFBackgroundWorker.DoWork += pdfwork.PDFBackgroundWorker_DoWork_NormalPDF;
            pdfwork.PDFBackgroundWorker.RunWorkerCompleted += pdfwork.PDFBackgroundWorker_RunWorkerCompleted;
            pdfwork.PDFBackgroundWorker.ProgressChanged += pdfwork.PDFBackgroundWorker_ProgressChanged;
            pdfwork.PDFBackgroundWorker.RunWorkerAsync();
            progressBarViewer.IsActive = true;

        }

        /// <summary>
        /// Video trimming from Media Elements Source file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewVideoTrim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /// Enable the cancellation button
                cancelPDFWorker.IsEnabled = true;

                // Get the path from the loaded video in the media element
                ViewerFile = media_wheel.Source.LocalPath;
                // Set the image_Wheels source to null
                WheelSource = null;

                //Create a new Intro video instance
                IntroVideos iv = new IntroVideos();
                if (vidReplace.IsChecked == true)
                {
                    FileInfo file = new FileInfo(ViewerFile);

                    if (IsHyperspin)
                        FileManagement.SendToTrash("HyperSpin", file, hsdb.SystemName, ColumnHeader, SelectedGame.RomName);

                    iv.VideoToTrim = iv.movedFile;
                    iv.VideoToTrimTo = ViewerFile;
                }
                else if (vidCopy.IsChecked == true)
                {
                    string filename = System.IO.Path.GetFileNameWithoutExtension(ViewerFile);
                    string path = System.IO.Path.GetDirectoryName(ViewerFile);
                    string ext = System.IO.Path.GetExtension(ViewerFile);
                    int i = 0;
                    while (File.Exists(ViewerFile))
                    {
                        ViewerFile = System.IO.Path.Combine(path, filename + i + ext);
                        i++;
                    }
                    iv.VideoToTrim = System.IO.Path.Combine(path, filename + ext);
                    iv.VideoToTrimTo = ViewerFile;
                }
                int SliderValue = (int)timelineSlider.Value;
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
                TimeSpan t = new TimeSpan(0, 0, Convert.ToInt32(amountToChop.Value));

                progressBarViewer.IsActive = true;
                _videoContextMenu.IsOpen = false;
                iv.StartConverting(ts, t);
            }
            catch (Exception)
            {
                cancelPDFWorker.IsEnabled = false;
                System.Windows.MessageBox.Show("Couldn't trim the video");
            }


        }
        private void sendToTrashHSViewer_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            SendToTrash();
        }
        #endregion

        #region Main Menu System Items
        private void rebuildSystems_Click(object sender, RoutedEventArgs e)
        {
            if (_comboMainMenu.Items.Count != 0)
            {
                if (_comboMainMenu.Items.Count == 1)
                    return;
                else
                {
                    hsdb.HyperspinDatabaseXml = hsdb.HSPath + "\\Databases\\Main Menu\\" + _comboMainMenu.SelectedItem + ".xml";
                    BuildMainMenuItems();
                }
            }
            else
                System.Windows.MessageBox.Show("Hyperspin main menu path: " + hsdb.HSPath + "\\Databases\\Main Menu\\");
        }
        private void _comboMainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_comboMainMenu.Items.Count != 0)
            {
                if (_comboMainMenu.Items.Count == 1)
                    return;
                else
                {
                     hsdb.HyperspinDatabaseXml = hsdb.HSPath + "\\Databases\\Main Menu\\" + _comboMainMenu.SelectedItem + ".xml";
                    _comboMenuManage.SelectedItem = _comboMainMenu.SelectedItem;

                    BuildMainMenuItems();
                     //var c = _comboMenuManage.SelectedItem as Menu;
                    // textboxSaveXmlName.Text = c.name;
                }
            }
            else
                System.Windows.MessageBox.Show("Hyperspin main menu path: " + hsdb.HSPath + "\\Databases\\Main Menu\\");
        }
        private void _comboMenuManage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            RebuildTriggerMenuManage();
        }
        private void RebuildTriggerMenuManage()
        {
            if (_comboMainMenu.Items.Count != 0 || _comboMenuManage.Items.Count != 0)
            {
                if (_comboMainMenu.Items.Count == 1 || _comboMenuManage.Items.Count == 1)
                    return;
                else
                {
                    hsdb.HyperspinDatabaseXml = hsdb.HSPath + "\\Databases\\Main Menu\\" + _comboMenuManage.SelectedItem + ".xml";
                    _comboMainMenu.SelectedItem = _comboMenuManage.SelectedItem;
                    textboxSaveXmlName.Text = _comboMenuManage.SelectedItem.ToString();
                   // BuildMainMenuItems();
                }
            }
            else
                System.Windows.MessageBox.Show("Hyperspin main menu path: " + hsdb.HSPath + "\\Databases\\Main Menu\\");
        }
        #endregion

        #region GUI Colors & Settings
        public void changeguiColor(string color, bool dark)
        {
            string darkOrLight = string.Empty;
            if (dark)
                darkOrLight = "BaseDark";
            else
                darkOrLight = "BaseLight";
            // get the theme from the current application
            var theme = MahApps.Metro.ThemeManager.DetectAppStyle(System.Windows.Application.Current);

            // now set the Green accent and dark theme
            MahApps.Metro.ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                        MahApps.Metro.ThemeManager.GetAccent(color),
                                        MahApps.Metro.ThemeManager.GetAppTheme(darkOrLight));

            Properties.Settings.Default.guiColor = color;
            Properties.Settings.Default.guiTheme = dark;
            Properties.Settings.Default.Save();
        }
        private void comboStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeguiColor(getColor(), getTheme());
        }
        private void guiThemeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            changeguiColor(getColor(), getTheme());
        }
        private void guiThemeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            changeguiColor(getColor(), getTheme());
        }
        private string getColor()
        {
            if (comboStyles.SelectedItem != null)
                return comboStyles.SelectedItem.ToString();
            else if (Properties.Settings.Default.guiColor != string.Empty)
                return Properties.Settings.Default.guiColor;
            else
                return "Red";
        }
        private bool getTheme()
        {
            bool dark = false;
            if (guiThemeCheckbox.IsChecked.Value == false)
                dark = false;
            else
                dark = true;
            return dark;
        }
        #endregion

        private void btn_SavePaths_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.hsPath = this.textboxHSPath.Text;
            Properties.Settings.Default.IMPath = this.txt_IMPath.Text;

            Properties.Settings.Default.feLauncherPath = this.txt_launchPath.Text;
            Properties.Settings.Default.feLauncherParams = this.txt_launchParams.Text;
            Properties.Settings.Default.launchMediaPath = this.txt_launchMediaPath.Text;

            Properties.Settings.Default.defaultAuthor = txt_author.Text;

            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// System ListBox Selections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listboxSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cheap = listboxSystem.SelectedItem as DatabaseMenu;
            

            if (cheap == null)
                return;

            hsdb.SystemName = cheap.name;
            try
            {
                simpleWheel.popGenre(hsdb.HSPath + "\\Databases\\" + hsdb.SystemName + "\\Genre.xml");
            }
            catch (Exception)
            {

            }

            if (toggleXML.IsChecked==false)
                DisplayHSColumns();

            // Reset scan rectangle/button color
            rectHsScan.Fill = System.Windows.Media.Brushes.Red; rectRLScan.Fill = System.Windows.Media.Brushes.Red;
            if (SelectedGame!=null)
                SelectedGame.RomName = string.Empty;

            DisplaySelectedImageSystem(hsdb.HSPath + "\\Media\\Main Menu\\Images\\Wheel\\" + hsdb.SystemName + ".png");
            
            buildGameObjectsFromXml();
                     
        }
        #region HS-Scan-Start
        private void btn_ScanHS_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_ScanHL_Click(object sender, MouseButtonEventArgs e)
        {
            if (!hsdb.bw.IsBusy)
            {
                DisableGUIControls();
                scanXTMediaFromDatabaseGameObjects();
            }
        }


        private void DisableGUIControls()
        {
            //rectHsScan.IsEnabled = false;
            //rectRLScan.IsEnabled = false;
            rectScanCancel.IsEnabled = true;
            //listView1.IsEnabled = false;
            //button1.IsEnabled = false;
            //HLRadio.IsEnabled = false;
            //HSRadio.IsEnabled = false;
            //dgSimple.IsEnabled = false;
        }
        private void EnableGUIControls()
        {
            rectHsScan.IsEnabled = true;
            rectRLScan.IsEnabled = true;
            rectScanCancel.IsEnabled = false;
            //listView1.IsEnabled = true;
            //HSRadio.IsEnabled = true;
            //HLRadio.IsEnabled = true;
            //button1.IsEnabled = true;
            //dgSimple.IsEnabled = true;
        }        
        #endregion
        #region Cancellations
        private void cancelButton(object sender, RoutedEventArgs e)
        {
            CancelWorkers();
        }
        private void CancelWorkers()
        {
            if (hsdb.bw.IsBusy)
            {
                hsdb.bw.CancelAsync();
                EnableGUIControls();
            }
        }
        #endregion
        // Setting romcounts to progressbar before running background workers
        //
        private void buildGameObjectsFromXml()
        {
            if (DatabaseGame !=null)
                DatabaseGame.Clear();
            // Load the system xml from directory

            hsdb.SetDatabaseFileAndCount();

            //hsdb.bw.DoWork += hsdb.bw.bw_DoWork;
            hsdb.bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            hsdb.bw.RunWorkerAsync(hsdb.DatabaseGameCollection);

            //XmlDocument xdoc = new XmlDocument();
            //HM.XMLPath = hsdb.HSPath + @"\Databases\" + hsdb.SystemName + "\\" + hsdb.SystemName + ".xml";
            //try
            //{
            //    try
            //    {
            //        xdoc.Load(HM.XMLPath);
            //    }
            //    catch (Exception)
            //    {                    
            //        System.Windows.MessageBox.Show("No xml found for this system"); return;
            //    }
                
            //    HM.GameCount = xdoc.SelectNodes("menu/game").Count;

            //    //progressBar.Maximum = HM.GameCount;

            //    DisableGUIControls();

            //    bw.WorkerReportsProgress = true;
            //    bw.WorkerSupportsCancellation = true;


            //    bw.DoWork -= bw_DoWorkMultiSystem;
            //    bw.RunWorkerCompleted -= bw_RunWorkerMultiSystemCompleted;
            //    bw.DoWork -= bw_DoWorkGetAllFavories;
            //    bw.RunWorkerCompleted -= bw_RunWorkerFaveScanSystemCompleted;

            //    bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            //    bw.ProgressChanged += bw_ProgressChanged;
            //    bw.DoWork += bw_DoWork;


            //    if (!bw.IsBusy)
            //    {
            //        progressBar.IsActive = true;
            //        InfoMessage = "Building list from:  " + HM.XMLPath;
            //        bw.RunWorkerAsync();
            //    }

            //}
            //catch (Exception)
            //{
            //    EnableGUIControls();
            //}

        }
        private void scanHSMediaFromGameObjects()
        {
            try
            {
                //progressBar.Maximum = HM.GameCount;
               // progressBar.Value = 0;

                //bwHS.WorkerReportsProgress = true;
                //bwHS.WorkerSupportsCancellation = true;
                //bwHS.RunWorkerCompleted += bwHS_RunWorkerCompleted;
                //bwHS.ProgressChanged += bwHS_ProgressChanged;
                //bwHS.DoWork += bwHS_DoWork;
                //bwHS.RunWorkerAsync();

                ///Run the background inside the frontend class
                HyperSpinMedia feHS = new HyperSpinMedia(DatabaseGame.ToList());
                feHS.HSPath = hsdb.HSPath;
                feHS.systemName = hsdb.SystemName;
                feHS.bw.RunWorkerCompleted +=bwHS_RunWorkerCompleted;
                feHS.bw.RunWorkerAsync();
                progressBar.IsActive = true;
                

            }
            catch (Exception)
            {
                EnableGUIControls();
            }

        }


        private void scanXTMediaFromDatabaseGameObjects()
        {
            try
            {
                progressBar.IsActive = true;
                RocketlaunchMedia fe_RL = new RocketlaunchMedia(DatabaseGame.ToList(),hsdb.RLMediaPath,hsdb.SystemName);
                fe_RL.bw.RunWorkerCompleted +=bwRL_RunWorkerCompleted;
                fe_RL.bw.RunWorkerAsync();
            }
            catch (Exception)
            {
                EnableGUIControls();
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                InfoMessage = "Last Operation: HS db xml parsed: " + hsdb.HyperspinDatabaseXml;

                try
                {
                    if (e.Result != null)
                    {
                        DatabaseGame = (ObservableCollection<DatabaseGame>)e.Result;
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                InfoMessage = "XML scan cancelled";
            }
            populateDataGrid();
            progressBar.IsActive = false;

            //populateDataGrid();
            //listboxSystem.Items.Refresh();

        }
        private void bwHS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                InfoMessage = "Last Operation: HS db xml parsed: " + hsdb.HyperspinDatabaseXml;

                try
                {
                    if (e.Result != null)
                    {
                      
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                InfoMessage = "XML scan cancelled";
            }
            populateDataGrid();
            rectHsScan.Fill = System.Windows.Media.Brushes.Green;
            progressBar.IsActive = false;

            //populateDataGrid();
            //listboxSystem.Items.Refresh();
        }
        private void bwRL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                InfoMessage = "Last Operation: HS db xml parsed: " + hsdb.HyperspinDatabaseXml;

                try
                {
                    if (e.Result != null)
                    {
                        
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                InfoMessage = "XML scan cancelled";
            }
            populateDataGrid();
            rectRLScan.Fill = System.Windows.Media.Brushes.Green;
            progressBar.IsActive = false;

            //populateDataGrid();
            //listboxSystem.Items.Refresh();
        }

        private void populateDataGrid()
        {
            //link business data to CollectionViewSource
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = DatabaseGame;

        }

        /// <summary>
        /// Creates a Multiple system database for Hyperspin
        /// Adds symbolic links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buildMultiXml_Click(object sender, RoutedEventArgs e)
        {

            if (multiSystemDataGrid.Items.Count == 0)
            {
                System.Windows.MessageBox.Show("You haven't added games. Add with right click menu in manager lists");
                return;
            }

            if (multiSystemDataGrid.Items.Count < 10)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("You have less than 10 games, really?", "", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            if (_textMultiSysName.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("You need to name this system.");
                return;
            }

            // Check if the set multi system name in text box exists and ask to overwrite if it does.
            if (File.Exists(hsdb.HSPath + "\\Databases\\" + _textMultiSysName.Text + ".xml"))
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Database already exists, do you want to overwrite?", "", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            disableGUI();
            progressBar.IsActive = true;
            // Setting up the multi system creator variables from UI
            MuliSystemCreator MultiCreate = new MuliSystemCreator
            {
                CreateSymlinks = _symLinks.IsChecked.Value,
                defaultTheme = _defaultTheme.IsChecked.Value,
                CreateRomMap = _RomMap.IsChecked.Value,
                CreateGenres = _genGenres.IsChecked.Value,
                SettingsFile = _textSettingsFile.Text,
                CurrentMenu = _comboMainMenu.SelectedItem.ToString(),
                multiSystemName = _textMultiSysName.Text,
                HSPath = hsdb.HSPath,
                RLPath = hsdb.RLPath
            };

            MultiCreate.bw.DoWork += MultiCreate.bw_DoWork;
            MultiCreate.bw.RunWorkerCompleted += bw_RunWorkerMultiSystemCompleted;
            
            mm.RemoveAt(0);

            List<object> menuandgames = new List<object>();
            menuandgames.Add(m_System);
            menuandgames.Add(mm);

            MultiCreate.bw.RunWorkerAsync(menuandgames);
        }
        /// <summary>
        /// Multi system creator callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerMultiSystemCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                InfoMessage = "Last Operation: Multi-System Created: " + hsdb.HyperspinDatabaseXml ;
            }
            else
            {
                InfoMessage = "";
            }
            //populateDataGrid();
            mm.Insert(0, new DatabaseMenu("Main Menu"));
            enableGUI();
            progressBar.IsActive = false;
            listboxSystem.ItemsSource = mm;
            listboxSystem.Items.Refresh();
            
            //BuildMainMenuItems();
        }
  
        private void disableGUI()
        {
            listboxSystem.IsEnabled = false;
            MainGrid.IsEnabled = false;
        }
        private void enableGUI()
        {
            listboxSystem.IsEnabled = true;
            MainGrid.IsEnabled = true;
        }

        #region Favorite Options    
        /// <summary>
        /// Run the scan favorites background worker invoking the favorite classes worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buildFaves_Click(object sender, RoutedEventArgs e)
        {
            disableGUI();
            progressBar.IsActive = true;

            Favorites fave = new Favorites();
            fave.HSPath = hsdb.HSPath;
            fave.bw.WorkerSupportsCancellation = true;
            fave.bw.WorkerReportsProgress = true;
            fave.bw.DoWork += fave.bw_DoWork;
            fave.bw.RunWorkerCompleted += bw_RunWorkerFaveScanSystemCompleted;
            
            fave.bw.RunWorkerAsync(mm);

            //bw.DoWork += bw_DoWorkGetAllFavories;
            //bw.RunWorkerCompleted += bw_RunWorkerFaveScanSystemCompleted;

            //bw.RunWorkerAsync();

        }
        /// <summary>
        /// Add returned gamelist into the multisystem datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerFaveScanSystemCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            enableGUI();
            progressBar.IsActive = false;
            if (!e.Cancelled)
            {
                InfoMessage = "Last Operation: Multi-System Created: " + hsdb.HyperspinDatabaseXml;
            }
            else
            {
                InfoMessage = "Failed";
            }
            //populateDataGrid();

            m_System = (List<DatabaseGame>)e.Result;
            multiSystemDataGrid.ItemsSource = m_System;
            multiSystemDataGrid.Items.Refresh();
        }
        #endregion

        private void hsDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //fc.Clear();
            //explorer.Items.Clear();
            //filesBox.Items.Refresh();
            
            //// Grab the current selected cells column name
            //// If null exit the method
            //var s = sender as System.Windows.Controls.DataGrid;

            //try
            //{
            //    if (s.CurrentColumn != null)
            //    {
            //        HM.LastSelectedColumn = SelectedColumn;
            //        ColumnHeader = s.CurrentColumn.Header.ToString();
            //        SelectedColumn = ColumnHeader;
            //    }
            //    else
            //        return;

            //    if (SelectedColumn == "Fav")
            //        return;

            //    var game_click = hsDatagrid.SelectedValue as DatabaseGame;

            //    if (game_click == null)
            //        return;

            //    textblockSelectedInfo.Text = game_click.Description;

            //    HM.LastSelectedRomname = SelectedGame.RomName;
            //    SelectedGame.RomName = game_click.RomName;

            //    if (HM.LastSelectedColumn == SelectedColumn)
            //    {
            //        if (HM.LastSelectedRomname != SelectedGame.RomName)
            //        {
            //            media_wheel.Source = null;
            //            Updatemedia_wheel(columnHeader, SelectedGame);
            //        }
            //    }
            //    else
            //    {
            //        media_wheel.Source = null;
            //        Updatemedia_wheel(columnHeader, SelectedGame);
            //    }
            //}
            //catch (Exception)
            //{
                
               
            //}

           
        }
        private void hsDatagrid_CurrentCellChanged(object sender, EventArgs e)
        {
           // fc.Clear();
            //filesBox.ItemsSource = null;
            // Grab the current selected cells column name
            // If null exit the method


            //var s = sender as System.Windows.Controls.DataGrid;
            //string columnHeader = string.Empty;
            //if (s.CurrentColumn != null)
            //{
            //    HM.LastSelectedColumn = SelectedColumn;
            //    columnHeader = s.CurrentColumn.Header.ToString();
            //    SelectedColumn = columnHeader;
            //}
            //else
            //    return;
                     
            //var game_click = hsDatagrid.SelectedValue as DatabaseGame;

            //if (game_click == null)
            //    return;
            //textblockSelectedInfo.Text = game_click.Description;

            //HM.LastSelectedRomname = SelectedGame.RomName;
            //SelectedGame.RomName = game_click.RomName;

            //if (HM.LastSelectedColumn == SelectedColumn)
            //{
            //    if (HM.LastSelectedRomname != SelectedGame.RomName)
            //    {
            //        media_wheel.Source = null;
            //        Updatemedia_wheel(columnHeader);
            //    }
            //    else
            //    {
            //        fc.Clear();
            //        filesBox.Items.Refresh();
            //    }
            //}
            //else
            //{
            //    fc.Clear();
            //    filesBox.Items.Refresh();
            //    media_wheel.Source = null;
            //    Updatemedia_wheel(columnHeader);
            //}
        }

        /// <summary>
        /// Mouse up selected cells on the main datagrid list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hsDatagrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Set the selected game object
            SelectedGame = hsDatagrid.SelectedValue as DatabaseGame;

            var s = sender as System.Windows.Controls.DataGrid;
            string columnHeader = string.Empty;

            //Check is the selected column is not null then continue setting lastselected & selected
            if (s.CurrentColumn != null)
            {
                LastSelectedColumn = SelectedColumn ;
                columnHeader = s.CurrentColumn.Header.ToString();
                SelectedColumn = columnHeader;
                filesBox.ItemsSource = null;
            }
            else
                return;

            media_wheel.Source = null;
            // Set the text info from game desc
            textblockSelectedInfo.Text = SelectedGame.Description;

            // If the last column selected isn't the current one check if rom was the same selection
            if (LastSelectedColumn == SelectedColumn)
            {
                if (LastSelectedRomName != SelectedGame.RomName)
                {
                    Updatemedia_wheel(columnHeader);
                }
            }
            else
            {
                Updatemedia_wheel(columnHeader);
            }

            //image_wheel.Source = WheelSource;
            LastSelectedRomName = SelectedGame.RomName;

        }

        /// <summary>
        /// Update the viewer wheel with video, images etc
        /// </summary>
        /// <param name="columnHeader"></param>
        private void Updatemedia_wheel(string columnHeader)
        {
            // get the full path for this cell
            FullPath = string.Empty;
            FullPath = hsdb.getFullPath(columnHeader, SelectedGame, hsdb.HSPath);
            // If its not Hyperspin cell set the rocketlauncher path
            if (!hsdb.IsHyperspin)
                FullPath = hsdb.getFullPath(columnHeader, SelectedGame, hsdb.RLMediaPath);
            

            WheelSource = null;
            explorer.Items.Clear();
            fc.Clear();
            DirectoryInfo di;

            if (!hsdb.IsHyperspin)
            {
                if (FullPath != null)
                {
                    di = new DirectoryInfo(FullPath);
                    if (di.Exists)
                    {
                        explorer.AddSelectedFoldersFromHyperSpin(di);
                        explorer.Focus();
                        fc = FileManagement.cellFilesRefresh(SelectedColumn, FullPath);

                        if (fc.Count ==0)
                        {
                            foreach (object o in explorer.Items)
                            {
                                object container = explorer.ItemContainerGenerator.ContainerFromItem(o);
                                TreeViewItem item = container as TreeViewItem;
                                if (item != null)
                                {
                                    item.IsExpanded = true;
                                    OpenMulticolumnItems(item);
                                }
                            }
                            //explorer.SelectedValuePath = FullPath + "\\Disk-Inlay";
                        }
                    }
                }
            }
            else
            {
                fc = GetFileCollection();

                if (SelectedColumn != "Video")
                {
                    if (ViewerFile != null)
                    {
                        if (SelectedColumn == "Snds-Wheel" || SelectedColumn == "Snds-Click" || SelectedColumn == "BG-Music")
                        {
                         media_wheel.Source =new Uri( ViewerFile);
                         media_wheel.Play();
                        }
                        else
                        {
                            WheelSource = ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath(ViewerFile)));
                            textMediaInfo.Text = ImageInfo.getImageInfo(ViewerFile);
                        }
                    }
                }
                else if (SelectedColumn == "Theme")
                {

                }
            }  

            filesBox.ItemsSource = fc;
            SelectedGame.UpdateGameBools(SelectedColumn, FullPath,SelectedGame);
        }

        public List<FileCollection> GetFileCollection()
        {
            string ext = "";
            //ViewerFile = FullPath + SelectedGame.RomName + ext;

            if (SelectedColumn == "Video")
            {
                DisplaySelectedVideo();
            }
            else
            {

                if (SelectedColumn == "Special" || SelectedColumn == "Letters" || SelectedColumn == "GenreBG"
                    || SelectedColumn == "GenreWheel")
                {
                    ext = ".png";
                    if (FileManagement.doesDirectoryContainFiles(FullPath, "*.png"))
                    {
                        DirectoryInfo di = new DirectoryInfo(FullPath);
                        FileInfo[] fi = di.GetFiles("*.png");
                        foreach (FileInfo item in fi)
                        {
                            fc.Add(new FileCollection(item.Name, item.Extension, FullPath));
                        }
                    }

                }
                else if (SelectedColumn =="Wheel" || SelectedColumn =="Artwork1" || SelectedColumn =="Artwork2"
                     || SelectedColumn == "Artwork3" || SelectedColumn == "Artwork4")
                {
                    ext = ".png";
                    //Get all the matching files in the directory
                    FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, SelectedGame.RomName);

                    foreach (var item in fi)
                    {
                        if (item != null)
                            fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));
                    }

                    ViewerFile = FullPath + SelectedGame.RomName + ".png";
                                       
                }
                else if (SelectedColumn == "Snds-Wheel")
                {
                    ext = ".mp3";
                    if (FileManagement.doesDirectoryContainFiles(FullPath, "*" + ext))
                    {
                        DirectoryInfo di = new DirectoryInfo(FullPath);
                        FileInfo[] fi = di.GetFiles("*.mp3");
                        foreach (FileInfo item in fi)
                        {
                            fc.Add(new FileCollection(item.Name, item.Extension, FullPath));
                        }

                        ViewerFile = fc.ElementAt(0).FullPath + fc.ElementAt(0).Name;
                    }
                }
                else if (SelectedColumn == "Snds-Click")
                {
                    fc.Add(new FileCollection("Wheel Click", ".mp3", hsdb.HSPath + "\\Media\\" + SelectedGame.RomName + "\\Sound\\"));
                    ViewerFile = fc.ElementAt(0).FullPath + fc.ElementAt(0).Name + ".mp3";
                }
                else if (SelectedColumn == "BG-Music")
                {
                    ext = ".mp3";

                    FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, SelectedGame.RomName);

                    foreach (var item in fi)
                    {
                        if (item != null)
                            fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));
                    }

                    if (fc.Count!=0)
                        ViewerFile = fc.ElementAt(0).FullPath + fc.ElementAt(0).Name + ".mp3";
                }
                else if (SelectedColumn=="Theme")
                {
                    ext = ".zip";

                    FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, SelectedGame.RomName);

                    foreach (var item in fi)
                    {
                        if (item != null)
                            fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));
                    }
                }
                else if (SelectedColumn == "Pointer")
                {
                    ext = ".png";

                    FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, "Pointer");

                    foreach (var item in fi)
                    {
                        if (item != null)
                            fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));
                    }

                    ViewerFile = fc.ElementAt(0).FullPath;
                }
                else if (FileManagement.CheckForFile(ViewerFile))
                {
                    fc.Add(new FileCollection(SelectedGame.RomName, ext, ViewerFile));
                }
            }
            
            return fc;
        }
        private void OpenMulticolumnItems(TreeViewItem item)
        {
            foreach (object o in item.Items)
            {
                object container = item.ItemContainerGenerator.ContainerFromItem(o);
                TreeViewItem child = container as TreeViewItem;
                if (child != null)
                {
                    child.IsExpanded = true;
                    OpenMulticolumnItems(child);
                }
            }
        }

        /// <summary>
        /// Display image for when the filesbox is clicked.
        /// </summary>
        private void DisplaySelectedImageFilesBox (string path)
        {
            WheelSource = null;
            media_wheel.Source = null;
            textBigIcon.Visibility = Visibility.Collapsed;
            pdfBigIcon.Visibility = Visibility.Collapsed;
            //filesBox.ItemsSource = null;
            txtToPDF.IsEnabled = false;
            PDFtoIMG.IsEnabled = false;
            //string ext = ".png";

            ViewerFile = path;
            
            InfoMessage = ViewerFile;
            //Add in here selection of the files            
            string upperExt = ViewerFile.ToUpper();

            // If its an image just display it in the Image element
            if (upperExt.EndsWith(".JPG") || upperExt.EndsWith(".PNG") || upperExt.EndsWith(".GIF") || upperExt.EndsWith(".JPEG")
                             || upperExt.EndsWith(".BMP"))
            {
                if (FileManagement.CheckForFile(ViewerFile))
                {
                    WheelSource = ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath(ViewerFile)));
                    //image_wheel.Source = WheelSource;
                    textMediaInfo.Text = ImageInfo.getImageInfo(ViewerFile);
                }

                
            }
            // Enable the convert TExt to PDF button
            else if (upperExt.EndsWith(".TXT"))
            {
                txtToPDF.IsEnabled = true;
                textBigIcon.Visibility= Visibility.Visible;
            }
            // Enable the PDF Converter
            else if (upperExt.EndsWith(".PDF"))
            {
                PDFtoIMG.IsEnabled = true;               
                pdfBigIcon.Visibility = Visibility.Visible;                
            }


        }
        private void DisplaySelectedVideo()
        {
            Uri uriSource;
            string pathToMediaFile = FullPath + SelectedGame.RomName + ".mp4";
            string pathToMediaFile2 = FullPath + SelectedGame.RomName + ".png";
            string name ;

            if (FileManagement.CheckForFile(pathToMediaFile))
            {
                fc.Clear();
                filesBox.ItemsSource = fc;
                WheelSource = null;
                name = System.IO.Path.GetFileNameWithoutExtension(pathToMediaFile);

                //Get all the matching files in the directory
                FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, SelectedGame.RomName);

                foreach (var item in fi)
                {
                    if (item !=null)
                        fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));                    
                }
                
                uriSource = new Uri(pathToMediaFile);
                media_wheel.Source = uriSource;
                media_wheel.Play();
                //if (checkAutoplay.IsChecked.Value==true)
                //    media_wheel.Play();
            }
            else if (FileManagement.CheckForFile(pathToMediaFile2))
            {
                //Get all the matching files in the directory
                FileInfo[] fi = FileManagement.SearchAllFilesNameByRomname(FullPath, SelectedGame.RomName);

                foreach (var item in fi)
                {
                    if (item != null)
                        fc.Add(new FileCollection(System.IO.Path.GetFileNameWithoutExtension(item.FullName), item.Extension, item.FullName));
                }

                WheelSource = ImageEdits.BitmapFromUri(new Uri(pathToMediaFile2));
            }
            else
                WheelSource = null;
        }
        private void DisplayVideoFromFileBox(FileCollection videoFile)
        {
            Uri uriSource;
            WheelSource = null;            
            uriSource = new Uri(videoFile.FullPath);
            media_wheel.Source = uriSource;
            media_wheel.Play();
        }
        private void DisplaySelectedImageSystem(string path)
        {
            Uri uriSource;
            if (FileManagement.CheckForFile(path))
            {
                systemImage.Source = null;
                uriSource = new Uri(path);
                systemImage.Source = new BitmapImage(uriSource);               
            }
        }

        #region Game Launching
        /// <summary>
        /// Launch game through rocketlauncher standard. system romname
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            RL.RLPath = hsdb.RLPath; 
            RL.RocketLaunchGameSTD(hsdb.SystemName, SelectedGame.RomName);
        }
        /// <summary>
        /// Launch a rocket launch mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RLModeClick(object sender, RoutedEventArgs e)
        {
            var mi = e.Source as System.Windows.Controls.MenuItem;
            string mode = mi.Header.ToString();
            mode = mode.ToLower();
            RL.RocketLaunchMode(mode, hsdb.SystemName, SelectedGame.RomName);
        }        
        #endregion

        /// <summary>
        /// Scan for Hyperspin media button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ScanHS_Click(object sender, MouseButtonEventArgs e)
        {
            if (!hsdb.bw.IsBusy)
            {
                DisableGUIControls();                
                scanHSMediaFromGameObjects();                               
            }
        }
        private void tempDatatest_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.MessageBox.Show(e.Key.ToString());
            if (e.Key == Key.Delete)
                System.Windows.MessageBox.Show("");
        }
        private void moveTrash_Click(object sender, RoutedEventArgs e)
        {
            FileInfo s = tempDatatest.SelectedItem as FileInfo;
            if (tempDatatest.Items.Count > 0)
                if (tempDatatest.SelectedItems != null)
                {
                    string PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory + "\\Rubbish\\Hyperspin\\" + hsdb.SystemName + "\\Wheels\\";
                    DirectoryInfo di = new DirectoryInfo(PathToMoveTo);
                    if (!di.Exists)
                        di.Create();

                    FileInfo d = tempDatatest.SelectedItem as FileInfo;
                    if (d.Exists)
                    {
                        d.MoveTo(AppDomain.CurrentDomain.BaseDirectory + "\\Rubbish\\Hyperspin\\" + hsdb.SystemName + "\\Wheels\\" + d.Name);
                        //d.Delete();
                        unusedFilesCollection.Remove(s);
                    }
                      
                    //MessageBox.Show("Move To: " + AppDomain.CurrentDomain.BaseDirectory + "\\Rubbish\\HyperSpin\\Wheels" );
                }
                
        }

        /// <summary>
        /// Favorites
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<DatabaseGame> gamelist = new List<DatabaseGame>(DatabaseGame);
            //HM.buildFavorites(gamelist);
        }

        #region Tools

        #region Simple Wheel

        private void presetRefresh()
        {
            presetListBox.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo("presets\\wheels");
            FileInfo[] Files;
            Files = dir.GetFiles();

            foreach (var item in Files)
            {
                presetListBox.Items.Add(item.Name);
            }

            gravityDropDown.SelectedIndex = 4;
        }

        /// <summary>
        /// Update the wheel creator UI values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void presetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (presetListBox.SelectedItems.Count != 0)
            {
                //string preset = presetListBox.SelectedItem.ToString();
                //presetTextBox.Text = preset.Remove(preset.Length - 4);

                //if (File.Exists(@"presets\wheels\" + preset))
                //    //LoadXMLPreset(@"presets\wheels\" + preset);
                //    WheelPresest.LoadXmlPreset("wheels");
                //else
                //    System.Windows.MessageBox.Show("Cannot find " + @"presets\wheels\" + preset);

                if (presetListBox.SelectedItems.Count == 0)
                    return;
                string[] elements = WheelPresest.LoadXmlPreset(@"presets\wheels\" + presetListBox.SelectedItem.ToString());

                presetTextBoxCards.Text = presetListBoxCards.SelectedItem.ToString().Replace(".xml", string.Empty);

                // Update the type radio button
                if (elements[0] == "Wheels")
                    selectWheels.IsChecked = true;
                else if (elements[0] == "Letters")
                    selectLetters.IsChecked = true;
                else if (elements[0] == "Genre")
                    selectGenre.IsChecked = true;
                else if (elements[0] == "Special Art 3")
                    selectSpecialArt.IsChecked = true;

                fontTextBox.Text = elements[1];

                string On = elements[2];
                if (On == "True")
                    CaptionCheckBox.IsChecked = true;
                else
                    CaptionCheckBox.IsChecked = false;

                gamenameTextBox.Text = elements[3];
                specialPrefixTextBox.Text = elements[4];

                numericUpDownWidth.Value = Convert.ToDouble(elements[5]);
                numericUpDownHeight.Value = Convert.ToDouble(elements[6]);
                numericUpDownArc.Value = Convert.ToDouble(elements[7]);
                               
                On = elements[8];
                if (On == "True") { trimCheckBox.IsChecked = true; }
                else { trimCheckBox.IsChecked = false; }

                var bc = new BrushConverter();
                fillColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(elements[9]);
                
                System.Windows.Media.Brush fillBackgroundColor = fillColorButton.Background;
                simpleWheel.R = ((System.Windows.Media.Color)fillBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).R;
                simpleWheel.G = ((System.Windows.Media.Color)fillBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).G;
                simpleWheel.B = ((System.Windows.Media.Color)fillBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).B;
                simpleWheel.FillColorRGB = simpleWheel.getRGB();

                strokeColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(elements[10]);
                System.Windows.Media.Brush strokeBackgroundColor = strokeColorButton.Background;
                simpleWheel.R = ((System.Windows.Media.Color)strokeBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).R;
                simpleWheel.G = ((System.Windows.Media.Color)strokeBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).G;
                simpleWheel.B = ((System.Windows.Media.Color)strokeBackgroundColor.GetValue(SolidColorBrush.ColorProperty)).B;
                simpleWheel.StrokeColorRGB = simpleWheel.getRGB();

                numericUpDownStroke.Value = Convert.ToDouble(elements[11]);

                gravityDropDown.Text = elements[12];

                On = elements[13];
                if (On == "True")
                    GravitycheckBox.IsChecked = true;
                else
                    GravitycheckBox.IsChecked = false;

                bgColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(elements[14]);
                System.Windows.Media.Brush bgColorButtonColor = bgColorButton.Background;
                simpleWheel.R = ((System.Windows.Media.Color)bgColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).R;
                simpleWheel.G = ((System.Windows.Media.Color)bgColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).G;
                simpleWheel.B = ((System.Windows.Media.Color)bgColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).B;
                simpleWheel.BackgroundColorRGB = simpleWheel.getRGB();

                On = string.Empty;
                On = elements[15];
                if (On == "True")
                    transCheckBox.IsChecked = true;
                else
                    transCheckBox.IsChecked = false;

                shadowColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(elements[16]);
                System.Windows.Media.Brush shadowColorButtonColor = shadowColorButton.Background;
                simpleWheel.R = ((System.Windows.Media.Color)shadowColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).R;
                simpleWheel.G = ((System.Windows.Media.Color)shadowColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).G;
                simpleWheel.B = ((System.Windows.Media.Color)shadowColorButtonColor.GetValue(SolidColorBrush.ColorProperty)).B;
                simpleWheel.ShadowColorRGB = simpleWheel.getRGB();

                On = elements[17];
                if (On == "True")
                    ShadowcheckBox.IsChecked = true;
                else
                    ShadowcheckBox.IsChecked = false;

               numericUpDownShadow.Value = Convert.ToDouble(elements[18]);
                numericUpDownShadowDistance.Value = Convert.ToDouble(elements[19]);
                               
               On = elements[20];
               if (On == "True")
                  SwapcheckBox.IsChecked = true;
                else
                  SwapcheckBox.IsChecked = false;

               On = elements[21];
               if (On == "True")
                  RepagecheckBox.IsChecked = true;
               else
                  RepagecheckBox.IsChecked = false;
            }
        }
        public void LoadXMLPreset(string XML)
        {
            using (XmlReader read = XmlReader.Create(XML))
            {
                while (read.Read())
                {
                    if (read.IsStartElement())
                    {
                        switch (read.Name)
                        {
                            case "wheelType":                                
                                simpleWheel.WheelType = read.ReadString();
                                if (simpleWheel.WheelType == "Wheels")
                                    selectWheels.IsChecked = true;
                                else if (simpleWheel.WheelType == "Letters")
                                    selectLetters.IsChecked = true;
                                else if (simpleWheel.WheelType == "Genre")
                                    selectGenre.IsChecked = true;
                                else if (simpleWheel.WheelType == "Special Art 3")
                                    selectSpecialArt.IsChecked = true;
                                break;
                            case "font":
                                fontTextBox.Text = read.ReadString();
                                break;
                            case "labelenabled":
                                string On = read.ReadElementContentAsString();
                                if (On == "True")
                                    CaptionCheckBox.IsChecked = true;
                                else
                                    CaptionCheckBox.IsChecked = false;
                                break;
                            case "game_text":
                                gamenameTextBox.Text = read.ReadString();
                                break;
                            case "prefix_text":
                                specialPrefixTextBox.Text = read.ReadString();
                                break;
                            case "sizeX":
                                numericUpDownHeight.Value = read.ReadElementContentAsDouble();
                                break;
                            case "sizeY":
                                numericUpDownHeight.Value = read.ReadElementContentAsDouble();
                                break;
                            case "arc":
                                numericUpDownArc.Value = read.ReadElementContentAsDouble();
                                break;
                            case "trim":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    trimCheckBox.IsChecked = true;
                                else
                                    trimCheckBox.IsChecked = false;
                                break;
                            case "fillColor":
                                var bc = new BrushConverter();
                                fillColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(read.ReadString());
                                break;
                            case "strokeColor":
                                bc = new BrushConverter();
                                strokeColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(read.ReadString());
                                break;
                            case "stroke":
                                numericUpDownStroke.Value = read.ReadElementContentAsDouble();
                                break;
                            case "gravity":
                                gravityDropDown.Text = read.ReadString();
                                break;
                            case "gravityOn":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    GravitycheckBox.IsChecked = true;
                                else
                                    GravitycheckBox.IsChecked = false;
                                break;
                            case "bgColor":
                                bc = new BrushConverter();
                                bgColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(read.ReadString());
                                break;
                            case "transp":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    transCheckBox.IsChecked = true;
                                else
                                    transCheckBox.IsChecked = false;
                                break;
                            case "shadowColor":
                                bc = new BrushConverter();
                                shadowColorButton.Background = (System.Windows.Media.Brush)bc.ConvertFrom(read.ReadString());
                                break;
                            case "shadowOn":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    ShadowcheckBox.IsChecked = true;
                                else
                                    ShadowcheckBox.IsChecked = false;
                                break;
                            case "shadowSize":
                                numericUpDownShadow.Value = read.ReadElementContentAsDouble();
                                break;
                            case "shadowDistance":
                                numericUpDownShadowDistance.Value = read.ReadElementContentAsDouble();
                                break;
                            case "swap":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    SwapcheckBox.IsChecked = true;
                                else
                                    SwapcheckBox.IsChecked = false;
                                break;
                            case "repage":
                                On = read.ReadElementContentAsString();
                                if (On == "True")
                                    RepagecheckBox.IsChecked = true;
                                else
                                    RepagecheckBox.IsChecked = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {


                    }



                    
                }


            }
        }

        private void fontSelect_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            System.Windows.Forms.DialogResult result = fontDlg.ShowDialog();
            string font;
            if (result != System.Windows.Forms.DialogResult.Cancel)
            {
                font = fontDlg.Font.Name;
                StringBuilder fontEdit = new StringBuilder(fontDlg.Font.Name);
                fontEdit.Replace(" ", "-");
                font = fontEdit.ToString();
                fontTextBox.Text = font.ToString();
            }
        }

        private void selectWheels_Checked(object sender, RoutedEventArgs e)
        {
            simpleWheel.WheelType = selectWheels.Content.ToString();
            simpleWheel.selectedWheels = true;
            simpleWheel.WheelType = "Wheels";

            if (gamenameTextBox !=null)
                gamenameTextBox.Text = "Mario Bros.";
        }
        private void selectSpecialArt_Checked(object sender, RoutedEventArgs e)
        {
            //gamenameTextBox.Text = "Current Genre: Horse Racing Games";
            //specialPrefixTextBox.Text = "Current Genre: ";
            // Image Size
            //numericUpDownWidth.Value = 571;
            //numericUpDownHeight.Value = 120;
            // Image Stroke
           // numericUpDownStroke.Value = 1;
            //numericUpDownShadowDistance.Value = 3;
            // numericUpDownArc.Value = 2;
            labelRadioButton.IsChecked = true;

            simpleWheel.WheelType = "Special Art 3";
        }
        private void selectLetters_Checked(object sender, RoutedEventArgs e)
        {
            //gamenameTextBox.Text = "A";
            //specialPrefixTextBox.Text = "";
            //numericUpDownWidth.Value = 320;
            //numericUpDownHeight.Value = 240;
            //numericUpDownStroke.Value = 5;

            //numericUpDownShadowDistance.Value = 1;
            //numericUpDownArc.Value = 2;
            simpleWheel.selectedLetters = true;
            simpleWheel.WheelType = "Letters";
        }
        private void selectGenre_Checked(object sender, RoutedEventArgs e)
        {
            simpleWheel.WheelType = "Genre";
            simpleWheel.selectedGenre = true;
        }

        // Color buttons
        private void fillColorButton_Click(object sender, RoutedEventArgs e)
        {
            simpleWheel.FillColor = SetColorForPicker(sender as System.Windows.Controls.Button);
            simpleWheel.FillColorRGB = simpleWheel.getRGB();
        }
        private void strokeColorButton_Click(object sender, RoutedEventArgs e)
        {
            simpleWheel.StrokeColor = SetColorForPicker(sender as System.Windows.Controls.Button);
            simpleWheel.StrokeColorRGB = simpleWheel.getRGB();
        }
        private void shadowColorButton_Click(object sender, RoutedEventArgs e)
        {
            simpleWheel.ShadowColor = SetColorForPicker(sender as System.Windows.Controls.Button);
            simpleWheel.ShadowColorRGB = simpleWheel.getRGB();
        }
        private void bgColorButton_Click(object sender, RoutedEventArgs e)
        {
            simpleWheel.BackgroundColor = SetColorForPicker(sender as System.Windows.Controls.Button);
            simpleWheel.BackgroundColorRGB = simpleWheel.getRGB();
        }
        private string SetColorForPicker(System.Windows.Controls.Button button)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.AllowFullOpen = true;
            dlg.FullOpen = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BrushConverter conv = new BrushConverter();
                string strColor = System.Drawing.ColorTranslator.ToHtml(dlg.Color);
                button.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(strColor));

                simpleWheel.R = dlg.Color.R;
                simpleWheel.G = dlg.Color.G;
                simpleWheel.B = dlg.Color.B;

                return dlg.Color.Name.ToString();

            }
            else
                return null;
        }

        private void setColorForPresets()
        {

        }


        /// <summary>
        /// Save a wheel preset from UI values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePreset_Click(object sender, RoutedEventArgs e)
        {
            WheelPresest.saveXML_Preset(presetTextBox.Text,simpleWheel.WheelType,
                fontTextBox.Text,CaptionCheckBox.IsChecked.Value.ToString(),
                gamenameTextBox.Text,specialPrefixTextBox.Text,
                numericUpDownWidth.Value.ToString(),
                numericUpDownHeight.Value.ToString(),
                numericUpDownArc.Value.ToString(),trimCheckBox.IsChecked.Value.ToString(),
                fillColorButton.Background.ToString(),strokeColorButton.Background.ToString(),
                numericUpDownStroke.Value.ToString(),gravityDropDown.Text,GravitycheckBox.IsChecked.Value.ToString(),
                bgColorButton.Background.ToString(),transCheckBox.IsChecked.Value.ToString(),shadowColorButton.Background.ToString(),
                ShadowcheckBox.IsChecked.Value.ToString(),numericUpDownShadow.Value.ToString(),numericUpDownShadowDistance.Value.ToString(),
                 SwapcheckBox.IsChecked.Value.ToString(),RepagecheckBox.IsChecked.Value.ToString()
                );
                                      
        }

        private void saveXML_Preset(string presetName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            if (Directory.Exists("presets\\wheels"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\wheels\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("wheelType", simpleWheel.WheelType);
                    write.WriteElementString("font", fontTextBox.Text);
                    write.WriteElementString("labelenabled", CaptionCheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("game_text", gamenameTextBox.Text);
                    write.WriteElementString("prefix_text", specialPrefixTextBox.Text);
                    write.WriteElementString("sizeX", numericUpDownWidth.Value.ToString());
                    write.WriteElementString("sizeY", numericUpDownHeight.Value.ToString());
                    write.WriteElementString("arc", numericUpDownArc.Value.ToString());
                    write.WriteElementString("trim", trimCheckBox.IsChecked.Value.ToString());

                    write.WriteElementString("fillColor", fillColorButton.Background.ToString());
                    write.WriteElementString("strokeColor", strokeColorButton.Background.ToString());
                    write.WriteElementString("stroke", numericUpDownStroke.Value.ToString());

                    write.WriteElementString("gravity", gravityDropDown.Text);
                    write.WriteElementString("gravityOn", GravitycheckBox.IsChecked.Value.ToString());

                    write.WriteElementString("bgColor", bgColorButton.Background.ToString());
                    write.WriteElementString("transp", transCheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("shadowColor", shadowColorButton.Background.ToString());
                    write.WriteElementString("shadowOn", ShadowcheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("shadowSize", numericUpDownShadow.Value.ToString());
                    write.WriteElementString("shadowDistance", numericUpDownShadowDistance.Value.ToString());
                    write.WriteElementString("swap", SwapcheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("repage", RepagecheckBox.IsChecked.Value.ToString());
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }

                WheelPresest.GetPresetsFromDirectory("wheel");
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            string openPath = @"Exports\HyperSpin\Media\" + hsdb.SystemName + @"\Images\";
            if (Directory.Exists(openPath))
                System.Diagnostics.Process.Start(openPath);
            else
                InfoMessage = "Path doesn't exist : " + openPath;
        }


        private void setGuiValues()
        {
            simpleWheel.SystemName = hsdb.SystemName;

            if (transCheckBox.IsChecked == true)
                simpleWheel.BackgroundColor = "Transparent";
            else
                simpleWheel.BackgroundColor = simpleWheel.BackgroundColorRGB;

            if (numericUpDownArc.Value > 0)
                simpleWheel.distortArcTextOn = true;

            simpleWheel.ImageExe = "Convert.exe";
            simpleWheel.font = fontTextBox.Text;
            simpleWheel.Caption = gamenameTextBox.Text;
            simpleWheel.StrokeAmount = (decimal)numericUpDownStroke.Value;
            simpleWheel.Width = (decimal)numericUpDownWidth.Value;
            simpleWheel.Height = (decimal)numericUpDownHeight.Value;
            simpleWheel.GravityOn = (bool)GravitycheckBox.IsChecked;
            simpleWheel.Gravity = gravityDropDown.Text;
            //simpleWheel.SystemName = sysText.Text;
            simpleWheel.distortTextAmount = (decimal)numericUpDownArc.Value;
           
            /// Convert the shadow buttons background BRUSH to a standard Color.
            ///
            System.Windows.Media.Brush _Brush = shadowColorButton.Background;
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(_Brush.ToString());
            simpleWheel.ShadowColor = col.Name;


            simpleWheel.ShadowOn = (bool)ShadowcheckBox.IsChecked;
            simpleWheel.ShadowAmount = (decimal)numericUpDownShadow.Value;
            simpleWheel.ShadowDistance = (decimal)numericUpDownShadowDistance.Value;
            simpleWheel.ShadowSwapOn = (bool)SwapcheckBox.IsChecked;
            simpleWheel.ShadowRepageOn = (bool)RepagecheckBox.IsChecked;
            simpleWheel.ShadeOn = (bool)shadeCheckBox.IsChecked; ;
            simpleWheel.shadeX = (decimal)numericUpDownShadeX.Value;
            simpleWheel.ShadeY = (decimal)numericUpDownShadeY.Value;

            simpleWheel.PreviewText = gamenameTextBox.Text;
            simpleWheel.Prefix = specialPrefixTextBox.Text;

            simpleWheel.selectedWheels = (bool)selectWheels.IsChecked;
            simpleWheel.selectedSpecialArt = (bool)selectSpecialArt.IsChecked;
            simpleWheel.selectedLetters = (bool)selectLetters.IsChecked;

            simpleWheel.OnlyMiss = (bool)OnlyMissCheckBox.IsChecked;
            simpleWheel.PreviewOn = (bool)previewCreatedCheckBox.IsChecked;

            simpleWheel.Trim = (bool)trimCheckBox.IsChecked;
            simpleWheel.CaptionOrLabel = (bool)CaptionCheckBox.IsChecked;

        }
        /// <summary> getImParameters
        /// Grab parameters from the GUI and set to string PAram
        /// </summary>
        /// <returns></returns>
        public string getImParameters()
        {
            string distort = string.Empty;
            string shade = string.Empty;
            if (simpleWheel.ShadowSwapOn) { simpleWheel.ShadowSwap = " +swap "; } else { simpleWheel.ShadowSwap = ""; }
            if (simpleWheel.ShadowRepageOn) { simpleWheel.ShadowRepage = " +repage "; } else { simpleWheel.ShadowRepage = ""; }
            if (simpleWheel.Trim) { simpleWheel.Trimmed = " -trim "; } else { simpleWheel.Trimmed = " "; }

            if (simpleWheel.ShadowOn)
                simpleWheel.shadow = @" ( +clone -background " + simpleWheel.ShadowColor + " -shadow " + simpleWheel.ShadowAmount + "x" + simpleWheel.ShadowDistance + "+5+5 ) " + simpleWheel.Trimmed + simpleWheel.ShadowSwap + simpleWheel.ShadowRepage + " -geometry -4-4 -composite ";
            else
                simpleWheel.shadow = " +swap +repage ";

            if (simpleWheel.distortArcTextOn)
            {
                simpleWheel.distortText = " -distort Arc " + simpleWheel.distortTextAmount + " "; // " -trim +repage  ";//  " -trim +repage ";
            }
            else
                distort = "";

            if (simpleWheel.ShadeOn)
                simpleWheel.shadeText = " -shade " + simpleWheel.shadeX + "x" + simpleWheel.ShadeY + " -auto-level +level" + " 10,90% ";
            else
                simpleWheel.shadeText = "";

            simpleWheel.Param = "-background " + simpleWheel.BackgroundColor + " -fill " + simpleWheel.FillColorRGB +
                  " -stroke " + simpleWheel.StrokeColorRGB + " -strokewidth " + simpleWheel.StrokeAmount +
                  " -size " + simpleWheel.Width + "x" + simpleWheel.Height +
                  " -font " + simpleWheel.font +
                  " -gravity " + simpleWheel.Gravity +
                  " Caption:" + simpleWheel.distortText + simpleWheel.shadeText +
                  simpleWheel.shadow;

            return simpleWheel.Param;
        }
        #endregion


        #region Simple Wheel
        private void buttonPreviewWheel_Click(object sender, RoutedEventArgs e)
        {

            //image_wheel.Source = WheelSource;

            if (fontTextBox.Text == "") return;
            setGuiValues();

            try
            {
                simpleWheel.Param = getImParameters();
                simpleWheel.Caption = gamenameTextBox.Text;
                simpleWheel.Label = "\"" + simpleWheel.Caption + "\"";

                StringBuilder captionEdit = new StringBuilder(simpleWheel.Param);
                if (simpleWheel.CaptionOrLabel) { captionEdit.Replace("Caption:", "Caption:" + simpleWheel.Label); } else { captionEdit.Replace("Caption:", "Label:" + simpleWheel.Label); }
                simpleWheel.EndPath = "Preview.png";

                simpleWheel.Param = captionEdit.ToString() + simpleWheel.EndPath;
                if (!File.Exists(simpleWheel.EndPath))
                    File.Create(simpleWheel.EndPath);

                simpleWheel.runImageMagick(simpleWheel.ImageExe, true);
                WheelSource = ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath("Preview.png")));
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void runWheelButton_Click(object sender, RoutedEventArgs e)
        {
            if (simpleWheel.wheelWorker.IsBusy)
                return;
            if (rectHsScan.Fill != System.Windows.Media.Brushes.Green && OnlyMissCheckBox.IsChecked.Value == true)
            {
                System.Windows.MessageBox.Show("You need to run a HS scan first for missing wheels.");
                return;
            }

            setGuiValues();
            runWheelButton.IsEnabled = false;
            simpleWheel.savedParam = "";

            List<DatabaseGame> Wheelgamelist = new List<DatabaseGame>(DatabaseGame);
            simpleWheel.Game_List = Wheelgamelist;

            if (selectWheels.IsChecked == true)
            {
                simpleWheel.Param = getImParameters();
                simpleWheel.savedParam = simpleWheel.Param;
                //simpleWheel.GameCount = HLM_List.Count;
                //progressBar1.Maximum = GameCount;
                simpleWheel.wheelWorker.RunWorkerAsync();
            }
            else if (selectSpecialArt.IsChecked == true)
            {
                simpleWheel.Param = getImParameters();
                simpleWheel.savedParam = simpleWheel.Param;
                //simpleWheel.popGenre();
                simpleWheel.Prefix = specialPrefixTextBox.Text;
                //simpleWheel.GameCount = genre_list.Count;
                // progressBar1.Maximum = GameCount;
                simpleWheel.wheelWorker.RunWorkerAsync();
            }

            if (selectLetters.IsChecked == true)
            {
                //    //param = getImageMagickParams();
                simpleWheel.Param = getImParameters();
                simpleWheel.savedParam = simpleWheel.Param;
                // progressBar1.Maximum = 41;
                simpleWheel.wheelWorker.RunWorkerAsync();
            }
            if (selectGenre.IsChecked == true)
            {
                simpleWheel.Param = getImParameters();
                //    savedParam = Param;
                simpleWheel.savedParam = simpleWheel.Param;
                //    GameCount = selectGenre.Items.Count;
                //    progressBar1.Maximum = GameCount;
                simpleWheel.wheelWorker.RunWorkerAsync();
            }
        }
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            simpleWheel.wheelWorker.CancelAsync();
            runWheelButton.IsEnabled = true;
            InfoMessage = "Simple wheel batch stopped";
        }
        private void previewCreatedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            simpleWheel.PreviewOn = true;
        }
        private void previewCreatedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            simpleWheel.PreviewOn = false;
        }
        private void OnlyMissCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Image PResets

        private void populateImagePresets()
        {
           // presetListBoxCards3.Items.Clear();
           DirectoryInfo di = new DirectoryInfo(@"presets\images\");
           foreach (var item in di.GetFiles("*.xml"))
             {
               presetListBoxCards3.Items.Add(item.Name);
             }
           _comboPresetFade.ItemsSource = presetListBoxCards3.Items;
        }

        /// <summary>
        /// Instruction card list selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void presetListBoxCards3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (presetListBoxCards3.SelectedItems.Count == 0)
                return;
            // set the preset name text box
            presetTextBoxCard3s.Text = presetListBoxCards3.SelectedItem.ToString().Replace(".xml", string.Empty);

            // get the array of strings from xml
            string[] elements = ImagePresets.LoadXMLPresetImages(@"presets\images\" + presetListBoxCards3.SelectedItem.ToString());
            
            // Update GUI Values
            _text_ImageAuthor.Text = elements[0];
            prefixTextbox.Text = elements[1];

            // Get the media type
            string mediaType = elements[2];
            if (mediaType == "Layer 1")
                Fade1.IsChecked = true;
            else if (mediaType == "Layer 2")
                Fade2.IsChecked = true;
            else if (mediaType == "Layer 3")
                Fade3.IsChecked = true;
            else if (mediaType == "Layer -1")
                Fade4.IsChecked = true;
            else if (mediaType == "Background")
                PauseBack.IsChecked = true;
            else if (mediaType == "Bezel Background")
                BezelBack.IsChecked = true;

            _numOutWidth.Value = Convert.ToDouble(elements[3]);
            _numOutHeight.Value = Convert.ToDouble(elements[4]);
            RatioComboBox1.Text = elements[5];
            ResizeCheckbox.IsChecked = Convert.ToBoolean(elements[6]);
            StretchCheckBox.IsChecked = Convert.ToBoolean(elements[7]);
            TileCheckBox.IsChecked = Convert.ToBoolean(elements[8]);
            TileW.Value = Convert.ToDouble(elements[9]);
            TileH.Value = Convert.ToDouble(elements[10]);
            FlipLCheck.IsChecked = Convert.ToBoolean(elements[11]);
            FlipRCheck.IsChecked = Convert.ToBoolean(elements[12]);
            JPGCheckbox.IsChecked = Convert.ToBoolean(elements[13]);

        }
        /// <summary>
        /// Save Other images xml preset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveXMLPresetImages(object sender, RoutedEventArgs e)
        {
            var presetType = OtherImages.RLMediaType.PauseBG;
            if (Fade1.IsChecked.Value)
                presetType = OtherImages.RLMediaType.Fade1;
            else if (Fade2.IsChecked.Value)
                presetType = OtherImages.RLMediaType.Fade2;
            else if (Fade3.IsChecked.Value)
                presetType = OtherImages.RLMediaType.Fade3;
            else if (Fade4.IsChecked.Value)
                presetType = OtherImages.RLMediaType.FadeExit;
            else if (BezelBack.IsChecked.Value)
                presetType = OtherImages.RLMediaType.BezelBG;
            else
                presetType = OtherImages.RLMediaType.PauseBG;

            ImagePresets.saveXMLPresetImages(
                presetTextBoxCard3s.Text,
                presetType, 
                _text_ImageAuthor.Text, // Author Text
                prefixTextbox.Text, // Prefix Text
                _numOutWidth.Value.ToString(),
            _numOutHeight.Value.ToString(),
            ResizeCheckbox.IsChecked.Value.ToString(),
            StretchCheckBox.IsChecked.Value.ToString(),
            RatioComboBox1.Text,
            TileCheckBox.IsChecked.Value.ToString(),
            TileW.Value.ToString(),
            TileH.Value.ToString(),
            FlipLCheck.IsChecked.Value.ToString(),
            FlipRCheck.IsChecked.Value.ToString(),
            JPGCheckbox.IsChecked.Value.ToString());

            presetListBoxCards3.Items.Clear();
            populateImagePresets();
        }
        private void saveXMLPresetImages(string presetName)
        { 

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            string presetType;
            if (Fade1.IsChecked == true)
                presetType = Fade1.Content.ToString();
            else if (Fade2.IsChecked == true)
                presetType = Fade2.Content.ToString();
            else if (Fade3.IsChecked == true)
                presetType = Fade3.Content.ToString();
            else if (Fade4.IsChecked == true)
                presetType = Fade4.Content.ToString();
            else if (BezelBack.IsChecked == true)
                presetType = BezelBack.Content.ToString();
            else
                presetType = PauseBack.Content.ToString();

            if (Directory.Exists("presets\\images"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\images\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("author", _text_ImageAuthor.Text);
                    write.WriteElementString("prefix", prefixTextbox.Text);  
                    write.WriteElementString("mediaType", presetType);
                    write.WriteElementString("outWidth", _numOutWidth.Value.ToString());
                    write.WriteElementString("outHeight", _numOutHeight.Value.ToString());
                    write.WriteElementString("resize", ResizeCheckbox.IsChecked.Value.ToString());
                    write.WriteElementString("stretch", StretchCheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("ratio", RatioComboBox1.Text);
                    write.WriteElementString("tile", TileCheckBox.IsChecked.Value.ToString());
                    write.WriteElementString("tileWidth", TileW.Value.ToString());
                    write.WriteElementString("tileHeight", TileH.Value.ToString());
                    write.WriteElementString("flipL", FlipLCheck.IsChecked.Value.ToString());
                    write.WriteElementString("flipR", FlipRCheck.IsChecked.Value.ToString());
                    write.WriteElementString("jpg", JPGCheckbox.IsChecked.Value.ToString());
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }
            }
        }
        private void LoadXMLPresetImages(string xml)
        {
            string xmlpath = @"presets\images\" + xml;
            if (File.Exists(xmlpath))
            {
                using (XmlReader read = XmlReader.Create(xmlpath))
                {
                    while (read.Read())
                    {
                        if (read.IsStartElement())
                        {
                            switch (read.Name)
                            {
                                case "author":
                                    _text_ImageAuthor.Text = read.ReadString();
                                    break;
                                case "prefix":
                                    prefixTextbox.Text = read.ReadString();
                                    break;
                                case "mediaType":
                                    string mediaType = read.ReadString();
                                    if (mediaType == "Layer 1")
                                        Fade1.IsChecked = true;
                                    else if (mediaType  == "Layer 2")
                                        Fade2.IsChecked = true;
                                    else if (mediaType == "Layer 3")
                                        Fade3.IsChecked = true;
                                    else if (mediaType == "Layer -1")
                                        Fade4.IsChecked = true;
                                    else if (mediaType == "Background")
                                        PauseBack.IsChecked = true;
                                    else if (mediaType == "Bezel Background")
                                        BezelBack.IsChecked = true;
                                    break;
                                case "outWidth":
                                    _numOutWidth.Value = Convert.ToDouble(read.ReadString());
                                    break;
                                case "outHeight":
                                    _numOutHeight.Value = Convert.ToDouble(read.ReadString());
                                    break;
                                case "ratio":
                                    RatioComboBox1.Text = read.ReadString();
                                    break;
                                case "resize":
                                    ResizeCheckbox.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                case "stretch":
                                    StretchCheckBox.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                case "tile":
                                    TileCheckBox.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                case "tileWidth":
                                    TileW.Value = Convert.ToDouble(read.ReadString());
                                    break;
                                case "tileHeight":
                                    TileH.Value = Convert.ToDouble(read.ReadString());
                                    break;
                                case "flipL":
                                    FlipLCheck.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                case "flipR":
                                    FlipRCheck.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                case "jpg":
                                    JPGCheckbox.IsChecked = Convert.ToBoolean(read.ReadString());
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

        }

        #endregion

        #region Youtube Downloading

        /// <summary>
        /// Button that search youtube from selected game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonSearchYoutube(object sender, RoutedEventArgs e)
        {
            if (!browser.IsEnabled)            
                browser.IsEnabled = true;

            if (SelectedGame !=null)
                browser.Navigate(Videos.YoutubeSearch(SelectedGame,hsdb.SystemName));            
        }

        private void btn_ytb_downloader_Click(object sender, RoutedEventArgs e)
        {          
            YoutubeSearch();
        }


        /// <summary>
        /// Download youtube
        /// </summary>
        private void DownloadVideo()
        {

            int size = (int)_comboResolution.SelectedItem;
            if (hsdb.IsHyperspin)
                Videos.FileToSaveAs = hsdb.HSPath + "\\Media" + "\\" + hsdb.SystemName + "\\" + "Video" + "\\" + SelectedGame.RomName;
            else { }

            Videos.DownloadVideo(txtUrl.Text, (int)_comboResolution.SelectedItem);

        }
        private void YoutubeSearch()
        {

            var koof = hsDatagrid.SelectedItem as DatabaseGame;
              if (koof ==null)
                  return;

            string GameName = koof.Description;

            //if (sender == YoutubeGame)
            string YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName;
            //if (sender == YoutubeGameSystem)
               YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName + "+" + hsdb.SystemName;
            //if (sender == YoutubeGameTrailer)
            //    YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName + "+" + Hlm.sysName + "+" + "Trailer";

            //else
            //{
            //    if (!Directory.Exists(Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom))
            //        Directory.CreateDirectory(Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom);
            //    FileToSaveAs = Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom + "\\" + selectedRom;
            //}

              if (!browser.IsEnabled)
              {
                  browser.IsEnabled = true;
                  browser.Navigate(Videos.YoutubeSearch(SelectedGame,YoutubeSearchLink));
              }
              else
              {
                  browser.Navigate(Videos.YoutubeSearch(SelectedGame, YoutubeSearchLink));
              }
            
            //HM.FileToSaveAs = FileToSaveAs;
            
        }
        #endregion

        private void TabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //browser.Navigate(Properties.Settings.Default.homePage);     
            browser.IsEnabled = false;
            //Uri browserUrl = new Uri("www.google.com");            
        }

        #region AssetSearcher
        private void btn_ytb_downloader_Copy_Click(object sender, RoutedEventArgs e)
        {
            DownloadVideo();
        }
        private void txtUrl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                browser.Navigate(txtUrl.Text);
        }
        private void browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }
        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((browser != null) && (browser.CanGoBack));
        }
        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoBack();
        }
        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((browser != null) && (browser.CanGoForward));
        }
        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoForward();
        }
        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                browser.Navigate(txtUrl.Text);
            }
            catch (Exception)
            {


            }

        }        
        #endregion

      #region MediaElement Controllers
                      // Play the media.
      void OnMouseDownPlayMedia(object sender, MouseButtonEventArgs args)
      {

         // The Play method will begin the media if it is not currently active or 
         // resume media if it is paused. This has no effect if the media is
         // already running.
         media_wheel.Play();

         // Initialize the MediaElement property values.
         InitializePropertyValues();

      }
      // Pause the media.
      void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
      {

         // The Pause method pauses the media if it is currently running.
         // The Play method can be used to resume.
         media_wheel.Pause();

      }
      // Stop the media.
      void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
      {

         // The Stop method stops and resets the media to be played from
         // the beginning.
         media_wheel.Stop();
         //MessageBox.Show(media_wheel.NaturalVideoWidth.ToString());
        // MessageBox.Show(media_wheel.NaturalDuration.ToString());
      }
      // Change the volume of the media.
      private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
      {
         media_wheel.Volume = (double)volumeSlider.Value;
      }
      // Change the speed of the media.
      private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
      {
         //myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
      }
      // When the media opens, initialize the "Seek To" slider maximum value
      // to the total number of miliseconds in the length of the media clip.
      private void Element_MediaOpened(object sender, EventArgs e)
      {
          try
          {
              timelineSlider.Maximum = media_wheel.NaturalDuration.TimeSpan.TotalMilliseconds;
          }
          catch (Exception)
          {
              
         
          }
       
      }
      // When the media playback is finished. Stop() the media to seek to media start.
      private void Element_MediaEnded(object sender, EventArgs e)
      {
         media_wheel.Stop();
         
      }
      // Jump to different parts of the media (seek to). 
      private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
      {
         int SliderValue = (int)timelineSlider.Value;
         
         // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds.
         // Create a TimeSpan with miliseconds equal to the slider value.
         TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
         media_wheel.Position = ts;
         media_wheelTime.Text = ts.ToString();
         //media_wheel.Pause();
         //media_wheel.Play();
      }
      void InitializePropertyValues()
      {
         // Set the media's starting Volume and SpeedRatio to the current value of the
         // their respective slider controls.
         media_wheel.Volume = (double)volumeSlider.Value;
         //myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
      }
        #endregion

        #region Flyouts
      private void ToggleFlyout(int index)
      {
          var flyout = this.Flyouts.Items[index] as Flyout;
          if (flyout == null)
          {
              return;
          }

          flyout.IsOpen = !flyout.IsOpen;
      }
      private void settingsFlyout_Click(object sender, RoutedEventArgs e)
      {
          ToggleFlyout(0);
          SearchDatagrid.ScrollIntoView(SearchDatagrid.SelectedIndex);

      }
      #endregion

      #region Filters
      private void openFilters_Click(object sender, RoutedEventArgs e)
      {
          ToggleFlyout(1);
      }
      private void toggleHS_Checked(object sender, RoutedEventArgs e)
      {
          //toggleXML.IsChecked = false;
          DisplayHSColumns();
      }
      private void toggleHS_Unchecked(object sender, RoutedEventArgs e)
      {
          disableHSColumns();
      }
      private void toggleRL_Checked(object sender, RoutedEventArgs e)
      {
          DisplayRLColumns();
      }
      private void toggleRL_Unchecked(object sender, RoutedEventArgs e)
      {
          disableRLColumns();
      }
      private void toggleXML_Checked(object sender, RoutedEventArgs e)
      {
          toggleHS.IsEnabled = false;
          toggleRL.IsEnabled = false;
          hsDatagrid.IsReadOnly = false;
          saveFaves.IsEnabled = true;
          saveXml.IsEnabled = true;
          saveGXml.IsEnabled = true;
          addFaveToGenre.IsEnabled = true;
          addFaveToGenre.IsEnabled = true;
          disable_All_ArtworkColumns();
          // Favorite column 0
          // XML 1 - 14 columns
          // RL        15 - 27

          hsDatagrid.Columns[0].Visibility = Visibility.Visible;
          hsDatagrid.Columns[1].Visibility = Visibility.Visible;
          hsDatagrid.Columns[2].Visibility = Visibility.Visible;
          hsDatagrid.Columns[3].Visibility = Visibility.Visible;
          hsDatagrid.Columns[4].Visibility = Visibility.Visible;
          hsDatagrid.Columns[5].Visibility = Visibility.Visible;
          hsDatagrid.Columns[6].Visibility = Visibility.Visible;
          hsDatagrid.Columns[7].Visibility = Visibility.Visible;
          hsDatagrid.Columns[8].Visibility = Visibility.Visible;
          hsDatagrid.Columns[9].Visibility = Visibility.Visible;

          _ManagerTab.IsSelected = true;


      }
      private void toggleXML_Unchecked(object sender, RoutedEventArgs e)
      {
          hsDatagrid.Columns[0].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[4].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[5].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[6].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[7].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[8].Visibility = Visibility.Collapsed;
          hsDatagrid.Columns[9].Visibility = Visibility.Collapsed;
          saveFaves.IsEnabled = false;
          saveXml.IsEnabled = false;
          saveGXml.IsEnabled = false;
          addFaveToGenre.IsEnabled = false;
          hsDatagrid.IsReadOnly = true;
          toggleHS.IsEnabled = true;
          toggleRL.IsEnabled = true;

          if (toggleHS.IsChecked == true)
              DisplayHSColumns();
          if (toggleRL.IsChecked == true)
              DisplayRLColumns();


      }
      private void disable_All_ArtworkColumns()
      {
          for (int i = 10; i < 40; i++)
          {
              if (hsDatagrid.Columns[i] != null)
              {
                  hsDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                  SearchDatagrid.Columns[i].Visibility = Visibility.Collapsed;
              }
          }

      }
      private void DisplayHSColumns()
      {
          if (toggleHS.IsChecked == true)
          {
              disableHSColumns();

              if (hsdb.SystemName == "Main Menu")
              {                  
                  // Disable the manufacturer and description for Main Menu
                  hsDatagrid.Columns[2].Visibility = Visibility.Collapsed;
                  hsDatagrid.Columns[3].Visibility = Visibility.Collapsed;

                  hsDatagrid.Columns[10].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[11].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[16].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[17].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[18].Visibility = Visibility.Visible;

                  SearchDatagrid.Columns[2].Visibility = Visibility.Collapsed;
                  SearchDatagrid.Columns[3].Visibility = Visibility.Collapsed;

                  SearchDatagrid.Columns[10].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[11].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[16].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[17].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[18].Visibility = Visibility.Visible;

                  

                  for (int i = 19; i < 25; i++)
                  {
                      if (hsDatagrid.Columns[i] != null)
                      {
                          hsDatagrid.Columns[i].Visibility = Visibility.Visible;
                          SearchDatagrid.Columns[i].Visibility = Visibility.Visible;
                      }
                  }

                  hsDatagrid.Columns[27].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[27].Visibility = Visibility.Visible;
              }
              else
              {

                  // Disable the manufacturer and description for Main Menu
                  hsDatagrid.Columns[2].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[3].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[2].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[3].Visibility = Visibility.Visible;

                  for (int i = 10; i < 19; i++)
                  {
                      if (hsDatagrid.Columns[i] != null)
                      {
                          hsDatagrid.Columns[i].Visibility = Visibility.Visible;
                          SearchDatagrid.Columns[i].Visibility = Visibility.Visible;
                      }
                  }
                  for (int i = 19; i < 23; i++)
                  {
                      hsDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                      SearchDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                  }

                  hsDatagrid.Columns[24].Visibility = Visibility.Collapsed;
                  hsDatagrid.Columns[25].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[26].Visibility = Visibility.Visible;
                  hsDatagrid.Columns[27].Visibility = Visibility.Collapsed;

                  SearchDatagrid.Columns[24].Visibility = Visibility.Collapsed;
                  SearchDatagrid.Columns[25].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[26].Visibility = Visibility.Visible;
                  SearchDatagrid.Columns[27].Visibility = Visibility.Collapsed;

              }

          }
      }
      private void disableHSColumns()
      {
          for (int i = 10; i < 28; i++)
          {
              if (hsDatagrid.Columns[i] != null)
              {
                  hsDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                  SearchDatagrid.Columns[i].Visibility = Visibility.Collapsed;
              }
          }
      }
      private void DisplayRLColumns()
      {
          if (toggleRL.IsChecked == true)
              for (int i = 28; i < 40; i++)
              {
                  if (hsDatagrid.Columns[i] != null)
                  {
                      hsDatagrid.Columns[i].Visibility = Visibility.Visible;
                      SearchDatagrid.Columns[i].Visibility = Visibility.Visible;
                  }
              }
      }
      private void disableRLColumns()
      {
          if (toggleRL.IsChecked == false)
              for (int i = 28; i < 40; i++)
              {
                  if (hsDatagrid.Columns[i] != null)
                  {
                      hsDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                      SearchDatagrid.Columns[i].Visibility = Visibility.Collapsed;
                  }
              }
      }
      /// <summary>
      /// Filtering by Text
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void filterText_TextChanged_1(object sender, TextChangedEventArgs e)
      {
          string filter = string.Empty;
          ICollectionView cv = null;
          if (hsDatagrid.ItemsSource != null)
          {
              System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
              filter = t.Text;
              cv = CollectionViewSource.GetDefaultView(hsDatagrid.ItemsSource);
          }

          if (filter == "")
              cv.Filter = null;
          else
          {
              cv.Filter = o =>
              {
                  DatabaseGame h = o as DatabaseGame;
                  return (h.Description.ToUpper().Contains(filter.ToUpper()));
              };
          }
      }
     /// <summary>
     /// Text Filtering
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
          private void filterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (hsDatagrid.ItemsSource != null)
            {
                System.Windows.Controls.TextBox t = (System.Windows.Controls.TextBox)sender;
                string filter = t.Text;
                ICollectionView cv;
                //if (lv.IsVisible)
                //{
                //    cv = CollectionViewSource.GetDefaultView(lv.ItemsSource);
                //}
                //else
                //{
                //    cv = CollectionViewSource.GetDefaultView(tables_grid.ItemsSource);
                //}

                cv = CollectionViewSource.GetDefaultView(hsDatagrid.ItemsSource);

                if (filter == "")
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        DatabaseGame g = o as DatabaseGame;
                        return (g.Description.ToUpper().Contains(filter.ToUpper()));
                    };
                }
            }
        }
      #endregion

        #region Database Editing
          /// <summary>
          /// Save favorites
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          private void saveFaves_Click(object sender, RoutedEventArgs e)
          {
              if (hsdb.SystemName == "Main Menu")
                  return;
              else
              {
                  string path = System.IO.Path.GetDirectoryName(hsdb.HyperspinDatabaseXml);
                  path = path + "\\favorites.txt";

                  if (!File.Exists(path))
                  {
                      File.CreateText(path);
                  }

                  try
                  {
                      using (StreamWriter writer =
                          new StreamWriter(path))
                      {
                          foreach (DatabaseGame item in hsDatagrid.Items)
                          {
                              if (item.IsFavorite)
                                  writer.WriteLine(item.RomName);
                          }

                          writer.Close();
                      }

                      InfoMessage = "Last Operation: Saved favorites:  " + path;
                  }
                  catch (Exception)
                  {

                      InfoMessage = "Error: Path:  " + path;
                  }
              }
          }          
          #endregion
        #endregion

        private void explorer_ExplorerError(object sender, ExplorerErrorEventArgs e)
        {
            System.Windows.MessageBox.Show(e.Exception.Message);
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            explorer.SelectedPath = txtPath.Text;
        }
        private void filesBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var koof = sender as System.Windows.Controls.DataGrid;
            if (koof.Items.Count == 0)
                return;

            var ff = koof.SelectedItem as FileCollection;

            if (ff !=null)
            {
                if (SelectedColumn == "Special" || SelectedColumn == "Letters"
                     || SelectedColumn == "GenreBG" || SelectedColumn == "GenreWheel")
                {
                    DisplaySelectedImageFilesBox(ff.FullPath + ff.Name);
                }
                else if (SelectedColumn == "Video" || SelectedColumn == "Videos")
                {
                    DisplayVideoFromFileBox(ff);
                }
                else if (SelectedColumn =="Snds-Wheel")
                {
                    media_wheel.Source = new Uri(ff.FullPath + ff.Name);
                    media_wheel.Play();
                }
                else 
                    DisplaySelectedImageFilesBox(ff.FullPath);
            }
                
        }
        private void explorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (explorer.SelectedItem != null)
            {
                if (!IsHyperspin)
                {
                    FullPath = explorer.SelectedPath;
                    fc.Clear();
                    fc = FileManagement.cellFilesRefresh(SelectedColumn,FullPath);
                    filesBox.ItemsSource = fc;
                    filesBox.Items.Refresh();

                    if (filesBox.Items.Count > 0)
                    {
                        var fc1 = filesBox.Items[0] as FileCollection;
                        DisplaySelectedImageFilesBox(fc1.FullPath);
                    }
                }
                else
                {
                    
                }
            }
        }
        private void cellFilesRefresh(bool HSSpecialArts = false)
        {            
            try
            {
                fc.Clear();
                fc = FileManagement.cellFilesRefresh(SelectedColumn,FullPath);

                filesBox.Items.Refresh();

                if (filesBox.Items.Count > 0)
                {
                    var fc1 = filesBox.Items[0] as FileCollection;
                    DisplaySelectedImageFilesBox(fc1.FullPath);
                }
                    //DisplaySelectedImageFilesBox(filesBox.Items[0] as FileCollection);

                //if (Contents.Items.Count >= 1)
                //{
                //    //    //Contents.Focus();
                //    Contents.SelectedIndex = 0;

                //}
            }

            catch (Exception)
            {

             
            }

        }

        /// <summary>
        /// Cancel background workers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectScanCancel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CancelWorkers();
        }
        private void saveXml_Click(object sender, RoutedEventArgs e)
        {
                //if (hsDatagrid.Items.Count > 0)
                //{
                //    List<Game> gam = new List<Game>(game);
                //    gam.Sort((x, y) => String.Compare(x.RomName, y.RomName));
                //    SerializeToXML(gam, hsdb.SystemName, hsdb.SystemName + ".xml");
                   
                //}
            if (hsdb.SystemName == "Main Menu")
                System.Windows.MessageBox.Show("Main menu not available");
            else
            {
                if (hsDatagrid.Items.Count > 0)
                {
                    List<DatabaseGame> game = new List<DatabaseGame>(DatabaseGame);
                    game.Sort((x, y) => String.Compare(x.RomName, y.RomName));
                    Database.SerializeToXML(game, hsdb.SystemName, hsdb.SystemName, hsdb.HSPath);
                }
            }

            buildGameObjectsFromXml();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="games"></param>
        /// <param name="systemname"></param>
        /// <param name="dbName"></param>
        static internal void SerializeToGenreXML(List<DatabaseGame> games, string systemname, string dbName, string HSPath = "")
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

        private void saveXmlMenu_Click(object sender, RoutedEventArgs e)
        {
            //mm = new List<Menu>();
            //foreach (var item in listboxSystem.Items)
            //{
            //    var neh = item as Menu;
            //    mm.Add(new Menu(item.ToString()));
            //    mm.Add(new Menu(item));
            //}

            DatabaseMenu m = new DatabaseMenu();
            string currentMenu = _comboMainMenu.SelectedItem.ToString();
            bool flagRefresh = false;
            if (textboxSaveXmlName.Text != "")
            {
                currentMenu = textboxSaveXmlName.Text;
                flagRefresh = true;
            }
            else
                textboxSaveXmlName.Text = currentMenu;
            m.SerializeMainMenuItems(currentMenu, MmManagerCurrent.ToList(), hsdb.HSPath + "\\Databases\\Main Menu\\");


            if (flagRefresh)
            {
                _comboMainMenu.ItemsSource = hsdb.getMainMenuXmls();
                _comboMainMenu.SelectedValue = currentMenu;
                _comboMenuManage.ItemsSource = hsdb.getMainMenuXmls();
                _comboMenuManage.SelectedValue = currentMenu;

            }
            //SerializeMainMenuItems();
           
            //mm.RemoveAt(0);
            //  Add lib namespace with empty prefix
                        
        }
        public string SerializeWithXMLWriter()
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(MyObject));
            //StringBuilder builder = new StringBuilder();
            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.OmitXmlDeclaration = true;

            //using (XmlWriter stringWriter = new StringWriter(builder, settings))
            //{
            //    serializer.Serialize(stringWriter, comments);
            //    return builder.ToString();
            //}

            return string.Empty;
        }
        private void listboxSystem_Drop(object sender, System.Windows.DragEventArgs e)
        {
  
        }
        private void listboxSystem_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Method for shutting down the browser
        /// </summary>
        private void DisposeOfBrowser()
        {
            browser.Dispose();
            browser = null;
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }
        private void menuEdit_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(1);
        }
        private void removeSystem_Click(object sender, RoutedEventArgs e)
        {
            // Make a temporary list of menu and loop through selected items.
            // Then loop over the new list of selected items and remove from the ViewCollectionSource
            // If you don't do this it can lock up over InvalidOperationException
            List<DatabaseMenu> lbi = new List<DatabaseMenu>();
            if (xmlListCurrent.SelectedItems.Count != 0)
            {

                foreach (DatabaseMenu item in xmlListCurrent.SelectedItems)
                {
                    lbi.Add(item);
                }

                foreach (DatabaseMenu item in lbi)
                {
                    MmManagerCurrent.Remove(item);
                }
                                
            }
            /// Adding Listbox into a list
            //A bit of LINQ should do it:-
            // var myOtherList = lbMyListBox.Items.Cast<String>().ToList();
            //Of course you can modify the Type parameter of the Cast to whatever type you have stored in the Items property.

            //List<Menu> m = xmlListCurrent.Items.Cast<Menu>().ToList();
            //foreach (var item in xmlListCurrent.SelectedItems)
            //{
            //    m.Remove(item as Menu);
            //}
            //xmlListCurrent.ItemsSource = m;
            //xmlListCurrent.Items.Refresh();
        }

        private void checkAutoplay_Unchecked(object sender, RoutedEventArgs e)
        {
            media_wheel.LoadedBehavior = MediaState.Manual;
        }
        private void checkAutoplay_Checked(object sender, RoutedEventArgs e)
        {
            media_wheel.LoadedBehavior = MediaState.Play;
        }

        private void openFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(FullPath))
                    Process.Start(FullPath);                
            }
            catch (Exception)
            {                             
            }

        }

        private void cancelPDFWorker_Click(object sender, RoutedEventArgs e)
        {
            cancelPDFWorker.IsEnabled = false;
            PDFWorker pdfworker = new PDFWorker();
            if ( pdfworker.PDFBackgroundWorker.IsBusy)
            {                
                pdfworker.PDFBackgroundWorker.CancelAsync();
                pdfworker.PDFBackgroundWorker.ReportProgress(80);
            }
            else if (IntroVideos.bw.IsBusy)
            {
                IntroVideos.bw.CancelAsync();
            }            
        }

        #region BEZEL EDITING
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            currentBezelLoaded = ViewerFile;
            Uri uri = new Uri(currentBezelLoaded);
            bezelImage.Source = ImageEdits.BitmapFromUri(uri);
            string folder = System.IO.Path.GetDirectoryName(ViewerFile);
            string file = System.IO.Path.GetFileNameWithoutExtension( ViewerFile) + ".ini";
            GetValueFromINI(System.IO.Path.Combine(folder,file));
            _tabCreate.IsSelected = true;
            _tabBezel.IsSelected = true;

        }
        private void GetValueFromINI(string Filename)
        {
            try
            {
                IniFileReader ini = new IniFileReader(Filename);
                LeftClickX.Text = ini.IniReadValue("General", "Bezel Screen Top Left X Coordinate");
                LeftClickY.Text = ini.IniReadValue("General", "Bezel Screen Top Left Y Coordinate");
                RightClickX.Text = ini.IniReadValue("General", "Bezel Screen Bottom Right X Coordinate");
                RightClickY.Text = ini.IniReadValue("General", "Bezel Screen Bottom Right Y Coordinate");
            }
            catch (Exception)
            {
                
            }

        }
        private void bezelImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(this);
            double x = e.GetPosition(bezelImage).X;
            double y = e.GetPosition(bezelImage).Y;
            System.Windows.Point pointed = MouseUtilities.CorrectGetPosition(bezelImage);
            System.Drawing.Image img = System.Drawing.Image.FromFile(currentBezelLoaded);
            double pXL, pYL, pXR, pYR;
            int pX = 0, pY = 0;

            System.Windows.Point p = new System.Windows.Point(pX, pY);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                pXL = Convert.ToInt32(x * img.Width / bezelImage.ActualWidth);
                pYL = Convert.ToInt32(y * img.Height / bezelImage.ActualHeight);
                LeftClickX.Text = pXL.ToString();
                LeftClickY.Text = pYL.ToString();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                pXR = Convert.ToInt32(x * img.Width / bezelImage.ActualWidth);
                pYR = Convert.ToInt32(y * img.Height / bezelImage.ActualHeight);
                RightClickX.Text = pXR.ToString();
                RightClickY.Text = pYR.ToString();
            }

            if (!string.IsNullOrWhiteSpace(RightClickX.Text) && (!string.IsNullOrWhiteSpace(LeftClickX.Text)))
            {
                int pxl = Convert.ToInt32(LeftClickX.Text);
                int pyl = Convert.ToInt32(LeftClickY.Text);
                int pxr = Convert.ToInt32(RightClickX.Text);
                int pyr = Convert.ToInt32(RightClickY.Text);

                int resizeWidth = 0;
                for (int i = pxl; i < pxr; i++)
                {
                    resizeWidth++;
                }

                int resizeHeight = 0;
                for (int i = pyl; i < pyr; i++)
                {
                    resizeHeight++;
                }
            }
        }

        /// <summary>
        /// Save the bezel from points in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBezel_Click(object sender, RoutedEventArgs e)
        {
            BezelEdit be = new BezelEdit();
            string[] points = new string[4];
            points[0] = LeftClickX.Text;
            points[1] = LeftClickY.Text;
            points[2] = RightClickX.Text;
            points[3] = RightClickY.Text;
            string ratio = RatioComboBox.Text;
            be.SelectedFilenamePath = ViewerFile;
            be.saveIni(points, ratio, descText.Text, authorText.Text);
        }
        #endregion

        /// <summary>
        /// Bezel loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //currentBezelLoaded = @"I:\RocketLauncher\Media\Bezels\MAME\_Default\Horizontal\Bezel.png";

        }

        #region Toggle Switches
        private void toggleMainMenu_Checked(object sender, RoutedEventArgs e)
        {
            listboxSystem.IsEnabled = false;
        }
        private void toggleMainMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            listboxSystem.IsEnabled = true;
        }
        #endregion

        /// <summary>
        /// Double click on the system check xml to open corresponding file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xmlListCurrents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DatabaseMenu m = xmlListCurrents.SelectedItem as DatabaseMenu;
            ColumnHeader = xmlListCurrents.CurrentColumn.Header.ToString();
            string dbPath = hsdb.HSPath + "\\Databases\\" + m.name + "\\" + m.name + ".xml";
            string genrePath = hsdb.HSPath + "\\Databases\\" + m.name + "\\" + "Genre.xml";

            if (ColumnHeader == "XML")
            {
                if (File.Exists(dbPath))
                    Process.Start(dbPath);
            }
            else if (ColumnHeader == "GenreXML")
            {
                if (File.Exists(genrePath))
                    Process.Start(genrePath);
            }
            
        }

        #region IntroVideos
        private void listbox1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var neh = listbox1.SelectedItem as IntroVideos;
            string temp;
            if (neh != null)
                temp = neh.Format;
        }
        /// <summary>
        /// Scan videos for intros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// Intro Videos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            List<IntroVideos> iv = new List<IntroVideos>();
            foreach (IntroVideos item in listbox2.Items)
            {
                iv.Add(item);
            }
            foreach (var item in iv)
            {
                listbox2.Items.Remove(item);
                introVideos.Add(item);
                listbox1.Items.Refresh();
            }

        }/// 
        private void scanVideos_Click(object sender, RoutedEventArgs e)
        {
            string pathToVideos = hsdb.HSPath + "\\Media\\" + hsdb.SystemName + "\\Video\\";
            DirectoryInfo dir = new DirectoryInfo(pathToVideos);
            if (!dir.Exists)
                return;

            introVideos = new List<IntroVideos>();
            FileInfo[] fi = dir.GetFiles("*.mp4");
            foreach (var item in fi)
            {
                introVideos.Add(new IntroVideos(item.FullName));

            }
            //if (introVideos !=null)
            //   System.Windows.MessageBox.Show(introVideos.ElementAt(10).Metadata.VideoData.Format);
            // List<string> introVideoFilesLists = new List<string>(introVideos.PopulateVideos(hsdb.HSPath + "\\Media\\" + hsdb.SystemName + "\\Video\\", "*.mp4"));
            listbox1.ItemsSource = introVideos;

           // if (listbox1.Items.Count >0)
                //var neh = listbox1.Items[0] as IntroVideos;

            //string[] videoFiles = new string[]
        }
        /// <summary>
        /// Intro Videos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (listbox1.SelectedItems.Count != 0)
            {
                IntroVideos iv = listbox1.SelectedItem as IntroVideos;
                iv.GetProperties(iv.FileName);

                listbox2.Items.Add(listbox1.SelectedItem);
                introVideos.Remove(listbox1.SelectedItem as IntroVideos);
                listbox1.Items.Refresh();
            }
        }
        /// <summary>
        /// Intro VIdeos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (listbox2.SelectedItems.Count != 0)
            {
                introVideos.Add(listbox2.SelectedItem as IntroVideos);
                listbox2.Items.Remove(listbox2.SelectedItem);
                listbox1.Items.Refresh();
            }

        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (listbox2.Items.Count == 0)
            {
                System.Windows.MessageBox.Show("Scan some videos first then add to list");
                return;
            }

            try
            {
                IntroVideos iv = new IntroVideos(startFrame.Text,
                framesText.Text, dissolveTime.Text,
                fadeIn.Text, fadeOut.Text);

                List<IntroVideos> lv = new List<IntroVideos>();
                string WheelPath = hsdb.HSPath + "\\Media\\" + hsdb.SystemName + "\\Images\\Wheel\\";

                foreach (IntroVideos item in listbox2.Items)
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(item.FileName);

                    if (incWheels.IsChecked.Value == true)
                    {
                        if (File.Exists(WheelPath + name + ".png"))
                            lv.Add(item);
                    }
                    else
                        lv.Add(item);

                }


                iv.createAvisynthScript(lv, @"Exports\Videos", incWheels.IsChecked.Value, resizeWheels.IsChecked.Value, WheelPath, (double)wheelOverVidX.Value, (double)wheelOverVidY.Value, (double)resizeW.Value, (double)resizeH.Value
                    );
            }
            catch (Exception)
            {

                System.Windows.MessageBox.Show("Script creation failed");
            }



        }
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            listbox2.Items.Clear();
            int listItems = listbox1.Items.Count;

            int VideoAmount = int.Parse(videoAmount.Text);
            int[] selectedNumber = new int[listItems];

            if (VideoAmount >= listItems)
            {
                System.Windows.MessageBox.Show("You don't have enough videos!");
            }
            else
            {
                Random r = new Random();

                for (int i = 0; i < VideoAmount; i++)
                {
                    listItems = listbox1.Items.Count;
                    int num = r.Next(0, listItems);
                    listbox2.Items.Add(listbox1.Items[num]);

                    introVideos.RemoveAt(num);


                }
            }
        }
        /// <summary>
        /// Cancel Intro Videos????
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            IntroVideos.bw.CancelAsync();
        }
        #endregion

        #region Main View Context Menu
        private void contextAddMultiSys_Click(object sender, RoutedEventArgs e)
        {
            if (hsDatagrid.SelectedItems.Count != 0)
            {

                foreach (DatabaseGame item in hsDatagrid.SelectedItems)
                {

                    item.System = hsdb.SystemName;
                    //item.ShouldSerializeSystemName = false;
                    if (item != null)
                        m_System.Add(item as DatabaseGame);

                }

                multiSystemDataGrid.Items.Refresh();
            }
        }
        private void contextAddMultiSysFav_Click(object sender, RoutedEventArgs e)
        {
            // Add favorites into the MultiSystemList
            foreach (DatabaseGame item in DatabaseGame)
            {
                if (item.IsFavorite)
                {
                    if (!m_System.Contains(item))
                    {
                        item.System = hsdb.SystemName;
                        m_System.Add(item);
                    }
                }

            }

            multiSystemDataGrid.Items.Refresh();
        }
        #endregion

        #region MultiSYstem
        /// <summary>
        /// Set multisystem hyperspin settings through dialog box
        /// Opens current hyperspin settings folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(hsdb.HSPath))
            {
                Microsoft.Win32.OpenFileDialog od = new Microsoft.Win32.OpenFileDialog();
                od.InitialDirectory = hsdb.HSPath + "\\Settings\\";
                Nullable<bool> result = od.ShowDialog();

                if (result == true)
                    _textSettingsFile.Text = od.FileName;
            }
        }
        /// <summary>
        /// Remove game from multisystem list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            try
            {
                m_System.Remove(multiSystemDataGrid.SelectedItem as DatabaseGame);
                multiSystemDataGrid.Items.Refresh();
            }
            catch (Exception)
            {
            }

        }
        #endregion

        #region GameLauncher
        private void _launchGame_Click(object sender, RoutedEventArgs e)
        {
            txtSystem.Text = hsdb.SystemName;

            if (SelectedGame != null)
                txtGame.Text = SelectedGame.RomName;
            else
                txtGame.Text = "";

            ToggleFlyout(2);
        }
        private void _buttonRandom_Click(object sender, RoutedEventArgs e)
        {
            Randomizer.HSdbPath = hsdb.HSPath + "\\Databases\\";
            string systemXML = Randomizer.GetSystem();
            txtSystem.Text = System.IO.Path.GetFileNameWithoutExtension(systemXML);

            txtGame.Text = Randomizer.GetGame(systemXML);

            Uri uri = new Uri(hsdb.HSPath + @"\Media\" + txtSystem.Text + "\\Images\\Wheel\\" + txtGame.Text + ".png");
            random_Wheel.Source = ImageEdits.BitmapFromUri(uri);

        }
        /// <summary>
        /// Launch game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonPlayGame_Click(object sender, RoutedEventArgs e)
        {
            RL.RLPath = hsdb.RLPath;
            RL.RocketLaunchGameSTD(txtSystem.Text, txtGame.Text);
            ToggleFlyout(2);
        }
        #endregion

        #region ImageEdits
        /// <summary>
        /// Send to image Edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            string path = WheelSource.ToString();
            try
            {
                picbox.Source = ImageEdits.BitmapFromUri(new Uri(path));
                _tabCreate.IsSelected = true;
                double w = Math.Round(picbox.Source.Width);
                double h = Math.Round(picbox.Source.Height);
                _tabImageEdit.IsSelected = true;
                TextImageSize.Content = "Original Image size: " + w + " x " + h;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Couldn't send {0} to image edit.", path);
            }

        }
        private void saveImageEdit_Click(object sender, RoutedEventArgs e)
        {
            if (picbox.Source != null)
            {
                ImageCreate ic = ImageCreateParams();
                ic.SystemName = hsdb.SystemName;
                ic.SetImageParameter(false);
                ic.createImage(false);
                System.Windows.MessageBox.Show(ic.OutputFileName);
            }
            //setImageParameter(false);
        }
        /// <summary>
        /// Preview Image Edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (picbox.Source != null)
            {
                ImageCreate ic = ImageCreateParams();
                ic.SystemName = hsdb.SystemName;
                ic.SetImageParameter(true);
                picbox.Source = ImageEdits.BitmapFromUri(new Uri(ic.createImage(true)));
            }
        }
        #endregion
        #region ImageEdits
        /// <summary>
        /// Create an ImageCreate object and set params from UI
        /// </summary>
        /// <returns></returns>
        private ImageCreate ImageCreateParams()
        {
            ImageCreate ic = new ImageCreate();
            if (Fade1.IsChecked == true)
                ic.FadeLayerName = Fade1.Content.ToString();
            else if (Fade2.IsChecked == true)
                ic.FadeLayerName = Fade2.Content.ToString();
            else if (Fade3.IsChecked == true)
                ic.FadeLayerName = Fade3.Content.ToString();
            else if (Fade4.IsChecked == true)
                ic.FadeLayerName = Fade4.Content.ToString();
            else if (BezelBack.IsChecked == true)
                ic.FadeLayerName = BezelBack.Content.ToString();
            else
                ic.FadeLayerName = PauseBack.Content.ToString();

            ic.Author = _text_ImageAuthor.Text;
            ic.CellFolder = SelectedColumn;
            ic.FlipL = FlipLCheck.IsChecked.Value;
            ic.FlipR = FlipRCheck.IsChecked.Value;

            ic.InputImage = ViewerFile;
            ic.FolderName = FullPath;
            ic.InWidth = Convert.ToInt32(Math.Round(picbox.Source.Width));
            ic.InHeight = Convert.ToInt32(Math.Round(picbox.Source.Height));
            ic.JPEG = JPGCheckbox.IsChecked.Value;
            ic.OutWidth = Convert.ToInt32(_numOutWidth.Value);
            ic.OutHeight = Convert.ToInt32(_numOutHeight.Value);
            ic.Prefix = prefixTextbox.Text;
            ic.Preview = true;
            ic.Ratio = RatioComboBox1.Text;
            ic.Resize = ResizeCheckbox.IsChecked.Value;
            ic.Stretch = StretchCheckBox.IsChecked.Value;
            ic.Tile = TileCheckBox.IsChecked.Value;
            ic.TileHeight = Convert.ToInt32(TileH.Value);
            ic.TileWidth = Convert.ToInt32(TileW.Value);
            ic.RLMediaPath = hsdb.RLMediaPath;
            ic.SystemName = SelectedGame.SystemName;
            ic.Rom = SelectedGame.RomName;
            return ic;
        }
        /// <summary>
        /// Create an ImageCreate object with values from preset xml
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        private ImageCreate ImageCreateParamsFromXML(string xmlPath, string mediaType)
        {

            ImageCreate ic = new ImageCreate();
            ic.FadeLayerName = mediaType;

            string path = ((BitmapImage)image_wheel.Source).UriSource.AbsolutePath;
            path = path.Replace("%20", " ");
            ic.InputImage = path;
            ic.InWidth = Convert.ToInt32(Math.Round(image_wheel.Source.Width));
            ic.InHeight = Convert.ToInt32(Math.Round(image_wheel.Source.Height));

            ic.CellFolder = SelectedColumn;
            ic.FolderName = FullPath;
            ic.RLMediaPath = hsdb.RLMediaPath;
            ic.SystemName = hsdb.SystemName;
            ic.Rom = SelectedGame.RomName;

            using (XmlReader read = XmlReader.Create(xmlPath))
            {
                while (read.Read())
                {
                    if (read.IsStartElement())
                    {
                        switch (read.Name)
                        {
                            case "author":
                                ic.Author = read.ReadString();
                                break;
                            case "prefix":
                                ic.Prefix = read.ReadString();
                                break;
                            case "outWidth":
                                ic.OutWidth = Convert.ToInt32(read.ReadString());
                                break;
                            case "outHeight":
                                ic.OutHeight = Convert.ToInt32(read.ReadString());
                                break;
                            case "resize":
                                ic.Resize = Convert.ToBoolean(read.ReadString());
                                break;
                            case "stretch":
                                ic.Stretch = Convert.ToBoolean(read.ReadString());
                                break;
                            case "ratio":
                                ic.Ratio = read.ReadString();
                                break;
                            case "tile":
                                ic.Tile = Convert.ToBoolean(read.ReadString());
                                break;
                            case "tileWidth":
                                ic.TileWidth = Convert.ToInt32(read.ReadString());
                                break;
                            case "tileHeight":
                                ic.TileHeight = Convert.ToInt32(read.ReadString());
                                break;
                            case "flipL":
                                ic.FlipL = Convert.ToBoolean(read.ReadString());
                                break;
                            case "flipR":
                                ic.FlipR = Convert.ToBoolean(read.ReadString());
                                break;
                            case "jpg":
                                ic.JPEG = Convert.ToBoolean(read.ReadString());
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {

                    }

                }
            }


            return ic;
        }
        /// <summary>
        /// Convert image from preset in context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _imageCreateInMenu(object sender, RoutedEventArgs e)
        {
            if (_comboPresetFade.Text != string.Empty)
            {
                string xmlPath = @"presets\images\" + _comboPresetFade.SelectedItem.ToString();
                var g = sender as System.Windows.Controls.MenuItem;
                ImageCreate ic = ImageCreateParamsFromXML(xmlPath, g.Header.ToString());
                ic.romName = SelectedGame.RomName;
                ic.SetImageParameter(false);
                ic.createImage(false);
            }
        }
        #endregion
       
        /// <summary>
        /// Open Exports Folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Process.Start(@"Exports");
        }
        /// <summary>
        /// Get video attributes, frame size etc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (listbox2.Items.Count !=0)
            {
                foreach (IntroVideos item in listbox2.Items)
                {
                    item.GetProperties(item.FileName);
                }
                listbox2.Items.Refresh();
            }
        }

        private void clear_m_system_Click(object sender, RoutedEventArgs e)
        {
            m_System.Clear();
            multiSystemDataGrid.Items.Refresh();
        }

        #region ScanStats
        /// <summary>
        /// Scan statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scanStats_Click(object sender, RoutedEventArgs e)
        {
            FileInfo[] statFiles = Statistics.getStatFiles(System.IO.Path.GetDirectoryName(hsdb.RLPath));
            List<Statistics> statArray = new List<Statistics>();

            stats.Clear();
            Statistics s = new Statistics();
            int i = 0;
            foreach (FileInfo Filename in statFiles)
            {
                statArray = s.get_Stats(Filename.FullName);

                foreach (var item in statArray)
                {
                    stats.Add(item);
                }

            }

            TotalOverAllTime = Statistics.TotalOverallTime;
        }
        /// <summary>
        /// Get statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonGetStat_Click(object sender, RoutedEventArgs e)
        {
            string rlStatsPath = System.IO.Path.GetDirectoryName(hsdb.RLPath);
            Statistics.StatsPath = System.IO.Path.Combine(rlStatsPath, @"Data\Statistics\");
            var list = Statistics.getSingleGameStats("Amstrad CPC", "Roboco urope)");

            if (list.Count == 0)
                System.Windows.MessageBox.Show("game not found");

            foreach (var item in list)
            {
                System.Windows.MessageBox.Show(item.ToString());
            }
        }
        #endregion

        private void _videoPreviewOpen_Click(object sender, RoutedEventArgs e)
        {
            string p = media_wheel.Source.LocalPath;
            media_wheel.Stop();
            Process.Start(p);
        }

        /// <summary>
        /// Save Favorites to XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveGXml_Click(object sender, RoutedEventArgs e)
        {
            if (hsdb.SystemName == "Main Menu")
                System.Windows.MessageBox.Show("Use the menu manager for main menu xml");
            else
            {
                Favorites.hsPath = hsdb.HSPath;
                Favorites.ConvertGameFavoritesToXml(DatabaseGame.ToList(), hsdb.SystemName, addFaveToGenre.IsChecked.Value);
            }
        }

        private void _copyMenuItems_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            foreach (var item in hsDatagrid.Columns)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    //_copyMenuItems.men
                }

            }
        }

        public void SendToTrash()
        {

            if ( filesBox.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("No files selected");
                return;
            }

            WheelSource = null;
            media_wheel.Source = null;

            List<FileCollection> fc1 = new List<FileCollection>();

            if (hsdb.IsHyperspin)
            {
                    foreach (FileCollection item in filesBox.SelectedItems)
                    {
                        FileInfo fi2 = new FileInfo(item.FullPath);

                        if (FileManagement.SendToTrash("Hyperspin", fi2,hsdb.SystemName,SelectedColumn,SelectedGame.RomName))
                        {
                            fc1.Add(item);
                            InfoMessage = ViewerFile + ": Sent to rubbish";

                        }
                        else
                            InfoMessage = "File in use, cannot move.";
                    }

                    foreach (var item2 in fc1)
                    {
                        fc.Remove(item2);
                    }
            

                    filesBox.Items.Refresh();
            }
            else
            {
                    foreach (FileCollection item in filesBox.SelectedItems)
                    {
                        FileInfo fi2 = new FileInfo(item.FullPath);

                        if (FileManagement.SendToTrash("RocketLauncher", fi2, hsdb.SystemName, SelectedColumn, SelectedGame.RomName))
                        {
                            fc.Remove(item);
                            InfoMessage = ViewerFile + ": Sent to rubbish";

                        }
                        else
                            InfoMessage = "File in use, cannot move.";
                    }

                    filesBox.Items.Refresh();
                }

              SelectedGame.UpdateGameBools(SelectedColumn, FullPath, SelectedGame);
        }

        /// <summary>
        /// Search testing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DatabaseSearch ds = new DatabaseSearch();
            ds.HSPath = hsdb.HSPath;
            ds.HyperspinDatabaseXml = ds.HSPath + "\\Databases\\Main Menu\\Main Menu.xml";
            List<DatabaseGame> searches = ds.SearchForGame("Dizzy");

            foreach (var item in searches)
            {
                System.Windows.MessageBox.Show(item.RomName);
            }
        }

        private void clonesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (hsDatagrid.ItemsSource != null)
            {
                ICollectionView cv = CollectionViewSource.GetDefaultView(hsDatagrid.ItemsSource);
                    cv.Filter = null;
            }
        }

        private void clonesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (hsDatagrid.ItemsSource != null)
            {
                System.Windows.Controls.CheckBox c = (System.Windows.Controls.CheckBox)sender;
                bool filter = c.IsChecked.Value;
                ICollectionView cv = CollectionViewSource.GetDefaultView(hsDatagrid.ItemsSource);
                if (filter)
                    cv.Filter = null;
                else
                {
                    cv.Filter = o =>
                    {
                        DatabaseGame h = o as DatabaseGame;
                        bool flag = false;
                        if (h.CloneOf != string.Empty)
                        {
                            flag = false;
                            return (flag);
                        }
                        else
                        {
                            flag = true;
                            return (flag);
                        }
                    };
                }
            }        
        }

        /// <summary>
        /// Export audits to text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonExportAudit_Click(object sender, RoutedEventArgs e)
        {
            //List<string> Mediafolders = new List<string> { "Artwork1", "Artwork2", "Artwork3", "Artwork4" };
            ExportMissHave.ExportMissingTextsHyperSpin(DatabaseGame.ToList(), hsdb.SystemName);
        }

        private void _buttonExportHave_Click(object sender, RoutedEventArgs e)
        {
            ExportMissHave.ExportHaveTextsHyperSpin(DatabaseGame.ToList(), hsdb.SystemName);
        }



    }
        

    public static class TreeViewHelper
    {
        public static void SelectItem(this ItemsControl parentContainer, List<object> path)
        {
            var head = path.First();
            var tail = path.GetRange(1, path.Count - 1);
            var itemContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(head) as TreeViewItem;

            if (itemContainer != null && itemContainer.Items.Count == 0)
            {
                itemContainer.IsSelected = true;

                var selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
                selectMethod.Invoke(itemContainer, new object[] { true });
            }
            else if (itemContainer != null)
            {
                itemContainer.IsExpanded = true;

                if (itemContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    itemContainer.ItemContainerGenerator.StatusChanged += delegate
                    {
                        SelectItem(itemContainer, tail);
                    };
                }
                else
                {
                    SelectItem(itemContainer, tail);
                }
            }
        }
    }
}