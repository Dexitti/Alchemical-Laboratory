using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class SubstancesCollection : ReadOnlyDictionary<string, Substance>, ISubstanceCollection<Substance>
    {

        public SubstancesCollection() : base(GetAllSubstances())
        {
        }

        static IDictionary<string, Substance> GetAllSubstances()
        {
            Dictionary<string, Substance> dict = new()
            {
                // default
                { "fire", new Substance("Огонь", "-") },
                { "water", new Substance("Вода", "-") },
                { "wind", new Substance("Ветер", "-") },
                { "earth", new Substance("Земля", "-") },

                // complex
                { "lava", new Substance("Лава", "-") }
            };
            return dict;
        }

        IEnumerable<Substance> ISubstanceCollection<Substance>.Values => GetAllSubstances().Values;
    }

    public interface ISubstanceCollection<out T> where T : ISubstance
    {
        IEnumerable<T> Values { get; }
    }
}