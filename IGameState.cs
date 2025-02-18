using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public interface IGameState
    {
        public double RiskLevel { get; set; }
        public bool RecipeDiscovered { get; set; }

        public void DisplayInventory();
        public Substance Mix2(Substance sub1, Substance sub2);
        public void IsSuccess();
    }
}