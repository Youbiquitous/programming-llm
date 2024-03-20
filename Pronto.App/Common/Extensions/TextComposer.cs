///////////////////////////////////////////////////////////////////
//
// Crionet TMS: Asset management system for sport events
// Copyright (c) Crionet
//
// Author: Youbiquitous Team
//




namespace Youbiquitous.Librogram.App.Common.Extensions
{
    public class TextComposer
    {
        public static string GoogleSearch(string query)
        {
            return $"https://www.google.com/search?q={query}";
        }

        public static string Title(string title, string message)
        {
            return $"{title}|{message}";
        }
    }
}