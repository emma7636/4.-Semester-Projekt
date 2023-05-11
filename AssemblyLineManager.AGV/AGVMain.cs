namespace AssemblyLineManager.AGV
{ 


class Program
    {
        static async Task Main(string[] args)
        {
            AGVClient agvClient = new AGVClient();
         
            // Load a program on the AGV
            string programName = "MoveToAssemblyOperation";
            Console.WriteLine($"Loading program: {programName}");
            string loadProgramResult = agvClient.LoadProgram(programName);
            Console.WriteLine("Load Program Result:");
            Console.WriteLine(loadProgramResult);
       
            Console.WriteLine(agvClient.GetState());
        }

    }

}

        