namespace AssemblyLineManager.Tests;

internal class AssemblyStationTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    AssemblyStation.AssemblyStation assemblyStation;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void Setup()
    {
        //Provides us with a fresh instance for each test
        Console.WriteLine("Preparing AssemblyStation tests");
        assemblyStation = new AssemblyStation.AssemblyStation();
    }

    [Test]
    public void AssemblyStationGetStatusTest()
    {
        //Tests if the State gets returned correctly and changed to it get recorded properly as well
        {
            assemblyStation.SendCommand("name", "emulator/operation", new string[] { "123" });
            KeyValuePair<string, string>[] test = assemblyStation.GetState();
            Console.WriteLine(test.ToString());
            Assert.That(test, Is.All.Not.Null);
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "123")));
        }

        {
            assemblyStation.SendCommand("name", "emulator/operation", new string[] { "456" });
            KeyValuePair<string, string>[] test = assemblyStation.GetState();
            Console.WriteLine(test.ToString());
            Assert.That(test, Is.All.Not.Null);
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("lastOperation", "123")));
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "456")));
        }

        {
            assemblyStation.SendCommand("name", "emulator/operation", new string[] { "789" });
            KeyValuePair<string, string>[] test = assemblyStation.GetState();
            Console.WriteLine(test.ToString());
            Assert.That(test, Is.All.Not.Null);
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("lastOperation", "456")));
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "789")));
        }
    }

    [Test]
    public void AssemblyStationSendCommandDefaultTest()
    {
        //Tests the command if we give it the default parameters
        Assert.That(assemblyStation.SendCommand("name", "emulator/operation"), Is.Not.False);
        Assert.That(assemblyStation.GetState(), Contains.Item(new KeyValuePair<string, string>("currentOperation", "12345")));
    }

    [Test]
    public void AssemblyStationSendCommandSpecifyIDTest()
    {
        //Tests the command if we give custom parameters
        Assert.That(assemblyStation.SendCommand("name", "emulator/operation", new string[] { "789" }), Is.Not.False);
        Assert.That(assemblyStation.GetState(), Contains.Item(new KeyValuePair<string, string>("currentOperation", "789")));
    }

    [Test]
    public void AssemblyStationSendCommandSpecifyFalseIDTest()
    {
        //Tests the command if we give unacceptable parameters
        Assert.Throws<ArgumentException>(() => assemblyStation.SendCommand("name", "emulator/operation", new string[] { "sdasfg" }), "Please insert numerical values only for processID");
    }

    [Test]
    public void AssemblyStationSendWrongCommandTest()
    {
        //Tests the command if we give a non-existent command
        Assert.Throws<ArgumentException>(() => assemblyStation.SendCommand("name", "emulator/operator", new string[] { "789" }), "This command doesn't exist");
    }

    [Test]
    public void AssemblyStationSendCommandHealthFailureTest()
    {
        //Tests the command if the assembly station reports a failed product
        Assert.That(assemblyStation.SendCommand("name", "emulator/operation", new string[] { "9999" }), Is.Not.True);
    }
}
