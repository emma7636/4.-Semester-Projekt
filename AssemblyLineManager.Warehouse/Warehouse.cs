using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System;
using System.Text;
using System.Xml;

namespace AssemblyLineManager.Warehouse
{
    public class Warehouse
    {
        //String URL = "http://localhost:8081/Service.asmx";

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
        private JsonArray? GetInventory() {

            return null;
        }
        
       private static HttpClient client = new HttpClient();
       private static string pathToPost = @"C:\\Users\\kimje\\OneDrive\\Documents\\GitKraken\\4.-Semester-Projekt\\AssemblyLineManager.Warehouse\\Post.xml";
       private static string fileForSC = File.ReadAllText(@pathToPost);
       private static StringContent sc = new StringContent(fileForSC, Encoding.UTF8, "application/xml");
      

        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8081");
            client.DefaultRequestHeaders.Accept.Clear();
        }

        public static async Task<string> GetInventoryAsync() {

            string getRequest = "";
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", sc);
            if (response.IsSuccessStatusCode)
            {
                getRequest = await response.Content.ReadAsStringAsync();
            }
            return getRequest;
        }

        public static void rewriteXML(int trayID)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\kimje\OneDrive\Documents\GitKraken\4.-Semester-Projekt\AssemblyLineManager.Warehouse\PickItem.xml");
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("m", "http://tempuri.org/");
            XmlNode? trayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:PickItem/m:trayId", nsmgr);
            if (trayIdNode?.Attributes != null)
            {
                string real = trayID.ToString();
                trayIdNode.InnerText=real; // Update the id attribute value
                Console.WriteLine(trayIdNode.OuterXml);
                
            }

            doc.Save(@"C:\Users\kimje\OneDrive\Documents\GitKraken\4.-Semester-Projekt\AssemblyLineManager.Warehouse\PickItem.xml");
        }

    }
}