using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyLineManager.CommonLib;

namespace AssemblyLineManager.Core;

internal class MAINPLEACEWORK
{
    public static void Main(String[] args)
    {
        // Alternatively, you can use reflection to get a list of all available implementations of the interface
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(ICommunicationController).IsAssignableFrom(t) && t.IsClass);
        var instances = types.Select(t => Activator.CreateInstance(t)).OfType<ICommunicationController>();

        // Iterate over the list of instances and call the Speak method on each object
        foreach (var instance in instances)
        {
            instance.GetState();
        }
    }
}
