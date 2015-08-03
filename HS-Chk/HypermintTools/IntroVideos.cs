using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediaToolkit;
using MediaToolkit.Options;
using MediaToolkit.Model;
using System.Threading;
using System.ComponentModel;

namespace Hypermint.HypermintTools
{
    public class IntroVideos : MediaToolkit.Model.MediaFile
    {
        public static ManualResetEvent mre = new ManualResetEvent(false);

        public string systemName { get; set; }
        public string FileName { get; set; }
        private string StartFrame = string.Empty;
        private string EndFrame = string.Empty;
        private string FadeInTime = string.Empty;
        private string FadeOutTime = string.Empty;
        private string DissolveTIme = string.Empty;
        public double FrameRate { get; set; }
        public string movedFile { get; set; }
        public  TimeSpan Start;
        public  TimeSpan End;
        public  string VideoToTrim;
        public  string VideoToTrimTo;

        private string format;
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public static BackgroundWorker bw = new BackgroundWorker();
        public static Engine engine;

        public IntroVideos(string sFrame,string eFrame, string dissolve,string fOut,string fIn)
        {
            this.StartFrame = sFrame;
            this.EndFrame = eFrame;
            this.DissolveTIme = dissolve;
            this.FadeOutTime = fOut;
            this.FadeInTime = fIn;
            this.FileName = string.Empty;
            
        }        
        public IntroVideos(string VideoFile)
        {
            this.FileName = VideoFile;

        }
        public IntroVideos (string inp,string outp)
        {
            this.VideoToTrim = inp;
            this.VideoToTrimTo = outp;
        }
        public IntroVideos()
        {
            // TODO: Complete member initialization
        }
        public static void IntroVideoss(string VideoFile)
        {
           // this.FileName = VideoFile;

            var inputFile = new MediaFile { Filename = VideoFile };
            var outputFile = new MediaFile {Filename = @"Z:\ddd.mp4"};
            var inputFiles = new MediaToolkit.Options.ConversionOptions();

             
            var engine = new MediaToolkit.Engine();
            

            TimeSpan t = new TimeSpan(0, 0, 2, 10, 49);
            TimeSpan d = new TimeSpan(0, 0, inputFile.Metadata.Duration.Minutes, inputFile.Metadata.Duration.Seconds, inputFile.Metadata.Duration.Milliseconds);
            d = inputFile.Metadata.Duration;
             d = d - t;

             inputFiles.CutMedia (t, d);
             engine.Convert(inputFile, outputFile, inputFiles);

        }

        public void GetProperties(string VideoFile)
        {
            var inputFile = new MediaFile { Filename = VideoFile };
            string GetFrameSize = string.Empty;

            try
            {
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    this.Format = inputFile.Metadata.VideoData.FrameSize;
                    this.FrameRate = inputFile.Metadata.VideoData.Fps;
                    engine.Dispose();
                }
            }
            catch (Exception)
            {                               
            }
        }

        public void StartConverting(TimeSpan start,TimeSpan length)
        {
            Start = start;
            End = length;
            
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).progressBarViewer.IsActive = false;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cancelPDFWorker.IsEnabled = false;
        }
        static void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
          
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var options = new MediaToolkit.Options.ConversionOptions();
            var inputFiled = new MediaFile { Filename = VideoToTrim };
            var outputFile = new MediaFile { Filename = VideoToTrimTo };

            string video2trim = "\"" + VideoToTrim + "\"";
            string video2trim2 = "\"" + VideoToTrimTo + "\"";
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFiled);
                options.CutMedia(Start, End);
                //fade=in:0:30
                engine.ConvertProgressEvent += ConvertProgressEvent;
                engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;
                engine.Convert(inputFiled, outputFile, options);

                //ffmpeg -i input.mp4 -filter:v 'fade=in:0:50' \ -c:v libx264 -crf 22 -preset veryfast -c:a copy output.mp4                
                //engine.CustomCommand("-ss " + Start + " -t " + End + " -i " + video2trim + " " + "\"" + " fade=in:0:03" + "\"" +  " " + video2trim2);
                //engine.CustomCommand("-ss " + Start + " -t " + End + " -i" + " " + video2trim + " -filter:v" + " fade=in=0:130 " + "-c:v libx264 -crf 22 -preset veryfast -c:a copy " + video2trim2);
                //std trim
               // string startTime = Start.Hours + ":" + Start.m
                //engine.CustomCommand("-ss " + Start. + " -t " + End + " -i " + video2trim + " -acodec copy -vcodec copy " + video2trim2);
            }
        }
        public static void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            //Console.WriteLine("\n------------\nConverting...\n------------");
            //Console.WriteLine("Bitrate: {0}", e.Bitrate);
            //Console.WriteLine("Fps: {0}", e.Fps);
            //Console.WriteLine("Frame: {0}", e.Frame);
            //Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            //Console.WriteLine("SizeKb: {0}", e.SizeKb);
            //Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
            //e.ProcessedDuration
            //bw.ReportProgress(50);


            if (bw.CancellationPending)
                engine.Dispose();
           // System.Windows.MessageBox.Show("");
            //Thread.Sleep(500);
        }
        private static void engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {
            //Console.WriteLine("\n------------\nConversion complete!\n------------");
            //Console.WriteLine("Bitrate: {0}", e.Bitrate);
            //Console.WriteLine("Fps: {0}", e.Fps);
            //Console.WriteLine("Frame: {0}", e.Frame);
            //Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            //Console.WriteLine("SizeKb: {0}", e.SizeKb);
            //Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);

            // This will let the loop continue
            mre.Set();
        }
        
        public FileInfo[] PopulateVideos(string pathToVideos, string extensionFilter)
        {            
            DirectoryInfo dir = new DirectoryInfo(pathToVideos);
            if (!dir.Exists) { return null; }

            FileInfo[] Files;
            Files = dir.GetFiles(extensionFilter);
            
            return Files;
        }

        public void createAvisynthScript(List<IntroVideos> iv_list, string exportScriptPath, bool wheelOverlay, bool wheelResize, string wheelPath,double wheelX = 0, double wheelY = 0, double WheelW = 0, double wheelH = 0
            )
        {

            int fileCount = iv_list.Count();

            int i = 0;
            string[] Vidname = new string[fileCount];
            string[] Audioname = new string[fileCount];
            string[] WheelName = new string[fileCount];
            string[] WheelNameAlpha = new string[fileCount];
            string[] AudioDub = new string[fileCount];
            string[] trimName = new string[fileCount];
            foreach (var file in iv_list)
            {
                if (!wheelOverlay)
                {
                    Vidname[i] = "V" + i + " = ffvideosource(" + "\"" + file.FileName + "\"" + ")";
                    Audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + file.FileName + "\"" + ")";
                    AudioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + StartFrame + "," + EndFrame + ")";
                    i++;
                }
                else
                {
                    string name=  System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    string path = wheelPath + name;
                    //int addWheels = fileCount;
                    trimName = new string[fileCount ];

                    Vidname[i] = "V" + i + " = ffvideosource(" + "\"" + file.FileName + "\"" + ")";
                    WheelName[i] = "W" + i + " = ImageSource(" + "\"" + wheelPath + name + ".png" + "\"" + ")";
                    WheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" + wheelPath + name + ".png" + "\"" + ", pixel_type=" + "\"" + "RGB32" + "\"" + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")";
                    Audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + file.FileName + "\"" + ")";

                    if (wheelResize)
                    {
                        WheelName[i] = "W" + i + " = ImageSource(" + "\"" + wheelPath  + name + ".png" + "\"" + ")" + @".BilinearResize(" + WheelW + @"," + wheelH + ")";
                        WheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" + wheelPath + name + ".png" + "\"" + ", pixel_type=" + "\"" + "RGB32" + "\"" + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")" +
                            @".BilinearResize(" + WheelW + @"," + wheelH + ")";
                    }


                    AudioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + StartFrame + "," + EndFrame + ").Overlay(" + "W" + i + "," + wheelX + "," + wheelY + ", Wa" + i + ",opacity = 0.7)";
                    //.Overlay(img, 0, 440,imgalpha,opacity = 0.7)
                    i++;
                }
            }

            i = 0;
            foreach (var item in iv_list)
            {
                trimName[i] = "Trim" + i + " = " + AudioDub[i];
                i++;
            }
            int lineTotal;
            if (!wheelOverlay)
                lineTotal = Vidname.Length + trimName.Length + Audioname.Length + AudioDub.Length + 1;
            else
                lineTotal = Vidname.Length + trimName.Length + Audioname.Length + AudioDub.Length + 1 + WheelName.Length + WheelNameAlpha.Length;
            string[] lines = new string[lineTotal];
            i = 0;
            int ii = 0;
            foreach (var item in Vidname)
            {
                lines[ii] = Vidname[i];
                ii++;

                if (wheelOverlay)
                {
                    lines[ii] = WheelName[i];
                    ii++;
                    lines[ii] = WheelNameAlpha[i];
                    ii++;
                }
                lines[ii] = Audioname[i];
                ii++;
                i++;
            }

            i = 0;

            StringBuilder sb = new StringBuilder();
            foreach (var item in trimName)
            {
                lines[ii] = trimName[i];
                sb.Append("Trim" + i + ",");
                i++;
                ii++;
            }
            sb.Append(DissolveTIme + ").FadeIn(" + FadeInTime + ").FadeOut(" + FadeOutTime + ")");

            lines[ii] = "Dissolve(" + sb.ToString();

            if (Directory.Exists(exportScriptPath))
                Directory.CreateDirectory(exportScriptPath);

            string scriptFile = systemName + ".avs";
            i = 1;
            while (File.Exists(exportScriptPath + scriptFile))
            {
                scriptFile = systemName + "(" + i + ")" + ".avs";
                i++;
            }

            File.WriteAllLines(exportScriptPath + scriptFile, lines);

            System.Windows.MessageBox.Show("AviSynth video script created");

        }


    }
}
