using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace AssemblyLineManager.CommonLib;

/// <summary>
/// This interface is used to allow any person to create a module that can interface with the AssemblyLineManager Core
/// This interface has two methods, GetState and SendCommand.
/// It is intended that each module only communicates with a single machine, but it's not like we'll stop you if you don't like that.
/// This version of the interface is for machines that have an extensive inventory
/// </summary>
public interface ICommunicationControllerWithInventory : ICommunicationController
{
    /// <summary>
    /// This method is used to get the current inventory of the machine utilised by the module.
    /// The inventory information shall be formatted as a list of information with each entry having a numeric id and a name.
    /// </summary>
    /// <returns>It shall return a dictionary, where the key is the name of the entry, and the value is the value of the entry.</returns>
    public Dictionary<int, string> GetInventory();
}