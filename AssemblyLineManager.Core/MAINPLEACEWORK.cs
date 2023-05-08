using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyLineManager.CommonLib;
using System.Reflection;

namespace AssemblyLineManager.Core;

internal class MAINPLEACEWORK
{
    static List<ICommunicationController> instances = new List<ICommunicationController>();
    public static void Main(String[] args)
    {
        LoadModules();

        foreach (var instance in instances)
        {
            Console.WriteLine(instance.ToString());
            foreach (var thing in instance.GetState())
            {
                Console.WriteLine(thing.ToString());
            }
        }
    }

    static void LoadModules()
    {
        Assembly.LoadFrom(@"AssemblyLineManager.AGV.dll");
        Assembly.LoadFrom(@"AssemblyLineManager.AssemblyStation.dll");
        Assembly.LoadFrom(@"AssemblyLineManager.Warehouse.dll");
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(ICommunicationController).IsAssignableFrom(t) && t.IsClass);
        instances.AddRange(types.Select(t => Activator.CreateInstance(t)).OfType<ICommunicationController>());
    }
}
