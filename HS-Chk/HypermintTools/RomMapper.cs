using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hypermint
{
    public struct RomMapper
    {
        public static void CreateGamesIni(string inputXML,string gamesIniPath)
        {
                //string filename = args[0];
                //Console.WriteLine(filename);
                System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.Load(inputXML);



                var di = new DirectoryInfo(gamesIniPath);
                var fi = new FileInfo(gamesIniPath + "\\games.ini");
                di.Attributes &= FileAttributes.Normal;

                if (File.Exists(fi.FullName))
                    fi.Attributes &= ~FileAttributes.ReadOnly;

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(gamesIniPath + "\\games.ini", true))
                {
                    file.WriteLine("# This file is only used for remapping specific games to other Emulators and/or Systems.");
                    file.WriteLine("# If you don't want your game to use the Default_Emulator, you would set the Emulator key here.");
                    file.WriteLine("# This file can also be used when you have Wheels with games from other Systems.");
                    file.WriteLine("# You would then use the System key to tell HyperLaunch what System to find the emulator settings.");
                    file.WriteLine("");
                    
                    
                }

                foreach (System.Xml.XmlNode node in xdoc.SelectNodes("menu/game"))
                {
                    string GameName = node.SelectSingleNode("@name").InnerText;
                    string sysName = node.SelectSingleNode("System").InnerText;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(gamesIniPath + "\\games.ini", true))
                    {
                        file.WriteLine("[{0}]", GameName);
                        file.WriteLine(@"System={0}", sysName);
                    }
                }



            }

        public static void CreateDefaultEmulatorsIni(string gamesIniPath)
        {

            if (File.Exists(gamesIniPath + "\\Emulators.ini"))
                return;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(gamesIniPath + "\\Emulators.ini", true))
            {
                file.WriteLine("[Roms]");
                file.WriteLine("Rom_Path=");
                file.WriteLine("Default_Emulator=");
                file.WriteLine("");
                file.WriteLine("[ExampleEmu]");
                file.WriteLine(@"Emu_Path=C:\Hyperspin\Emulators\Emu_Name\emulator.exe");
                file.WriteLine(@"Rom_Extension=7z|bin");
                file.WriteLine(@"Module=Custom_Module_Name_If_Different_Then_Emu_Name");
                file.WriteLine("Pause_Save_State_Keys=Read_Guide_To_Use_These");
                file.WriteLine("Pause_Load_State_Keys=Read_Guide_To_Use_These");

                file.Close();
            }


        }
        }
    }
