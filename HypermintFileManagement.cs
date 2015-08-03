using System;

namespace Hypermint
{
    /// <summary>
    /// Holds methods for managing hyperspin/rocketlaunncher files.
    /// </summary>
    public static class FileManagment
    {
        /// <summary>
        /// Send a file to trash. Moving file to "Hyperspin" or "Rocketlauncher" and placing in correct directory for backup.
        /// </summary>
        /// <param name="TrashFolder"></param>
        /// <param name="file"></param>
        /// <param name="systemName"></param>
        /// <param name="column"></param>
        /// <param name="rom"></param>
        public static void SendFileToTrash(string TrashFolder, System.IO.FileInfo file, string systemName, string column, string rom)
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
    }
}