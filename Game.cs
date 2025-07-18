using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using Alchemical_Laboratory.Properties;
using NLog;
using System.Drawing;
using System.Diagnostics;
using Kurukuru;
using System.Data;

namespace Alchemical_Laboratory
{
    public class Game
    {
        readonly string[] mode = [Resource.UnlimitedResources, Resource.LimitedResourcesHard];
        string gameMode;

        //byte lifePoints = 3;
        bool showRulesFirst = true;
        bool synthesisLocked = true;
        GameState gameState;
        IServiceCollection services;
        
        public static IServiceProvider Services { get; private set; } = null!;
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Game()
        {
            Console.Title = "Alchemical Laboratory";
            Prompt.ColorSchema.PromptSymbol = ConsoleColor.DarkYellow;
            Prompt.ColorSchema.Select = (ConsoleColor)12;

            try
            {
                LogManager.Setup().LoadConfigurationFromFile("Properties/Nlog.config");
            }
            catch (UserException ex) { logger.Error(ex); }

            AlchemyBook book = new AlchemyBook();
            book.Import("Properties/Substances.json", "Properties/Recipes.json", "Properties/Combinations.json");

            services = new ServiceCollection()
                .AddSingleton(book)
                .AddSingleton<AlchemyManager>();

            Start();
        }

        void Start() //Preprocessing
        {
            logger.Info("Starting Game.");
            Console.CursorVisible = false;
            Console.Write(string.Concat(Enumerable.Repeat(' ', 18)));
            foreach (char c in Resource.AlchemicalLaboratory)
            {
                Console.Write(c);
                Thread.Sleep(40);
            }
            Extensions.MakeDelay(800);
            Console.WriteLine();
            Console.CursorVisible = true;
            // Show Logo, Downloading menu or Preview

            while (true)
            {
                gameMode = Prompt.Select(Resource.SelectGameMode, mode);
                bool confirm = Prompt.Confirm(Resource.AreYouSure + "?", true);
                Extensions.CleanStrings(2);
                if (confirm)
                {
                    Console.WriteLine(Resource.GameMode + ": " + gameMode);
                    break;
                }
            }
            logger.Info("Selected game mode: {mode}.", gameMode);

            if (gameMode == Resource.UnlimitedResources)
            {
                services.AddSingleton<IInventory, UnlimitedInventory>() // ковариантность
                        .AddSingleton<GameState, UnlimitedGameState>();
            }
            else if (gameMode == Resource.LimitedResourcesHard)
            {
                services.AddSingleton<IInventory, LimitedInventory>()
                        .AddSingleton<GameState, LimitedGameState>();
            }
            Services = services.BuildServiceProvider();
            logger.Debug("Services have been initialized successfully.");
            gameState = Services.GetRequiredService<GameState>();

            if (File.Exists("State.json"))
            {
                bool confirm = Prompt.Confirm(Resource.YouAlreadyHaveProgressDownload);
                if (confirm)
                {
                    Deserialize();
                    showRulesFirst = false;
                }
                else
                {
                    Extensions.CleanStrings(2);
                    Console.Write($"  {Resource.StartNewGame}");
                    Extensions.MakeDelay(2000);
                }
            }
            Console.Clear();
            //ShowRules();

            Services.GetRequiredService<AlchemyManager>().GetFirstSubs();
            Options();
        }

        void Options() // Game menu
        {
            List<string> strings =
            [
                Resource.Inventory, // menu: показать параметры (HP, Risk Level, Прогресс...) и сделать auto-save (напр. каждые 5 subs)
                Resource.Mix,
                Resource.GetHint,
                Resource.ShowRules,
                Resource.Save,
                Resource.Load,
                Resource.Quit
            ];
            List<Action> actions = [ OpenInventory, Mix, GetHint, ShowRules, Serialize, Deserialize, Quit ];

            while (true)
            {
                Console.Clear();
                Console.WriteLine(Resource.GameMode + ": " + gameMode);
                Services.GetRequiredService<AlchemyManager>().PrintGoal();

                if (synthesisLocked && gameState.ReadinessToMagisterium())
                {
                    gameState.synthesisUnlocked();
                    strings.Insert(2, Resource.Synthesize);
                    actions.Insert(2, Synthesize);
                    synthesisLocked = false;
                }

                string menus = Prompt.Select(Resource.ChooseAnAction, strings);
                logger.Debug("User selected {menu} button.", menus);
                int index = strings.IndexOf(menus);
                actions[index]();
            }
        }

        void OpenInventory()
        {
            Extensions.CleanStrings(1);
            // [█████░░░░░░]
            Console.WriteLine($"{Resource.RiskLevel}: {gameState.HeatmapPrint(gameState.RiskLevel, 100)}\n");
            int[] pr = gameState.Progress();
            Console.WriteLine($"{Resource.AlchemyBookProgress}: {pr[0]+"/"+pr[1]} ({Math.Round((double)pr[0] / (double)pr[1] * 100, 2)}%)");
            gameState.DisplayInventory();
            Console.ReadKey();
        }

        void Mix()
        {
            var inventory = Services.GetRequiredService<IInventory>();
            IEnumerable<Substance> list = inventory.Substances;
            List<string> sus = list.Select(s => s.ToString()).Order().ToList(); // gameState.Inventory <=> List<string>
            Substance[] subs = new Substance[2];

            for (int i = 0; i < 2; i++)
            {
                while (true)
                {
                    string su = Prompt.Select($"{Resource.SelectSubstance} {i + 1}", sus, 20);
                    subs[i] = list.First(s => s.ToString() == su);
                    if (!inventory.IsEnough(subs[i]))
                    {
                        Console.Write($"{Resource.ResourceSub} {su} {Resource.NotEnoughChooseAnother}");
                        Thread.Sleep(500);
                        Extensions.CleanStrings(2);
                        continue;
                    }
                    break;
                }
                inventory.Remove(subs[i]);
            }

            Recipe? outcome = gameState.Mix(subs);
            logger.Info("Mix result: {res}: IsDiscovered: {dis}", outcome, outcome?.Result.IsDiscovered);
            if (outcome == null)
            {
                Console.WriteLine(Resource.MixFailed); // Идея: сохранять список несуществующих рецептов | историю попыток -
                Extensions.MakeDelay(1700);
            }
            else if (outcome.Result.IsDiscovered)
            {
                Console.WriteLine(Resource.FamiliarRecipe);
                Extensions.MakeDelay(1700);
            }
            else
            {
                if (outcome.AuxiliaryResults.Count > 0)
                {
                    Extensions.CleanStrings(3);
                    Console.WriteLine($"\n{Resource.NewSubstanceObtained} – {outcome.Result}!");
                    string auxList = string.Join(", ", outcome.AuxiliaryResults);
                    Console.WriteLine($"{ Resource.FoundAuxilary} {{{auxList}}} ...");
                    Console.ReadKey();
                    inventory.Add(outcome.Result);
                    foreach (var aux in outcome.AuxiliaryResults.Where(s => !s.IsDiscovered))
                        inventory.Add(aux);
                }
                else
                    inventory.Add(outcome.Result);
            }
        }

        void Synthesize()
        {
            var inventory = Services.GetRequiredService<IInventory>();
            IEnumerable<Substance> list = inventory.Substances;
            List<string> sus = list.Select(s => s.ToString()).Order().ToList();
            Substance[] subs = new Substance[3];

            for (int i = 0; i < 3; i++)
            {
                while (true)
                {
                    string su = Prompt.Select($"{Resource.SelectSubstance} {i+1}", sus, 20);
                    subs[i] = list.First(s => s.ToString() == su);
                    if (!inventory.IsEnough(subs[i]))
                    {
                        Console.Write($"{Resource.ResourceSub} {su} {Resource.NotEnoughChooseAnother}");
                        Thread.Sleep(500);
                        Extensions.CleanStrings(2);
                        continue;
                    }
                    break;
                }
                inventory.Remove(subs[i]);
            }

            Recipe? outcome = gameState.Mix(subs);
            logger.Info("Mix result: {res}: IsDiscovered: {dis}", outcome, outcome?.Result.IsDiscovered);
            if (outcome == null)
            {
                Console.WriteLine(Resource.MixFailed);
                Extensions.MakeDelay(1700);
            }
            else if (outcome.Result.IsDiscovered)
            {
                Console.WriteLine(Resource.FamiliarRecipe);
                Extensions.MakeDelay(1700);
            }
            else
            {
                if (outcome.AuxiliaryResults.Count > 0)
                {
                    Extensions.CleanStrings(3);
                    Console.WriteLine($"\n{Resource.NewSubstanceObtained} – {outcome.Result}!");
                    string auxList = string.Join(", ", outcome.AuxiliaryResults);
                    Console.WriteLine($"{Resource.FoundAuxilary} {{{auxList}}} ...");
                    Console.ReadKey();
                    inventory.Add(outcome.Result);
                    foreach (var aux in outcome.AuxiliaryResults.Where(s => !s.IsDiscovered))
                        inventory.Add(aux);
                }
                else
                    inventory.Add(outcome.Result);
            }
        }

        void GetHint()
        {
            Extensions.CleanStrings(1);
            if (gameState.AmountOfHints <= 0)
                Console.WriteLine(Resource.NotEnoughHints);
            else
            {
                Console.WriteLine($"{Resource.AmountOfHints}: {gameState.AmountOfHints}");
                if (gameState.GetRecipeForHint() == null)
                {
                    Console.WriteLine(Resource.AllRecipesDiscovered);
                    Extensions.MakeDelay(1700);
                    return;
                }
                // Сделать несколько подсказок, типа resultSub | Show one of the ingredients | Display ingredient and result description
                string[] hints = [Resource.ShowIngredient, Resource.DisplayResult, Resource.DisplayDescription/*, ""*/];
                string hintType = Prompt.Select(Resource.GetHint, hints/*, defaultValue: ""*/);
                logger.Info("Hint selected: {type}", hintType);
                gameState.AmountOfHints--; // Можно сделать разную цену подсказок

                Extensions.CleanStrings(2);
                Random rand = new Random();

                if (hintType == Resource.ShowIngredient)
                {
                    Substance randIngredient = rand.Select(gameState.GetRecipeForHint().Components);
                    string[] s = Resource.YouMayMix_With.Split('?');
                    Console.WriteLine(s[0] + randIngredient + s[1]);
                }
                else if (hintType == Resource.DisplayResult)
                {
                    Substance randUnknownSubstance = gameState.GetRecipeForHint().Result;
                    Console.WriteLine(Resource.YouMayGet + randUnknownSubstance);
                }
                else if (hintType == Resource.DisplayDescription)
                {
                    Recipe randUnknownRecipe = gameState.GetRecipeForHint();
                    Substance randIngredient = rand.Select(randUnknownRecipe.Components);
                    Substance randResult = randUnknownRecipe.Result;
                    Console.WriteLine($"{Resource.DoYouRememberSub}\n   {Resource.ResourceManager.GetString(randIngredient.Description)}");
                    Console.WriteLine($"{Resource.SomehowRelatedTo}\n   {Resource.ResourceManager.GetString(randResult.Description)}");
                }
                else return;
            }
            Console.ReadKey();
        }

        void ShowRules()
        {
            string title = Resource.Rules;
            Console.WriteLine(string.Concat(Enumerable.Repeat(' ', 46)) + title);
            string[] rules = Resource.RulesThemselves.Split('?');
            if (showRulesFirst)
            {
                foreach (string rule in rules)
                {
                    Extensions.MakeDelay(500);
                    Console.WriteLine();
                    foreach (char c in rule)
                    {
                        Console.Write(c);
                        Thread.Sleep(5);
                    }
                }
                Extensions.MakeDelay(1800);
                Console.Write("\n" + Resource.PressToStart);
                logger.Debug("Rules shown.");
            }
            else
            {
                foreach (string rule in rules)
                {
                    Console.WriteLine();
                    Console.Write(rule);
                }
                Console.Write("\n" + Resource.PressToContinue + "...");
            }
            Console.ReadKey();
            Console.Clear();
            showRulesFirst = false;
        }

        void Serialize()
        {
            // Идея: сделать несколько (3) ячеек сейвов и показывать прогресс
            gameState.SaveGame();
            Console.WriteLine(Resource.SuccessfullySaving);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(Resource.PressToContinue + "...");
            Console.ResetColor();
            Extensions.MakeDelay(1700);
        }

        void Deserialize()
        {
            gameState.LoadGame();
            Extensions.MakeDelay(1500);
        }

        void Quit() { Environment.Exit(0); }
    }
}