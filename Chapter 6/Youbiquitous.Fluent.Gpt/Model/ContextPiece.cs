///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


namespace Youbiquitous.Fluent.Gpt.Model;

/// <summary>
/// Class describing a context piece, to be provided to some prompt
/// </summary>
public class ContextPiece
{
    /// <summary>
    /// Full text of the context piece
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Summary of the context piece
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Some kind of reference for this context piece
    /// </summary>
    public string? Reference { get; set; }



    /// <summary>
    /// Returns a standard string version of the context piece
    /// </summary>
    public override string ToString()
    {
        return Text;
    }

    /// <summary>
    /// Returns a shorter version of the context piece
    /// </summary>
    public string? ToShortString()
    {
        return Summary;
    }
}