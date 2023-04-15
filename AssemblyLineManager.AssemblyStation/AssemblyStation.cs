using AssemblyLineManager.CommonLib;

namespace AssemblyLineManager.AssemblyStation
{
    public partial class AssemblyStation : ICommunicationController
    {
        private Task Task { get; set; }
        public AssemblyStation()
        {
            Task = ConnectToClient(); //Assigning it to a variable so that it keeps running in the background since constructors are not async
        }

        public KeyValuePair<int, string> GetState()
        {
            throw new NotImplementedException();
        }

        public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
        {
            throw new NotImplementedException();
        }

        ~AssemblyStation()
        {
            Task = DisconnectFromClient();
        }
    }
}