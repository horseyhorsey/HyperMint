using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hypermint
{
    interface IHyperspinFavorite
    {
        bool IsFavorite { get; set; }
    }

    interface IRocketLaunchStats
    {
        int TimesPlayed { get; set; }
        DateTime LastTimePlayed { get; set; }
        TimeSpan AvgTimePlayed { get; set; }
        TimeSpan TotalTimePlayed { get; set; }
        TimeSpan TotalOverallTime { get; set; }
    }

    interface IHyperspinGame
    {
        string RomName { get; set; }
    }

    /// <summary>
    /// Struct class to manage all statistics from Rocketlauncher
    /// </summary>
    internal class Statistics : DatabaseGame
    {
        public static List<string> gameStatsList = new List<string>();
        public static string StatsPath { get; set; }

        public int TimesPlayed { get; set; }
        public DateTime LastTimePlayed { get; set; }
        public TimeSpan AvgTimePlayed { get; set; }
        public TimeSpan TotalTimePlayed { get; set; }
        public string _systemName { get; set; }
        public string Rom { get; set; }
        public static TimeSpan TotalOverallTime;

        public Statistics()
        { }

        public Statistics(string sys, string rom, int timesplayed, DateTime lasttimeplayed,
            TimeSpan avgtimeplayed, TimeSpan totaltimeplayed)
        {
            this._systemName = sys;
            this.Rom = rom;
            this.TimesPlayed = timesplayed;
            this.LastTimePlayed = lasttimeplayed;
            this.AvgTimePlayed = avgtimeplayed;
            this.TotalTimePlayed = totaltimeplayed;
        }

        /// <summary>
        /// Convert the time from the seconds string
        /// Convert to double & Timespan
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        private static string timeConverter(string Time)
        {
            double time = Convert.ToDouble(Time);
            TimeSpan obj = TimeSpan.FromSeconds(time);
            string chin = Convert.ToString(obj);

            return chin;
        }

        public List<Statistics> get_Stats(string statFile)
        {
            List<Statistics> statList = new List<Statistics>();

            string sysStatIniName = Path.GetFileNameWithoutExtension(statFile);
            if (sysStatIniName == "desktop" || sysStatIniName == "Global Statistics")
                return statList;

            IniFile ini = new IniFile();
            string[] genStats = { "General", "TopTen_Time_Played", "TopTen_Times_Played", "Top_Ten_Average_Time_Played" };
            ini.Load(statFile);
            int count = ini.Sections.Count;
            string time = "";
            //ini.GetSection(filter);
            foreach (IniFile.IniSection s in ini.Sections)
            {

                string section = s.Name.ToString();
                if (genStats[0] != section)
                    if (genStats[1] != section)
                        if (genStats[2] != section)
                            if (genStats[3] != section)
                            {
                                if (section == "General")
                                {

                                }
                                else
                                {
                                    TimeSpan t = new TimeSpan();
                                    Rom = section;
                                    _systemName = sysStatIniName;
                                    try
                                    {
                                        TimesPlayed = Convert.ToInt32(ini.GetKeyValue(section, "Number_of_Times_Played"));

                                    }
                                    catch (Exception)
                                    {

                                    }

                                    try
                                    {
                                        LastTimePlayed = Convert.ToDateTime(ini.GetKeyValue(section, "Last_Time_Played"));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    try
                                    {

                                        //AvgTimePlayed = ;
                                        var avgTime = TimeSpan.Parse(ini.GetKeyValue(section, "Average_Time_Played")).Days;
                                        AvgTimePlayed = new TimeSpan(0, 0, avgTime);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    try
                                    {
                                        var TotalTime = TimeSpan.Parse(ini.GetKeyValue(section, "Total_Time_Played")).Days;

                                        TotalTimePlayed = new TimeSpan(0, 0, TotalTime);
                                        TotalOverallTime = TotalOverallTime + TotalTimePlayed;


                                    }
                                    catch (Exception)
                                    {


                                    }


                                    statList.Add(new Statistics(_systemName, Rom, TimesPlayed, LastTimePlayed, AvgTimePlayed, TotalTimePlayed));
                                }
                            }
            }
            return statList;

        }

        /// <summary>
        /// Returns the 4 values from the ini for the game sent to this
        /// times played, last time played , average, total time
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="romName"></param>
        /// <returns></returns>
        public static List<string> getSingleGameStats(string systemName, string romName)
        {
            IniFile ini = new IniFile();
            ini.Load(StatsPath + "\\" + systemName + ".ini");

            var i = ini.GetSection(romName);
            if (i == null)
                return new List<string>();
            //            List<string> gameStatsList = new List<string>();

            gameStatsList.Add(ini.GetKeyValue(romName, "Number_of_Times_Played"));
            gameStatsList.Add(ini.GetKeyValue(romName, "Last_Time_Played"));
            gameStatsList.Add(ini.GetKeyValue(romName, "Average_Time_Played"));
            gameStatsList.Add(ini.GetKeyValue(romName, "Total_Time_Played"));

            return gameStatsList;
        }

        /// <summary>
        /// Get all stats files from Rocketlaunch data
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

    }
}
