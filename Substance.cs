using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Substance
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, double> Properties { get; set; }
        public List<string> RequiredTools { get; set; }

        public Substance(string name, string description)
        {
            Name = name;
            Description = description;
            Properties = new Dictionary<string, double>();
            RequiredTools = new List<string>();
        }

        public virtual void DisplayInfo() //Подробнее...
        {
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Описание: {Description}");
            Console.WriteLine("Свойства:");
            foreach (var property in Properties)
            {
                Console.WriteLine($"  - {property.Key}: {property.Value}");
            }
            if (RequiredTools.Any())
            {
                Console.WriteLine("Необходимые инструменты: " + string.Join(", ", RequiredTools));
            }
        }
    }

}