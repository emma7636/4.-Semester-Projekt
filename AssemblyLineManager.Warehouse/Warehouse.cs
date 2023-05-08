using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System;
using System.Text;
using System.Xml;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections;
using AssemblyLineManager.CommonLib;
using System.Reflection.Metadata;

namespace AssemblyLineManager.Warehouse
{
    public class Item
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Item (int id, string content)
        {
            this.Id = id;
            this.Content = content;
        }
        public override string ToString()
        {
            return "Id: " + Id + " Content: " + Content;
        }
    }
    public class Warehouse : ICommunicationControllerWithInventory
    {
        //String URL = "http://localhost:8081/Service.asmx";

        static string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static HttpClient client = new HttpClient();
        private static bool connected = false;
        public Warehouse()
        {
            RunAsync().Wait();
        }
        public static bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }
        /**
         * Sends a SOAP xml file to the warehouse and picks an item on the shelf with the same id
         * 
         * @param int id
         * 
         * @return Task<String>
         */
        public static async Task<string> PickItem(int id)
        {
            string xmlName = "PickItem.xml";
            RewriteXML(id, null, xmlName);
            string postRequest = "";
            string result;
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC(xmlName));
            if (response.IsSuccessStatusCode)
            {
                postRequest = await response.Content.ReadAsStringAsync();
                result = "Picked item with id: " + id;
                return result;
            }
            else
            {
                result = "Failed to pick item";
                return result;
            }
        }
        /**
         * Sends a SOAP xml file to the warehouse and inserts an item on the shelf with the same id
         * 
         * @param int id
         * @param string name
         * 
         * @return Task<String>
         */
        public static async Task<string> InsertItem(int id, string name)
        {
            string xmlName = "InsertItem.xml";
            RewriteXML(id, name, xmlName);
            string postRequest = "";
            string result;
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC(xmlName));
            bool failChecker = InsertChecker(response);
            if (failChecker)
            {
                postRequest = await response.Content.ReadAsStringAsync();
                result = "Inserted item " + name + " on " + id;
                return result;
            }
            else
            {
                result = "Failed to insert item";
                return result;
            }

        }
        private static bool InsertChecker(HttpResponseMessage response)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(response.Content.ReadAsStream());
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNode? trayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body", nsmgr);
            if (trayIdNode?.InnerText != null)
            {
                string innerText;
                innerText = trayIdNode.InnerText;
                string fail = "Operation could not be handled. Check inventory status.";
                if (innerText == fail)
                {
                    return false;
                }

            }

            return true;
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
            string pathToPost = path + @"\" + XmlFileName;
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
            Warehouse.Connected = true;

        }

        /**
         * Sends a SOAP XML command to the machine and gets back the inventory of the system
         * 
         * @return Task<string>
         */
        public static async Task<string> GetInventoryWithStatus()
        {

            string getRequest = "";
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC("GetInventory.xml"));
            if (response.IsSuccessStatusCode)
            {
                getRequest = await response.Content.ReadAsStringAsync();
            }
            return getRequest;
        }

        /**
         *  Rewrites the SOAP XML files and saves them at their location. Made generic for rewriting both insert and pick item
         *  
         *  @param int trayId
         *  @param string? name
         *  @param string xmlName
         *  
         *  @return void
         */
        private static void RewriteXML(int trayId, string? name, string xmlName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path + @"\" + xmlName);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("m", "http://tempuri.org/");
            XmlNode? pickTrayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:PickItem/m:trayId", nsmgr);
            XmlNode? insertTrayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:InsertItem/m:trayId", nsmgr);
            XmlNode? nameNode = doc.SelectSingleNode("//s:Envelope/s:Body/m:InsertItem/m:name", nsmgr);
            if (pickTrayIdNode?.InnerText != null)
            {
                string real = trayId.ToString();
                pickTrayIdNode.InnerText = real; // Update the id attribute value
                Console.WriteLine(pickTrayIdNode.OuterXml);
            }
            if (insertTrayIdNode?.InnerText != null && name != null && nameNode?.InnerText != null)
            {
                string real = trayId.ToString();
                insertTrayIdNode.InnerText = real;
                nameNode.InnerText = name;
                Console.WriteLine(insertTrayIdNode.OuterXml);
                Console.WriteLine(nameNode.OuterXml);
            }

            doc.Save(path + @"\" + xmlName);
        }
        /**
         * Sends the GetInventory post command to the server
         * 
         * @return Task<HttpResponseMessage>
         */
        private static async Task<HttpResponseMessage> SendGetInventory()
        {
            HttpResponseMessage response = await client.PostAsync("/Service.asmx", SetSC("GetInventory.xml"));
            return response;
        }
        /**
         * Gets the list of items in the inventory, makes them into Item objects and creates a List of them
         * 
         * @return ArrayList
         */
        private static ArrayList GetInventoryList()
        {
            dynamic dyna = IterateInventory();
            JArray? array = (JArray)dyna["Inventory"];
            ArrayList ItemList = new ArrayList();
            foreach (JObject obj in array.Cast<JObject>())
            {
                int? id = (int?)obj["Id"];
                string? content = (string?)obj["Content"];
                if (content != null && id != null)
                {
                    Item newItem = new Item(id.Value, content);
                    ItemList.Add(newItem);
                }
            }
            return ItemList;
        }

        /**
         * Gets a specific item from the inventory, returns an error item if there's an error
         * 
         * @params int id
         * @return Item
         */
        public static Item GetInventoryItem(int id)
        {
            id--;
            ArrayList ItemList = GetInventoryList();
            Item? item;
            Item defaultItem = new Item(11, "Error");
            if (ItemList[id] != null)
            {
                item = (Item?)ItemList[id];
                if (item != null)
                {
                    return item;
                }
            }
            return defaultItem;
        }
        /**
         * Shows how many items in the inventory
         * 
         * @return int
         */
        public static int GetInventoryCount()
        {
            ArrayList list = GetInventoryList();
            return list.Count;
        }

        /**
         * Creates a KeyValuePair array for the inventory so it can be sent to the front end.
         * 
         * @return KeyValuePair<int, string>
         */
        public KeyValuePair<int, string>[] GetInventory()
        {
            ArrayList list = GetInventoryList();
            List<KeyValuePair<int, string>> inventory = new List<KeyValuePair<int, string>>();
            foreach (Item item in list)
            {
                inventory.Add(new KeyValuePair<int, string>(item.Id, item.Content));
            }
            //KeyValuePair<int, string>[] newInventory = inventory.ToArray();

            return inventory.ToArray();
        }
        /**
         * Creates a KeyValuePair array for the state of the machine so it can be sent to the front end
         * 
         * @return KeyValuePair<string, string>
         */
        public KeyValuePair<string, string>[] GetState()
        {
            dynamic dyna = IterateInventory();
            int state = (int)dyna["State"];
            List<KeyValuePair<string, string>> stateList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("State: ", state.ToString())
            };
            return stateList.ToArray();
        }
        /**
         * Sends commands to the machine from the front end
         * 
         * @param string machineName, string command, string[]? commandParameters = null
         * @return bool
         */
        public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
        {
            if (commandParameters != null) {
                int itemId = Int32.Parse(commandParameters[0]);
                if (command == "Pick Item")
                {
                   PickItem(itemId).Wait();
                   return true;
                    
                }
                else if (command == "Insert Item")
                {
                    string itemName = commandParameters[1];
                    InsertItem(itemId,itemName).Wait();
                    return true;
                }
            }
            return false;
        }

        /**
         * Iterates through the inventory SOAP XML to find specific values
         * 
         * @return dynamic
         */
        private static dynamic IterateInventory()
        {
            HttpResponseMessage response = SendGetInventory().Result;
            XmlDocument doc = new XmlDocument();
            doc.Load(response.Content.ReadAsStream());
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNode? trayIdNode = doc.SelectSingleNode("//s:Envelope/s:Body", nsmgr);
            if (trayIdNode?.InnerText != null)
            {
                string innerText;
                innerText = trayIdNode.InnerText;
                dynamic? dyna = JObject.Parse(innerText);
                return dyna;
            }
            return "";
        }
    }
}