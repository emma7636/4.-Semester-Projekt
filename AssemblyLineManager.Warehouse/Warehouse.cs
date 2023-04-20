using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System;

namespace AssemblyLineManager.Warehouse
{
    public class Warehouse
    {
        string URL = "http://localhost:8081/Service.asmx";

        private Dictionary<int, string> stateLUT;

        Warehouse()
        {
            // Not sure if this was what the dictionary was meant to be used as.
            stateLUT = new Dictionary<int, string>();
            stateLUT.Add(0, "Idle");
            stateLUT.Add(1, "Executing");
            stateLUT.Add(2, "Error");
        }
        private void PickItem(int id) // Method that sends command via SOAP to warehouse for picking an item
        { }
        private void InsertItem(int id, string name) // Method that sends command via SOAP to warehouse for inserting an item
        { }
        private JsonArray? GetInventory() //Method to get the inventory info
        {

            return null;
        }
        private static string RemoveAllNamespaces(string xmlDocument) //Removes all namespaces with a string
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));
            return xmlDocumentWithoutNs.ToString();
        }

        public static XElement RemoveAllNamespaces(XElement xmlDocument) //Removes all namespaces with a xElement
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

       static void Main(string[] args)
        {
            string path = "";
            string message = File.ReadAllText(path);

            /*XElement xmlWithNoNs = RemoveAllNamespaces(XElement.Parse(message));
            var xmlDocuWithNoNs = xmlWithNoNs.ToString();*/
            Console.WriteLine(message);
        }

    }
}
/*
 * using (HttpClient client = new HttpClient()) {
 
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        
                        using (HttpContent content = res.Content)
                        {
                            
                            var data = await content.ReadAsStringAsync();
                            
                            if (data != null)
                            {
                            
                                Console.WriteLine("data------------{0}", data);
                            }
                            else
                            {
                                Console.WriteLine("NO Data----------");
                            }
                        }
                    }
        }
*/