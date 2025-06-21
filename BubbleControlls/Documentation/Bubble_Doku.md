# Bubble – Komponentendokumentation

## Übersicht
`Bubble` ist ein vollständig themenfähiges, interaktives UI-Control in WPF, das als kreis- oder kapselartige Schaltfläche dient. Es ist zentraler Bestandteil des `cdx.Bubbles`-Projekts und eignet sich für Menüeinträge, Statussymbole oder interaktive Steuerelemente.

---

## Technische Struktur

### Dateitypen
- **XAML:** `Bubble.xaml` (Layoutdefinition)
- **Code-Behind:** `Bubble.xaml.cs` (Logik und Effekte)

### Typ
- `UserControl`
- Namespace: `BubbleControlls.ControlViews`

---

## Visuelle Struktur

- `Grid` als Wurzel mit transparentem Hintergrund
- `OuterBorder`: äußerer Rahmen mit CornerRadius
- `InnerBorder`: innerer Rahmen, optisch abgesetzt
- `Grid` (`ContentGrid`) für Text und Icon
- `TextBlock` (`BubbleText`)
- `Image` (`IconImage`)

---

## Interaktive Features

- Fokusverhalten mit Glow-Effekt (`OnGotFocus`, `OnLostFocus`)
- Hover-Schatten per `DropShadowEffect`
- Klick-Animation (Verkleinerung durch `ScaleTransform`)
- Rechts- und Linksklick getrennt behandelt
- ToolTip via `ToolTipText`-Property

---

## Layoutlogik

- Automatische CornerRadius-Anpassung bei Größenänderung
- Dynamische Grid-Layouts je nach `TextIconLayout`:
  - `IconAboveText` (Standard)
  - `IconLeftOfText`
  - `Text` oder `Icon` alleine
- Abstand und MinWidth werden intelligent berechnet

---

## Zentrale DependencyProperties (DP)

### Inhalte
- `Text`
- `Icon`
- `TextIconLayout` (Enum)

### Farben & Darstellung
- `BackgroundBrush`
- `OuterBorderBrush`
- `BorderBrushInner`
- je mit:
  - `HighlightColor`
  - `DarkColor`

### Schrift
- `FontFamilyName`, `FontSizeValue`, `FontWeightValue`, `FontStyleValue`
- `Foreground` via Theme gesetzt

### Rahmen
- `BorderDistance` (Offset zw. Outer/Inner)
- `OuterBorderThickness`
- `InnerBorderThickness`

### Stil
- `RenderStyle` (`StylePlane` oder `Style3D`)
- 3D-Stil verwendet:
  - `RadialGradientBrush` für Background
  - `LinearGradientBrush` für Rahmen

---

## Methoden

- `ApplyTheme(BubbleVisualTheme theme)`:
  - Setzt alle Properties gemäß Theme
  - Zentrale Methode zur visuellen Steuerung

- `ActivateGlow()` / `DeactivateGlow()`:
  - Steuerung des Fokus-Leuchteffekts

---

## Beispielverwendung

```xaml
<ctrls:Bubble
    Text="Starte Analyse"
    Icon="Assets/Analyse.png"
    TextIconLayout="IconAboveText"
    RenderStyle="Style3D"
    ToolTipText="Startet die Analyse"
    OuterBorderBrush="DarkBlue"
    BackgroundBrush="MidnightBlue"
    BorderBrushInner="SteelBlue" />
```

```csharp
myBubble.ApplyTheme(BubbleVisualThemes.NeonEdge());
```

---

## Besonderheiten

- Automatische Layoutanpassung bei Text/Icon-Änderung
- Designer-Unterstützung mit `d:DesignInstance`
- Optimal für transparente, rahmenlose Anwendungen
- HitTest auf InnerBorder ermöglicht exakte Interaktionen

---

## Ziel und Nutzen

Die `Bubble`-Komponente ist ein vielseitig einsetzbares UI-Baustein für moderne, symbolbasierte Benutzeroberflächen – ideal für Menüs, Aktionen und Statusanzeigen mit klarer, futuristischer Optik.
