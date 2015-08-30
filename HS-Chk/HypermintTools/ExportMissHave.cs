using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Hypermint.HypermintTools
{
    public static class ExportMissHave
    {

        public static void ExportMissingTextsHyperSpin(List<DatabaseGame> gamesList,string system)
        {

            string SystemName = system;

            if (gamesList == null)
            {
                MessageBox.Show("System must be scanned first!");
                return;
            }

            string pathToExport = Path.Combine("Exports", "HyperSpin\\Audits\\Missing\\",SystemName);

            int i=0;

            if (!Directory.Exists(pathToExport))
                Directory.CreateDirectory(pathToExport);

            #region Wheels
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Wheels.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Missing Wheel");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveWheel == false)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Themes
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Themes.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Missing Themes");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveTheme == false)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Background
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Backgrounds.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Missing Backgrounds");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveBackgroundsHS == false)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Videos
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Videos.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Missing Videos");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveVideo == false)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region BG-Music
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\BG_Music.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Missing BG_Music");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveBGMusic == false)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion

            if (SystemName != "Main Menu")
            {
                #region Artwork1
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork1.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Artwork1");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt1 == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork2
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork2.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Artwork2");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt2 == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork3
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork3.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Artwork3");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt3 == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork4
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork4.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Artwork4");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt4 == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Snds_start
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Snds_start.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Snds_start");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Start == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Snds_exit
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Snds_exit.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Snds_exit");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Exit == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
            }
            else
            {
                #region Letters
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Letters.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Letters");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveLetters == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Special
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Special.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Special");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveSpecial == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region GenreWheel
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\GenreWheel.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing GenreWheel");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveGenreWheel == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region GenreBG
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\GenreBG.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing GenreBG");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveGenreBG == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Pointer
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Pointer.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing Pointer");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HavePointer == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region snds-wheels
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\snds-wheels.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing snds-wheels");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Wheel == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region snds-click
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\snds-click.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Missing snds-click");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Click == false)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
            }
         }

        public static void ExportHaveTextsHyperSpin(List<DatabaseGame> gamesList, string system)
        {

            string SystemName = system;

            if (gamesList == null)
            {
                MessageBox.Show("System must be scanned first!");
                return;
            }

            string pathToExport = Path.Combine("Exports", "HyperSpin\\Audits\\Have\\", SystemName);

            int i = 0;

            if (!Directory.Exists(pathToExport))
                Directory.CreateDirectory(pathToExport);

            #region Wheels
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Wheels.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Have Wheel");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveWheel == true)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Themes
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Themes.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Have Themes");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveTheme == true)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Background
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Backgrounds.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Have Backgrounds");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveBackgroundsHS == true)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region Videos
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\Videos.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Have Videos");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveVideo == true)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion
            #region BG-Music
            using (StreamWriter writer = new StreamWriter(pathToExport + "\\BG_Music.txt"))
            {
                i = 0;
                writer.WriteLine("------------------");
                writer.WriteLine("Have BG_Music");
                writer.WriteLine("------------------");

                foreach (var game in gamesList)
                {
                    if (game.RomName != "_Default")
                    {
                        if (game.HaveBGMusic == true)
                        {
                            writer.WriteLine(game.RomName);
                            i++;
                        }
                    }
                }
                writer.WriteLine("------------------");
                writer.WriteLine("Total: " + i);
                writer.WriteLine("------------------");
            }
            #endregion

            if (SystemName != "Main Menu")
            {
                #region Artwork1
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork1.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Artwork1");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt1 == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork2
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork2.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Artwork2");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt2 == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork3
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork3.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Artwork3");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt3 == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Artwork4
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Artwork4.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Artwork4");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveArt4 == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Snds_start
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Snds_start.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Snds_start");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Start == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Snds_exit
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Snds_exit.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Snds_exit");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Exit == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
            }
            else
            {
                #region Letters
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Letters.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Letters");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveLetters == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Special
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Special.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Special");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveSpecial == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region GenreWheel
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\GenreWheel.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have GenreWheel");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveGenreWheel == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region GenreBG
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\GenreBG.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have GenreBG");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveGenreBG == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region Pointer
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\Pointer.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have Pointer");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HavePointer == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region snds-wheels
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\snds-wheels.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have snds-wheels");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Wheel == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
                #region snds-click
                using (StreamWriter writer = new StreamWriter(pathToExport + "\\snds-click.txt"))
                {
                    i = 0;
                    writer.WriteLine("------------------");
                    writer.WriteLine("Have snds-click");
                    writer.WriteLine("------------------");

                    foreach (var game in gamesList)
                    {
                        if (game.RomName != "_Default")
                        {
                            if (game.HaveS_Click == true)
                            {
                                writer.WriteLine(game.RomName);
                                i++;
                            }
                        }
                    }
                    writer.WriteLine("------------------");
                    writer.WriteLine("Total: " + i);
                    writer.WriteLine("------------------");
                }
                #endregion
            }
        }


        }


    }
