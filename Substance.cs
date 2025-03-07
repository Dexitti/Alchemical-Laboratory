using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Substance : ISubstance
    {
        public string Name { get; set; }
        public string Description { get; set; } // bla-bla - цвет?!, запах
        public Dictionary<string, double> Properties { get; set; } // температура, давление?!/твердость, летучесть, вязкость, токсичность, горючесть, радиоактивность
        //В событии: совместить свойства вещества с емкостью (как: обертка над enum или метод проверок)
        public EnumExtension.Tools[] RequiredTools { get; set; }

        public Substance(string name, string description = "описание...", Dictionary<string, double> properties = null, EnumExtension.Tools[] tools = null)
        {
            Name = name;
            Description = description;
            Properties = new Dictionary<string, double>();
            Properties = properties;
            RequiredTools = tools;
        }


        public virtual void DisplayInfo() //Подробнее...
        {
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Описание: {Description}");
            if (Properties.Count > 0)
            {
                Console.WriteLine("Свойства:");
                foreach (var property in Properties)
                {
                    Console.WriteLine($"  - {property.Key}: {property.Value}");
                }
            }
            if (RequiredTools.Any())
            {
                Console.WriteLine("Необходимые инструменты: " + string.Join(", ", RequiredTools));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface ISubstance
    {
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, double> Properties { get; }
        public EnumExtension.Tools[] RequiredTools { get; }

        public void DisplayInfo();
    }
}