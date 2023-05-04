using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Author: ChatGPT
/// </summary>
public interface IAnimal
{
    void Speak();
}

public class Dog : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Woof!");
    }
}

public class Cat : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Meow!");
    }
}

class Program
{
    /*static void Main(string[] args)
    {/*
        // Create a list of objects that implement the IAnimal interface
        var animals = new List<IAnimal>();
        animals.Add(new Dog());
        animals.Add(new Cat());

        // Iterate over the list and call the Speak method on each object
        foreach (var animal in animals)
        {
            animal.Speak();
        }
        
        // Alternatively, you can use reflection to get a list of all available implementations of the interface
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(IAnimal).IsAssignableFrom(t) && t.IsClass);
        var instances = types.Select(t => Activator.CreateInstance(t)).OfType<IAnimal>();

        // Iterate over the list of instances and call the Speak method on each object
        foreach (var instance in instances)
        {
            instance.Speak();
        }
    }*/
}
