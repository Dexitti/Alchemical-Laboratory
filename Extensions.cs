using System;
using System.Collections.Generic;
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
            for (int i = 0; i < c; i++) // Очищает c строк выше
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
            }
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}