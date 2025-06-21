# BubbleMsgBox – Dokumentation (mit Theme-Unterstützung)

Die `BubbleMsgBox` ist eine stilisierte, visuell moderne Ersatzkomponente für die klassische `MessageBox` in WPF-Anwendungen. Sie basiert auf dem Bubble-System und verwendet `BubbleInfoControl` zur Anzeige von Text + Icon sowie `Bubble`-Elemente als Schaltflächen.

---

## 🧩 Struktur & Bestandteile

- **Typ:** `Window`
- **Namespace:** `BubbleControlls.ControlViews`
- **Basis-Komponenten:**
  - `BubbleInfoControl` (Text + Icon)
  - `Bubble` (Buttons)
- **Themefähig:** ja, via `ApplyTheme(BubbleVisualTheme)`

---

## 🧱 Layoutstruktur

Die `BubbleMsgBox` besteht aus drei Hauptbereichen:

1. **Titelzeile:**  
   - Hintergrundfarbe und Schriftfarbe werden durch Theme gesteuert (`TitleBackground`, `Foreground`)
   - Kann per Drag mit der Maus verschoben werden

2. **Inhaltsbereich:**  
   - Zeigt eine `BubbleInfoControl` mit Text + optionalem Icon
   - Wird thematisch mitgestylt über `ApplyTheme`

3. **Buttonbereich:**  
   - Rechtsbündig, horizontale `Bubble`-Schaltflächen
   - Theme wird auf jeden Button einzeln angewendet

---

## 🎨 Theme-Unterstützung

Die `BubbleMsgBox` unterstützt die Zuweisung eines vollständigen `BubbleVisualTheme`-Objekts. Dabei werden folgende Werte aus dem Theme angewendet:

| Zielbereich         | Theme-Property                    |
|---------------------|-----------------------------------|
| Fensterhintergrund  | `BubbleVisualBase.Background`     |
| Titelzeile (BG)     | `BubbleVisualBase.TitleBackground`|
| Titeltext           | `BubbleVisualBase.Foreground`     |
| InfoContent         | `BubbleInfoControl.ApplyTheme(...)` |
| Buttons             | `Bubble.ApplyTheme(...)` je Button |

---

## ⚙️ API & Methoden

### `static MessageBoxResult Show(...)`

Die zentrale Methode zum Anzeigen der BubbleMsgBox. Es gibt folgende Überladungen:

```csharp
BubbleMsgBox.Show(string message);
BubbleMsgBox.Show(string message, string caption);
BubbleMsgBox.Show(string message, string caption, MessageBoxButton buttons);
BubbleMsgBox.Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon);
BubbleMsgBox.Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon, Window? owner);
BubbleMsgBox.Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon, Window? owner, BubbleVisualTheme? theme);
```

---

### `ApplyTheme(BubbleVisualTheme theme)`

Diese Methode:

- wendet das Theme auf die `BubbleInfoControl` an
- setzt Farben der Titelzeile
- stylt jeden Button neu mit dem übergebenen Theme

---

## 📦 Assets

Icons für `MessageBoxImage` müssen bereitgestellt werden unter:

```
Assets/
├─ info.png
├─ warning.png
├─ error.png
└─ question.png
```

Verwendung im Theme via:

```csharp
MessageBoxImage.Warning => "../Assets/warning.png"
```

---

## 🧪 Beispiel

```csharp
var theme = BubbleVisualThemes.HudBlue();

var result = BubbleMsgBox.Show(
    "Möchten Sie fortfahren?",
    "Frage",
    MessageBoxButton.YesNoCancel,
    MessageBoxImage.Question,
    null,
    theme
);

if (result == MessageBoxResult.Yes)
{
    // ...
}
```

---

## 📌 Verhalten & Besonderheiten

- Fenster ist `Topmost`, `Transparent`, `SizeToContent`
- Positioniert sich automatisch in der Bildschirmmitte
- Keine Systemrahmen, kein Taskbar-Eintrag
- Mausverschiebung über Titelzeile
- Buttons reagieren mit Bubble-typischem Hover & Glow
