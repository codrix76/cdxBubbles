
# BubbleRingControl – Komponentendokumentation

## Übersicht
`BubbleRingControl` ist ein erweitertes WPF-Canvas-Control zur **platzsparenden, elliptischen Anordnung von UI-Elementen entlang eines Rings**. Es dient als visuelle Trägerbahn für `Bubble`-Elemente oder andere beliebige `UIElement`s und unterstützt **Scrollverhalten, Gloweffekt, Ausrichtung und Themes**.

---

## Technische Struktur

- **Typ:** `Canvas`
- **Namespace:** `BubbleControlls.ControlViews`
- **Datei:** `BubbleRingControl.cs`

---

## Hauptfunktion
Das Control rendert einen elliptischen Ring mit einstellbarem Innen- und Außenradius, Rotation und Segmentierung (Start-/Endwinkel) und platziert darin UI-Elemente. Es berechnet dynamisch die Scrollbarkeit (inkl. Hitboxen für Scrollpfeile) und unterstützt Glow- und Theme-Effekte.

---

## Visuelle Eigenschaften (DependencyProperties)

| Property                  | Typ       | Beschreibung |
|--------------------------|-----------|--------------|
| `RingBackground`         | `Brush`   | Füllung der Ringbahn |
| `RingBorderBrush`        | `Brush`   | Rahmenfarbe der Bahn |
| `RingOpacity`            | `int`     | Transparenz der Füllung |
| `RingBorderOpacity`      | `int`     | Transparenz des Rands |
| `RingBorderThickness`    | `int`     | Stärke des Rahmens |
| `ScrollArrowHeight`      | `double`  | Höhe der Scrollpfeile |
| `Center`                 | `Point`   | Zentrum der Bahn |
| `RadiusX`, `RadiusY`     | `double`  | Halbachsen der Ellipse |
| `StartAngle`, `EndAngle` | `double`  | Sichtbarer Bereich (in Grad) |
| `RingRotation`           | `double`  | Rotation der gesamten Bahn |
| `PathWidth`              | `double`  | Breite des sichtbaren Rings |
| `IsGlowActive`           | `bool`    | Leuchteffekt bei Hover |
| `ElementDistance`        | `double`  | Abstand zwischen Elementen auf der Bahn |

---

## Verhalten und Logik

### Interne Steuerung

| Feld/Property      | Beschreibung |
|--------------------|--------------|
| `_elements`        | Aktuelle Elemente auf dem Ring |
| `_positions`       | Platzierungsdaten berechnet durch `BubblePlacer` |
| `ScrollOffset`     | Aktuelle Scrollposition |
| `_scrollTarget`    | Zielscrollwert für sanftes Scrolling |
| `_canScroll`       | Wird durch `UpdateScrollLimits()` ermittelt |
| `IsCentered`       | Sorgt bei wenigen Elementen für symmetrische Darstellung |
| `IsInverted`       | Dreht Scrollrichtung und Sortierung um |

---

## Wichtige Methoden

| Methode                  | Funktion |
|--------------------------|----------|
| `AddElements()`          | Fügt UIElemente dem Ring hinzu |
| `RemoveElements()`       | Entfernt alle Elemente |
| `AdjustPlacement()`      | Platziert Elemente neu (nach Größenberechnung) |
| `UpdateScrollLimits()`   | Berechnet Scrollbereich und ermöglicht Zentrierung |
| `ApplyTheme()`           | Wendet Werte aus `BubbleVisualTheme.BubbleRingVisuals` an |
| `Refresh()`              | Erzwingt Neuberechnung bei externen Änderungen |

---

## Zeichenlogik

Das eigentliche Rendering erfolgt in `OnRender` mit Unterstützung durch:
- `BubbleRingRenderer.DrawRing()` für Ringdarstellung
- `DrawGlow()` bei aktivem Hovereffekt
- `DrawArrow()` für Scrollpfeile

---

## Nutzung mit Bubble-Komponenten

```csharp
var ring = new BubbleRingControl
{
    RadiusX = 200,
    RadiusY = 150,
    StartAngle = 90,
    EndAngle = 270,
    Center = new Point(250, 250)
};

ring.AddElements(new List<UIElement>
{
    new Bubble { Text = "Projekt" },
    new Bubble { Text = "Analyse" },
    new Bubble { Text = "Ergebnisse" }
});

ring.ApplyTheme(BubbleVisualThemes.Dark());
```

---

## Besonderheiten

- Mausradsteuerung zum Scrollen
- Hover-Glow per `IsGlowActive`
- Scroll-Pfeile mit Hitbox-Erkennung
- Animiertes Scrollen durch `_scrollTarget` und `ScrollOffset`
- Berechnet automatisch Sichtbarkeit vs. Inhalt (z. B. bei zu wenigen Elementen)

---

## Ziel und Nutzen

Ideal für symbolbasierte Menüs, orbitale UI-Navigation und visuelle Pfadsysteme. `BubbleRingControl` ist **eine der Kernkomponenten** für die Umsetzung der Bubble-Menüvision.

