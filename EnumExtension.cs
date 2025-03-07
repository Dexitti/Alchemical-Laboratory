using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemical_Laboratory
{
    public static class EnumExtension
    {
        public enum Tools
        {
            gloves = 0,
            hood,
            glasses,
            respirator
        }

        public enum Types
        {
            round_bottom_flask = 0,
            erlenmeyer_flask,
            mortar,
            beaker,
            bottle,
            jar,
            test_tube,
            dish,
            box
        }

        public enum Materials
        {
            thin_glass = 0,
            tempered_glass,
            heat_resistant_glass,
            enameled_glass,
            plastic,
            ceramic
        }

        public static string GetName(Tools el)
        {
            switch (el)
            {
                case Tools.gloves: return "перчатки";
                case Tools.hood: return "вытяжка";
                case Tools.glasses: return "очки";
                case Tools.respirator: return "респиратор";
            }
            return el.ToString();
        }

        public static string GetName(Types el)
        {
            switch (el)
            {
                case Types.round_bottom_flask: return "Круглодонная колба";
                case Types.erlenmeyer_flask: return "Коническая плоскодонная колба";
                case Types.mortar: return "Ступка";
                case Types.beaker: return "Стакан";
                case Types.bottle: return "Склянка";
                case Types.jar: return "Банка";
                case Types.test_tube: return "Пробирка";
                case Types.dish: return "Чашка";
                case Types.box: return "Коробка";
            }
            return el.ToString();
        }

        public static string GetName(Materials el)
        {
            switch (el)
            {
                case Materials.thin_glass: return "Тонкое стекло";
                case Materials.tempered_glass: return "Каленное стекло";
                case Materials.heat_resistant_glass: return "Термостойкое стекло";
                case Materials.enameled_glass: return "Эмалированное стекло";
                case Materials.plastic: return "Пластик";
                case Materials.ceramic: return "Керамика";
            }
            return el.ToString();
        }
    }
}