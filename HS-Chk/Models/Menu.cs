using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hypermint.Models
{
    [XmlType("game")]
    public class Menu : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Class for serialising objects to a Hyperspin Main Menu.xml
        /// </summary>
        public Menu() 
        {

        }

        public Menu(string _name,int _enabled=1)
        {
            this.name = _name;
            this.enabled = _enabled;
        }
        public Menu(string _name,Uri pathToIcon,int _enabled=1)
        {
            this.name = _name;
            this.enabled = _enabled;
            if (pathToIcon != null)
                this.sysIcon = pathToIcon;
        }


        public List<Menu> getMainMenuItemsFromXml(string xmlPath,string pathToIcons="")
        {
            if (!System.IO.File.Exists(xmlPath))
                return null;

            List<Menu> tempMenuList = new List<Menu>();

            using (XmlTextReader reader = new XmlTextReader(xmlPath))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(xmlPath);
                int sysCount = xdoc.SelectNodes("menu/game").Count;
                Uri img;

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            string name = reader.GetAttribute("name");
                            int enabled = Convert.ToInt32(reader.GetAttribute("enabled"));
                            string icon = pathToIcons + "\\" + name + ".png";                           

                            if (!System.IO.File.Exists(icon))
                                tempMenuList.Add(new Models.Menu(name,enabled));
                            else
                            {
                                img = new Uri(icon);
                                tempMenuList.Add(new Menu(name, img,enabled));
                            }
                        }
                }
            }

            return tempMenuList;
        }

        ///Usage: Builds main menu xml from folder contents.
        ///
        //Models.Menu m = new Models.Menu();
        //List<Models.Menu> men = m.getMainMenuItemsFromIcons(@"I:\RocketLauncher\Media\Icons", "", "*.png");
        //m.SerializeMainMenuItems("MasterTest", men, @"I:\Hyperspin Apps\Hypermint\Databases\");
       
        /// <summary>
        /// Search a folder with filter. Returns a list of menu items scanned from the folder.
        /// Add a path to icons if wanting to add system icon to list
        /// </summary>
        public List<Menu> getMainMenuItemsFromIcons(string path, string pathToIcons="", string filter = "*.*")
        {
            if (!System.IO.Directory.Exists(path))
                return null;
            List<Menu> tempMenuList = new List<Menu>();
            foreach (var item in System.IO.Directory.GetFiles(path,filter))
            {
                string nameNoExt = System.IO.Path.GetFileNameWithoutExtension(item);

                Uri img = new Uri(pathToIcons + item + ".png");
                tempMenuList.Add(new Menu(nameNoExt, img));
            }

            return tempMenuList;            
        }
        /// <summary>
        /// Serialize Main Menu objects to to xml. Provide menuXmlName no ".xml" extension
        /// </summary>
        /// <param name="menuXml"></param>
        public void SerializeMainMenuItems(string menuXmlName, List<Menu> listToSerialize, string pathToSave)
        {
            if (!System.IO.Directory.Exists(pathToSave))
                System.IO.Directory.CreateDirectory(pathToSave);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlRootAttribute root = new XmlRootAttribute("menu");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Menu>), root);
            System.IO.TextWriter textWriter = new System.IO.StreamWriter(pathToSave + "\\" + menuXmlName + ".xml");

            serializer.Serialize(textWriter, listToSerialize , ns);
            textWriter.Close();
        }

        public void SerializeGenreItems(List<Menu> listToSerialize, string pathToSave)
        {
            if (!System.IO.Directory.Exists(pathToSave))
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));
                    
            }

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlRootAttribute root = new XmlRootAttribute("menu");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Menu>), root);
            System.IO.TextWriter textWriter = new System.IO.StreamWriter(pathToSave);

            serializer.Serialize(textWriter, listToSerialize, ns);
            textWriter.Close();
        }

        private string _name;
        [XmlAttribute(AttributeName = "name")]
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }
        private int _enabled;
        [XmlAttribute(AttributeName = "enabled")]
        public int enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged("_enabled");
            }
        }
        [XmlIgnore]
        public int GameCount
        {
            get { return gameCount; }
            set { gameCount = value; }
        }
        private bool xmlExists;
        [XmlIgnore]
        public bool XmlExists
        {
            get { return xmlExists; }
            set { xmlExists = value; }
        }
        private bool genreExists;
        [XmlIgnore]
        public bool GenreExists
        {
            get { return genreExists; }
            set { genreExists = value; }
        }
        private int gameCount;
        [XmlIgnore]
        public Uri SysIcon  
        {
            get { return sysIcon; }
            set { sysIcon = value;
            OnPropertyChanged("SysIcon");
            }
        }
        private Uri sysIcon;

    }


}

