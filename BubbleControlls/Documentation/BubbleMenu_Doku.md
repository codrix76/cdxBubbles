
# BubbleMenu – Komponentendokumentation

## Übersicht
`BubbleMenu` ist ein zentrales Steuerelement zur Darstellung eines bogenförmig oder orbital strukturierten Menüs aus interaktiven `Bubble`-Elementen. Es unterstützt mehrstufige Navigation, Pfadanzeige, kontextbezogene Optionen sowie freie oder dockbare Ausrichtung am Bildschirmrand.

---

## Struktur

- **Typ:** `UserControl`
- **Namespace:** `BubbleControlls.ControlViews`
- **Datei:** `BubbleMenu.xaml` + `BubbleMenu.xaml.cs`
- **Visualisierung:** basiert auf drei `BubbleRingControl`-Instanzen
- **Datenhaltung:** über `BubbleMenuViewModel`, `BubbleMenuHandler` und `BubbleMenuItem`

---

## Komponenten im XAML

- `MainGrid` → Layoutcontainer
- `MenuCanvas` → Hauptfläche für Darstellung von Ringsystem und Bubbles

---

## Ringe & Ebenen

| Ring                | Zweck                        |
|---------------------|------------------------------|
| `_pathMenuRing`     | Menüpfad (zuvor geklickte Bubbles) |
| `_selectedMenuRing` | Aktuelle Submenüs            |
| `_additionalMenuRing` | Kontextmenüs (rechte Maustaste) |

Diese Ringe werden dynamisch positioniert und gestylt (über Theme).

---

## Wichtige DependencyProperties

| Property               | Typ                   | Beschreibung |
|------------------------|------------------------|-------------|
| `BubbleMenuStyle`      | `BubbleRenderStyle`    | 2D oder 3D-Stil der Bubbles |
| `BubbleMenuAlignment`  | `BubbleMenuAlignmentType` | Positionierung: z. B. TopLeft, LeftEdge, Free |
| `BubbleMainMenuSize`   | `double`               | Größe der zentralen MainBubble |
| `BubbleMenuBigSize`    | `double`               | Größe der SubBubbles |
| `BubbleMenuSmallSize`  | `double`               | Größe der Pfadbubbles |
| `BubbleMenuSpacing`    | `double`               | Abstand zwischen Bubble-Ebenen |

---

## Verhalten & Navigation

- Bei Hover auf das Menü wird der Ring eingeblendet (`MouseEnter`)
- Nach `MenuHideSeconds` Inaktivität → automatische Ausblendung (`DispatcherTimer`)
- Klick auf Bubbles:
  - **Links:** Menüstruktur verändern (Pfad, Submenüs)
  - **Rechts:** Kontextmenüs einblenden

---

## Menüaufbau (Main → Sub → Kontext)

- `MainMenu`: zentrale Einstiegspunkt-Bubble
- Navigation erfolgt durch Klick (Pfad erweitert sich oder springt zurück)
- Kontextmenüs (rechte Maustaste) werden temporär im äußeren Ring angezeigt

---

## Datenmodell

- **`BubbleMenuItem`**: Einzelelement mit Text, Icon, Aktionen, SubItems, ContextItems
- **`BubbleMenuHandler`**: Verwaltet Hauptstruktur und Navigation (Pfad, Auswahl, Kontext)
- **`BubbleMenuViewModel`**: Brücke zwischen UI und Modell inkl. Layoutberechnung

### Unterstützte Hierarchieebenen:
- Beliebig tief dank rekursiver `SubItems`
- Dynamische Inhalte durch `LoadSubItems` und `LoadContextItems`

---

## Ausrichtung & Layout

- Automatische Berechnung der Menüposition über `BubbleMenuAlignmentType`
- Zentrierte oder bogenförmige Darstellung möglich
- Berechnung über `UpdateAlignmentValues()` im ViewModel

---

## Nutzung (Beispiel)

```csharp
BubbleMenu menu = new();
menu.MainMenu = new BubbleMenuItem { Text = "Start" };
menu.MenuStyleTheme = BubbleVisualThemes.HudBlue();
menu.BubbleMenuAlignment = BubbleMenuAlignmentType.TopLeftCorner;
```

---

## Besondere Features

- Flexible Layoutlogik mit `BubbleAlignmentValues`
- Theme-basierte Gestaltung aller Ebenen
- Responsives Verhalten: Ringe blenden bei Mausbewegung automatisch ein/aus
- Unterstützung für ToolTips, Icons, Klick-Events und dynamische Inhalte

---

## Ziel und Nutzen

`BubbleMenu` eignet sich ideal als visuelles Hauptmenü für kreative Anwendungen, Spiele, Dashboards oder Tools mit symbolischer Navigation. Es bietet eine intuitive, visuell ansprechende Interaktionsebene jenseits klassischer Menüleisten.
