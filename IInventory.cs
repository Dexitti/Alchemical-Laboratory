using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    interface IInventory
    {
        IEnumerable<Substance> Substances { get; }

        void Add(Substance sub);

        bool IsEnough(Substance sub, int number);

        bool Remove(Substance sub);

        void Display();
    }
}