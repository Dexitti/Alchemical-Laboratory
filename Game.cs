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

        public static IServiceProvider ?Services { get; private set; }

        //public delegate void GetNewSubstance(Substance substance);
        //public event GetNewSubstance NewSubstance;

        //ResultSubstance goal = new();
        byte lifePoints = 3;
        bool showFirst = true;
        IGameState gameState;

        public Game()
        {
            Console.Title = "Alchemical Laboratory";
            Prompt.ColorSchema.PromptSymbol = ConsoleColor.DarkYellow;
            Prompt.ColorSchema.Select = (ConsoleColor)12;

            AlchemyBook book = new AlchemyBook();
            book.Import("Properties/Substances.json", "Properties/Recipes.json");

            var services = new ServiceCollection()
                .AddSingleton<LimitedInventory>()
                .AddSingleton<UnlimitedInventory>()
                .AddSingleton<AlchemyBook>();
                //.AddSingleton<ResultSubstance>();
            Services = services.BuildServiceProvider();
            Start();
        }

        public void Start() //Preprocessing or game?!
        {
            while (true)
            {
                gameMode = Prompt.Select(Resource.SelectGameMode, mode);
                bool confirm = Prompt.Confirm(Resource.AreYouSure + "?");
                Console.SetCursorPosition(0, Console.CursorTop - 1); // очищает 2 строки выше
                Console.Write(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                if (confirm)
                {
                    Console.WriteLine(Resource.GameMode + ": " + gameMode);
                    break;
                }
            }

            //ShowRules(); //debug

            if (gameMode == Resource.UnlimitedResources)
            {
                gameState = new UnlimitedGameState();
                //событие: получено новое вещество!
                Options();
            }
            else if (gameMode == Resource.LimitedResourcesHard)
            {
                gameState = new LimitedGameState(); //!!!
                //событие: получено новое вещество!
                Options();
            }
        }

        void Options()
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
            Action[] actions = new Action[] { OpenInventory, Mix, GetHint, ShowRules, Serialize, Deserialize, Quit };

            while (true)
            {
                Console.Clear();
                Console.WriteLine(Resource.GameMode + ": " + gameMode);
                //goal.PrintGoal();

                string menus = Prompt.Select(Resource.ChooseAnAction, strings);
                int index = Array.IndexOf(strings, menus);
                actions[index]();
            }
        }

        public void OpenInventory()
        {
            gameState.DisplayInventory();
            Console.ReadKey();
        }

        public void Mix()
        {
            IEnumerable<Substance> list = Services.GetRequiredService<IInventory>().Substances;
            List<string> sus = list.Select(s => s.Name).ToList(); // gameState.Inventory <=> List<string>

            string su1 = Prompt.Select(Resource.SelectSubstance + " 1", sus);
            Substance? sub1 = list.FirstOrDefault(s => s.Name == su1);

            string su2 = Prompt.Select(Resource.SelectSubstance + " 2", sus);
            Substance? sub2 = list.FirstOrDefault(s => s.Name == su2);

            Recipe desiredRecipe = gameState.Mix2(sub1, sub2);
            if (desiredRecipe == null)
            {
                Console.WriteLine("Увы! Ничего не получилось.");
            }
            else
            {
                Substance resultSub = desiredRecipe.Result;
                //NewSubstance.Invoke(resultSub);
            }
        }

        public void GetHint() // Where to delegate? - to IGameState?!
        {
            // Сделать несколько подсказок, типа resultSub | show one of the ingredients | Display result or component description
            Substance randUnknownSubstance = gameState.GetRecipeForHint().Result;
            Console.WriteLine($"U might mix smth to get {randUnknownSubstance}"); // Example
        }

        public void ShowRules()
        {
            string title = Resource.Rules;
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title + "\n");
            string[] rules = [
                "0. Вы – алхимик-любитель, стремящийся усовершенствовать своё мастерство и постичь алхимическую истину.",
                "\n1. Ваша текущая цель – получить целевое вещество (его рецепт).",
                "\n2. Чтобы получить новые вещества, смешивайте по два компонента (позже - три).",
                "\n3. При смешивании необходимо учитывать свойства веществ и внешние факторы.",
                "\n4. Не забывайте о мерах предосторожности – вы ведь пока еще любитель!",
                "\n5. При их нарушении вы потеряете часть репутации и повысите \"ОС\" (Опасность Среды).", // ОП - очки пыток, ОС - очки страдания
                "\n6. Условия в разных режимах схожи, но из-за ограничений вам придется действовать по-разному.",
                "\n                      Удачи и увлекательной алхимии!",
                "\n"
                ];
            if (showFirst)
            {
                foreach (string rule in rules)
                {
                    Thread.Sleep(500);
                    foreach (char c in rule)
                    {
                        Console.Write(c);
                        Thread.Sleep(5);
                    }
                }
                Thread.Sleep(1800);
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
            showFirst = false;
        }

        public void Serialize()
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

        public void Deserialize()
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

        public static void Quit() { Environment.Exit(0); }
    }
}