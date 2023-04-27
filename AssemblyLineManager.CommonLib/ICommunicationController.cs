namespace AssemblyLineManager.CommonLib
{
    /**
     * This interface is used to allow any person to create a module that can interface with the AssemblyLineManager Core.
     * This interface has two methods, GetState and SendCommand.
     * It is intended that each module only communicates with a single machine, but it's not like we'll stop you if you don't like it.
     */
    public interface ICommunicationController
    {
        /**
         * This method is used to get the current state of the machine utilised by the module.
         * The state information shall be formatted as a list of information with each entry having a name and a value.
         * It shall return an array of key-value pairs, where the key is the name of the entry, and the value is the value of the entry.
         */
        public KeyValuePair<string, string>[] GetState();

        /**
         * This method is used to send a command to the machine utilised by the module.
         * The machineName is the name of the machine that the command is to be sent to.
         * The command is the name of the command to be sent.
         * The commandParameters is an array of strings that the module can use however it wants to receive additional information to sent along with the command.
         * Please refer to the documentation of the module to see what commands are available and what parameters they require.
         */
        public bool SendCommand(string machineName, string command, string[]? commandParameters = null);
    }
}