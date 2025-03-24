using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class UnlimitedGameState : IGameState
    {
        readonly AlchemyBook book = Game.Services.GetRequiredService<AlchemyBook>();
        private SortedSet<Substance> Inventory { get; set; } = new SortedSet<Substance>();
        public double RiskLevel { get; set; } = 0;
        public bool RecipeDiscovered { get; set; } = false; // End flag

        //Game.NewSubstance += OnGetNewSubstance(Substance sub);

        public void DisplayInventory()
        {
            Console.WriteLine("Инвентарь:");
            int c = 1;
            foreach (Substance substance in Game.Services.GetRequiredService<IInventory>().Substances)
            {
                Console.WriteLine($" {c}. {substance.Name}");
                c++;
            }
        }

        public Recipe Mix2(Substance sub1, Substance sub2)
        {
            HashSet<Substance> mixedSubs = [sub1, sub2];
            Recipe desiredRecipe = book.Recipes.FirstOrDefault(r => r.Components == mixedSubs);
            return desiredRecipe;
        }

        public void IsSuccess()
        {
            if (RecipeDiscovered) { }
        }
    }
}