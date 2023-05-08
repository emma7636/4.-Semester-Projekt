namespace AssemblyLineManager.AGV
{ 


class Program
    {
        static async Task Main(string[] args)
        {
            AGVClient agvClient = new AGVClient();
            /*
            // Get the AGV status
            string status = await agvClient.GetStatus();
            Console.WriteLine("Current AGV Status:");
            Console.WriteLine(status);
            */
            // Load a program on the AGV
            string programName = "MoveToAssemblyOperation";
            Console.WriteLine($"Loading program: {programName}");
            string loadProgramResult = agvClient.LoadProgram(programName);
            Console.WriteLine("Load Program Result:");
            Console.WriteLine(loadProgramResult);
            /*
            // Execute the loaded program
            Console.WriteLine("Executing program");
            Console.WriteLine("Execute Program Result:");
            string executeProgramResult = await agvClient.ExecuteProgram();
            Console.WriteLine(executeProgramResult);
            */
            Console.WriteLine(agvClient.GetState());
        }

    }

}

        