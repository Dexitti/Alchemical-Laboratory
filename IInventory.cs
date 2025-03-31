using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public interface IInventory
    {
        IEnumerable<Substance> Substances { get; }

        public event Action<Substance> NewSubstance;

        void Add(Substance sub);

        bool IsEnough(Substance sub);

        bool Remove(Substance sub);

        void Display();
    }
}