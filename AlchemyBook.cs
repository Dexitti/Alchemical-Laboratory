using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Alchemical_Laboratory
{
    public class AlchemyBook : IAlchemyBook<Substance>, IDiscoverer<Substance>
    {
        public List<Substance> Substances { get; private set; } = [];
        public List<Recipe> Recipes { get; private set; } = [];

        public void Import(string substancesPath, string recipesPath)
        {
            ImportSubstances(substancesPath);
            ImportRecipes(recipesPath);
        }

        void ImportSubstances(string path)
        {
            try
            {
                string data = File.ReadAllText(path);
                Substances = JsonConvert.DeserializeObject<List<Substance>>(data);
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
            }
            
        }

        void ImportRecipes(string path)
        {
            try
            {
                string data = File.ReadAllText(path);
                dynamic recipes = JToken.Parse(data);
                foreach (dynamic recipe in recipes)
                {
                    HashSet<Substance> components = [];
                    foreach (string name in recipe.Components)
                    {
                        components.Add(GetSubstance(name));
                    }
                    List<Substance> aux = [];
                    JArray? names = recipe.AuxiliaryResults;
                    foreach (dynamic name in names ?? [])
                    {
                        aux.Add(GetSubstance(name));
                    }
                    Recipes.Add(new(GetSubstance(recipe.Result), components, aux));
                }
            
                Substance GetSubstance(dynamic name) => Substances.First(x => x.Name == name.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        IEnumerable<Substance> IAlchemyBook<Substance>.Substances => Substances;

        public void Discover(Substance substance)
        {
            substance.IsDiscovered = true;
        }
    }

    public interface IDiscoverer<in TSubstance> where TSubstance : ISubstance
    {
        void Discover(TSubstance substance);
    }

    public interface IAlchemyBook<out T> where T : ISubstance
    {
        IEnumerable<T> Substances { get; }
    }
}