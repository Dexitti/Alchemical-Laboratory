using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{

    public static class Extensions
    {
        public static string? Translate<T>(this T sth) where T : Enum
        {
            return Resource.ResourceManager.GetString(sth.ToString());
        }

        public static T Select<T>(this Random rand, IEnumerable<T> items) =>
            (from item in items orderby rand.Next() select item).First();

        public static void MakeDelay(int ms)
        {
            int elapsed = 0;
            while (elapsed < ms)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey();
                    return;
                }
                Thread.Sleep(50);
                elapsed += 50;
            }
        }

        public static void CleanStrings(int c)
        {
            for (int i = 0; i < c; i++) // Очищает "c" строк выше
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
            }
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public static T Choice<T>(this Random rand, IReadOnlyList<T> items) => items[rand.Next(items.Count)];

        public static CancellationTokenSource cts = new();
        static object consoleLock = new();

        public static void UpdateInputDisplay(string inputLabel, StringBuilder input)
        {
            lock (consoleLock)
            {
                // Перемещаем курсор на строку ввода
                Console.SetCursorPosition(0, 1);
                Console.Write(new string(' ', Console.WindowWidth)); // Очищаем строку
                Console.SetCursorPosition(0, 1);
                Console.Write(inputLabel + input.ToString()); // Отображаем текущий ввод
            }
        }

        public static void StartStarDisplay(Rectangle drawArea, ConsoleColor[] colors, int appearanceRate = 40, int starLifeTime = 1500, CancellationToken token = default)
        {
            int originX = Console.CursorLeft;
            int originY = Console.CursorTop;
            Point anchor = new(originX, originY);

            if (drawArea.Top > drawArea.Bottom || originX + drawArea.Right > Console.WindowWidth || originY + drawArea.Bottom > Console.WindowHeight)
                throw new ArgumentOutOfRangeException(nameof(drawArea), "Incorrect drawing area.");

            if (colors == null || colors.Length == 0)
                throw new ArgumentException("You should select at least one color.");

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    SpinWait.SpinUntil(() => !Console.KeyAvailable);
                    int randomX = Random.Shared.Next(drawArea.Left, drawArea.Right);
                    int randomY = Random.Shared.Next(drawArea.Top, drawArea.Bottom);
                    ConsoleColor randomColor = Random.Shared.Choice(colors);
                    int randomDelay = Random.Shared.Next(starLifeTime / 2, starLifeTime);
                    _ = Task.Run(async () => await DisplayStarAsync(originX + randomX, originY + randomY, anchor, randomColor, randomDelay));
                    await Task.Delay(appearanceRate);
                }
            }, token);

        }

        static async Task DisplayStarAsync(int x, int y, Point anchor, ConsoleColor color, int delay = 100)
        {
            char[] ss = ['*', ' '];
            foreach (char c in ss)
            {
                SpinWait.SpinUntil(() => !Console.KeyAvailable);
                lock (consoleLock)
                {
                    Console.CursorLeft = x;
                    Console.CursorTop = y;
                    Console.ForegroundColor = color;
                    Console.Write(c);
                    Console.CursorLeft = anchor.X;
                    Console.CursorTop = anchor.Y;
                    Console.ResetColor();
                }
                if (c == '*') await Task.Delay(delay);
            }
        }

        public static void StopStarDisplay()
        {
            cts.Cancel();
            cts = new();
        }

    }
}