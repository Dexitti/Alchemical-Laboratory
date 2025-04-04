using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Alchemical_Laboratory
{
    public class AlchemyBook : IAlchemyBook<Substance>, IDiscoverer<Substance>
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public List<Substance> Substances { get; set; } = [];
        public List<Recipe> Recipes { get; private set; } = [];

        public void Import(string substancesPath, string recipesPath)
        {
            ImportSubstances(substancesPath);
            ImportRecipes(recipesPath);
            logger.Error("Recipes and subs have loaded.");
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
                logger.Fatal(ex.Message);
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
                    bool advanced = recipe.Advanced ?? false;
                    Recipes.Add(new(GetSubstance(recipe.Result), components, aux, advanced));
                }
            
                Substance GetSubstance(dynamic name) => Substances.First(x => x.Name == name.ToString());
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
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