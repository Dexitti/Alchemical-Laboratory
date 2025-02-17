using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public sealed class ResultSubstance
    {
        readonly List<CompoundSubstance> substances;
        readonly List<Recipe> allRecipes;
        CompoundSubstance RandomSubstance { get; set; }

        public static readonly ResultSubstance instance = new ResultSubstance(); //только 1 экземпляр!
        public ResultSubstance() {
            var rand = new Random();
            RandomSubstance = substances[rand.Next(substances.Count)];
        }

        public void PrintGoal()
        {
            Console.Write("Ваша цель: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(RandomSubstance.Name);
            Console.ResetColor();
            //Console.Write(". Фантазируйте, химичьте и экспериментируйте. Удачи!");
        }


    }
}