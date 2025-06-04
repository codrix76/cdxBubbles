# cdx.Bubbles – Vision & Umsetzung

Dieses Dokument vereint die konzeptionelle Vision des Projekts (ehemals „cdx.Orbis“) mit der konkreten technischen Umsetzung der `Bubble`-Komponente.

---

## 🌟 Vision: cdx.Bubbles

# cdx.Bubbles – Open your mind for future

**Projektstart:** Mai 2025  
**Vision:** Ein symbolisches, zustandsorientiertes Menüsystem – neu gedacht, frei navigierbar, visuell und funktional vereint.  
**Slogan:** _Open your mind for future_

---

## 🎯 Ursprungsidee

Die Idee entstand aus der Unzufriedenheit mit klassischen Menüs:
- Zu viele Einträge
- Unübersichtliche Listen
- Keine inhaltliche Führung
- Kaum emotionale Wirkung

Ziel war ein Menü, das nicht nur Optionen zeigt, sondern **Entscheidungspfade sichtbar macht**.  
Inspiriert von Tools wie IntelliJ, Visual Studio, Iron-Man-HUDs und neuronaler Navigation.

---

## 🧠 Designphilosophie

- **Pfad statt Liste:** Der Nutzer geht durch Entscheidungen, nicht durch Menüpunkte.
- **Symbolik statt Fließtext:** Menüeinträge bestehen aus Icons mit Tooltips und optional Text.
- **Zustandsgesteuert:** Der Kontext entscheidet, welche Optionen erscheinen.
- **Ästhetik mit Bedeutung:** Das Menü ist ein Erlebnis, kein Verwaltungswerkzeug.
- **Maximale Offenheit:** Entwickler entscheiden selbst, wie tief oder flach das Menü wird.

---

## 🌀 Visuelle Inspirationen

- **Golden Ratio Spirale** als Menüfluss
- **Mandelbrotstruktur** für Verzweigungen und Pfade
- **Orbitale Navigation:** Startpunkt als Sonne, Funktionen als Planeten und Monde

---

## 🧩 Technische Umsetzung

### Struktur:

- `cdx.Orbis` (WPF UserControl Bibliothek)
- `cdx.OrbisDemoApp` (Testumgebung)
- `.gitignore`, `.sln`, `README.md`, `LICENSE`

### Kernkomponenten:

- `OrbisMenuControl`: Das visuelle Control
- `OrbisMenuNode`: Datenmodell für Menüpunkte
- `OrbisStateController`: Steuert Zustände und Navigation
- Symbolische Navigation + Shortcuts (z. B. ALT+P → L → N → A)

### Besonderheiten:

- Nur der aktuelle Kontext + nächster logischer Schritt sichtbar
- Reduzierte Hauptmenüs (z. B. Projekt, Node, Analyse)
- Unterstützt Touch, Maus, Keyboard (Multimodal)
- Optional: Toggle/Checkbox-Elemente

---

## 📦 Verteilung & Offenheit

- **GitHub Repo:** öffentlich, MIT-Lizenz
- **Geplant:** Bereitstellung als NuGet-Paket
- **Ziel:** Open Source Beitrag für kreative Entwickler, Designer, UX-Denker

---

## 📣 Zielgruppe

- Entwickler:innen, die keine Menülisten mehr wollen
- Designer:innen, die Struktur fühlen wollen
- Menschen, die mit Software **denken** wollen – nicht nur klicken

---

## 🔮 Langfristige Vision

- Optionales Rendering-Backend (OpenGL oder Unity)
- Integration als „Launcher-Modul“ für cdx.Projekte
- UX-Benchmark für symbolbasierte Systeme

---

## ✍️ Notiz aus der Genesis

> „Ein Menü ist kein Dogma – es ist ein Angebot.  
> Der Nutzer entscheidet, ob er es missbraucht oder meistert.“  
> – Codrix & ChatGPT, Mai 2025

---

## 🛠 Umsetzung: Die `Bubble`-Komponente

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

