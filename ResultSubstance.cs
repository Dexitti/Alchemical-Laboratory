﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Alchemical_Laboratory.Properties;

namespace Alchemical_Laboratory
{
    //public sealed class ResultSubstance
    //{
    //    Substance RandomSubstance { get; set; }

    //    //public static readonly ResultSubstance instance = new ResultSubstance(); //только 1 экземпляр!
    //    public ResultSubstance() {
    //        var rand = new Random();
    //        RandomSubstance = substances[rand.Next(substances.Count)];
    //        BuildRecipeTree(RandomSubstance);
    //    }

    //    public void PrintGoal()
    //    {
    //        Console.Write("Ваша цель: ");
    //        Console.ForegroundColor = ConsoleColor.DarkMagenta;
    //        Console.Write(RandomSubstance.Name);
    //        Console.ResetColor();
    //        //Console.Write(". Фантазируйте, химичьте и экспериментируйте. Удачи!");
    //    }

    //    private RecipeNode BuildRecipeTree(Substance targetSubstance)
    //    {
    //        Recipe recipe = allRecipes.FirstOrDefault(r => r.Result == targetSubstance);

    //        if (recipe == null)
    //        {
    //            return new RecipeNode(new Recipe(targetSubstance, new List<Substance>()));
    //        }

    //        RecipeNode node = new RecipeNode(recipe);

    //        foreach (Substance component in recipe.Components)
    //        {
    //            node.Children.Add(BuildRecipeTree(component));
    //        }

    //        return node;
    //    }
    //}


    //public class RecipeNode
    //{
    //    public Recipe Recipe { get; }
    //    public List<RecipeNode> Children { get; } = new List<RecipeNode>();

    //    public RecipeNode(Recipe recipe)
    //    {
    //        Recipe = recipe;
    //    }

    //    public override string ToString()
    //    {
    //        return Recipe.ToString(); // Можно изменить.
    //    }
    //}
}