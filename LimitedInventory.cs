using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class LimitedInventory : IInventory
    {
        readonly Dictionary<Substance, int> buckets = [];

        public IEnumerable<Substance> Substances => buckets.Keys;

        public event Action<Substance>? NewSubstance;

        public void Add(Substance sub)
        {
            if (!buckets.TryGetValue(sub, out int count))
            {
                count = 2;
                NewSubstance?.Invoke(sub);
            }
            buckets[sub] = count + 1;
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
                    Console.WriteLine($"{c}. {Resource.ResourceManager.GetString(pair.Key.Name)}: {pair.Value}");
                    c++;
                }
            }
        }

        public bool IsEnough(Substance sub) => buckets.TryGetValue(sub, out int count) && count > 0;

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