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

            ResultSubstance goal = new();
            goal.PrintGoal();

            if (gameMode == "unlimited resources")
            {
                UnlimitedGameState gameState = new UnlimitedGameState();
            }
            else if (gameMode == "limited resources")
            {
                LimitedGameState gameState2 = new LimitedGameState();
            }
        }

        public void ShowRules()
        {
            string title = "Rules";
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title + "\n");
            Console.WriteLine("0. Вы – алхимик-любитель, стремящийся усовершенствовать своё мастерство и постичь алхимическую истину.");
            Console.WriteLine("1. Ваша текущая цель – получить целевое вещество (его рецепт).");
            Console.WriteLine("2. Чтобы получить новые вещества, смешивайте по два компонента (позже - три).");
            Console.WriteLine("3. При смешивании необходимо учитывать свойства веществ и внешние факторы.");
            Console.WriteLine("4. Не забывайте о мерах предосторожности – вы ведь пока еще любитель!");
            Console.WriteLine("5. При их нарушении вы потеряете часть репутации и повысите \"ОС\" (Опасность Среды)."); // ОП - очки пыток, ОС - очки страдания
            Console.WriteLine("6. Условия в разных режимах схожи, но из-за ограничений вам придется действовать по-разному.");
            Console.WriteLine("                      Удачи и увлекательной алхимии!");
        }
    }
}