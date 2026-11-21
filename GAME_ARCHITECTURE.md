# TRASH RUSH 3D — Game Architecture

## 1. Technical Foundation

| Component | Detail |
|---|---|
| **Unity Version** | 2021.3.40f1 LTS |
| **Render Pipeline** | Built-in (Standard) |
| **Input System** | Legacy InputManager |
| **Color Space** | Gamma (`m_ActiveColorSpace: 1`) |
| **Physics** | 3D Physics (PhysicsManager) |
| **UI Framework** | Unity UI (uGUI) + TextMeshPro 3.0.6 |
| **Scripting Backend** | Mono (.NET Standard 2.1, `apiCompatibilityLevel: 6`) |
| **Target Resolution** | 1920×1080 |
| **Target Platform** | Windows Standalone (x86_64) |

## 2. Existing Project State

The project is a **fresh Unity 2021.3 LTS project** with:
- Default `SampleScene` containing only a Main Camera and Directional Light.
- Default packages: TextMeshPro, Visual Scripting, Timeline, Test Framework, uGUI.
- No custom scripts, prefabs, materials, or assets.
- No scenes registered in EditorBuildSettings.
- No custom tags or layers defined.
- Default InputManager axes (Horizontal, Vertical, Fire1-3, Mouse X/Y, etc.).
- Standard physics settings (gravity -9.81).

## 3. Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     GAME MANAGER                        │
│            (State Machine - Singleton)                  │
│  States: MainMenu │ Playing │ Paused │ Won │ Lost      │
└───────┬──────────────┬──────────────┬───────────────────┘
        │              │              │
   ┌────▼────┐   ┌─────▼─────┐  ┌────▼────┐
   │  SCORE  │   │   TIMER   │  │  TRASH  │
   │ MANAGER │   │  MANAGER  │  │ MANAGER │
   └─────────┘   └───────────┘  └────┬────┘
                                     │
        ┌────────────────────────────┤
        │                            │
   ┌────▼────┐              ┌────────▼────────┐
   │  TRASH  │              │  RECYCLING BIN  │
   │  ITEM   │              │   (per bin)     │
   └─────────┘              └─────────────────┘

   ┌─────────────┐    ┌──────────────────┐
   │   PLAYER    │    │     CAMERA       │
   │ CONTROLLER  │───▶│   CONTROLLER     │
   └──────┬──────┘    └──────────────────┘
          │
   ┌──────▼──────┐
   │   PLAYER    │
   │ INTERACTION │
   └─────────────┘

   ┌─────────────┐    ┌──────────────────┐
   │  UI MANAGER │    │  AUDIO MANAGER   │
   │  (Screens)  │    │   (Singleton)    │
   └─────────────┘    └──────────────────┘
```

## 4. Scene Architecture

### Scene 1: `MainMenu`
- Main Menu UI Canvas
- Background environment (optional: blurred park view)
- MainMenuController.cs

### Scene 2: `GameScene`
- Park environment (terrain, props, decorations)
- Player character
- Recycling bins (4)
- Scattered trash items (12–20)
- Game UI Canvas (HUD, Pause, Victory, Game Over)
- Managers (GameManager, ScoreManager, TimerManager, TrashManager, AudioManager, UIManager)
- Lighting setup
- Camera

## 5. Script Architecture

### 5.1 Core Systems (`Assets/Scripts/Core/`)

#### `GameManager.cs` — Singleton
Central game state controller.
```
Responsibilities:
- Game state machine (Menu, Playing, Paused, Won, Lost)
- Scene loading
- Game initialization / restart
- Coordinate between managers
- Time.timeScale control for pause

Public API:
- StartGame()
- PauseGame() / ResumeGame()
- GameWon() / GameLost()
- RestartGame()
- ReturnToMenu()
- GameState CurrentState { get; }

Events:
- OnGameStateChanged(GameState newState)
```

#### `ScoreManager.cs` — Singleton
```
Responsibilities:
- Track current score
- Apply correct/incorrect sorting points
- Prevent duplicate scoring (track scored item IDs)

Public API:
- AddCorrectSort()     → +10 points
- AddIncorrectSort()   → -5 points
- int CurrentScore { get; }
- int CorrectSorts { get; }
- int IncorrectSorts { get; }
- ResetScore()

Events:
- OnScoreChanged(int newScore)
```

#### `TimerManager.cs` — Singleton
```
Responsibilities:
- Countdown timer (configurable, default 180s)
- Pause-aware (respects Time.timeScale)
- Notify when time expires

Public API:
- StartTimer(float duration)
- float TimeRemaining { get; }
- bool IsRunning { get; }
- ResetTimer()

Events:
- OnTimerUpdate(float timeRemaining)
- OnTimerExpired()
```

### 5.2 Player Systems (`Assets/Scripts/Player/`)

#### `PlayerController.cs`
```
Responsibilities:
- WASD movement using CharacterController
- Gravity handling
- Smooth movement with configurable speed
- Ground detection
- Respect game state (no movement when paused)

Configuration:
- moveSpeed: 6f
- gravity: -20f

Dependencies:
- CharacterController (Unity component)
- GameManager (check state)
```

#### `CameraController.cs`
```
Responsibilities:
- Third-person follow camera
- Mouse X/Y input for orbit
- Vertical angle clamping
- Smooth follow with offset
- Cursor lock management

Configuration:
- offset: Vector3(0, 4, -6)
- mouseSensitivity: 2f
- minVerticalAngle: -20f
- maxVerticalAngle: 60f
- smoothSpeed: 10f

Dependencies:
- Player Transform (target)
```

#### `PlayerInteraction.cs`
```
Responsibilities:
- Detect nearby interactable objects (trash, bins)
- Handle E key press
- Pickup: attach trash to carry point
- Deposit: transfer carried trash to bin
- Manage carry state (one item at a time)
- Trigger interaction prompts

Configuration:
- interactionRange: 2.5f
- carryPoint: Transform (child of player)

Dependencies:
- TrashItem, RecyclingBin (detected via trigger/raycast)
- UIManager (interaction prompts)
- ScoreManager (scoring on deposit)
```

### 5.3 Trash Systems (`Assets/Scripts/Trash/`)

#### `TrashItem.cs` — MonoBehaviour (on each trash prefab)
```
Responsibilities:
- Define waste category (enum: Plastic, Paper, Metal, Organic)
- Track collection state (Available, Carried, Sorted)
- Unique ID for duplicate prevention
- Visual state changes

Properties:
- WasteCategory category
- TrashState state
- int itemId (unique)

Public API:
- Collect() → change state, disable collider
- Deposit() → change state
- ReturnToWorld() → reset if incorrectly sorted (optional)
```

#### `TrashManager.cs` — Singleton
```
Responsibilities:
- Register all trash items in scene
- Track total, collected, correctly sorted, remaining
- Detect win condition (all sorted correctly)

Public API:
- int TotalTrash { get; }
- int TrashRemaining { get; }
- int TrashSorted { get; }
- RegisterTrashItem(TrashItem item)
- OnTrashSorted(TrashItem item)
- bool AllTrashSorted { get; }

Events:
- OnTrashCountChanged(int remaining, int sorted)
- OnAllTrashSorted()
```

### 5.4 Recycling Systems (`Assets/Scripts/Recycling/`)

#### `RecyclingBin.cs` — MonoBehaviour (on each bin prefab)
```
Responsibilities:
- Define accepted waste category
- Validate deposited items
- Visual feedback on correct/incorrect deposit
- Communicate result to ScoreManager

Properties:
- WasteCategory acceptedCategory
- Color binColor

Public API:
- bool TryDeposit(TrashItem item) → returns true if correct category
```

### 5.5 UI Systems (`Assets/Scripts/UI/`)

#### `UIManager.cs` — Singleton
```
Responsibilities:
- Manage all UI panels (HUD, Pause, Victory, GameOver)
- Update HUD elements (score, timer, trash count)
- Show/hide interaction prompts
- Listen to manager events

Panels:
- HUD Panel
- Pause Panel
- Victory Panel
- Game Over Panel
- Interaction Prompt

Dependencies:
- ScoreManager, TimerManager, TrashManager, GameManager
- TextMeshPro for text elements
```

#### `MainMenuController.cs`
```
Responsibilities:
- Play button → load GameScene
- Instructions button → show instructions panel
- Quit button → Application.Quit()
```

#### `PauseMenuController.cs`
```
Responsibilities:
- ESC key detection
- Resume → unpause
- Restart → reload GameScene
- Main Menu → load MainMenu scene
```

#### `GameOverController.cs`
```
Responsibilities:
- Display final score and stats
- Restart button
- Main Menu button
```

### 5.6 Audio Systems (`Assets/Scripts/Audio/`)

#### `AudioManager.cs` — Singleton (DontDestroyOnLoad)
```
Responsibilities:
- Play background music (loop)
- Play SFX (one-shot)
- Volume control (master, music, SFX)
- Persist across scenes

Audio Clips:
- bgMusic
- pickupSFX
- correctSortSFX
- incorrectSortSFX
- buttonClickSFX
- victorySFX
- gameOverSFX

Public API:
- PlayMusic(AudioClip clip)
- StopMusic()
- PlaySFX(AudioClip clip)
- SetMusicVolume(float vol)
- SetSFXVolume(float vol)
```

## 6. Data Model

### Enums

```csharp
public enum WasteCategory
{
    Plastic,
    Paper,
    Metal,
    Organic
}

public enum TrashState
{
    Available,    // In the world, can be picked up
    Carried,      // Attached to player
    Sorted        // Deposited in a bin (correct or incorrect)
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Won,
    Lost
}
```

## 7. Communication Patterns

### Event-Driven Architecture

Managers communicate through C# events/delegates (not Unity Messages) for loose coupling:

```
ScoreManager.OnScoreChanged  →  UIManager updates score display
TimerManager.OnTimerUpdate   →  UIManager updates timer display
TimerManager.OnTimerExpired  →  GameManager triggers GameLost
TrashManager.OnAllTrashSorted → GameManager triggers GameWon
GameManager.OnGameStateChanged → UIManager shows/hides panels
                                → PlayerController enables/disables input
                                → TimerManager pauses/resumes
```

### Singleton Access Pattern

All managers use a simple `Instance` singleton pattern:
```csharp
public static T Instance { get; private set; }
void Awake() {
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
}
```

AudioManager additionally uses `DontDestroyOnLoad` to persist across scenes.

## 8. Interaction System Design

```
Player approaches trash (within interactionRange)
    → Raycast/OverlapSphere detects TrashItem
    → UIManager shows "Press E to pick up [ItemName]"
    → Player presses E
        → If not carrying anything:
            → TrashItem.Collect()
            → Parent trash to carryPoint
            → UIManager hides prompt

Player approaches bin (within interactionRange)
    → Raycast/OverlapSphere detects RecyclingBin
    → UIManager shows "Press E to deposit in [BinCategory] bin"
    → Player presses E
        → If carrying trash:
            → RecyclingBin.TryDeposit(carriedTrash)
            → If correct: ScoreManager.AddCorrectSort(), play correct SFX
            → If incorrect: ScoreManager.AddIncorrectSort(), play incorrect SFX
            → TrashItem.Deposit()
            → Destroy or disable trash object
            → TrashManager.OnTrashSorted()
            → Check win condition
```

## 9. Physics & Collision Setup

| Object | Collider | Layer | Tag |
|---|---|---|---|
| Player | CharacterController (capsule) | Default | Player |
| Trash Items | BoxCollider (trigger) | Interactable | Trash |
| Recycling Bins | BoxCollider (trigger) | Interactable | RecyclingBin |
| Environment | MeshCollider / BoxCollider | Default | — |
| Ground/Terrain | MeshCollider | Default | — |

### Required Tags:
- `Player`
- `Trash`
- `RecyclingBin`

### Required Layers:
- `Interactable` (Layer 6) — for trash and bins

## 10. Folder Structure

```
Assets/
├── Art/
│   ├── Models/          ← Procedural model prefab sources
│   ├── Textures/        ← Any textures if needed
│   └── Materials/       ← Shared materials (grass, path, plastic, paper, metal, organic, bin colors)
├── Audio/
│   ├── Music/           ← Background music clips
│   └── SFX/             ← Sound effect clips
├── Prefabs/
│   ├── Player/          ← Player character prefab
│   ├── Trash/           ← TrashItem prefabs (bottle, paper, can, organic)
│   ├── RecyclingBins/   ← RecyclingBin prefabs (4 colored bins)
│   ├── Environment/     ← Trees, benches, lamps, decorations
│   └── UI/              ← UI prefabs if needed
├── Scenes/
│   ├── MainMenu.unity
│   └── GameScene.unity
├── Scripts/
│   ├── Core/            ← GameManager, ScoreManager, TimerManager
│   ├── Player/          ← PlayerController, CameraController, PlayerInteraction
│   ├── Trash/           ← TrashItem, TrashManager
│   ├── Recycling/       ← RecyclingBin
│   ├── UI/              ← UIManager, MainMenuController, PauseMenuController, GameOverController
│   └── Audio/           ← AudioManager
├── UI/                  ← UI assets (fonts, sprites, icons)
└── Settings/            ← Scriptable objects or config files
```

## 11. Configuration & Tuning

All gameplay values will be exposed as serialized fields in the Inspector:

| Parameter | Default | Location |
|---|---|---|
| Move Speed | 6 | PlayerController |
| Mouse Sensitivity | 2 | CameraController |
| Interaction Range | 2.5 | PlayerInteraction |
| Correct Sort Points | +10 | ScoreManager |
| Incorrect Sort Points | -5 | ScoreManager |
| Timer Duration | 180s | TimerManager |
| Camera Offset | (0, 4, -6) | CameraController |
| Camera Min Angle | -20° | CameraController |
| Camera Max Angle | 60° | CameraController |

## 12. Build Configuration

- **Platform:** Windows Standalone (x86_64)
- **Scenes in Build:** MainMenu (index 0), GameScene (index 1)
- **Resolution:** 1920×1080 (windowed, resizable)
- **Company Name:** TrashRush3D Team (to be customized)
- **Product Name:** Trash Rush 3D
