using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System;

namespace AssemblyLineManager.CommonLib;

/// <summary>
/// This interface is used to allow any person to create a module that can interface with the AssemblyLineManager Core
/// This interface has two methods, GetState and SendCommand.
/// It is intended that each module only communicates with a single machine, but it's not like we'll stop you if you don't like that.
/// </summary>
public interface ICommunicationController
{
    /// <summary>
    /// This property is used to get the name of the module so each module can be individually referenced.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// This method is used to get the current state of the machine utilised by the module.
    /// The state information shall be formatted as a list of information with each entry having a name and a value.
    /// </summary>
    /// <returns>It shall return a dictionary, where the key is the name of the entry, and the value is the value of the entry.</returns>
    public Dictionary<string, string> GetState();

    /// <summary>
    /// This method is used to send a command to the machine utilised by the module.
    /// Please refer to the documentation of the module to see what commands are available and what parameters they require.
    /// </summary>
    /// <param name="machineName">machineName is the name of the machine that the command is to be sent to.</param>
    /// <param name="command">command is the name of the command to be sent.</param>
    /// <param name="commandParameters">The commandParameters is an array of strings that the module can use however it wants to receive additional information to sent along with the command.</param>
    /// <returns>Returns a boolean describing success or failure of the operation attempted</returns>
    public bool SendCommand(string command, string[]? commandParameters = null);
}