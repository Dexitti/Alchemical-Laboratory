using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class LimitedInventory(AlchemyBook book) : IInventory
    {
        readonly Dictionary<Substance, int> buckets = [];
        public IEnumerable<Substance> Substances => buckets.Keys;

        public event Action<Substance>? NewSubstance;

        public IEnumerable JsonData
        {
            get => buckets.Select(p => (p.Key.Name, p.Value)).ToList();
            set
            {
                foreach (dynamic p in value)
                {
                    buckets.Add(book.Substances.First(s => s.Name == p.Name), p.Count);
                }
            }
        }

        public int Income { get; set; } = 1; // при выборе инструментов увеличивать на 20%/40%

        public void Add(Substance sub)
        {
            if (!buckets.TryGetValue(sub, out int count))
            {
                count = 2;
                NewSubstance?.Invoke(sub);
            }
            buckets[sub] = count + Income; // или вообще отдельная штука, так как inventory.Add() -> eventHandler
        }

        public void Display()
        {
            if (buckets.Count == 0)
                Console.WriteLine(Resource.InventoryEmpty);
            else
            {
                Console.WriteLine(Resource.Inventory + ":");
                int c = 1;
                foreach (var pair in buckets)
                {
                    if (pair.Value < 0) continue;
                    Console.WriteLine($"{c}. {pair.Key.Bin}({pair.Key.Bin.Material}) [{pair.Key} ({pair.Value})]");
                    c++;
                }
            }
        }

        public bool IsEnough(Substance sub) => buckets.TryGetValue(sub, out int count) && count > 0;

        public int AmountOf(Substance sub) => buckets[sub];

        public bool Remove(Substance sub)
        {
            if (buckets.TryGetValue(sub, out int count) && count > 0)
            {
                buckets[sub] = count - 1;
                return true;
            }
            return false;
        }
    }
}