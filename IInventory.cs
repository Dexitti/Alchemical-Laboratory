using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Alchemical_Laboratory
{
    public interface IInventory
    {
        [JsonIgnore]
        IEnumerable<Substance> Substances { get; }

        IEnumerable JsonData { get; set; }

        public event Action<Substance> NewSubstance;

        void Add(Substance sub);

        bool IsEnough(Substance sub);

        bool Remove(Substance sub);

        void Display();
    }
}