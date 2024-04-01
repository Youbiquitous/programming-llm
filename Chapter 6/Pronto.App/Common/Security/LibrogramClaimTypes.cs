///////////////////////////////////////////////////////////////////
//
// Librogram: Reference application for social management of reads
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//




namespace Youbiquitous.Librogram.App.Common.Security;

/// <summary>
/// Facade for the names of custom claims to be saved in the app cookie
/// </summary>
public class LibrogramClaimTypes
{
    /// <summary>
    /// Claim name to store the app name in the auth cookie
    /// </summary>
    public static string Name => "Librogram";

    /// <summary>
    /// Claim name to store the controller name in the auth cookie (IF NECESSARY)
    /// </summary>
    public static string Controller => "controller";

    /// <summary>
    /// Claim name to store the user company name in the auth cookie
    /// </summary>
    public static string CorporateName => "company";
        
    /// <summary>
    /// Claim name to store the user ID in the auth cookie
    /// </summary>
    public static string UserId => "userid";
}