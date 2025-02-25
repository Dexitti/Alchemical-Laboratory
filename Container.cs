using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Container
    {
        public string Type { get; set; } // круглодонная колба, коническая плоскодонная колба, ступка, стакан, склянка, банка, пробирка, чашка, коробка
        public string Material { get; set; } // тонкое стекло, каленное стекло, термостойкое стекло, эмалированное стекло, пластик, керамика
        public double Volume { get; set; } // 50<V<200

        public Container(string type, string material, double volume = 0)
        {
            Type = type;
            Material = material;
            Volume = volume;
        }

        public enum Types
        {
            round_bottom_flask = 0,
            erlenmeyer_flask,
            mortar,
            beaker,
            bottle, 
            jar,
            test_tube, 
            dish, 
            box
        }

        enum Materials
        {
            thin_glass = 0,
            tempered_glass,
            heat_resistant_glass,
            enameled_glass,
            plastic,
            ceramic
        }
    }
}