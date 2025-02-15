using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class UnlimitedGameState
    {
        public SortedSet<Substance> Inventory { get; set; } = new SortedSet<Substance>();
        public double RiskLevel { get; set; } = 0;
        public bool RecipeDiscovered { get; set; } = false; // End flag

        public Substance Mix2(Substance sub1, Substance sub2)

        public void DisplayInventory()
        {
            Console.WriteLine("Инвентарь:");
            int c = 1;
            foreach (Substance substance in Inventory)
            {
                Console.WriteLine($" {c}. {substance.Name}");
                c++;
            }
        }

        public void IsSuccess()
        {
            if (RecipeDiscovered) { }
        }
    }
}