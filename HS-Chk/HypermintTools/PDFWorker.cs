using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp;
using System.Windows;
using ImageMagick;

namespace Hypermint.HypermintTools
{

    /// <summary>
    /// Uses iTextSharp for read writing text pdf and imagemagick for converting to images
    /// </summary>
    public class PDFWorker
    {
        public bool exportPDF, exportPDFCancelled;
        public string PDFfilename2, PDFfilename;
        public BackgroundWorker PDFBackgroundWorker = new BackgroundWorker();
        MagickReadSettings settings = new MagickReadSettings();
        MagickImageCollection images = new MagickImageCollection();

        private string inputFilename;
        public string InputFilename
        {
            get { return inputFilename; }
            set { inputFilename = value; }
        }
        public string GameTitle { get; set; }
        public string SystemName { get; set; }
        public string SelectedFullPath { get; set; }

        public string directoryName { get; set; }
        public string FileNoExt { get; set; }
        string exportFilePath = string.Empty;

        public int PageCount { get; set; }
        public bool chop1st { get; set; }
        public bool chopLast { get; set; }

        public PDFJobType JobType;
        public enum PDFJobType
        {
            None,
            TextPDF,
            NormalPDF,
            NormalPDFChop

        };

        public PDFWorker()
        {
        }
                
        public PDFWorker(string file,string gameTitle, PDFJobType jobType)
        {
            this.InputFilename = file;
            this.JobType = jobType;
            this.GameTitle = gameTitle;
            this.directoryName = System.IO.Path.GetDirectoryName(this.InputFilename);
            this.FileNoExt = System.IO.Path.GetFileNameWithoutExtension(this.InputFilename); ;
        }

        /// <summary>
        /// Constructor used for when you need to chop booklets
        /// </summary>
        /// <param name="file"></param>
        /// <param name="gameTitle"></param>
        /// <param name="jobType"></param>
        public PDFWorker(string file, string gameTitle, PDFJobType jobType,bool chop1st,bool choplast)
        {
            this.InputFilename = file;
            this.JobType = jobType;
            this.GameTitle = gameTitle;
            this.directoryName = System.IO.Path.GetDirectoryName(this.InputFilename);
            this.FileNoExt = System.IO.Path.GetFileNameWithoutExtension(this.InputFilename);
            this.chop1st = chop1st;
            this.chopLast = choplast;
            // Build background worker object

            PDFBackgroundWorker.WorkerReportsProgress = true;
            PDFBackgroundWorker.WorkerSupportsCancellation = true;
            
        }

        /// <summary>
        /// Return a page count for pdf
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public int PDFPageCount(string pdfPath)
        {
            try
            {
                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(pdfPath);
                int pageCount = pdfReader.NumberOfPages;
                pdfReader.Close();
                return pageCount;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Literally just runs the background worker. Set the jobtype with enum
        /// Probably could remove this function and just add one method that knows what the jobtype is.
        /// Case switch the enum??
        /// </summary>
        private void TextToPDF()
        {
           
        }

        public void runPDFJob()
        {
            switch (JobType)
            {
                case PDFJobType.None:
                    break;
                case PDFJobType.TextPDF:
                   // PDFBackgroundWorker.RunWorkerAsync();
                    break;
                case PDFJobType.NormalPDF:
                        ConvertNormalPDF();
                    break;
                case PDFJobType.NormalPDFChop:
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Old menu selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertPDFChopped(object sender, RoutedEventArgs e)
        {
            //if (e.Source.ToString().Contains("Leave First Page"))
            //{
            //    if (FirstPagePDF.IsChecked)
            //        FirstPagePDF.IsChecked = false;
            //    else
            //        FirstPagePDF.IsChecked = true;
            //}

            //if (e.Source.ToString().Contains("Leave Last Page"))
            //{
            //    if (LastPagePDF.IsChecked)
            //        LastPagePDF.IsChecked = false;
            //    else
            //        LastPagePDF.IsChecked = true;
            //}

            //if (e.Source.ToString().Contains("Normal"))
            //{
            //    ConvertNormalPDF();
            //}

            //if (e.Source.ToString().Contains("Chop"))
            //{
            //    ChopPDFBooklet();
            //}
        }

        private void ConvertNormalPDF()
        {
            PageCount = PDFPageCount(InputFilename);
            PDFBackgroundWorker.RunWorkerAsync();
        }
        private void ChopPDFBooklet()
        {
            PageCount = PDFPageCount(InputFilename);

            if (PageCount >= 1)
            {
                PageCount = (PageCount * 2);
                if (chop1st) { PageCount -= 1; }
                if (chopLast) { PageCount -= 1; }

                PDFBackgroundWorker.RunWorkerAsync();
            }
            else
            { MessageBox.Show("PageCount 0"); }
        }

        // Background worker
        public void PDFBackgroundWorker_DoWork_TextPDF(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            int ii = 1;
            exportFilePath = string.Empty;

                if (JobType == PDFJobType.TextPDF)
                {
                    exportFilePath = directoryName + "\\" + FileNoExt + ".pdf";

                    while (System.IO.File.Exists(exportFilePath))
                    {
                        exportFilePath = directoryName + "\\" + FileNoExt + "_" + ii + ".pdf";
                        ii++;
                    }

                    using (FileStream fs = new FileStream(exportFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.B4);

                        // iTextSharp.text. Document doc = new Document(PageSize.A2);
                        // doc.SetMargins(100, 200, 0, 0); 
                        //Document doc = new Document(PageSize.A5, 36, 72, 108, 180);
                        //Document doc = new Document(PageSize.A3.Rotate(),400,0,0,0);
                        //var doc = new Document(new iTextSharp.text.Rectangle(100f, 300f));                        
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                        iTextSharp.text.pdf.BaseFont bfTimes = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.TIMES_BOLD, iTextSharp.text.pdf.BaseFont.CP1252, false);
                        // doc.SetPageSize(PageSize.A1);
                        // doc.SetMargins(76, 0, 0, 0);
                        doc.SetMarginMirroring(false);
                        iTextSharp.text.Font times = new iTextSharp.text.Font(bfTimes, 14, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.CYAN);

                        if (PDFBackgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            PDFBackgroundWorker.ReportProgress(i);
                            return;
                        }

                        using (StreamReader read = new StreamReader(InputFilename))
                        {

                            string text = read.ReadToEnd();
                            // AddDocMetaData();
                            doc.AddTitle(GameTitle);
                            doc.AddSubject("");
                            doc.AddKeywords(GameTitle + ",  " + FileNoExt + " : by Hypermint,  ");
                            doc.AddCreator("HLM-Chk");
                            doc.AddAuthor(GameTitle);
                            doc.AddHeader(FileNoExt, GameTitle);
                            doc.Open();
                            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                            paragraph.Alignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            paragraph.Add(text);
                            doc.Add(paragraph);

                            //Paragraph paragraph = new Paragraph(text);               
                            doc.Close();
                        }

                    }
                }
       
            }           
            //#region normalPDF
            //    else if (JobType == PDFJobType.NormalPDF)
            //{
            //    if (Directory.Exists(@"C:\Program Files\gs") || Directory.Exists(@"C:\Program Files (x86)\gs"))
            //    {
            //        MagickReadSettings settings = new MagickReadSettings();
            //        settings.Density = new MagickGeometry(154, 154);

            //        using (MagickImageCollection images = new MagickImageCollection())
            //        {
            //            //MagickNET.SetCacheThreshold(524288000);
            //            images.Read(InputFilename, settings);

            //            if (PDFBackgroundWorker.CancellationPending)
            //            {
            //                e.Cancel = true;
            //                PDFBackgroundWorker.ReportProgress(i);
            //                return;
            //            }
            //            int count = images.Count;

            //            int page = 1;

            //            foreach (MagickImage image in images)
            //            {
            //                if (PDFBackgroundWorker.CancellationPending)
            //                {
            //                    e.Cancel = true;
            //                    PDFBackgroundWorker.ReportProgress(i);
            //                    return;
            //                }

            //                string num;
            //                if (i > 9)
            //                { num = ""; }
            //                else num = "0";
            //                image.Crop((image.Width), image.Height);
            //                image.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");

            //                i++;
            //                page++;
            //                PDFBackgroundWorker.ReportProgress(i);
            //            }
            //        }

            //    }
            //    else
            //    {
            //        MessageBox.Show("Needs GhostScript installed.");
            //        e.Cancel = true;
            //        PDFBackgroundWorker.ReportProgress(i);
            //        return;
            //    }

            //}
            //#endregion
            //else
            //{
            //    if (Directory.Exists(@"C:\Program Files\gs"))
            //    {
            //        MagickReadSettings settings = new MagickReadSettings();
            //        i = 0;
            //        settings.Density = new MagickGeometry(154, 154);
            //        // MagickNET.SetCacheThreshold(524288000);
            //        using (MagickImageCollection images = new MagickImageCollection())
            //        {
            //            images.Read(SelectedFullPath + "\\" + FileNoExt, settings);
            //            int page = 1;

            //            foreach (MagickImage image in images)
            //            {
            //                if (PDFBackgroundWorker.CancellationPending)
            //                {
            //                    e.Cancel = true;
            //                    PDFBackgroundWorker.ReportProgress(i);
            //                    return;
            //                }

            //                int count = images.Count;
            //                MagickImage image2 = new MagickImage(image);

            //                string num;
            //                if (i > 9)
            //                { num = ""; }
            //                else num = "0";

            //                if (page == 1 && chop1st == true)
            //                {
            //                    image.Crop(image.Width, image.Height);
            //                    image.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");
            //                    page++;
            //                    i++;
            //                }
            //                else if (page == count && chopLast == true)
            //                {
            //                    image.Crop(image.Width, image.Height);
            //                    image.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");
            //                    i++;
            //                }
            //                else
            //                {
            //                    image.Crop((image.Width / 2), image.Height, Gravity.West);
            //                    image.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");
            //                    i++;

            //                    image2.Crop((image2.Width / 2), image2.Height, Gravity.East);
            //                    image2.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");
            //                    // page++;
            //                    i++;
            //                    page++;
            //                    // Writing to a specific format works the same as for a single image
            //                    //image.Format = MagickFormat.Ptif;
            //                    // image.Write("Snakeware.Page" + page + ".tif");
            //                    PDFBackgroundWorker.ReportProgress(i);
            //                }

            //            }
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Needs GhostScript Installed.");
            //        e.Cancel = true;
            //        PDFBackgroundWorker.ReportProgress(i);
            //        return;
            //    }
            //}
            
        public void PDFBackgroundWorker_DoWork_NormalPDF(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            int page = 1;
            int count = 0;
            string num = string.Empty;

                settings.Density = new MagickGeometry(154, 154);

                using (images)
                {
                    
                    images.Read(InputFilename, settings);
                    count = images.Count;                    
                    page = 1;

                    foreach (MagickImage image in images)
                    {
                        if (PDFBackgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            PDFBackgroundWorker.ReportProgress(i);
                            return;
                        }

                        if (i > 9)
                        { num = ""; }
                        else num = "0";
                        image.Crop((image.Width), image.Height);
                        image.Write(SelectedFullPath + "\\" + FileNoExt + " " + num + i + ".jpg");

                        i++;
                        page++;
                        PDFBackgroundWorker.ReportProgress(i);
                    }
                }

        }
        public void PDFBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //label2.Text = "Processed pages : " + e.ProgressPercentage.ToString();
            //progressBar1.Value = e.ProgressPercentage;

            if (PDFBackgroundWorker.CancellationPending)
            {
                PDFBackgroundWorker.CancelAsync();
                //label2.Text = ("Cancelled...");
                //    //listView1.Enabled = true; listView3.Enabled = true; listView4.Enabled = true;
                //    //dataGridView1.Enabled = true;
            }
        }
        public void PDFBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           // cellFilesRefresh();
           // EnableGUIControls();
            ((MainWindow)System.Windows.Application.Current.MainWindow).progressBarViewer.IsActive = false;
            ((MainWindow)System.Windows.Application.Current.MainWindow).cancelPDFWorker.IsEnabled = false;
            
        }

       
    }
}
