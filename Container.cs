using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Alchemical_Laboratory.Properties;
using static Alchemical_Laboratory.Container;

namespace Alchemical_Laboratory
{
    public struct Container
    {
        public Types Type { get; set; }
        public Materials Material { get; set; }

        public Container(Types type, Materials material)
        {
            Type = type;
            Material = material;
            // Volume = Math.Clamp(volume, 0, 20);
        }

        public override string ToString()
        {
            return Resource.ResourceManager.GetString(Type.ToString());
        }
    }
}