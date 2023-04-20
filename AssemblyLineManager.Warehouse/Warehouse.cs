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
        Warehouse()
        {
            // Not sure if this was what the dictionary was meant to be used as.
            stateLUT = new Dictionary<int, string>();
            stateLUT.Add(0, "Idle");
            stateLUT.Add(1, "Executing");
            stateLUT.Add(2, "Error");
        }
        public static async Task<String> PickItem(int id) // Method that sends command via SOAP to warehouse for picking an item
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
        public static async Task<String> InsertItem(int id, string name) // Method that sends command via SOAP to warehouse for picking an item
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
        
        private static HttpClient client = new HttpClient();
        private static StringContent SetSC(string XmlFileName)
        {
            string pathToPost = path+@"\"+XmlFileName;
            string fileForSC = File.ReadAllText(pathToPost);
            StringContent sc = new StringContent(fileForSC, Encoding.UTF8, "application/xml");
            return sc;
        }
       
        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8081");
            client.DefaultRequestHeaders.Accept.Clear();
        }

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