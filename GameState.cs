using System;
using System.Collections.Generic;
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
        private int amountOfHints = 0;
        private bool isTargetReached = false;

        public AlchemyBook Book { get; }
        public IInventory Inventory { get; }

        public int RiskLevel
        {
            get => riskLevel;
            set
            {
                riskLevel = value;
                if (riskLevel > 100)
                    CheckEnd();
            }
        }
        public int AmountOfHints { get => amountOfHints; set => amountOfHints = value; }
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
            IsGameEnd += OnSuccess;
        }

        public void DisplayInventory() => Inventory.Display();
        
        public void OnGetNewSubstance(Substance sub) // событие: получено новое вещество (interface+functional)
        {
            sub.IsDiscovered = true;

            Console.Clear();
            Console.WriteLine(Resource.NewSubstanceObtained);
            Console.WriteLine($"{sub} – {Resource.ResourceManager.GetString(sub.Description)}\n");

            // совмещение свойств вещества с емкостью

            for (int r = 0; r < 2; r++) // выбор рекомендуемых инструментов
            {
                // string?[] tools = Prompt.MultiSelect(Resource.ChooseRequiredTools, Enum.GetValues<Tools>().Select(Extensions.Translate)).ToArray();
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

        public Substance? Mix2(Substance sub1, Substance sub2)
        {
            HashSet<Substance> mixedSubs = [sub1, sub2];
            Recipe? desiredRecipe = Book.Recipes.FirstOrDefault(r => r.Components.SetEquals(mixedSubs));
            return desiredRecipe?.Result; // NullConditionalOperator
        }

        public Recipe GetRecipeForHint()
        {
            IEnumerable<Substance> subsList = Inventory.Substances;
            List<Recipe> allRecipes = Book.Recipes;

            List<Recipe> accessibleRecipes = allRecipes
                .Where(r => r.Components.Any(sub => subsList.Contains(sub)) && !r.IsDiscovered).ToList();

            Random rand = new Random();
            int randSubIndex = rand.Next(accessibleRecipes.Count);
            return accessibleRecipes[randSubIndex];
        }

        public static void CheckEnd()
        {
            IsGameEnd?.Invoke();
        }

        public void OnRiskLevelHigh()
        {
            Console.Clear();
            Console.WriteLine(Resource.HighRiskLevel);
            Console.WriteLine(Resource.Defeat);
            bool again = Prompt.Confirm(Resource.TryAgain);
            if (again)
            {
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
            }
            else
                Environment.Exit(0);
        }

        public void OnSuccess()
        {

            Console.WriteLine(Resource.EndCongratulations);
        }
    }
}