using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class UnlimitedInventory(AlchemyBook book) : IInventory
    {
        readonly HashSet<Substance> substances = [];
        public IEnumerable<Substance> Substances => substances;

        public IEnumerable JsonData
        {
            get => substances.Select(s => s.Name).ToList();
            set
            {
                foreach (dynamic name in value)
                {
                    substances.Add(book.Substances.First(s => s.Name == name.ToString()));
                }
            }
        }

        public event Action<Substance>? NewSubstance;

        public void Add(Substance sub)
        {
            if (substances.Add(sub))
                NewSubstance?.Invoke(sub);
        }

        public void Display()
        {
            if (substances.Count == 0)
                Console.WriteLine(Resource.InventoryEmpty);
            else
            {
                Console.WriteLine(Resource.Inventory + ":");
                int c = 1;
                foreach (var sub in substances)
                {
                    Console.WriteLine($"{c}. {sub}");
                    c++;
                }
            }
        }

        public bool IsEnough(Substance sub) => substances.Contains(sub);

        public bool Remove(Substance sub) => substances.Contains(sub);
    }
}