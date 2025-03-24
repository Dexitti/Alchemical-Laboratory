using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    class UnlimitedInventory : IInventory
    {
        private readonly HashSet<Substance> substances = [];
        public IEnumerable<Substance> Substances => substances;

        public void Add(Substance sub) => substances.Add(sub);

        public void Display()
        {
            Console.WriteLine(Resource.UnlimitedInventory);
            foreach (var sub in substances)
            {
                Console.WriteLine(Resource.ResourceManager.GetString(sub.Name));
            }
        }

        public bool IsEnough(Substance sub, int number) => substances.Contains(sub);

        public bool Remove(Substance sub) => substances.Contains(sub);
    }
}