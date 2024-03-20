///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//




namespace Pronto.App.Common.Extensions;

public static class MiscExtensions
{
    /// <summary>
    /// Different string based on Boolean value
    /// </summary>
    /// <param name="theValue"></param>
    /// <param name="yes"></param>
    /// <param name="no"></param>
    /// <returns></returns>
    public static string ToDefault(this bool theValue, string yes = "yes", string no = "")
    {
        return theValue
            ? yes
            : no;
    }
}