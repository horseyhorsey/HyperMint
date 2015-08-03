using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Hypermint
{
    /// <summary>
    /// Classes for all presets
    /// </summary>
    public class Presets : HyperMintBaseClass
    {
        /// <summary>
        /// Returns string[] of all the presets in given directory
        /// </summary>
        /// <returns></returns>
        public static string[] GetPresetsFromDirectory(string dirname)
        {
            DirectoryInfo dir = new DirectoryInfo("presets\\" + dirname);
            FileInfo[] Files;
            Files = dir.GetFiles("*.xml");
            string[] presetFiles = new string[Files.Length];
            int i = 0;
            foreach (var item in Files)
            {
                presetFiles[i] = item.Name;
                i++;
            }

            return presetFiles;
        }
    }

    /// <summary>
    /// Instruction card preset class
    /// </summary>
    class CardPresets : Presets
    {
        /// <summary>
        /// Saves the instructon card to xml file
        /// </summary>
        /// <param name="presetName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="description"></param>
        /// <param name="author"></param>
        /// <param name="pos"></param>
        /// <param name="resize"></param>
        /// <param name="stretch"></param>
        public static void SavePreset(string presetName, string width, string height, string description, string author, string pos, bool resize, bool stretch)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            if (Directory.Exists("presets\\cards"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\\cards\\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("author", author);
                    write.WriteElementString("description", description);
                    write.WriteElementString("position", pos);

                    write.WriteElementString("width", width);
                    write.WriteElementString("height", height);

                    write.WriteElementString("resize", resize.ToString());
                    write.WriteElementString("stretch", stretch.ToString());
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }
            }
        }
        /// <summary>
        /// Read in xml values for preset
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        public static string[] LoadPreset(string XML)
        {
            string[] elements = new string[7];
            using (XmlReader read = XmlReader.Create(XML))
            {
                while (read.Read())
                {
                    if (read.IsStartElement())
                    {
                        switch (read.Name)
                        {
                            case "author":
                                elements[0] = read.ReadString();
                                break;
                            case "description":
                                elements[1] = read.ReadString();
                                break;
                            case "position":
                                elements[2] = read.ReadString();
                                break;
                            case "width":
                                elements[3] = read.ReadString();
                                break;
                            case "height":
                                elements[4] = read.ReadString();
                                break;
                            case "resize":
                                elements[5] = read.ReadString();
                                break;
                            case "stretch":
                                elements[6] = read.ReadString();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {


                    }

                }
            }

            return elements;

        }

    }
    class ImagePresets : Presets
    {
        /// <summary>
        /// Save Preset from given parameters.
        /// </summary>
        /// <param name="presetName"></param>
        /// <param name="rlmedia"></param>
        /// <param name="author"></param>
        /// <param name="prefix"></param>
        /// <param name="outW"></param>
        /// <param name="outH"></param>
        /// <param name="resize"></param>
        /// <param name="stretch"></param>
        /// <param name="ratio"></param>
        /// <param name="tile"></param>
        /// <param name="tileW"></param>
        /// <param name="tileH"></param>
        /// <param name="FlipL"></param>
        /// <param name="FlipR"></param>
        /// <param name="JPG"></param>
        public static void saveXMLPresetImages(string presetName, OtherImages.RLMediaType rlmedia,
            string author,string prefix,string outW,string outH,string resize,string stretch,string ratio,
            string tile,string tileW,string tileH,string FlipL,string FlipR,string JPG)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            //Change the enum into string for fade names etc.
            string type = SetImagePresetType(rlmedia);

            if (Directory.Exists("presets\\images"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\images\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("author", author);
                    write.WriteElementString("prefix", prefix);
                    write.WriteElementString("mediaType", type);
                    write.WriteElementString("outWidth", outW);
                    write.WriteElementString("outHeight", outH);
                    write.WriteElementString("resize", resize);
                    write.WriteElementString("stretch", stretch);
                    write.WriteElementString("ratio", ratio);
                    write.WriteElementString("tile", tile);
                    write.WriteElementString("tileWidth", tileW);
                    write.WriteElementString("tileHeight", tileH);
                    write.WriteElementString("flipL", FlipL);
                    write.WriteElementString("flipR", FlipR);
                    write.WriteElementString("jpg", JPG);
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }
            }
        }
        public static string[] LoadXMLPresetImages(string xml)
        {
            string[] elements = new string[14];

            if (File.Exists(xml))
            {
                using (XmlReader read = XmlReader.Create(xml))
                {
                    while (read.Read())
                    {
                        if (read.IsStartElement())
                        {
                            switch (read.Name)
                            {
                                case "author":
                                    elements[0] = read.ReadString();
                                    break;
                                case "prefix":
                                    elements[1] = read.ReadString();
                                    break;
                                case "mediaType":
                                    elements[2] = read.ReadString();
                                    break;
                                case "outWidth":
                                    elements[3] = read.ReadString();
                                    break;
                                case "outHeight":
                                    elements[4] = read.ReadString();
                                    break;
                                case "ratio":
                                    elements[5]  = read.ReadString();
                                    break;
                                case "resize":
                                    elements[6] = read.ReadString();
                                    break;
                                case "stretch":
                                    elements[7] = read.ReadString();
                                    break;
                                case "tile":
                                    elements[8] = read.ReadString();
                                    break;
                                case "tileWidth":
                                    elements[9] = read.ReadString();
                                    break;
                                case "tileHeight":
                                    elements[10] = read.ReadString();
                                    break;
                                case "flipL":
                                    elements[11] = read.ReadString();
                                    break;
                                case "flipR":
                                    elements[12] = read.ReadString();
                                    break;
                                case "jpg":
                                    elements[13] = read.ReadString();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return elements;
        }
        public static string SetImagePresetType(OtherImages.RLMediaType rlmediaType)
        {
            string rlmediaTypestring = string.Empty;

            if (rlmediaType == OtherImages.RLMediaType.Fade1)
                rlmediaTypestring = "Layer 1";
            else if (rlmediaType == OtherImages.RLMediaType.Fade2)
                rlmediaTypestring = "Layer 2";
            else if (rlmediaType == OtherImages.RLMediaType.Fade3)
                rlmediaTypestring = "Layer 3";
            else if (rlmediaType == OtherImages.RLMediaType.FadeExit)
                rlmediaTypestring = "Layer -1";
            else if (rlmediaType == OtherImages.RLMediaType.PauseBG)
                rlmediaTypestring = "Background";
            else if (rlmediaType == OtherImages.RLMediaType.BezelBG)
                rlmediaTypestring = "Bezel Background";

            return rlmediaTypestring;
        }
   
    }
    sealed class WheelPresest : Presets
    {
        /// <summary>
        /// Save a wheel preset from UI values
        /// </summary>
        /// <param name="presetName"></param>
        /// <param name="WheelType"></param>
        /// <param name="font"></param>
        /// <param name="labelenabled"></param>
        /// <param name="gameText"></param>
        /// <param name="prefix_text"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="arc"></param>
        /// <param name="trim"></param>
        /// <param name="fillcolor"></param>
        /// <param name="strokecolor"></param>
        /// <param name="stroke"></param>
        /// <param name="gravity"></param>
        /// <param name="gravityOn"></param>
        /// <param name="bgcolor"></param>
        /// <param name="transp"></param>
        /// <param name="shadColor"></param>
        /// <param name="shadOn"></param>
        /// <param name="shadSize"></param>
        /// <param name="shadDist"></param>
        /// <param name="swap"></param>
        /// <param name="repage"></param>
        public static void saveXML_Preset(string presetName, string WheelType,string font,string labelenabled,
            string gameText,string prefix_text,string sizeX,string sizeY,string arc,string trim,
            string fillcolor,string strokecolor,string stroke, string gravity,string gravityOn,
            string bgcolor,string transp,string shadColor,string shadOn,string shadSize,
            string shadDist,string swap,string repage)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            if (Directory.Exists("presets\\wheels"))
            {
                using (XmlWriter write = XmlWriter.Create(@"presets\wheels\" + presetName + ".xml", settings))
                {
                    write.WriteStartElement("preset");
                    write.WriteElementString("wheelType", WheelType);
                    write.WriteElementString("font", font);
                    write.WriteElementString("labelenabled", labelenabled);
                    write.WriteElementString("game_text", gameText);
                    write.WriteElementString("prefix_text", prefix_text);
                    write.WriteElementString("sizeX", sizeX);
                    write.WriteElementString("sizeY", sizeY);
                    write.WriteElementString("arc", arc);
                    write.WriteElementString("trim", trim);

                    write.WriteElementString("fillColor", fillcolor);
                    write.WriteElementString("strokeColor", strokecolor);
                    write.WriteElementString("stroke", stroke);

                    write.WriteElementString("gravity", gravityOn);
                    write.WriteElementString("gravityOn", gravity);

                    write.WriteElementString("bgColor", bgcolor);
                    write.WriteElementString("transp", transp);
                    write.WriteElementString("shadowColor", shadColor);
                    write.WriteElementString("shadowOn", shadOn);
                    write.WriteElementString("shadowSize", shadSize);
                    write.WriteElementString("shadowDistance", shadDist);
                    write.WriteElementString("swap", swap);
                    write.WriteElementString("repage", repage);
                    write.WriteEndElement();
                    write.Flush();
                    write.Close();
                }
            }
        }

        public static string[] LoadXmlPreset(string xml)
        {
            string[] elements = new string[22];

            if (File.Exists(xml))
            {
                using (XmlReader read = XmlReader.Create(xml))
                {
                    while (read.Read())
                    {
                        if (read.IsStartElement())
                        {
                            switch (read.Name)
                            {
                           case "wheelType":
                                elements[0] = read.ReadString();
                                break;
                            case "font":
                                elements[1] = read.ReadString();
                                break;
                            case "labelenabled":
                                elements[2] = read.ReadString();
                                break;
                            case "game_text":
                                elements[3] = read.ReadString();
                                break;
                            case "prefix_text":
                                elements[4] = read.ReadString();
                                break;
                            case "sizeX":
                                elements[5] = read.ReadString();
                                break;
                            case "sizeY":
                                elements[6] = read.ReadString();
                                break;
                            case "arc":
                                elements[7] = read.ReadString();
                                break;
                            case "trim":
                                elements[8] = read.ReadString();
                                break;
                            case "fillColor":
                                elements[9] = read.ReadString();
                                break;
                            case "strokeColor":
                                elements[10] = read.ReadString();
                                break;
                            case "stroke":
                                elements[11] = read.ReadString();
                                break;
                            case "gravity":
                                elements[12] = read.ReadString();
                                break;
                            case "gravityOn":
                                elements[13] = read.ReadString();
                                break;
                            case "bgColor":
                                elements[14] = read.ReadString();
                                break;
                            case "transp":
                                elements[15] = read.ReadElementContentAsString();
                                break;
                            case "shadowColor":
                                elements[16] = read.ReadString();
                                break;
                            case "shadowOn":
                                elements[17] = read.ReadElementContentAsString();
                                break;
                            case "shadowSize":
                                elements[18] = read.ReadElementContentAsString();
                                break;
                            case "shadowDistance":
                                elements[19] = read.ReadElementContentAsString();
                                break;
                            case "swap":
                                elements[20] = read.ReadElementContentAsString();
                                break;
                            case "repage":
                                elements[21] = read.ReadElementContentAsString();
                                break;
                            default:
                                break;                        
                            }
                        }
                    }
                }
            }

            return elements;
        }

    }
}

