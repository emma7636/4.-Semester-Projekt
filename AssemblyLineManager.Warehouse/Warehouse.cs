using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System;
using System.Text;
using System.Xml;
using System.Reflection;

namespace AssemblyLineManager.Warehouse
{
    public class Warehouse
    {
        //String URL = "http://localhost:8081/Service.asmx";

        private Dictionary<int, string> stateLUT;
        static string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static HttpClient client = new HttpClient();
        Warehouse()
        {
            // Not sure if this was what the dictionary was meant to be used as.
            stateLUT = new Dictionary<int, string>();
            stateLUT.Add(0, "Idle");
            stateLUT.Add(1, "Executing");
            stateLUT.Add(2, "Error");
        }
        /**
         * Sends a SOAP xml file to the warehouse and picks an item on the shelf with the same id
         * 
         * @param int id
         * 
         * @return Task<String>
         */
        public static async Task<String> PickItem(int id)
        {
            string xmlName = "PickItem.xml";
            RewriteXML(id, null, xmlName);
            string postRequest = "";
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC(xmlName));
            if (response.IsSuccessStatusCode)
            {
                postRequest = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine(postRequest);
            return postRequest;
        }
        /**
         * Sends a SOAP xml file to the warehouse and inserts an item on the shelf with the same id
         * 
         * @param int id
         * @param string name
         * 
         * @return Task<String>
         */
        public static async Task<String> InsertItem(int id, string name)
        {
            string xmlName = "PickItem.xml";
            RewriteXML(id, name, xmlName);
            string postRequest = "";
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC(xmlName));
            if (response.IsSuccessStatusCode)
            {
                postRequest = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine(postRequest);
            return postRequest;
        }
        
        /**
         * Creates a StringContent object with the text of an xml file
         *  
         * @param string XmlFileName
         * 
         * @return StringContent sc
         */
        private static StringContent SetSC(string XmlFileName)
        {
            string pathToPost = path+@"\"+XmlFileName;
            string fileForSC = File.ReadAllText(pathToPost);
            StringContent sc = new StringContent(fileForSC, Encoding.UTF8, "application/xml");
            return sc;
        }
        

        /**
         * Connects to the simulation of the assembly line
         */
        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8081");
            client.DefaultRequestHeaders.Accept.Clear();
        }

        /**
         * Sends a SOAP XML command to the machine and gets back the inventory of the system
         * 
         * @return Task<string>
         */
        public static async Task<string> GetInventoryAsync() {

            string getRequest = "";
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC("GetInventory.xml"));
            if (response.IsSuccessStatusCode)
            {
                getRequest = await response.Content.ReadAsStringAsync();
            }
            Console.WriteLine(getRequest);
            return getRequest;
        }
        
        /**
         *  Rewrites the SOAP XML files and saves them at their location. Made generic for rewriting both insert and pick item
         *  
         *  @param int trayId
         *  @param string? name
         *  @param string xmlName
         *  
         */

        private static void RewriteXML(int trayId, string? name, string xmlName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path+@"\"+xmlName);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("m", "http://tempuri.org/");
            XmlNode? pickTrayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:PickItem/m:trayId", nsmgr);
            XmlNode? insertTrayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:InsertItem/m:trayId", nsmgr);
            XmlNode? nameNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:InsertItem/m:name", nsmgr);
            if (pickTrayIdNode?.InnerText != null)
            {
                string real = trayId.ToString();
                pickTrayIdNode.InnerText=real; // Update the id attribute value
                Console.WriteLine(pickTrayIdNode.OuterXml);
            }
            if (insertTrayIdNode?.InnerText != null && name !=null && nameNode?.InnerText !=null)
            {
                string real = trayId.ToString();
                insertTrayIdNode.InnerText=real;
                nameNode.InnerText = name;
                Console.WriteLine(insertTrayIdNode.OuterXml);
                Console.WriteLine(nameNode.OuterXml);
                
            }

            doc.Save(path+@"\"+xmlName);
        }

    }
}