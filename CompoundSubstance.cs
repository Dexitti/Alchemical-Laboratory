using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Alchemical_Laboratory
{
    public class CompoundSubstance : Substance
    {
        public Dictionary<Substance, double> Components { get; set; }
        public string Recipe { get; set; }

        public CompoundSubstance(string name, string description) : base(name, description)
        {
            Components = new Dictionary<Substance, double>();
            var recipe = Game.Services.GetRequiredService<RecipesCollection>().Keys.FirstOrDefault(r => r == name);
            Recipe = $"Рецепт: {recipe}";
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine("Состав:");
            foreach (var component in Components)
            {
                Console.WriteLine($"  - {component.Key.Name}: {component.Value}");
            }
            Console.WriteLine($"Рецепт: {Recipe}");
        }
    }

}