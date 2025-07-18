using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemical_Laboratory.Properties;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using static System.Net.Mime.MediaTypeNames;
using static Alchemical_Laboratory.Game;

namespace Alchemical_Laboratory
{
    public class LimitedGameState : GameState
    {
        public LimitedGameState(AlchemyBook book, IInventory inventory, AlchemyManager manager) : base(book, inventory, manager)
        {
            inventory.NewSubstance += CheckUpgrade;
        }

        void CheckUpgrade(Substance sub)
        {
            // Upgrades inventory, increases resources income, contains checks for adding action and quest
            if (!sub.IsDiscovered) return; // Для определения вызова после главного обработчика в GameState?!
            if (Book.Substances.Count(s => s.IsDiscovered) == 12)
            {
                Console.Clear();
                //Console.WriteLine("\n" + Resource);
                Extensions.MakeDelay(2000);
            }
            // ((LimitedInventory)Services.GetRequiredService<IInventory>()).Income += Book.Substances.Count(s => s.IsDiscovered) / 15;
        }

        public override void OnGetNewSubstance(Substance sub)
        {
            base.OnGetNewSubstance(sub);

            // Создание контейнера
            Console.Clear();
            Console.WriteLine($"Создайте контейнер для {sub}");
            var shape = Prompt.Select("Выберите форму", Enum.GetValues<Types>(), textSelector: Extensions.Translate);
            Types shapeEntered = Enum.GetValues<Types>().First(t => t == shape);
            var material = Prompt.Select("Выберите материал", Enum.GetValues<Materials>(), textSelector: Extensions.Translate);
            Materials materialEntered = Enum.GetValues<Materials>().First(m => m == material);
            // int amount = ((LimitedInventory)Services.GetRequiredService<IInventory>()).AmountOf(sub); // Еще не знаю сколько, так как событие обрабатывается раньше добавления в инвентарь
            Container vessel = new(shape, material);
            sub.Bin = vessel; // Хранить собранный контейнер у вещества. Проблема: разные сейвы для режимов (↓ сложность можно, ↑ нельзя)

            // сопоставление свойств вещества с емкостью
            Console.Clear();
            Console.WriteLine(string.Concat(Enumerable.Repeat(' ', 16)) + "Посмотрите, что у вас получилось");
            const int LEN = 50;
            Console.WriteLine($"{"Вещество:", -LEN}{"Контейнер:"}");
            Console.WriteLine($"{sub, -LEN}{vessel}");
            string desc = Resource.ResourceManager.GetString(sub.Description);
            if (desc.Length < LEN)
                Console.WriteLine($"{desc, -LEN}{vessel.Material.Translate()}");
            else
            {
                string part1 = desc.Substring(0, LEN-5).Trim();
                string part2 = desc.Substring(LEN-5).Trim();
                Console.WriteLine($"{part1,-LEN}{vessel.Material.Translate()}");
                Console.WriteLine($"{part2,-LEN}");
            }
            Dictionary<Property, double> binRess = GetBinProperties(vessel);
            var allProperties = sub.Characteristics.Keys.Union(binRess?.Keys ?? Enumerable.Empty<Property>()).Distinct().ToList();
            foreach (var prop in allProperties)
            {
                string propName = prop.Translate();
                string subValue = sub.Characteristics.TryGetValue(prop, out double sVal) ? sVal.ToString() : "N/A";
                string binValue = "x";
                if (binRess != null)
                    binValue = binRess.TryGetValue(prop, out double cVal) ? cVal.ToString() : "N/A";
                Console.WriteLine($" {propName,1 - LEN / 2} {subValue,1 - LEN / 2} {binValue}");
            }

            // Анализ и результат
            Console.WriteLine();
            string result = ComparisonAnalysis(sub, vessel);
            // Console.WriteLine("[Вам начислено ...\n ]Уровень риска увеличился/уменьшился/не изменился");
            Console.WriteLine(result);
            Console.ReadKey();
        }

        public Dictionary<Property, double>? GetBinProperties(Container bin) =>
            Book.Combinations.FirstOrDefault(c => (c.Type == bin.Type && c.Material == bin.Material))?.Resistances;

        string ComparisonAnalysis(Substance sub, Container bin)
        {
            Dictionary<Property, double> binProps = GetBinProperties(bin);
            if (binProps == null || sub.Characteristics == null)
                return "Ты дурачок такое создавать?! Поздравляю, ты совместил несовместимое)";

            var report = new StringBuilder();
            report.AppendLine("┌".PadRight(48, '─'));

            int posMes = 0;
            int allMes = 0;
            foreach (Property prop in Enum.GetValues(typeof(Property)))
            {
                if (prop == Property.none) continue;

                bool subHasProp = sub.Characteristics.TryGetValue(prop, out double subValue);
                bool binHasProp = binProps.TryGetValue(prop, out double binValue);

                if (!subHasProp && !binHasProp) continue; // -

                string propName = prop.Translate();
                string message;
                var (good, bad) = Combination.PropertyMessages[prop];
                if (binHasProp && (!subHasProp || binValue >= subValue)) // ✔
                {
                    message = good;
                    posMes++;
                }
                else if (subHasProp && (!binHasProp || subValue > binValue)) // ✖
                {
                    message = bad;
                }
                else continue;

                report.Append(message + ";\n");
                allMes++;
            }
            report.AppendLine("└".PadRight(48, '─'));

            report.Insert(0, $"Контейнер:{GetReportResult(posMes, allMes)}({posMes}/{allMes})\u001b[0m\n"); // процент закрытых свойств (не тот цвет!) 
            // RiskLevel/Hints/Income ...
            return report.ToString();
        }

        string GetReportResult(int pos, int total)
            => pos == total ? "\u001b[38;2;20;255;0m ИДЕАЛЬНЫЙ" :
               pos >= total * 0.8 ? "\u001b[38;2;127;255;0m ХОРОШИЙ" :
               pos >= total * 0.5 ? "\u001b[38;2;255;128;0m ДОПУСТИМЫЙ" : "\u001b[38;2;255;60;0m НЕПОДХОДЯЩИЙ";
    }
}