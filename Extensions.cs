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

        //public static string GetName(Properties el) //переделать в Resources!
        //{
        //    switch (el)
        //    {
        //        case Properties.Temperature: return "температура";
        //        case Properties.Hardness: return "твердость";
        //        case Properties.Volatility: return "летучесть";
        //        case Properties.Viscosity: return "вязкость";
        //        case Properties.Toxicity: return "токсичность";
        //        case Properties.Flammability: return "горючесть";
        //        case Properties.Radioactivity: return "радиоактивность";
        //    }
        //    return el.ToString();
        //}

        //public static string GetName(Tools el)
        //{
        //    switch (el)
        //    {
        //        case Tools.gloves: return "перчатки";
        //        case Tools.hood: return "вытяжка";
        //        case Tools.glasses: return "очки";
        //        case Tools.respirator: return "респиратор";
        //    }
        //    return el.ToString();
        //}

        //public static string GetName(Types el)
        //{
        //    switch (el)
        //    {
        //        case Types.roundBottomFlask: return "Круглодонная колба";
        //        case Types.erlenmeyerFlask: return "Коническая плоскодонная колба";
        //        case Types.mortar: return "Ступка";
        //        case Types.beaker: return "Стакан";
        //        case Types.bottle: return "Склянка";
        //        case Types.jar: return "Банка";
        //        case Types.testTube: return "Пробирка";
        //        case Types.dish: return "Чашка";
        //        case Types.box: return "Коробка";
        //    }
        //    return el.ToString();
        //}

        //public static string GetName(Materials el)
        //{
        //    switch (el)
        //    {
        //        case Materials.thinGlass: return "Тонкое стекло";
        //        case Materials.temperedGlass: return "Каленное стекло";
        //        case Materials.heatResistantGlass: return "Термостойкое стекло";
        //        case Materials.enameledGlass: return "Эмалированное стекло";
        //        case Materials.plastic: return "Пластик";
        //        case Materials.ceramic: return "Керамика";
        //    }
        //    return el.ToString();
        //}
    }
}