using System;

namespace AssemblyLineManager.AGV
{
    public partial class AGVClient
    {
        static void Main(string[] strings)
        {
            AssemblyLineManager.AGV.AGVClient agvClient = new AssemblyLineManager.AGV.AGVClient();
            string status = await agvClient.GetStatus();
            await agvClient.LoadProgram("MoveToChargerOperation");
            await agvClient.ExecuteProgram();
            Console.WriteLine(status);
        }
    }
}