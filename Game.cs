using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    public class Game
    {
        readonly string[] mode = [Resource.UnlimitedResources, Resource.LimitedResourcesHard];
        string gameMode;

        byte lifePoints = 3;
        bool showFirst = true;
        GameState gameState;
        IServiceCollection services;

        public static IServiceProvider Services { get; private set; } = null!;

        public Game()
        {
            Console.Title = "Alchemical Laboratory";
            Prompt.ColorSchema.PromptSymbol = ConsoleColor.DarkYellow;
            Prompt.ColorSchema.Select = (ConsoleColor)12;

            AlchemyBook book = new AlchemyBook();
            book.Import("Properties/Substances.json", "Properties/Recipes.json");

            services = new ServiceCollection()
                .AddSingleton(book)
                .AddSingleton<AlchemyManager>();

            Start();
        }

        void Start() //Preprocessing
        {
            // Show Logo, Downloading menu or Preview
            ShowRules(); //debug

            while (true)
            {
                gameMode = Prompt.Select(Resource.SelectGameMode, mode);
                bool confirm = Prompt.Confirm(Resource.AreYouSure + "?");
                Extensions.CleanStrings(2);
                if (confirm)
                {
                    Console.WriteLine(Resource.GameMode + ": " + gameMode);
                    break;
                }
            }

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
            gameState = Services.GetRequiredService<GameState>();

            Services.GetRequiredService<AlchemyManager>().GetFirstSubs();

            Options();
        }

        void Options() // Game menu
        {
            string[] strings =
            [
                Resource.Inventory,
                Resource.Mix,
                Resource.GetHint,
                Resource.ShowRules,
                Resource.Save,
                Resource.Load,
                Resource.Quit
            ];
            Action[] actions = [ OpenInventory, Mix, GetHint, ShowRules, Serialize, Deserialize, Quit ];

            while (true)
            {
                Console.Clear();
                Console.WriteLine(Resource.GameMode + ": " + gameMode);
                Services.GetRequiredService<AlchemyManager>().PrintGoal();

                string menus = Prompt.Select(Resource.ChooseAnAction, strings);
                int index = Array.IndexOf(strings, menus);
                actions[index]();
            }
        }

        void OpenInventory()
        {
            Extensions.CleanStrings(1);
            gameState.DisplayInventory();
            Console.ReadKey();
        }

        void Mix()
        {
            var inventory = Services.GetRequiredService<IInventory>();
            IEnumerable<Substance> list = inventory.Substances;
            List<string> sus = list.Select(s => s.Name).ToList(); // gameState.Inventory <=> List<string>

            string su1, su2;
            Substance? sub1, sub2;
            while (true)
            {
                su1 = Prompt.Select(Resource.SelectSubstance + " 1", sus);
                sub1 = list.First(s => s.Name == su1);
                if (!inventory.IsEnough(sub1))
                {
                    Console.Write($"{Resource.ResourceSub} {su1} {Resource.NotEnoughChooseAnother}");
                    Thread.Sleep(500);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop); // Очищает 2 строки выше
                    continue;
                }
                break;
            }
            inventory.Remove(sub1);

            while (true)
            {
                su2 = Prompt.Select(Resource.SelectSubstance + " 2", sus);
                sub2 = list.First(s => s.Name == su2);
                if (!inventory.IsEnough(sub2))
                {
                    Console.Write($"{Resource.ResourceSub} {su2} {Resource.NotEnoughChooseAnother}");
                    Thread.Sleep(500);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    continue;
                }
                break;
            }
            inventory.Remove(sub2);

            Substance? result = gameState.Mix2(sub1, sub2);
            if (result == null)
            {
                Console.WriteLine(Resource.MixFailed); // Идея: сохранять список несуществующих рецептов | историю попыток
                Extensions.MakeDelay(1700);
            }
            else
                inventory.Add(result);
        }

        void GetHint()
        {
            Extensions.CleanStrings(1);
            if (gameState.AmountOfHints <= 0)
                Console.WriteLine(Resource.NotEnoughHints);
            else
            {
                Console.WriteLine($"{Resource.AmountOfHints}: {gameState.AmountOfHints}");
                // Сделать несколько подсказок, типа resultSub | Show one of the ingredients | Display ingredient and result description
                string[] hints = [Resource.ShowIngredient, Resource.DisplayResult, Resource.DisplayDescription];
                string hintType = Prompt.Select(Resource.GetHint, hints);
            
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
                    Console.WriteLine($"{Resource.DoYouRememberSub}\n   {randIngredient}");
                    Console.WriteLine($"{Resource.SomehowRelatedTo}\n   {randResult}");
                }
            }
            Extensions.MakeDelay(2500);
        }

        void ShowRules()
        {
            string title = Resource.Rules;
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title);
            string[] rules = Resource.RulesThemselves.Split('?');
            if (showFirst)
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
            }
            else
            {
                foreach (string rule in rules)
                {
                    Console.Write(rule);
                }
                Console.Write("\n" + Resource.PressToContinue + "...");
            }
            Console.ReadKey();
            Console.Clear();
            showFirst = false;
        }

        void Serialize()
        { }
        //    string fl = Prompt.Select<string>("Choose format", new[] { "To xml", "To json" });
        //    if (fl == "To xml")
        //    {
        //        const string path = @"C:\Working folder of Dima\Univer Sem_3\CSharp OOP\LW_3\Graphic.xml";
        //        string file = Prompt.Input<string>("Path to save", defaultValue: path);

        //        editor.ToXml(file);
        //    }
        //    else if (fl == "To json")
        //    {
        //        const string path = @"C:\Working folder of Dima\Univer Sem_3\CSharp OOP\LW_3\Graphic.json";
        //        string file = Prompt.Input<string>("Path to save", defaultValue: path);
        //        editor.ToJson(file);
        //    }
        //    Console.WriteLine("Serialization successful");
        //    Console.ForegroundColor = ConsoleColor.DarkGray;
        //    Console.Write("Press any key to continue...");
        //    Console.ResetColor();
        //    Console.ReadKey();
        //}

        void Deserialize()
        { }
        //    string fl = Prompt.Select<string>("Choose format", new[] { "From xml", "From json" });
        //    if (fl == "From xml")
        //    {
        //        try
        //        {
        //            const string path = @"C:\Working folder of Dima\Univer Sem_3\CSharp OOP\LW_3\Graphic.xml";
        //            string file = Prompt.Input<string>("Import path", defaultValue: path);
        //            var retmyEditor = GraphicsEditor.FromXml(file);
        //            editor = retmyEditor;
        //        }
        //        catch (Exception ex) { Console.WriteLine($"Error! {ex.Message}"); Console.ReadKey(); }
        //    }
        //    else if (fl == "From json")
        //    {
        //        try
        //        {
        //            const string path = @"C:\Working folder of Dima\Univer Sem_3\CSharp OOP\LW_3\Graphic.json";
        //            string file = Prompt.Input<string>("Import path", defaultValue: path);
        //            var retmyEditor = GraphicsEditor.FromJson(file);
        //            editor = retmyEditor;
        //        }
        //        catch (Exception ex) { Console.WriteLine($"Error! {ex.Message}"); Console.ReadKey(); }
        //    }
        //}

        void Quit() { Environment.Exit(0); }
    }
}