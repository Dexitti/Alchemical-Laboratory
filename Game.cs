using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharprompt;

namespace Alchemical_Laboratory
{
    public class Game
    {
        readonly string[] mode = ["unlimited resources", "limited resources (hard)"];
        string gameMode;
        byte lifePoints = 3;
        bool showFirst = true;
        IGameState gameState;

        public Game()
        {
            Console.Title = "Alchemical Laboratory";
            Prompt.ColorSchema.PromptSymbol = ConsoleColor.DarkYellow;
            Prompt.ColorSchema.Select = (ConsoleColor)12;
            Start();
        }

        public void Start() //Preprocessing or game?!
        {
            while (true)
            {
                gameMode = Prompt.Select("Выберете режим:", mode);
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

            ShowRules();

            if (gameMode == "unlimited resources")
            {
                gameState = new UnlimitedGameState();
                //событие: получено новое вещество!
                Options();
            }
            else if (gameMode == "limited resources")
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
                "Inventory",
                "Mix",
                "GetHint",
                "ShowRules",
                "Save",
                "Load",
                "Quit"
            ];
            Action[] actions = new Action[] { OpenInventory, Mix2, GetHint, ShowRules, Serialize, Deserialize, Quit };

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Режим игры: {gameMode}");
                //ResultSubstance goal = new();
                //goal.PrintGoal();

                string menus = Prompt.Select("Select option", strings);
                switch (menus)
                {
                    case "Inventory":
                        OpenInventory(); break;
                    case "Mix":
                        Mix2(); break;
                    case "GetHint":
                        GetHint(); break;
                    case "ShowRules":
                        ShowRules(); break;
                    case "Save":
                        Serialize(); break;
                    case "Load":
                        Deserialize(); break;
                    case "Quit":
                        Quit(); break;
                }
            }
        }

        public void OpenInventory()
        {
            gameState.DisplayInventory();
        }

        public void Mix2() { }

        public void GetHint() { }



        public void ShowRules()
        {
            string title = "Rules";
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