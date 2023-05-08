using AssemblyLineManager.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.Core;

public partial class Core
{
    static readonly List<ICommunicationController> instances = new List<ICommunicationController>();

    public Core()
    {
        LoadModules();
        /*foreach (var instance in instances)
        {
            Console.WriteLine(instance.GetState().ToString());
            if (instance is ICommunicationControllerWithInventory)
            {
                Console.WriteLine("woohoo");
            }
        }*/
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
