using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class Combination
    {
        public Types Type { get; set; }
        public Materials Material { get; set; }
        public Dictionary<Property, double> Resistances { get; set; }

        public Combination(Types type, Materials material, Dictionary<Property, double> characteristics)
        {
            Type = type;
            Material = material;
            Resistances = characteristics;
        }

        public static readonly Dictionary<Property, (string positive, string negative)> PropertyMessages = new()
        {
            [Property.temperature] =
            (
                Resource.Temperature_Positive,
                Resource.Temperature_Negative
            ),
            [Property.hardness] =
            (
                Resource.Hardness_Positive,
                Resource.Hardness_Negative
            ),
            [Property.volatility] =
            (
                Resource.Volatility_Positive,
                Resource.Volatility_Negative
            ),
            [Property.viscosity] =
            (
                Resource.Viscosity_Positive,
                Resource.Viscosity_Negative
            ),
            [Property.toxicity] =
            (
                Resource.Toxicity_Positive,
                Resource.Toxicity_Negative
            ),
            [Property.flammability] =
            (
                Resource.Flammability_Positive,
                Resource.Flammability_Negative
            ),
            [Property.explosiveness] =
            (
                Resource.Explosiveness_Positive,
                Resource.Explosiveness_Negative
            ),
            [Property.radioactivity] =
            (
                Resource.Radioactivity_Positive,
                Resource.Radioactivity_Negative
            )
        };
    }

    [Flags]
    public enum Types
    {
        roundBottomFlask = 1,
        erlenmeyerFlask = 2,
        mortar = 4,
        beaker = 8,
        bottle = 16,
        jar = 32,
        testTube = 64,
        dish = 128,
        box = 256
    }

    [Flags]
    public enum Materials
    {
        thinGlass = 1,
        temperedGlass = 2,
        heatResistantGlass = 4,
        enameledGlass = 8,
        plastic = 16,
        ceramic = 32,
        wood = 64
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
}