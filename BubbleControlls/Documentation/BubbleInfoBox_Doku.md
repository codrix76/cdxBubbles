
# BubbleInfoBox – Komponentendokumentation

## Übersicht
`BubbleInfoBox` ist ein spezialisiertes, visuell anpassbares WPF-Fenster zur Anzeige kontextsensitiver Informationen. Es ist Bestandteil der `BubbleControlls`-Bibliothek und zeichnet sich durch ein anpassbares Aussehen, flexible Themes und klare visuelle Struktur aus.

---

## Komponentenstruktur

### `BubbleInfoBox.xaml`
- **Typ:** `Window`
- **Größe:** über `BoxSizeToContent` steuerbar
- **Transparenz:** `AllowsTransparency=True`, `Background=Transparent`
- **Struktur:**
  - `Grid` als Layout-Root
  - `Rectangle` als klickbare Fläche (Platzhalter)
  - `OuterBorder` (`Border`) – äußerer Rahmen
  - `InnerBorder` (`Border`) – innerer Rahmen mit Hintergrund + Text
  - `TextBlock` – Anzeige für `DisplayText`

### Bindings (alle via `RelativeSource AncestorType=Window`):
- Visuelle Eigenschaften (`Brush`, `Font`, `Thickness`, `Opacity`, etc.)
- Text- und Layoutsteuerung

---

## Zentrale DependencyProperties (DP)

### Farben und Effekte
- `OuterBorderBrush`
- `OuterBorderEffectBrush`
- `InnerBorderBrush`
- `InnerBorderEffectBrush`
- `BackgroundBrush`
- `BackgroundEffectBrush`
- `Foreground`
- `BackgroundOpacity`

### Schrift
- `FontFamily`, `FontSize`, `FontWeight`, `FontStyle`, `TextAlignment`

### Rahmen und Layout
- `BorderThicknessOuter`, `BorderThicknessInner`
- `BorderOffset` (Abstand zwischen Outer/InnerBorder)
- `BoxSizeToContent` (WPF `SizeToContent`)
- `TextMargin` (Innenabstand für Textblock)

### Inhalt
- `DisplayText`

---

## Theme-Anbindung

Die Methode `ApplyTheme(BubbleVisualTheme theme)` übernimmt gezielt Einstellungen aus der `BubbleInfoBoxVisuals`-Sektion sowie gemeinsame Schriftdefinitionen aus `BubbleVisuals`. 

Spezielle Highlight-Farben (z. B. für plastische Effekte) werden ebenfalls angewendet und in GradientBrushes umgewandelt.

---

## Dynamische Logik

### Hintergrund-Rendering
- Methode `UpdateBackgroundBrush(Brush)` erzeugt automatisch einen abgestuften `LinearGradientBrush` auf Basis von `BackgroundBrush` und `BackgroundEffectBrush`.

### Rahmen-Rendering
- `CreateOuterBorderBrush()` und `CreateInnerBorderBrush()` erzeugen lineare Verläufe.

### Initiallogik
- `ApplyAllVisuals()` setzt bei `Loaded`-Event alle DP-basierten UI-Zustände korrekt.

---

## Nutzungsbeispiel

```csharp
var infoBox = new BubbleInfoBox();
infoBox.DisplayText = "Dies ist ein Hinweistext.";
infoBox.ApplyTheme(BubbleVisualThemes.Standard());
infoBox.Show();
```

---

## Ziel und Nutzen
Die `BubbleInfoBox` ist ideal für visuelle Tooltips, detailreiche Kontextinformationen oder statusbezogene UI-Overlays. Dank Theme-Unterstützung lässt sie sich flexibel in jedes UI-Design integrieren.
