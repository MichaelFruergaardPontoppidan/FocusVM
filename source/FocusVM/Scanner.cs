using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FocusVM
{
    class Scanner
    {
        const string aosFolder = @"C:\AOSService\PackagesLocalDirectory";
        public HashSet<string> RequiredModels = new HashSet<string>();
        int depth = 0;

        public void scanAOSPackages()
        {
            this.scanFolders(aosFolder);
        }

        public void includeFolder(string path, string from = "")
        {
 
            if (!RequiredModels.Contains(path.ToLowerInvariant()))
            {
                RequiredModels.Add(path.ToLowerInvariant());
                //string message = string.Format("Include model: {0} from {1}", path.PadLeft(depth), from);
                //Console.WriteLine(message);

                depth++;
                this.scanDescriptorFolder(aosFolder + @"\" + path);
                depth--;
            }
        }

        void scanFolders(string path)
        {
            string[] files = null;

            if (Directory.Exists(path))
            {
                string[] folders = System.IO.Directory.GetDirectories(path);
                foreach (string folder in folders)
                {
                    scanDescriptorFolder(folder);
                }
            }
        }

        void scanDescriptorFolder(string path)
        {
            path += "\\descriptor";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.xml");

                foreach (string file in files)
                {
                    var refs = this.getReferencedModels(file);

                    foreach (var model in refs)
                    {
                        this.includeFolder(model, path);
                    }                    
                }
            }
        }

        void scanFile(string filename)
        {
            if (File.Exists(filename))
            {
                var refs = this.getReferencedModels(filename);
                /*
                                if (fileText != processedText)
                                {
                                    System.Text.Encoding outEncoding;
                                    outEncoding = SourceFile.fileEncoding;

                                    SourceFile = null;
                                    File.SetAttributes(filename, FileAttributes.Archive);
                                    FileStream destinationStream = new FileStream(filename, FileMode.Create);
                                    using (StreamWriter destinationFile = new StreamWriter(destinationStream, outEncoding))
                                    {
                                        destinationFile.Write(processedText);
                                    }
                                    System.IO.File.Move(filename, filename.Replace("mfp1", name));
                                }
                                return name;

                    */
            }
        }

        private List<string> getReferencedModels(string filename)
        {
            if (File.Exists(filename))
            {
                List<string> res = new List<string>();
                XmlReader SourceFile = new XmlReader(filename);

                XmlNodeList nodes = extractMultipleFromXML(SourceFile.Text(), "//AxModelInfo/ModuleReferences/d2p1:string");

                foreach (XmlNode node in nodes)
                {
                    res.Add(node.InnerText);
                }

                return res;
            }
            return null;
        }

        static public XmlNodeList extractMultipleFromXML(string xml, string xpath)
        {
            //xml = RemoveAllNamespaces(xml);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("i", "http://www.w3.org/2001/XMLSchema-instance");
            namespaceManager.AddNamespace("d2p1", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");

            return doc.SelectNodes(xpath, namespaceManager);
        }

        public static string RemoveAllNamespaces(string _xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(_xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

    }
}
