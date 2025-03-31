using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public sealed class AlchemyManager
    {
        IInventory inventory;
        List<Substance> substances;

        public Substance ResultSubstance { get; set; }

        public AlchemyManager(IInventory inventory, AlchemyBook book) // Сервисы сервису передаем в конструктор!
        {
            this.inventory = inventory;
            this.substances = book.Substances;
            var rand = new Random();

            List<Substance> results = [.. substances.Where(s => s.IsFinal)]; // Create Collection on base [.. list]
            ResultSubstance = results[rand.Next(results.Count)];
        }

        public void GetFirstSubs()
        {
            foreach (var el in substances.Where(s => s.IsPrimary))
            {
                inventory.Add(el);
            }
        }

        public void PrintGoal()
        {
            Console.Write(Resource.YourGoal + ": ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(ResultSubstance);
            Console.ResetColor();
            //Console.Write(". Фантазируйте, химичьте и экспериментируйте. Удачи!");
        }

    }
}