using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Alchemical_Laboratory.Container;

namespace Alchemical_Laboratory
{
    public class Container
    {
        public EnumExtension.Types Type { get; set; }
        public EnumExtension.Materials Material { get; set; }
        public double Volume { get; set; }

        /// <param name="type">is chosen from enum</param>
        /// <param name="material">is chosen from enum</param>
        /// <param name="volume">is chosen between 50 and 200</param>
        public Container(EnumExtension.Types type, EnumExtension.Materials material, double volume = 0)
        {
            Type = type;
            Material = material;
            Volume = Math.Clamp(volume, 50, 200);
        }
    }

}