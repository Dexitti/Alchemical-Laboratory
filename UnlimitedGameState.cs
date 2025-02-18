using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Alchemical_Laboratory
{
    public class UnlimitedGameState : IGameState
    {
        public SortedSet<Substance> Inventory { get; set; } = new SortedSet<Substance>();
        public double RiskLevel { get; set; } = 0;
        public bool RecipeDiscovered { get; set; } = false; // End flag


        public void DisplayInventory()
        {
            Console.WriteLine("Инвентарь:");
            int c = 1;
            foreach (Substance substance in Game.Services.GetRequiredService<KnownSubstances>())
            {
                Console.WriteLine($" {c}. {substance.Name}");
                c++;
            }
        }

        public Substance Mix2(Substance sub1, Substance sub2)
        {
            if (!Inventory.Contains(sub1) || !Inventory.Contains(sub2))
            {
                throw new ArgumentException("Вещество отсутсвует!");
            }
            Recipe matchingRecipe = Game.Services.GetRequiredService<RecipesCollection>().FirstOrDefault(recipe =>
            recipe.Components.Count == 2 &&
            recipe.Components.Any(s => s.Equals(sub1)) &&
            recipe.Components.Any(s => s.Equals(sub2))
            );

            if (matchingRecipe != null)
            {
                return matchingRecipe.Result;
            }
            // else if (knownRecipes.Contains(matchingRecipe))
            //{
            //    Console.WriteLine("Рецепт уже изучен!");
            //}
            else
            {
                Console.WriteLine("Такого рецепта не существует!");
                return null;
            }
        }

        public void IsSuccess()
        {
            if (RecipeDiscovered) { }
        }
    }
}