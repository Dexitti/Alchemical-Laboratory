using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Alchemical_Laboratory.Container;

namespace Alchemical_Laboratory
{
    public class Container
    {
        public Types Type { get; set; }
        public Materials Material { get; set; }
        public double Volume { get; set; }

        /// <param name="type">is chosen from enum</param>
        /// <param name="material">is chosen from enum</param>
        /// <param name="volume">is chosen between 0 and 20</param>
        public Container(Types type, Materials material, double volume = 0)
        {
            Type = type;
            Material = material;
            Volume = Math.Clamp(volume, 0, 20);
        }
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
        ceramic = 32
    }
}