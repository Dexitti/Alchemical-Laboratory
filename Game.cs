using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;

namespace Alchemical_Laboratory
{
    public class Game
    {
        readonly string[] mode = ["безграничные ресурсы", "ограниченные ресурсы (сложно)"]; // unlimited resources, limited resources (hard)
        string gameMode;

        public static IServiceProvider ?Services { get; private set; }

        //ResultSubstance goal = new();
        byte lifePoints = 3;
        bool showFirst = true;
        IGameState gameState;

        public Game()
        {
            Console.Title = "Alchemical Laboratory";
            Prompt.ColorSchema.PromptSymbol = ConsoleColor.DarkYellow;
            Prompt.ColorSchema.Select = (ConsoleColor)12;

            var services = new ServiceCollection()
                .AddSingleton<SubstancesCollection>()
                .AddSingleton<RecipesCollection>()
                .AddSingleton<KnownSubstances>()
                .AddSingleton<KnownRecipes>()
                .AddSingleton<ResultSubstance>();
            Services = services.BuildServiceProvider();
            Start();
        }

        public void Start() //Preprocessing or game?!
        {
            while (true)
            {
                gameMode = Prompt.Select("Выберете режим", mode);
                bool confirm = Prompt.Confirm("Вы уверены?");
                Console.SetCursorPosition(0, Console.CursorTop - 1); // очищает 2 строки выше
                Console.Write(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                if (confirm)
                {
                    Console.WriteLine($"Режим игры: {gameMode}");
                    break;
                }
            }

            //ShowRules(); debug

            if (gameMode == "безграничные ресурсы")
            {
                gameState = new UnlimitedGameState();
                //событие: получено новое вещество!
                Options();
            }
            else if (gameMode == "ограниченные ресурсы (сложно)")
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
                "Инвентарь", // Inventory
                "Смешать", // Mix
                "Получить подсказку", //Get Hint
                "Открыть правила", // Show Rules
                "Сохранить", // Save
                "Загрузить", // Load
                "Выйти" // Quit
            ];
            Action[] actions = new Action[] { OpenInventory, Mix, GetHint, ShowRules, Serialize, Deserialize, Quit };

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Режим игры: {gameMode}");
                //goal.PrintGoal();

                string menus = Prompt.Select("Выберите действие", strings);
                switch (menus)
                {
                    case "Инвентарь":
                        OpenInventory(); break;
                    case "Смешать":
                        Mix(); break;
                    case "Получить подсказку":
                        GetHint(); break;
                    case "Открыть правила":
                        ShowRules(); break;
                    case "Сохранить":
                        Serialize(); break;
                    case "Загрузить":
                        Deserialize(); break;
                    case "Выйти":
                        Quit(); break;
                }
            }
        }

        public void OpenInventory()
        {
            gameState.DisplayInventory();
            Console.ReadKey();
        }

        public void Mix()
        {
            List<string> sus = Services.GetRequiredService<KnownSubstances>().Select(s => s.Name).ToList(); // gameState.Inventory <--> List<string>
            string su1 = Prompt.Select("Выберите вещество 1", sus);
            string su2 = Prompt.Select("Выберите вещество 2", sus);
            Substance sub1 = Services.GetRequiredService<KnownSubstances>().FirstOrDefault(s => s.Name == su1);
            Substance sub2 = Services.GetRequiredService<KnownSubstances>().FirstOrDefault(s => s.Name == su2);
            Substance resultSub = gameState.Mix2(sub1, sub2);
            //событие: получено новое вещество!
        }

        public void GetHint()
        {

        }

        public void ShowRules()
        {
            string title = "Правила"; // Rules
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
                Console.Write("\nНажмите, чтобы начать");
            }
            else
            {
                foreach (string rule in rules)
                {
                    Console.Write(rule);
                }
                Console.Write("\nНажмите, чтобы продолжить...");
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