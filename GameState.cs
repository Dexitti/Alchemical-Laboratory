using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Alchemical_Laboratory.Properties;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using Sharprompt;
using static Alchemical_Laboratory.Game;

namespace Alchemical_Laboratory
{
    public abstract class GameState
    {
        public static event Action? IsGameEnd;
        // public event Func<bool> IsRiskLevelHigh;

        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        const string gameStatePath = "State.json";
        const string resultPath = "Result.json";
        const string bookPath = "Book.json";
        const string inventoryPath = "Inventory.json";
        int riskLevel = 0;

        [JsonIgnore]
        public AlchemyBook Book { get; set; }
        [JsonIgnore]
        public IInventory Inventory { get; set; }

        public int RiskLevel
        {
            get => riskLevel;
            set
            {
                bool f = false;
                if (value > riskLevel) f = true;
                riskLevel = Math.Clamp(value, 0, 100);
                if (f) CheckHighRiskLevel(); //  Возможно изменить систему: сделать, чтобы токсичные и опасные вещества Risk++ ?
            }
        }
        public int AmountOfHints { get; set; } = 0;

        public GameState(AlchemyBook book, IInventory inventory)
        {
            Book = book;
            Inventory = inventory;

            inventory.NewSubstance += OnGetNewSubstance;
            IsGameEnd += CheckMasterEmerald;
            Substance.IsTargetReached += OnSuccess;
            Substance.MaxProgress += CheckFullProgress;
            logger.Debug("All events have subscribed.");
        }

        public void DisplayInventory() => Inventory.Display();
        
        public int[] Progress()
        {
            int opened = Book.Substances.Count(s => s.IsDiscovered);
            int total = Book.Substances.Count();
            return [opened, total];
        }

        public string PrintRiskLevel()
        {
            switch (RiskLevel)
            {
                case <= 10: return $"\x1b[38;2;0;255;0m{RiskLevel}/{100}\x1b[0m";
                case > 10 and <= 15: return $"\x1b[38;2;85;255;0m{RiskLevel}/{100}\x1b[0m";
                case > 15 and <= 30: return $"\x1b[38;2;170;255;0m{RiskLevel}/{100}\x1b[0m";
                case > 30 and <= 45: return $"\x1b[38;2;255;255;0m{RiskLevel}/{100}\x1b[0m";
                case > 45 and <= 60: return $"\x1b[38;2;255;170;0m{RiskLevel}/{100}\x1b[0m";
                case > 60 and <= 85: return $"\x1b[38;2;255;85;0m{RiskLevel}/{100}\x1b[0m";
                case > 85 and <= 100: return $"\x1b[38;2;255;0;0m{RiskLevel}/{100}\x1b[0m";
                default: return $"{RiskLevel}/{100}";
            }
        }

        public void OnGetNewSubstance(Substance sub) // событие: получено новое вещество (interface+functional)
        {
            Console.Clear();
            Console.WriteLine(Resource.NewSubstanceObtained + "!");
            logger.Info("Got new sub – {sub}!", sub);
            Console.WriteLine($"{sub} – {Resource.ResourceManager.GetString(sub.Description)}\n");

            // совмещение свойств вещества с емкостью

            for (int r = 0; r < 2; r++) // выбор рекомендуемых инструментов
            {
                var selectedTools = Prompt.MultiSelect(Resource.ChooseRequiredTools, Enum.GetValues<Tools>(), minimum: 0, textSelector: Extensions.Translate);
                Tools toolsEntered = selectedTools.Aggregate(default(Tools), (s, x) => s | x);
                //for (int i = 0; i < tools.Length; i++)
                //{
                //    toolsEntered |= tools[i];
                //}
                Tools toolsValid = sub.RequiredTools;
                if (toolsEntered == toolsValid)
                {
                    Console.WriteLine(Resource.YourToolsAreRight);
                    Extensions.MakeDelay(1700);
                    AmountOfHints++;
                    RiskLevel -= 1;
                    break;
                }
                else if ((toolsEntered & toolsValid) == 0)
                {
                    Console.WriteLine(Resource.YourToolsAreWrong);
                    if (r == 0) Console.Write(Resource.RepeatChoice);
                    Extensions.MakeDelay(1700);
                    if (r == 1)
                    {
                        RiskLevel += 10;
                    }
                    continue;
                }
                else
                {
                    Console.WriteLine(Resource.YourToolsArePartlyRight);
                    if (r == 0) Console.Write(Resource.RepeatChoice);
                    Extensions.MakeDelay(1700);
                    if (r == 1)
                    {
                        AmountOfHints++;
                        RiskLevel += 5;
                    }
                    continue;
                }
            }
            sub.IsDiscovered = true;
        }

        public Recipe? Mix(params Substance[] subs)
        {
            HashSet<Substance> mixedSubs = new HashSet<Substance>(subs);
            Recipe? desiredRecipe = Book.Recipes.FirstOrDefault(r => r.Components.SetEquals(mixedSubs));
            return desiredRecipe; // NullConditionalOperator
        }

        List<Recipe> GetAvailableRecipe()
        {
            List<Recipe> allRecipes = Book.Recipes;
            return allRecipes.Where(r => 
                r.Components.All(sub => sub.IsDiscovered && Inventory.IsEnough(sub)) && !r.Result.IsDiscovered).ToList();
        }

        public Recipe? GetRecipeForHint()
        {
            List<Recipe> accessibleRecipes = GetAvailableRecipe();
            if (accessibleRecipes.Count <= 0) return null;
            Random rand = new Random();
            int randSubIndex = rand.Next(accessibleRecipes.Count);
            return accessibleRecipes[randSubIndex];
        }

        public bool ReadinessToMagisterium()
        {
            List<Recipe> advancedRecipes = GetAvailableRecipe().Where(r => r.Advanced).ToList();
            if (advancedRecipes.Count > 0) return true;
            return false;
        }

        public static void CheckEnd()
        {
            IsGameEnd?.Invoke();
        }

        void CheckHighRiskLevel()
        {
            if (riskLevel >= 100)
            {
                Console.Clear();
                Console.WriteLine(Resource.HighRiskLevel);
                Console.WriteLine(Resource.Defeat);
                if (Prompt.Confirm(Resource.TryAgain))
                {
                    Process.Start(Environment.ProcessPath);
                    Console.Clear();
                    Environment.Exit(0);
                }
                else
                    Environment.Exit(0);
            }
        }

        void CheckMasterEmerald()
        {
            IEnumerable<Substance> gems = Services.GetRequiredService<AlchemyBook>().Substances.Where(s => s.IsGem);
            if (gems.All(g => g.IsDiscovered))
            {
                Substance masterGem = Book.Substances.First(s => s.ToString() == Resource.Master_Emerald);
                Inventory.Add(masterGem);
            }
        }

        void CheckFullProgress()
        {
            if (Book.Substances.All(s => s.IsDiscovered))
            {
                Console.Clear();
                Console.WriteLine("\n" + Resource.Congratulations + " " + Resource.AllSubstances);
                Extensions.MakeDelay(2500);
            }
        }

        void OnSuccess(Substance sub)
        {
            var result = Services.GetRequiredService<AlchemyManager>().ResultSubstance;
            if (sub != result) return;
            Console.Clear();
            Console.WriteLine(Resource.EndCongratulations);
            if (Prompt.Confirm(Resource.Continue)) return;
            else
            {
                logger.Info("Game ended successfully.");
                Extensions.CleanStrings(1);
                if (Prompt.Confirm(Resource.TryAgain))
                {
                    Process.Start(Environment.ProcessPath);
                    Console.Clear();
                    Environment.Exit(0);
                }
                else
                    Environment.Exit(0);
            }
        }

        public void SaveGame()
        {
            string GSjson = JsonConvert.SerializeObject(this, Formatting.Indented);
            string Rjson = JsonConvert.SerializeObject(Services.GetRequiredService<AlchemyManager>(), Formatting.Indented);
            string Bjson = JsonConvert.SerializeObject(Book.Substances, Formatting.Indented);
            string Ijson = JsonConvert.SerializeObject(Inventory, Formatting.Indented);

            File.WriteAllText(gameStatePath, GSjson);
            File.WriteAllText(resultPath, Rjson);
            File.WriteAllText(bookPath, Bjson);
            File.WriteAllText(inventoryPath, Ijson);
            logger.Debug("Successfully saving into {f0}, {f1}, {f2}, {f3}.", GSjson, Rjson, Bjson, Ijson);
        }

        public void LoadGame()
        {
            if (File.Exists(gameStatePath) & File.Exists(resultPath) & File.Exists(bookPath) & File.Exists(inventoryPath))
            {
                try
                {
                    string GSjson = File.ReadAllText(gameStatePath);
                    string Rjson = File.ReadAllText(resultPath);
                    string Bjson = File.ReadAllText(bookPath);
                    string Ijson = File.ReadAllText(inventoryPath);

                    JsonConvert.PopulateObject(GSjson, this);
                    JsonConvert.PopulateObject(Rjson, Services.GetRequiredService<AlchemyManager>());
                    JsonConvert.PopulateObject(Bjson, Book.Substances);
                    JsonConvert.PopulateObject(Ijson, Services.GetRequiredService<IInventory>());
                    logger.Debug("Successfully loading from {f0}, {f1}, {f2}, {f3}.", GSjson, Rjson, Bjson, Ijson);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Файл сохранения не найден. Начинается новая игра.");
                return;
            }
        }
    }
}