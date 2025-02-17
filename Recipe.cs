using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Recipe
    {
        public Substance Result { get; set; }
        public List<Substance> Components { get; set; }

        public Recipe(Substance result, List<Substance> components)
        {
            Result = result;
            Components = components;
        }

        public override string ToString()
        {
            return $"{Result} = {string.Join(" + ", Components)}";
        }
    }
}