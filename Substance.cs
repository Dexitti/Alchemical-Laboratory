using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class Substance : ISubstance
    {
        public string Name { get; set; }
        public string Description { get; set; } // bla-bla - цвет, запах
        public Dictionary<Property, double> Characteristics { get; set; }
        //В событии: совместить свойства вещества с емкостью (как: обертка над enum или метод проверок)
        public Tools[] RequiredTools { get; set; }
        // Событие: выбор необходимых инструментов

        [JsonIgnore]
        public bool IsDiscovered { get; set; }

        public Substance(string name, string description = "описание...", Dictionary<Property, double>? properties = null, Tools[]? tools = null)
        {
            Name = name;
            Description = description;
            Characteristics = properties ?? [];
            RequiredTools = tools ?? [];
            IsDiscovered = false;
        }


        public virtual void DisplayInfo() //Подробнее...
        {
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Описание: {Description}");
            if (Characteristics.Count > 0)
            {
                Console.WriteLine("Свойства:");
                foreach (var property in Characteristics)
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

    [Flags]
    public enum Property
    {
        temperature = 1,
        hardness = 2,
        volatility = 4,
        viscosity = 8,
        toxicity = 16,
        flammability = 32,
        explosiveness = 64,
        radioactivity = 128
    }

    [Flags]
    public enum Tools
    {
        gloves = 1,
        hood = 2,
        glasses = 4,
        respirator = 8
    }

    public interface ISubstance
    {
        public string Name { get; }
        public string Description { get; }
        public Dictionary<Property, double> Characteristics { get; }
        public Tools[] RequiredTools { get; }

        public void DisplayInfo();
    }
}