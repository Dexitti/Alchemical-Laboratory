﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Alchemical_Laboratory.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace Alchemical_Laboratory
{
    public class Substance : ISubstance
    {
        private bool isDiscovered = false;

        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<Property, double> Characteristics { get; set; }
        public Tools RequiredTools { get; set; }

        public bool IsDiscovered
        {
            get => isDiscovered;
            set
            {
                if (isDiscovered == value)
                    return;
                isDiscovered = value;
                var result = Game.Services.GetRequiredService<AlchemyManager>().ResultSubstance;
                if (isDiscovered && (this == result || IsGem))
                {
                    GameState.CheckEnd();
                }
            }
        }

        public bool IsPrimary { get; set; }
        public bool IsFinal { get; set; }
        public bool IsGem { get; set; }

        public Substance(string name, string description = "...", Dictionary<Property, double>? properties = null, Tools tools = 0)
        {
            Name = name;
            Description = description;
            Characteristics = properties ?? [];
            RequiredTools = tools;
        }


        public virtual void DisplayInfo() //Подробнее...
        {
            Console.WriteLine($"{Resource.Name}: {Name}");
            Console.WriteLine($"{Resource.Description}: {Resource.ResourceManager.GetString(Description)}");
            if (Characteristics.Count > 0)
            {
                Console.WriteLine(Resource.Properties + ":");
                foreach (var property in Characteristics)
                {
                    Console.WriteLine($"  - {property.Key.Translate()}: {property.Value}");
                }
            }
            if (RequiredTools != 0)
            {
                Console.WriteLine($"{Resource.RequiredTools}: {string.Join(", ", RequiredTools)}");
            }
        }

        public override string ToString()
        {
            return Resource.ResourceManager.GetString(Name);
        }
    }

    [Flags]
    public enum Property
    {
        none = 0,
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
        none = 0,
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
        public Tools RequiredTools { get; }

        public void DisplayInfo();
    }
}