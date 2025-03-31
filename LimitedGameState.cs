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
        public LimitedGameState(AlchemyBook book, IInventory inventory) : base(book, inventory)
        {
        }
    }
}