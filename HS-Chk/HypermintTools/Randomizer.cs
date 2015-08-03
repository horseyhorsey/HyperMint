using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Hypermint
{
    /// <summary>
    /// Struct to randomize game selection from a Hyperspin database
    /// </summary>
    public struct Randomizer
    {
        public static string HSdbPath = string.Empty;

        /// <summary>
        /// Deprecated?
        /// </summary>
        public static void GetRandomXML()
        {
            GetSystem();
            DirectoryInfo di = new DirectoryInfo(HSdbPath);
            FileInfo[] fi = di.GetFiles();

            Console.WriteLine(fi[RandomInt(fi.Length)].FullName);
        }

        /// <summary>
        /// Returns a random system xml from the Main Menu.xml
        /// </summary>
        /// <returns></returns>
        public static string GetSystem()
        {
            string mainMenuXml = Path.Combine(HSdbPath, @"Main Menu\Main Menu.xml");

            // Creat null Xdocument 
            XDocument xdoc = null;

            // Using XmlReader to be able to load into xdoc
            using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(mainMenuXml))
            {
                xdoc = XDocument.Load(xr);
            }

            // Query with LINQ
            var systems =
                from item in xdoc.Descendants("game")
                select item.Attribute("name").Value;

            string systemName = systems.ElementAt(RandomInt(systems.Count()));
            systemName = Path.Combine(HSdbPath, systemName, systemName + ".xml");
            return systemName;

        }

        /// <summary>
        /// Pick random game from a systemXML
        /// </summary>
        /// <param name="systemXML"></param>
        /// <returns></returns>
        public static string GetGame(string systemXML)
        {
            XDocument xdoc = null;
            //Load into stream

            try
            {

                if (File.Exists(systemXML))
                {
                    using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(systemXML))
                    {
                        xdoc = XDocument.Load(xr);
                    }

                    var games =
                        from game in xdoc.Descendants("game")
                        select game.Attribute("name").Value;


                    return games.ElementAt(RandomInt(games.Count()));
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Get a random int from count of items
        /// </summary>
        /// <param name="countOfItems"></param>
        /// <returns></returns>
        private static int RandomInt(int countOfItems)
        {
            Random r = new Random();
            int randomNumber = r.Next(0, countOfItems);
            return randomNumber;
        }

    }
}
