using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Alchemical_Laboratory.Properties;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        const string bookPath = "Book.json";
        const string inventoryPath = "Inventory.json";
        int riskLevel = 0;

        [JsonIgnore]
        public AlchemyBook Book { get; set; }
        [JsonIgnore]
        public IInventory Inventory { get; set; }
        [JsonIgnore]
        public AlchemyManager Manager { get; set; }

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

        public string ResultName { get => Manager.ResultSubstance.Name; set => Manager.ResultSubstance = Book.Substances.First(s => s.Name == value); }

        public GameState(AlchemyBook book, IInventory inventory, AlchemyManager manager)
        {
            Book = book;
            Inventory = inventory;
            Manager = manager;

            inventory.NewSubstance += OnGetNewSubstance; // можно ли упростить события?
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

        public string HeatmapPrint(int value, int total)
        {
            switch ((double)value / (double)total)
            {
                case <= 0.1: return $"\x1b[38;2;0;255;0m{value}/{total}\x1b[0m";
                case > 0.1 and <= 0.15: return $"\x1b[38;2;85;255;0m{value}/{total}\x1b[0m";
                case > 0.15 and <= 0.3: return $"\x1b[38;2;170;255;0m{value}/{total}\x1b[0m";
                case > 0.3 and <= 0.45: return $"\x1b[38;2;255;255;0m{value}/{total}\x1b[0m";
                case > 0.45 and <= 0.6: return $"\x1b[38;2;255;170;0m{value}/{total}\x1b[0m";
                case > 0.0 and <= 0.85: return $"\x1b[38;2;255;85;0m{value}/{total}\x1b[0m";
                case > 0.85 and <= 1: return $"\x1b[38;2;255;0;0m{value}/{total}\x1b[0m";
                default: return $"{value}/{total}";
            }
        }

        public virtual void OnGetNewSubstance(Substance sub) // event handler: получено новое вещество (interface+functional)
        {
            Console.Clear();
            Console.WriteLine(Resource.NewSubstanceObtained + "!");
            logger.Info("Got new sub – {sub}!", sub);
            Console.WriteLine($"{sub} – {Resource.ResourceManager.GetString(sub.Description)}\n");

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

        public void synthesisUnlocked()
        {
            Console.CursorVisible = false;
            string congrats = Resource.Congratulations + " " + Resource.AAA;
            var area = new Rectangle(0, 1, congrats.Length, 6);
            ConsoleColor[] colors = [ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkBlue, ConsoleColor.DarkCyan];
            Extensions.StartStarDisplay(area, colors, appearanceRate: 100, starLifeTime: 1100, token: Extensions.cts.Token);
            Console.WriteLine(congrats);

            Extensions.MakeDelay(10000);
            Extensions.StopStarDisplay();
            Thread.Sleep(1100);
            Console.SetCursorPosition(0, 2);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.CursorVisible = true;
            
            logger.Info("The synthesis option is unlocked ({openedSubs} substances had been discovered).", Progress()[0]);
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
            IEnumerable<Substance> gems = Book.Substances.Where(s => s.IsGem);
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
                Console.CursorVisible = false;
                var area = new Rectangle(0, 2, Console.WindowWidth, Console.WindowHeight - 2);
                ConsoleColor[] colors = [ConsoleColor.White, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta];
                Extensions.StartStarDisplay(area, colors, token: Extensions.cts.Token);
                Console.WriteLine("\n" + Resource.Congratulations + " " + Resource.AllSubstances);
                Extensions.MakeDelay(10000);
                Extensions.StopStarDisplay();
                Thread.Sleep(1200);
                Console.CursorVisible = true;
            }
        }

        void OnSuccess(Substance sub)
        {
            var result = Services.GetRequiredService<AlchemyManager>().ResultSubstance;
            if (sub != result) return;
            Console.Clear();

            Console.CursorVisible = false;
            var area = new Rectangle(0, 2, Console.WindowWidth, Console.WindowHeight - 2);
            ConsoleColor[] colors = [ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.DarkMagenta];
            Extensions.StartStarDisplay(area, colors, appearanceRate: 30, starLifeTime: 1800, token: Extensions.cts.Token);

            Console.WriteLine(Resource.EndCongratulations);
            bool continued = false;
            StringBuilder _inputString = new();
            Extensions.UpdateInputDisplay(Resource.Continue + ": ", _inputString);

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true); // Считываем нажатую клавишу без отображения
                string input = _inputString.ToString().Trim().ToLower();

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (_inputString.Length > 0)
                    {
                        _inputString.Remove(_inputString.Length - 1, 1);
                        Extensions.UpdateInputDisplay(Resource.Continue + ": ", _inputString);
                    }
                }
                else if ((input.ToLower() == "y" || input.ToLower() == "yes") && keyInfo.Key == ConsoleKey.Enter)
                {
                    continued = true;
                    Extensions.StopStarDisplay();
                    Thread.Sleep(1800);
                    break;
                }
                else if ((input.StartsWith("n", StringComparison.OrdinalIgnoreCase)) && keyInfo.Key == ConsoleKey.Enter)
                {
                    continued = false;
                    break;
                }
                else
                {
                    _inputString.Append(keyInfo.KeyChar); // Добавляем введенный символ
                    Extensions.UpdateInputDisplay(Resource.Continue + ": ", _inputString);
                }
            }
            if (!continued)
            {
                logger.Info("Game ended successfully.");
                Console.SetCursorPosition(0, 1);
                Console.Write(new string(' ', Console.WindowWidth));
                _inputString.Clear();
                Extensions.UpdateInputDisplay(Resource.TryAgain + ": ", _inputString);

                while (true)
                {
                    var keyInfo = Console.ReadKey(intercept: true); // Считываем нажатую клавишу без отображения
                    string input = _inputString.ToString().Trim().ToLower();

                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (_inputString.Length > 0)
                        {
                            _inputString.Remove(_inputString.Length - 1, 1);
                            Extensions.UpdateInputDisplay(Resource.TryAgain + ": ", _inputString);
                        }
                    }
                    else if ((input.StartsWith("y", StringComparison.OrdinalIgnoreCase)) && keyInfo.Key == ConsoleKey.Enter)
                    {
                        Extensions.StopStarDisplay();
                        Thread.Sleep(1800);
                        Process.Start(Environment.ProcessPath);
                        Console.Clear();
                        Environment.Exit(0);
                    }
                    else if ((input.StartsWith("n", StringComparison.OrdinalIgnoreCase)) && keyInfo.Key == ConsoleKey.Enter)
                    {
                        Extensions.StopStarDisplay();
                        Thread.Sleep(1800);
                        Environment.Exit(0);
                    }
                    else
                    {
                        _inputString.Append(keyInfo.KeyChar); // Добавляем введенный символ
                        Extensions.UpdateInputDisplay(Resource.TryAgain + ": ", _inputString);
                    }
                }
            }
            Console.CursorVisible = true;
        }

        public void SaveGame()
        {
            string GSjson = JsonConvert.SerializeObject(this, Formatting.Indented);
            string Bjson = JsonConvert.SerializeObject(Book.SubsJson, Formatting.Indented);
            string Ijson = JsonConvert.SerializeObject(Inventory.JsonData, Formatting.Indented);

            File.WriteAllText(gameStatePath, GSjson);
            File.WriteAllText(bookPath, Bjson);
            File.WriteAllText(inventoryPath, Ijson);
            logger.Debug("Successfully saving into {f0}, {f1}, {f2}.", GSjson, Bjson, Ijson);
        }

        public void LoadGame()
        {
            if (File.Exists(gameStatePath) & File.Exists(bookPath) & File.Exists(inventoryPath))
            {
                try
                {
                    string GSjson = File.ReadAllText(gameStatePath);
                    string Bjson = File.ReadAllText(bookPath);
                    string Ijson = File.ReadAllText(inventoryPath);

                    JsonConvert.PopulateObject(GSjson, this);
                    Book.SubsJson = JsonConvert.DeserializeObject<IEnumerable<(string, bool)>>(Bjson);
                    Services.GetRequiredService<IInventory>().JsonData = JsonConvert.DeserializeObject<JArray>(Ijson);
                    logger.Debug("Successfully loading from {f0}, {f1}, {f2}.", GSjson, Bjson, Ijson);
                    Console.WriteLine(Resource.SaveLoaded);
                }
                catch (FileNotFoundException ex)
                {
                    logger.Error(ex);
                    return;
                }
            }
            else
            {
                Console.WriteLine(Resource.FileNotFound);
                return;
            }
        }
    }
}