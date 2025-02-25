using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Alchemical_Laboratory
{
    public class RecipesCollection : ReadOnlyDictionary<string, Recipe>
    {
        public RecipesCollection() : base(GetAllRecipes())
        {
        }

        static IDictionary<string, Recipe> GetAllRecipes()
        {
            var allSubs = Game.Services.GetRequiredService<SubstancesCollection>();
            Dictionary<string, Recipe> dict = new()
            {
                { "lava", new Recipe((CompoundSubstance)allSubs["lava"], new List<Substance>() {allSubs["fire"], allSubs["earth"] }) },

            };

            return dict;
        }
    }
}