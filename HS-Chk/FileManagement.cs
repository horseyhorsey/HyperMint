using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Hypermint
{
    /// <summary>
    /// Static methods for Hyperspin and rocketlauncher file management
    /// </summary>
    public static class FileManagement
    {
        /// <summary>
        /// Get all the statistic files in the given folder
        /// </summary>
        /// <param name="rlPath"></param>
        /// <returns></returns>
        public static FileInfo[] getStatFiles(string rlPath)
        {
            IniFile ini = new IniFile();
            DirectoryInfo dir = new DirectoryInfo(rlPath + "\\Data\\Statistics\\");
            FileInfo[] Files = dir.GetFiles();

            return Files;
        }
        /// <summary>
        /// Search all sub directories for files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool doesDirectoryContainFiles(string path, string filter = "*")
        {
            if (!Directory.Exists(path)) return false;
            return Directory.EnumerateFiles(path, filter, SearchOption.AllDirectories).Any();
        }
        /// <summary>
        /// Send a file to the Hypermint trash folder
        /// </summary>
        /// <param name="TrashFolder"></param>
        /// <param name="file"></param>
        /// <param name="systemName"></param>
        /// <param name="column"></param>
        /// <param name="rom"></param>
        //public static void SendFileToTrash(string TrashFolder, System.IO.FileInfo file, string systemName, string column, string rom)
        //{
        //    if (!file.Exists)
        //        return;

        //    string PathToMoveTo;
        //    if (TrashFolder == "RocketLauncher")
        //        PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
        //         "Rubbish\\" + TrashFolder + "\\" + systemName + "\\" + column + "\\" + rom + "\\";
        //    else
        //        PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
        //                        "Rubbish\\" + TrashFolder + "\\" + systemName + "\\" + column + "\\";

        //    DirectoryInfo di = new DirectoryInfo(PathToMoveTo);
        //    if (!di.Exists)
        //        di.Create();

        //    string moveFilenameNoExt = Path.GetFileNameWithoutExtension(file.FullName);
        //    string moveFilenameNew = moveFilenameNoExt;
        //    int i = 1;
        //    while (File.Exists(PathToMoveTo + moveFilenameNew + file.Extension))
        //    {
        //        moveFilenameNew = moveFilenameNoExt + "_" + i;
        //        i++;
        //    }

        //    try
        //    {
        //        file.MoveTo(PathToMoveTo + moveFilenameNew + file.Extension);
        //        string movedFile = PathToMoveTo + moveFilenameNew + file.Extension;
        //        return;
        //    }

        //    catch (IOException e)
        //    {
        //        System.Windows.MessageBox.Show(
        //              "{0}: Cannot send to rubbish " +
        //              "because the file is in use.",
        //              e.GetType().Name);
        //        return;
        //    }
        //}
        /// <summary>
        /// Send selected items to RUbbish (Trash)
        /// </summary>
        /// <param name="TrashFolder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool SendToTrash(string TrashFolder, FileInfo file, string systemName, string column, string rom)
        {
            if (!file.Exists)
                return false;

            string PathToMoveTo;
            if (TrashFolder == "RocketLauncher")
                PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
                 "Rubbish\\" + TrashFolder + "\\" + systemName + "\\" + column + "\\" + rom + "\\";
            else
                PathToMoveTo = AppDomain.CurrentDomain.BaseDirectory +
                                "Rubbish\\" + TrashFolder + "\\" + systemName + "\\" + column + "\\";

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
                string movedFile = PathToMoveTo + moveFilenameNew + file.Extension;
                return true;
            }

            catch (IOException e)
            {
                System.Windows.MessageBox.Show(
                      "{0}: Cannot send to rubbish " +
                      "because the file is in use.",
                      e.GetType().Name);
                return false;
            }
        }
        /// <summary>
        /// Check all files in directory with given Extension
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="extFilter"></param>
        /// <returns></returns>
        public static bool CheckMediaFolderFiles(string fullpath, string extFilter)
        {
            // if the path doesn't exist which should return fals
            if (!Directory.Exists(fullpath))
                return false;

            string[] getFiles;
            getFiles = Directory.GetFiles(fullpath, extFilter);
            if (getFiles.Length != 0)
                return true;
            else return false;
        }
        public static bool CheckForFile(string filenamePath)
        {
            if (File.Exists(filenamePath))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Return all files named by romname with Romname *.*
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="Romname"></param>
        /// <returns></returns>
        public static FileInfo[] SearchAllFilesNameByRomname(string FullPath, string Romname)
        {
            //set the directory
            DirectoryInfo di = new DirectoryInfo(FullPath);
            //set the array size to the file search count
            FileInfo[] fi = new FileInfo[di.GetFiles(Romname + "*.*").Count()];
            //return the FIleinfo array 
            return di.GetFiles(Romname + "*.*");
        }
        /// <summary>
        /// Refresh the files box
        /// </summary>
        /// <param name="column"></param>
        /// <param name="HSSpecialArts"></param>
        /// <returns></returns>
        public static List<FileCollection> cellFilesRefresh(string column, string FullPath, bool HSSpecialArts = false)
        {
            string[] extArray = { ".mp4", ".png" };

            DirectoryInfo dir = new DirectoryInfo(FullPath);
            if (!dir.Exists)
                return new List<FileCollection>();
            FileInfo[] Files = dir.GetFiles();

            if (Files.Length == 0)
                return new List<FileCollection>();

            string name;
            List<FileCollection> fc = new List<FileCollection>();
            foreach (FileInfo file in Files)
            {
                if (file.Name != "Thumbs.db")
                    if (column == "Bezels")
                    {
                        name = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
                        if (!name.Contains("Instruction"))
                            fc.Add(new FileCollection(name, file.Extension, file.FullName));
                    }
                    else if (column == "Cards")
                    {
                        name = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
                        if (!name.Contains("Bezel"))
                            fc.Add(new FileCollection(name, file.Extension, file.FullName));
                    }
                    else
                    {
                        name = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
                        fc.Add(new FileCollection(name, file.Extension, file.FullName));
                    }
            }

            return fc;
        }
        private static bool IsFileLocked(string filename)
        {
            FileInfo file = new FileInfo(filename);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        public static void MoveFile(string from, string to)
        {
            try
            {
                FileInfo file = new FileInfo(from);
                // check if the file exists

                if (file.Exists)
                {
                    // check if the file is not locked
                    if (IsFileLocked(from) == false)
                    {
                        // move the file
                        File.Move(from, to);
                    }
                }
            }
            catch (Exception e)
            {
                ;
            }
        }
    }


    public static class SymLink
    {
        public enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public static string symbolicLink { get; set; }
        public static string fileName { get; set; }
        public static SymbolicLink symlinkType { get; set; }

        [DllImport("kernel32.dll")]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        //CreateSymbolicLink(symbolicLink, fileName, SymbolicLink.File);

        public static void CheckThenCreate(string FileToLink, string tempSymlinkFile)
        {

            if (File.Exists(FileToLink))
            {
                symbolicLink = tempSymlinkFile;
                fileName = FileToLink;
                symlinkType = SymLink.SymbolicLink.File;
                CreateSymbolicLink(symbolicLink, fileName, symlinkType);
            }
        }

        public static void CreateDirectory(string tempSymlinkFile)
        {
            if (!Directory.Exists(tempSymlinkFile))
                Directory.CreateDirectory(tempSymlinkFile);
        }
    }
}