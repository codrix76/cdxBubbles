# Bubble

**Typ:** UserControl\
**Namespace:** BubbleControlls.ControlViews

## Zweck

Das `Bubble`-Control ist ein visuell und funktional vielseitiges UI-Element in Kreis- oder Kapsel-Form, das als Basis für Buttons, Menüpunkte oder Spezialdarstellungen dient. Es unterstützt Text, Icon, Hover-, Fokus- und Klick-Effekte und kann vollständig über Themes stilisiert werden.

***

## Eigenschaften

### Text & Darstellung

* `Text`: Anzeigeinhalt

* `Icon`: Bildsymbol

* `TextIconLayout`: Positionierung von Text und Icon (Auto, IconAboveText, IconLeftOfText)

* `FontFamilyName`, `FontSizeValue`, `FontWeightValue`, `FontStyleValue`: Textdarstellung

### Interaktivität & Fokus

* `Focusable`, `IsTabStop`: Fokusfähigkeit über Tastatur/Maus

* `MouseEnter`, `MouseLeave`, `MouseDown`, `MouseUp`, `GotFocus`, `LostFocus`: visuelle Reaktionen

* Fokus-Glow, Klick-Skalierung, Hover-Effekt

### Layout & Struktur

* `OuterBorder` und `InnerBorder`: zwei verschachtelte Rahmen mit eigenem Styling

* Dynamisches Größenverhalten: Bubble ist standardmäßig kreisförmig, dehnt sich bei langem Text zur Kapsel

* `CornerRadius` wird automatisch basierend auf Höhe berechnet

### Farben & Brushes

* `BackgroundBrush`, `BorderBrushInner`, `OuterBorderBrush`: Grundfarben

* `Foreground`: Textfarbe

* **jeweils mit:**

  * `HighlightColor`

  * `DarkColor`

* Insgesamt 6 zusätzliche Farbproperties für plastische 3D-Darstellung:

  * `BackgroundHighlightColor`, `BackgroundDarkColor`

  * `InnerBorderHighlightColor`, `InnerBorderDarkColor`

  * `OuterBorderHighlightColor`, `OuterBorderDarkColor`

### Ränder

* `BorderDistance`: Abstand zwischen Außen- und Innenrahmen

* `OuterBorderThickness`, `InnerBorderThickness`: Dicke beider Rahmen

### Stil & Visualisierung

* `RenderStyle`: Umschaltung zwischen `StylePlane` (flach) und `Style3D` (plastisch)

* 3D-Stil wird durch `RadialGradientBrush` (für Background) und `LinearGradientBrush` (für Border) erzeugt

### Tooltip

* `ToolTipText`: Standard-WPF-Tooltip am UserControl über `ToolTipService`

### Theme-Unterstützung

* Methode `ApplyTheme(BubbleVisualTheme theme)` zur vollständigen Stilzuweisung

* Kompatibel mit Theme-Presets wie `HudBlue`, `Dark`, `NeonEdge`, `EclipseCore`

***

## Besondere Merkmale

* Unterstützt Designer-Vorschau durch `d:DesignInstance`

* `HitTestVisible` aktiv, um Interaktionen im transparenten Bereich sicherzustellen

* Effektlogik wie Fokus-Glow und Hover-Schatten direkt in `.xaml.cs` realisiert

* Optimal vorbereitet für transparente, rahmenlose Fenster (z. B. Launcher-UI oder Overlay-Menüs)

***

## Verwendung

```xaml
<ctrls:Bubble
    Text="Analyse starten"
    Icon="Assets/Analyse.png"
    ToolTipText="Startet die Analyse"
    TextIconLayout="IconAboveText"
    RenderStyle="Style3D"
    OuterBorderBrush="DarkBlue"
    BackgroundBrush="MidnightBlue"
    BorderBrushInner="SteelBlue" />
```

```csharp
myBubble.ApplyTheme(BubbleVisualThemes.NeonEdge());
```

***

## Status

Die aktuelle Version markiert den funktionalen Abschluss der Bubble-Komponente: Modular, themenfähig, visuell ausgereift und einsatzbereit für jede UI-Situation.
