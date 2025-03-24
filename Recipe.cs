using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Recipe
    {
        public Substance Result { get; set; }
        public HashSet<Substance> Components { get; set; }
        public List<Substance> AuxiliaryResults { get; set; }

        public bool IsDiscovered { get; set; }

        public Recipe(Substance result, HashSet<Substance> components, List<Substance>? auxiliaryResults = null)
        {
            Result = result;
            Components = components;
            AuxiliaryResults = auxiliaryResults ?? [];
            IsDiscovered = false;
        }

        public override string ToString()
        {
            return $"{Result} = {string.Join(" + ", Components)}";
        }
    }
}