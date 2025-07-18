using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public class Combination
    {
        public Types Type { get; set; }
        public Materials Material { get; set; }
        public Dictionary<Property, double> Resistances { get; set; }

        public Combination(Types type, Materials material, Dictionary<Property, double> characteristics)
        {
            Type = type;
            Material = material;
            Resistances = characteristics;
        }

        public static readonly Dictionary<Property, (string positive, string negative)> PropertyMessages = new()
        {
            [Property.temperature] =
            (
                "│ [+] Контейнер держит температуру лучше термоса",
                "│ [-] Температура меняется как настроение у кота"
            ),
            [Property.hardness] =
            (
                "│ [+] Прочнее гранитного характера",
                "│ [-] Хрупкое как бумажный меч"
            ),
            [Property.volatility] =
            (
                "│ [+] Летучесть под контролем",
                "│ [-] Испаряется быстрее, чем интерес к трендам"
            ),
            [Property.viscosity] =
            (
                "│ [+] Идеальная вязкость - прямо как у мёда",
                "│ [-] Токсичность зашкаливает - прям как в чате рандомов"
            ),
            [Property.toxicity] =
            (
                "│ [+] Яды нейтрализованы алхимическим фильтром",
                "│ [-] Токсичность - как в комментариях под постом"
            ),
            [Property.flammability] =
            (
                "│ [+] Огнеупорности позавидует даже дракон",
                "│ [-] Воспламеняется от взгляда"
            ),
            [Property.explosiveness] =
            (
                "│ [+] Взрывобезопасность: 100%",
                "│ [-] Готов рвануть громче новогодних петард"
            ),
            [Property.radioactivity] =
            (
                "│ [+] Безопаснее, чем магический учебник для первокурсников",
                "│ [-] Светится, как новогодняя ёлка"
            )
        };
    }

    [Flags]
    public enum Types
    {
        roundBottomFlask = 1,
        erlenmeyerFlask = 2,
        mortar = 4,
        beaker = 8,
        bottle = 16,
        jar = 32,
        testTube = 64,
        dish = 128,
        box = 256
    }

    [Flags]
    public enum Materials
    {
        thinGlass = 1,
        temperedGlass = 2,
        heatResistantGlass = 4,
        enameledGlass = 8,
        plastic = 16,
        ceramic = 32,
        wood = 64
    }

    [Flags]
    public enum Property
    {
        none = 0,
        temperature = 1,
        hardness = 2,
        volatility = 4,
        viscosity = 8,
        toxicity = 16,
        flammability = 32,
        explosiveness = 64,
        radioactivity = 128
    }
}