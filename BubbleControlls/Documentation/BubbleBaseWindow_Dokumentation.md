
# BubbleBaseWindow – Dokumentation

## Übersicht

**Typ:** Basisklasse für transparente, themenfähige Fenster  
**Namespace:** `BubbleControlls.ControlViews`  
**Verwendung:** Als Grundlage für eigene, stilisierte Fenster innerhalb des `cdx.Bubbles`-Systems  
**Abhängigkeiten:** `BubbleVisualTheme`, `System.Windows`, `WPF`

---

## Zweck und Einordnung

Die `BubbleBaseWindow`-Klasse bildet die visuelle und strukturelle Grundlage für rahmenlose Fenster in der Bubble-Welt. Sie ersetzt das klassische WPF-Fenster durch ein hochgradig anpassbares Layout mit Fokus auf ästhetische Darstellung, Theme-Unterstützung und Interaktivität. Sie eignet sich für Anwendungen, die eine moderne, nicht-standardisierte Fensterdarstellung benötigen – etwa Tool-Overlays, Floating Menüs oder modulare UI-Komponenten.

---

## Aufbau

### Visuelle Struktur
- **Titelbereich:** Enthält Icon, Titeltext und Steuerbuttons (Minimieren, Maximieren, Schließen)
- **Inhaltsbereich:** Zwei verschachtelte `Border`-Elemente (`OuterBorder`, `InnerBorder`) für plastische Darstellung
- **Resize-Element:** Transparente rechteckige Fläche unten rechts, die manuell Größenänderung erlaubt
- **(Optional) Footer:** Ist vorbereitet, aber derzeit auskommentiert

### Code-Struktur
- Konstruktor initialisiert das komplette Layout manuell (keine XAML-Nutzung)
- Einzelne Hilfsmethoden wie `CreateOuterBorder()` kapseln visuelle Teilschritte
- Theme-Anpassung erfolgt über `ApplyTheme()`

---

## Funktionen & Eigenschaften

### Fensterverhalten
- `AllowsTransparency = true` + `WindowStyle = None`: macht das Fenster vollständig benutzerdefiniert
- `SizeToContent = Manual`: explizite Kontrolle über Größe
- `ResizeMode = CanResize`: erlaubt Größenänderung, auch wenn manuell gelöst

### Theme-Unterstützung
- `ApplyTheme(BubbleVisualTheme theme)` setzt Farben, Schriftarten, Rahmen, Verläufe
- `Show3D` steuert, ob 3D-Effekte per `LinearGradientBrush` dargestellt werden

### Titelzeile
- Draggable durch `MouseLeftButtonDown → DragMove()`
- Dynamische Buttons über `UpdateTitleButtons()` abhängig von `ShowMinMax`
- Steuericons (🗕, 🗖, ✖) sind Unicode-Textzeichen, keine Bildsymbole

### Inhalte
- Content-Zugriff über Property `BubbleContent`
- Kein eigenes Layout-Element für Inhalt – Übergabe an `ContentPresenter`

---
