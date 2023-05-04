namespace AssemblyLineManager.CommonLib
{
    public interface ICommunicationController
    {
        public KeyValuePair<string, string> GetState();

        public bool SendCommand(string machineName, string command, string[]? commandParameters = null);
    }
}