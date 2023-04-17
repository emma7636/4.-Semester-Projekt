﻿using AssemblyLineManager.CommonLib;
using MQTTnet;

namespace AssemblyLineManager.AssemblyStation;

public partial class AssemblyStation : ICommunicationController
{
    private static Dictionary<int, string> stateLUT = new Dictionary<int, string>(); //Lookup table for the various states of the assembly station

    static AssemblyStation(){
        //Generate a lookup table for the various states
        stateLUT.Add(0, "Idle");
        stateLUT.Add(1, "Executing");
        stateLUT.Add(2, "Error");

        //Just shut up already
        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
    }

    public AssemblyStation()
    {
        ConnectToClient().Wait();
    }

    public KeyValuePair<int, string> GetState()
    {
        return new KeyValuePair<int, string>(latestStatus.State, stateLUT[latestStatus.State]);
    }

    public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
    {
        //Deleting old data so we don't get confused
        latestEcho = null;
        latestCheckHealth = null;

        if(command.Equals("emulator/operation")) //Checking what command they want us to send
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("Connection to assembly station lost!");
                return false;
            }

            SendCommand().Wait(); //Send the command

            while (_mqttClient.IsConnected) //Just in case we lose connection while waiting
            {
                DateTime time = DateTime.Now;
                while (latestCheckHealth == null) //Wait for the checkHealth packet to arrive
                {
                    if ((DateTime.Now - time).TotalSeconds > 12) //If we don't get a response in 12 seconds, something is wrong
                    {
                        Console.WriteLine("CheckHealth packet didn't arrive, is everything okay?\nContinuing...");
                        return false;
                    }
                    Thread.Sleep(100);
                }
                return latestCheckHealth.IsHealthy;
            }

            Console.WriteLine("Connection to assembly station lost!");
            return false;
        }
        else
        {
            throw new ArgumentException("This command doesn't exist"); //You made a spelling mistake fool
        }
    }

    ~AssemblyStation()
    {
        DisconnectFromClient().Wait();
    }

    public static void Main(string[] args)
    {
        AssemblyStation assemblyStation = new AssemblyStation();
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine();
            Console.WriteLine(assemblyStation.GetState());
            Console.WriteLine(assemblyStation.SendCommand("name", "emulator/operation"));
        }
    }
}