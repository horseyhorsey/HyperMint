using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Hypermint
{
    public abstract class HyperMintBaseClass : IHyperspin, IRocketLaunch, IimageMagick
    {
        private string hsPath;
        /// <summary>
        /// HyperSpin Main Directory
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

        private string rlPath;
        /// <summary>
        /// Rocket launcher Main Directory
        /// </summary>
        public string RLPath
        {
            get
            {
                return rlPath;
            }
            set
            {
                rlPath = value;
            }
        }

        private string imPath;
        /// <summary>
        /// Image Magick Path
        /// </summary>
        public string IMPath
        {
            get
            {
                return imPath;
            }
            set
            {
                imPath = value;
            }
        }

        private string rlMediaPath;
        /// <summary>
        /// Rocket Launcher Media Path
        /// </summary>
        public string RLMediaPath
        {
            get
            {
                return rlMediaPath;
            }
            set
            {
                rlMediaPath = value;
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

        [XmlIgnore]
        public BackgroundWorker bw;
        public virtual void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        public virtual void bw_DoWork(object sender, DoWorkEventArgs e)
        {
        }

    }
}
