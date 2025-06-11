namespace BubbleControlls.Models;

/// <summary>
/// Gibt an, wie die Elemente auf der Laufbahn im sichtbaren Bereich ausgerichtet werden.
/// </summary>
public enum BubbleTrackAlignment
{
    /// <summary>
    /// Laufbahn zentriert im sichtbaren Bereich.
    /// </summary>
    Center,

    /// <summary>
    /// Laufbahn beginnt am Anfang des sichtbaren Bereichs.
    /// </summary>
    Start,

    /// <summary>
    /// Laufbahn endet am Ende des sichtbaren Bereichs (rückwärts ausgerichtet).
    /// </summary>
    End
}