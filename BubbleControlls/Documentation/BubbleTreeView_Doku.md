# BubbleTreeView – Technische Dokumentation

## Übersicht

`BubbleTreeView` ist eine spezialisierte WPF-TreeView-Komponente mit flacher, linearer Darstellung auf einem `Canvas`. Sie verwendet `BubbleSwitch`-Elemente für visuelle Knoten und unterstützt Pfadnavigation, SubNodes, Eventhandling und visuelles Rendering per Linien.

---

## Hauptfunktionen

- Strukturierte Baumdarstellung mit:
  - **RootNode**
  - **ParentNodes**
  - **PfadNodes** (lineare Kette unterhalb eines Parents)
  - **SubNodes** (Kinder des letzten PfadElements oder direkt des Parents)
- Visuelle Darstellung mit:
  - **Indentation** (Einrückung)
  - **L-förmige Verbindungslinien**
  - **Vertikale Pfadverbindungen**
- Eingebettetes Eventsystem (`NodeClick`, `NodeExpanded` etc.)
- Dynamische Größenberechnung über `MeasureOverride`
- Stilisierung über `BubbleVisualTheme`

---

## Struktur und Layout

### Einrückung

- `Root`: ganz links
- `Parent`: `+1 * _horizontalStep`
- `PathNode`: `+2 * _horizontalStep`
- `SubNode`: `+2` oder `+3` je nach Pfad

### Vertikale Positionierung

- Positionierung erfolgt sequentiell durch `_lastPos.Y += Height + Spacing`

### Knoten-Typen

- `BubbleTreeViewItem` enthält `ID`, `Label`, `Parent`, `Children`
- `BubbleSwitch` ist die UI-Darstellung pro Knoten
- `SwitchInfo` (in `.Tag`) verbindet UI mit Logik

---

## Zeichnen der Verbindungen

### In `OnRender(...)`

1. **Root → Parent** → L-förmig, `horizontalFirst = false`
2. **Parent → erster PfadNode** → L-förmig, `horizontalFirst = true`
3. **PathNode → PathNode** → vertikal
4. **(Letzter) PfadNode → SubNodes** → L-förmig, `horizontalFirst = false`
5. **Falls kein Pfad existiert: Parent → SubNodes**

---

## Interaktion

### Events

- `NodeClick`, `NodeRightClick`: beim Klicken der `BubbleSwitch`
- `NodeExpanded`, `NodeCollapsed`: bei Toggle über `SwitchToggle`

### Interne Logik

- `SwitchToggle` analysiert die Tiefe (`lvl`) und passt den Pfad (`_pathList`) an
- `RebuildTree()` baut die gesamte sichtbare Darstellung neu auf
- Alle sichtbaren Knoten landen in `_keepList`, alte werden entfernt

---

## Erweiterbarkeit

- Neue Knoten via `Root.Add(...)`
- Farben pro Knoten via `CustomColor`
- Export-, Analyse- oder Suchfunktionen können einfach hinzugefügt werden

---

## Technische Besonderheiten

- Zeichnen via `DrawingContext` (kein Canvas-Element für Linien)
- Absolute Kontrolle über Position und Größe (manuelles Layout)
- Ereignisbindung über Tag-ID (`SwitchInfo`)
- Optimierte Redraw-Logik mit `EnsureSwitch()` + `InvalidateVisual()`

---

## Hinweise

- `UpdateLayout()` nach Baumänderungen ist wichtig, damit Linien korrekt gezeichnet werden
- `ActualWidth/Height` des Canvas ist entscheidend für `OnRender`

---
## Verwendung
Bespielsweise in xaml
<Border Width="300" Height="300"
        Margin="2"
        BorderBrush="Gray" BorderThickness="1">
    <ScrollViewer >
        <controlViews:BubbleTreeView 
            x:Name="bblTree"/>
    </ScrollViewer>
</Border>

BubbleTreeView selber hat keinen Rand und passt sich automatisch seinem Inhalt an. Verwednung auf dieser Art wird es korrekt mir Rand und Scrollbalken angezeigt.

© aktuelle Version: automatisch dokumentiert