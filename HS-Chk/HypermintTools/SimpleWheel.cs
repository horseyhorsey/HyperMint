using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Hypermint.HypermintTools
{
    class SimpleWheel
    {
        #region Variables
        public List<DatabaseGame> Game_List = new List<DatabaseGame>();
        public List<SimpleWheel> genre_list = new List<SimpleWheel>();
        public BackgroundWorker wheelWorker = new BackgroundWorker();

        char letter;
        public string font = "Impact";
        public string ImPath { get; set; }
        public string EndPath { get; set; }
        public string Romname, SystemName, Databases, MenuXML;
        public string HsPath { get; set; }
        public string path { get; set; }

        public string Param = "", savedParam = "";
        public string PreviewText;
        public string Label, Caption, SpecialName, Prefix, genreName;

        public int R = 0, G = 0, B = 0;
        public string Color { get; set; }
        public string ColorRGB { get; set; }
        public string FillColor { get; set; }
        public string FillColorRGB { get; set; }

        public string BackgroundColor = "Transparent";
        public string BackgroundColorRGB = "";

        public bool StrokeOn { get; set; }
        public decimal StrokeAmount { get; set; }
        public string StrokeColor { get; set; }
        public string StrokeColorRGB { get; set; }

        public bool ShadowOn { get; set; }
        public string ShadowColor { get; set; }
        public string ShadowColorRGB { get; set; }
        public decimal ShadowAmount { get; set; }
        public decimal ShadowDistance { get; set; }
        public bool ShadowRepageOn, ShadowSwapOn;
        public string ShadowRepage, ShadowSwap;
        public bool ShadeOn;
        public decimal shadeX, ShadeY;
        public string shadeText;

        public string WheelType  { get; set; }

        public bool Trim;
        public string Trimmed;
        public bool GravityOn { get; set; }
        public string Gravity { get; set; }

        public bool PreviewOn, OnlyMiss;
        public bool CaptionOrLabel;
        public string ImageExe { get; set; }

        public decimal distortArc { get; set; }
        public bool distortArcTextOn;
        public decimal distortTextAmount;
        public string distortText;
        public string xmlPath { get; set; }
        public string Genre { get; set; }
        public string specialArtName = "";
        public decimal Width , Height;
        public bool customRun = false;
        public string CurrentSystem { get; set; }

        public string shadow;
        public bool selectedWheels = true, selectedLetters=false, selectedSpecialArt, selectedGenre;
        public int enabled = 0;
        #endregion

        public SimpleWheel()
        {
            PreviewOn = true;
            wheelWorker.WorkerReportsProgress = true;
            wheelWorker.DoWork += wheelWorker_DoWork;
            wheelWorker.RunWorkerCompleted += wheelWorker_RunWorkerCompleted;
            wheelWorker.ProgressChanged += wheelWorker_ProgressChanged;
            wheelWorker.WorkerSupportsCancellation = true;

            //presetRefresh();            
        }


        public void runImageMagick(string exe, bool isPreviewOn)
        {
            ImPath = Properties.Settings.Default.IMPath;
            //// Setup the ImageMagick Process       
            ImageExe = "Convert.exe";
            ProcessStartInfo ImageMagick = new ProcessStartInfo();
            if (customRun)
                ImageMagick.FileName = ImPath + @"\" + ImageExe;
            else
                ImageMagick.FileName = ImPath + @"\" + exe;

            if (!System.IO.File.Exists(ImageMagick.FileName))
            {
               System.Windows.MessageBox.Show( "Error: Imagemagick doesn't exist: " + ImageMagick.FileName);
                return;
            }

            ImageMagick.Arguments = Param;
            ImageMagick.UseShellExecute = false;
            ImageMagick.CreateNoWindow = true;
            ImageMagick.RedirectStandardOutput = true;
            ImageMagick.RedirectStandardError = true;
            using (Process proc = System.Diagnostics.Process.Start(ImageMagick))
            {
                ////    // This needs to be before WaitForExit() to prevent deadlocks, for details: 
                ////    // http://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput%28v=VS.80%29.aspx    
                proc.StandardError.ReadToEnd();
                ////    // Wait for exit
                if (PreviewOn)
                    proc.WaitForExit();
            }

        }
        private void wheelWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
            // label3.Text = e.ProgressPercentage.ToString();
            if (selectedWheels)
            {
                //processINFO.Content = Romname;
                // Preview Box is checked Slow down ImageMagick with the PreviewOn
                if (PreviewOn)
                {
                    path = @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Wheel\" + Romname + ".png";
                }
            }
            else if (selectedSpecialArt)
            {
                if (PreviewOn)
                {
                    path = @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Special\" + specialArtName + ".png";
                }

            }
            else if (selectedLetters)
            {
                if (PreviewOn)
                {
                    path = @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Letters\" + letter + ".png";
                }

            }
            else if (selectedGenre)
            {
                if (PreviewOn)
                {
                    path = @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Genre\Wheel\" + SpecialName + ".png";
                }

            }

            //((MainWindow)System.Windows.Application.Current.MainWindow).image_wheel.Source = null;
             //TextBlock1.Text = "Setting Text from My Program";
            if (File.Exists(path))
                ((MainWindow)System.Windows.Application.Current.MainWindow).WheelSource =
                    ImageEdits.BitmapFromUri(new Uri(System.IO.Path.GetFullPath(path)));
        }
        private void wheelWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
             ((MainWindow)System.Windows.Application.Current.MainWindow).runWheelButton.IsEnabled = true;

            if (e.Cancelled)
            {
                System.Windows.MessageBox.Show("Cancelled");
                //progressBar1.Value = 0;
            }
            else
            {
                Thread.Sleep(200);
                System.Windows.MessageBox.Show("Done");
            }
        }
        private void wheelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Load a new XmlDocument 
            int ii;
            int i = 0;
            ImageExe = "Convert.exe";

            try
            {



            if (!customRun)
            {
                if (selectedWheels)
                {
                    // Create A Directory for the game Wheels
                    Directory.CreateDirectory(@"Exports\HyperSpin\Media\" + SystemName + @"\Images\Wheel");
                    // Loop each Node in the XML. selectnodes Menu/game

                    foreach (var item in Game_List)
                    {
                        i = i + 1;
                        Romname = item.RomName ;
                        Caption = item.Description;
                        string path = HsPath + "\\Media\\" + SystemName + @"\Images\Wheel\" + Romname + ".png";

                        if (wheelWorker.CancellationPending)      //checks for cancel request
                        {
                            e.Cancel = true;
                            break;
                        }

                        if (OnlyMiss == true & item.HaveWheel==false)
                        {
                            // Use filters on the description ... (E),(F)
                            Caption = getFilteredDescriptions(Caption);
                            // Wrap label with "" quotes
                            Label = "\"" + Caption + "\"";
                            // Get the EndPath to save the wheel file to
                            EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Wheel\" + Romname + ".png" + "\"";
                            // Grab the saved parameter for when the button was pushed
                            Param = savedParam;
                            // Convert the parameter string and replace the caption with rom description / Label.
                            StringBuilder newParmName = new StringBuilder(Param);
                            if (CaptionOrLabel) { newParmName.Replace("Caption:", "Caption:" + Label); } else { newParmName.Replace("Caption:", "Label:" + Label); }
                            Param = newParmName.ToString();
                            // Join the new IM parameter and runImageMagick
                            Param = Param + EndPath;
                            runImageMagick(ImageExe, true);
                        }
                        if (OnlyMiss == false)
                        {
                            // Use filters on the description ... (E),(F)
                            Caption = getFilteredDescriptions(Caption);
                            // Wrap label with "" quotes
                            Label = "\"" + Caption + "\"";
                            // Get the EndPath to save the wheel file to
                            EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Wheel\" + Romname + ".png" + "\"";
                            // Grab the saved parameter for when the button was pushed
                            Param = savedParam;
                            // Convert the parameter string and replace the caption with rom description / Label.
                            StringBuilder newParmName = new StringBuilder(Param);
                            newParmName.Replace("Caption:", "Caption:" + Label);
                            Param = newParmName.ToString();
                            // Join the new IM parameter and runImageMagick
                            Param = Param + EndPath;
                            runImageMagick("Convert.exe", true);
                        }

                        // Get the BackGround Worker to report its progress & update the UI
                        wheelWorker.ReportProgress(i);
                        if (PreviewOn == true & !OnlyMiss)
                            Thread.Sleep(100);
                    }
                }

                #region selectedSpecialArt


                if (selectedSpecialArt)
                {
                    // Create A Directory for the Special Art
                    Directory.CreateDirectory(@"Exports\HyperSpin\Media\" + SystemName + @"\Images\Special");
                    ii = genre_list.Count;
                    foreach (var li in genre_list)
                    {
                        i = i + 1;

                        if (wheelWorker.CancellationPending)      //checks for cancel request
                        {
                            e.Cancel = true;
                            break;
                        }

                        Label = "";
                        specialArtName = li.Genre;
                        // If the Prefix label is greater than one set it to the Label
                        if (Prefix.Length > 0)
                            Label = "\"" + Prefix + li.Genre + "\"";
                        else
                            Label = "\"" + li.Genre + "\"";

                        StringBuilder newGenre = new StringBuilder(Label);
                        Label = newGenre.ToString();

                        EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Special\" + li.Genre.ToString() + ".png" + "\"";
                        Param = savedParam;
                        StringBuilder newParmName = new StringBuilder(Param);
                        newParmName.Replace("Caption:", "Caption:" + Label);
                        Param = newParmName.ToString();
                        // Join the new IM parameter and runImageMagick
                        Param = Param + EndPath;
                        runImageMagick("Convert.exe", true);

                        // Get the BackGround Worker to report its progress & update the UI
                        wheelWorker.ReportProgress(i);
                        if (PreviewOn)
                            Thread.Sleep(200);
                    }
                }
                #endregion

                #region Letters
                if (selectedLetters)
                {

                    i = 0;
                    ii = 36;
                    Directory.CreateDirectory(@"Exports\HyperSpin\Media\" + SystemName + @"\Images\Letters");

                    for (char c = 'A'; c <= 'Z'; c++)
                    {
                        i = i + 1;

                        if (wheelWorker.CancellationPending)      //checks for cancel request
                        {
                            e.Cancel = true;
                            break;
                        }

                        Label = "\"" + c + "\"";

                        EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Letters\" + c + ".png" + "\"";
                        Param = savedParam;
                        StringBuilder newParmName = new StringBuilder(Param);
                        newParmName.Replace("Caption:", "Caption:" + Label);
                        Param = newParmName.ToString();
                        Param = Param + EndPath;
                        letter = c;
                        runImageMagick("Convert.exe", true);
                        // Get the BackGround Worker to report its progress & update the UI
                        wheelWorker.ReportProgress(i);
                        if (PreviewOn == true)
                            Thread.Sleep(200);

                    }

                    for (char c = '0'; c <= '9'; c++)
                    {
                        i = i + 1;
                        if (wheelWorker.CancellationPending)      //checks for cancel request
                        {
                            e.Cancel = true;
                            break;
                        }
                        Label = "\"" + c + "\"";

                        EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Letters\" + c + ".png" + "\"";
                        Param = savedParam;
                        StringBuilder newParmName = new StringBuilder(Param);
                        newParmName.Replace("Caption:", "Caption:" + Label);
                        Param = newParmName.ToString();
                        Param = Param + EndPath;
                        letter = c;
                        runImageMagick("Convert.exe", true);
                        // Get the BackGround Worker to report its progress & update the UI
                        wheelWorker.ReportProgress(i);
                        if (PreviewOn == true)
                            Thread.Sleep(500);
                    }
                }


            }
                #endregion

                 #region Genres
            if (selectedGenre)
            {
                Directory.CreateDirectory(@"Exports\HyperSpin\Media\" + SystemName + @"\Images\Genre\Wheel");
                ii = genre_list.Count;
                foreach (var li in genre_list)
                {
                    var g = li as SimpleWheel;
                    SpecialName = g.Genre;
                    i = i + 1;

                    if (wheelWorker.CancellationPending)      //checks for cancel request
                    {
                        e.Cancel = true;
                        break;
                    }

                    Label = "";
                    // If the Prefix label is greater than one set it to the Label
                    if (Prefix.Length > 0)
                        Label = "\"" + SpecialName + "\"";
                    else
                        Label = "\"" + SpecialName + "\"";

                    StringBuilder newGenre = new StringBuilder(Label);
                    //  newName.Clear();
                    //  newName.Append(genreName);                              
                    Label = newGenre.ToString();

                    EndPath = "\"" + @"Exports\HyperSpin\Media\" + SystemName + @"\Images\Genre\Wheel\" + SpecialName + ".png" + "\"";
                    Param = savedParam;
                    StringBuilder newParmName = new StringBuilder(Param);
                    newParmName.Replace("Caption:", "Caption:" + Label);
                    Param = newParmName.ToString();
                    // Join the new IM parameter and runImageMagick
                    Param = Param + EndPath;
                    runImageMagick("Convert.exe", true);

                    // Get the BackGround Worker to report its progress & update the UI
                    wheelWorker.ReportProgress(i);
                    if (PreviewOn == true)
                        Thread.Sleep(100);
                }
            }
            #endregion

            }
            catch (Exception)
            {

                throw;
            }
            //if (selectedLetters)
            //{
            //    string currentLetter;

            //    i = 0;
            //    ii = 36;
            //    Directory.CreateDirectory(SystemName + @"\Images\Letters");

            //    for (char c = 'A'; c <= 'Z'; c++)
            //    {
            //        i = i + 1;

            //        if (backgroundWorker1.CancellationPending)      //checks for cancel request
            //        {
            //            e.Cancel = true;
            //            break;
            //        }

            //        Label = "\"" + c + "\"";

            //        EndPath = "\"" + SystemName + @"\Images\Letters\" + c + ".png" + "\"";
            //        Param = savedParam;
            //        StringBuilder newParmName = new StringBuilder(Param);
            //        newParmName.Replace("Caption:", "Caption:" + Label);
            //        Param = newParmName.ToString();
            //        Param = Param + EndPath;
            //        letter = c;
            //        runImageMagick("Convert.exe", true);
            //        // Get the BackGround Worker to report its progress & update the UI
            //        backgroundWorker1.ReportProgress(i);
            //        if (PreviewOn == true)
            //            Thread.Sleep(500);

            //    }

            //    for (char c = '0'; c <= '9'; c++)
            //    {
            //        i = i + 1;
            //        if (backgroundWorker1.CancellationPending)      //checks for cancel request
            //        {
            //            e.Cancel = true;
            //            break;
            //        }
            //        label = "\"" + c + "\"";

            //        EndPath = "\"" + SystemName + @"\Images\Letters\" + c + ".png" + "\"";
            //        Param = savedParam;
            //        StringBuilder newParmName = new StringBuilder(Param);
            //        newParmName.Replace("Caption:", "Caption:" + Label);
            //        Param = newParmName.ToString();
            //        Param = Param + EndPath;
            //        letter = c;
            //        runImageMagick("Convert.exe", true);
            //        // Get the BackGround Worker to report its progress & update the UI
            //        backgroundWorker1.ReportProgress(i);
            //        if (PreviewOn == true)
            //            Thread.Sleep(500);
            //    }
            //}


      
        }

        public void popGenre(string genreXML)
        {
            MenuXML = genreXML;
            StringBuilder newName = new StringBuilder();

            // Check to see if Genre.XML exists before populating
            if (File.Exists(MenuXML))
            {
                XmlTextReader reader = new XmlTextReader(MenuXML);

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                    {
                        if (reader.HasAttributes)
                        {
                            genreName = reader.GetAttribute("name");
                            StringBuilder newGenre = new StringBuilder(genreName);
                            //  newName.Clear();
                            //  newName.Append(genreName);                                   
                            //  MessageBox.Show(newName.ToString());
                            //MessageBox.Show(genreName);
                            //StringBuilder caption2 = new StringBuilder(caption);
                            genre_list.Add(new SimpleWheel()
                            {
                                Genre = newGenre.ToString(),
                            });
                        }
                    }
                }
            }
            else
            {
                //Genre XML Doesn't Exist or bad
            }

        }

        /// <summary> getFilteredDescriptions
        /// Run this on the caption to remove all the regions from description
        /// </summary>
        /// <param name="romDescription"></param>
        /// <returns></returns>
        public string getFilteredDescriptions(string romDescription)
        {
            string newGameName = Regex.Replace(romDescription, @"\(.*", "");
            return newGameName;
        }

        #region Presets

        #endregion

        public string getRGB()
        {
            ColorRGB = "rgb(" + R + "," + G + "," + B + ")";
            return ColorRGB;
        }
        public string getRGB(object BackColor)
        {
            ColorRGB = "rgb(" + R + "," + G + "," + B + ")";
            return ColorRGB;
        }
    }
}
