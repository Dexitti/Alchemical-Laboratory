using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Alchemical_Laboratory
{
    public class Recipe
    {
        public Substance Result { get; set; }
        public HashSet<Substance> Components { get; set; }
        public List<Substance> AuxiliaryResults { get; set; }

        public bool Advanced { get; set; }

        public Recipe(Substance result, HashSet<Substance> components, List<Substance>? auxiliaryResults = null, bool advanced = false)
        {
            Result = result;
            Components = components;
            AuxiliaryResults = auxiliaryResults ?? [];
            Advanced = advanced;
        }

        public override string ToString()
        {
            if (AuxiliaryResults.Count > 0)
                return $"{Result} + {string.Join(" + ", AuxiliaryResults)} = {string.Join(" + ", Components)}";
            else
                return $"{Result} = {string.Join(" + ", Components)}";
        }
    }
}