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
        private bool isTargetReached = false;

        public AlchemyBook Book { get; }
        public IInventory Inventory { get; }

        public int RiskLevel
        {
            get => riskLevel;
            set
            {
                riskLevel = Math.Clamp(value, 0, 100);
                if (riskLevel >= 100)
                    CheckEnd();
            }
        }
        public int AmountOfHints { get; set; } = 0;
        public bool IsTargetReached
        {
            get => isTargetReached;
            set
            {
                isTargetReached = value;
                if (isTargetReached)
                    CheckEnd();
            }
        }

        public GameState(AlchemyBook book, IInventory inventory)
        {
            Book = book;
            Inventory = inventory;

            inventory.NewSubstance += OnGetNewSubstance;
            IsGameEnd += OnRiskLevelHigh;
            IsGameEnd += CheckMasterEmerald;
            IsGameEnd += OnSuccess;
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
            sub.IsDiscovered = true;

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
        }

        public Recipe? Mix(params Substance[] subs)
        {
            HashSet<Substance> mixedSubs = new HashSet<Substance>(subs);
            Recipe? desiredRecipe = Book.Recipes.FirstOrDefault(r => r.Components.SetEquals(mixedSubs));
            return desiredRecipe; // NullConditionalOperator
        }

        public List<Recipe> GetAvailableRecipe()
        {
            List<Recipe> allRecipes = Book.Recipes;
            return allRecipes.Where(r => 
                r.Components.All(sub => sub.IsDiscovered && Inventory.IsEnough(sub)) && !r.IsDiscovered).ToList();
        }

        public Recipe GetRecipeForHint()
        {
            List<Recipe> accessibleRecipes = GetAvailableRecipe();

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

        void OnRiskLevelHigh()
        {
            if (riskLevel >= 100)
            {
                Console.Clear();
                Console.WriteLine(Resource.HighRiskLevel);
                Console.WriteLine(Resource.Defeat);
                bool again = Prompt.Confirm(Resource.TryAgain);
                if (again)
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

        void OnSuccess()
        {

            Console.WriteLine(Resource.EndCongratulations);
        }
    }
}