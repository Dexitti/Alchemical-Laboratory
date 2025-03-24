using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    class LimitedInventory : IInventory
    {
        readonly Dictionary<Substance, int> buckets = [];

        public IEnumerable<Substance> Substances => buckets.Keys;

        public void Add(Substance sub)
        {
            if (!buckets.TryGetValue(sub, out int count))
            {
                count = 0;
            }
            buckets[sub] = count + 1;
        }

        public void Display()
        {
            Console.WriteLine(Resource.LimitedInventory);
            foreach (var pair in buckets)
            {
                if (pair.Value <= 0)
                    continue;
                Console.WriteLine(Resource.ResourceManager.GetString(pair.Key.Name) + ": " + pair.Value);
            }
        }

        public bool IsEnough(Substance sub, int number) => buckets.TryGetValue(sub, out int count) && count >= number;

        public bool Remove(Substance sub)
        {
            if (buckets.TryGetValue(sub, out int count) && count > 1)
            {
                buckets[sub] = count - 1;
                return true;
            }
            return false;
        }
    }
}