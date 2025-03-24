using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Alchemical_Laboratory
{
    public interface IGameState
    {
        public double RiskLevel { get; set; }
        public bool RecipeDiscovered { get; set; }

        public void DisplayInventory();

        public Recipe Mix2(Substance sub1, Substance sub2);

        public Recipe GetRecipeForHint()
        {
            IEnumerable<Substance> subsList = Game.Services.GetRequiredService<IInventory>().Substances;
            List<Recipe> allRecipes = Game.Services.GetRequiredService<AlchemyBook>().Recipes;

            List<Recipe> accessibleRecipes = allRecipes
                .Where(r => r.Components.Any(sub => subsList.Contains(sub)) && !r.IsDiscovered).ToList();

            Random rand = new Random();
            int randSubIndex = rand.Next(accessibleRecipes.Count);
            return accessibleRecipes[randSubIndex];
        }

        public void IsSuccess();
    }
}