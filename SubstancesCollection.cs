using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class SubstancesCollection : ReadOnlyCollection<Substance>
    {

        public SubstancesCollection() : base(GetAllSubstances())
        {
        }

        private static IList<Substance> GetAllSubstances()
        {
            List<Substance> list = 
            [
                new Substance("Огонь", "-"),
                new Substance("Вода", "-"),
                new Substance("Ветер", "-"),
                new Substance("Земля", "-")

            ];
            return list;
        }
    }
}