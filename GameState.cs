using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Alchemical_Laboratory.Properties;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using static Alchemical_Laboratory.Game;

namespace Alchemical_Laboratory
{
    public abstract class GameState
    {
        public static event Action? IsGameEnd;
        // public event Func<bool> IsRiskLevelHigh;

        private int riskLevel = 0;

        public AlchemyBook Book { get; }
        public IInventory Inventory { get; }

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
        }

        public void DisplayInventory() => Inventory.Display();
        
        public int[] Progress()
        {
            int opened = Book.Substances.Count(s => s.IsDiscovered);
            int total = Book.Substances.Count();
            return [opened, total];
        }

        public void OnGetNewSubstance(Substance sub) // событие: получено новое вещество (interface+functional)
        {
            Console.Clear();
            Console.WriteLine(Resource.NewSubstanceObtained + "!");
            Console.WriteLine($"{sub} – {Resource.ResourceManager.GetString(sub.Description)}\n");

            // совмещение свойств вещества с емкостью

            for (int r = 0; r < 2; r++) // выбор рекомендуемых инструментов
            {
                Tools[] tools = (Tools[])Prompt.MultiSelect(Resource.ChooseRequiredTools, Enum.GetValues<Tools>());
                Tools toolsEntered = default;
                for (int i = 0; i < tools.Length; i++)
                {
                    toolsEntered |= tools[i];
                }
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
                r.Components.All(sub => sub.IsDiscovered && Inventory.IsEnough(sub)) && !r.IsDiscovered).ToList();
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
            IEnumerable<Substance> gems = Game.Services.GetRequiredService<AlchemyBook>().Substances.Where(s => s.IsGem);
            if (gems.All(g => g.IsDiscovered))
            {
                Substance masterGem = Book.Substances.First(s => s.ToString() == Resource.Master_Emerald);
                Inventory.Add(masterGem);
            }
        }

        void OnSuccess(Substance sub)
        {
            var result = Game.Services.GetRequiredService<AlchemyManager>().ResultSubstance;
            if (sub != result) return;
            Console.Clear();
            Console.WriteLine(Resource.EndCongratulations);
            if (Prompt.Confirm(Resource.Continue)) return;
            else
            {
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

    }
}