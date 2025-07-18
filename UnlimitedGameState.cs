using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Alchemical_Laboratory.Properties;
using Sharprompt;

namespace Alchemical_Laboratory
{
    public class UnlimitedGameState : GameState
    {
        public UnlimitedGameState(AlchemyBook book, IInventory inventory, AlchemyManager manager) : base(book, inventory, manager)
        {
        }

        //OnGetNewSubstance(Substance sub) в родительском классе
    }
}