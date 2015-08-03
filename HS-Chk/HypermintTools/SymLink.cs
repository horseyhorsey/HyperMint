using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Hypermint.HypermintTools
{
    public static class SymLink
    {
        public  enum SymbolicLink
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
