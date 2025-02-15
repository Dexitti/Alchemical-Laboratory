using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class LimitedGameState
    {
        public List<KeyValuePair<Substance, int>> Inventory { get; set; } = new List<KeyValuePair<Substance, int>>();
        public double RiskLevel { get; set; } = 0;
        public bool RecipeDiscovered { get; set; } = false; // End flag

        public void DisplayInventory()
        {
            if (Inventory.Count == 0)
            {
                Console.WriteLine("Инвентарь пуст.");
                return;
            }

            Console.WriteLine("Инвентарь:");
            for (int i = 0; i < Inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Inventory[i].Key.Name}: {Inventory[i].Value}");
            }
        }
    }
}