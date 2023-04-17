namespace AssemblyLineManager.CommonLib
{
    public interface ICommunicationController
    {
        public KeyValuePair<int, string> GetState();

        public bool SendCommand(string machineName, string command, string[]? commandParameters = null);
    }
}