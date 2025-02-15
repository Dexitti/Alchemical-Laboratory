using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Container
    {
        public string Type { get; set; } // круглая колба, колба, склянка, пробирка, коробка
        public string Material { get; set; } // стекло, каленное (термостойкое) стекло, пластик
        public double Volume { get; set; } // 50<V<200

        public Container(string type, string material, double volume)
        {
            Type = type;
            Material = material;
            Volume = volume;
        }
    }

    enum Types
    {

    }
}