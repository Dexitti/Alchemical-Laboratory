using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class RecipesCollection : ReadOnlyCollection<Recipe>
    {
        public RecipesCollection() : base(GetAllRecipes())
        {
        }

        private static IList<Recipe> GetAllRecipes()
        {
            List<Recipe> list = new List<Recipe>();
            return list;
        }
    }
}