using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;
using Sharprompt;
using static Alchemical_Laboratory.Game;

namespace Alchemical_Laboratory
{
    public class LimitedGameState : GameState
    {
        public LimitedGameState(AlchemyBook book, IInventory inventory, AlchemyManager manager) : base(book, inventory, manager)
        {
            inventory.NewSubstance += CheckUpgrade;
        }

        void CheckUpgrade(Substance sub)
        {
            // Upgrades inventory, increases resources income, contains checks for adding action and quest
            if (!sub.IsDiscovered) return; // Для определения вызова после главного обработчика в GameState?!
            if (Book.Substances.Count(s => s.IsDiscovered) == 12)
            {
                Console.Clear();
                //Console.WriteLine("\n" + Resource);
                Extensions.MakeDelay(2000);
            }
            // Inventory.Income += Book.Substances.Count(s => s.IsDiscovered) / 15;
        }
    }
}