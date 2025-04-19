using System;
using System.Reflection;
using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.Utility
{
    public static class Log
    {
        public static void Info(object context, string message, string color = "white")
        {
            Debug.Log($"{ColorTag(GetTag(context), color)} {message}");
        }

        public static void Warning(object context, string message, string color = "yellow")
        {
            Debug.LogWarning($"{ColorTag(GetTag(context), color)} {message}");
        }

        public static void Error(object context, string message, string color = "red")
        {
            Debug.LogError($"{ColorTag(GetTag(context), color)} {message}");
        }

        private static string GetTag(object context)
        {
            var type = context.GetType();
            var attr = type.GetCustomAttribute<LogTagAttribute>();

            return attr?.Tag ?? type.Name;
        }

        private static string ColorTag(string tag, string color)
        {
            return $"<color={color}>[{tag}]</color>";
        }
    }
}