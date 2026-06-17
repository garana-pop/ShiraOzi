# ShiraOzi Technical Project Overview

## 1. Project Description
**ShiraOzi** is a 2D narrative-driven adventure game built with Unity 6. The experience focuses on point-and-click exploration, dialogue-heavy storytelling, and inventory-based puzzle solving. The project utilizes a localized, multi-language system (English, Japanese, Simplified/Traditional Chinese) and follows a structured chapter-based progression.

**Core Pillars:**
- **Narrative Depth:** Storytelling through a sophisticated dialogue system with speaker mapping.
- **Atmospheric Exploration:** 2D environments using URP's 2D Renderer and lighting.
- **Interaction & Puzzles:** Item pickup and use, NPC interactions, and logic-based puzzles (e.g., password puzzles).

## 2. Gameplay Flow / User Loop
1.  **Boot & Load:** The game initializes via the `TitleScene`. `SaveManager` automatically attempts to load existing `save.json` data into the `GameState` ScriptableObject.
2.  **Navigation:** Users transition from the Title to the `OpeningScene` (narrative intro) and finally to the `MainScene`.
3.  **Core Loop:**
    - **Explore:** Move through scenes and hover over objects.
    - **Interact:** Click on `InteractableObject` components to trigger events, pick up items via `ItemPickup`, or talk to NPCs.
    - **Dialogue:** Engage in conversations managed by `DialogueManager`, advancing text with the `TypewriterEffect`.
    - **Inventory Use:** Select items from the `InventoryUI` to change the `activeItem` in `GameState`, unlocking specific dialogue branches in `NPCInteraction`.
4.  **Save/Exit:** Game state is automatically persisted to disk upon application exit or manual triggers through `SaveManager`.

## 3. Architecture
The project employs a **Service-Oriented Singleton Pattern** for global management, combined with **ScriptableObject-based State Management** for data decoupled from scene logic.

- **Centralized State:** The `GameState` ScriptableObject acts as the "Single Source of Truth" for progress, inventory, and flags.
- **Singleton Managers:** Global systems like `DialogueManager`, `UIManager`, and `SaveManager` persist across scenes via `DontDestroyOnLoad`.
- **Event-Driven UI:** UI components subscribe to `GameState` events (e.g., `OnItemChanged`) to update automatically without direct coupling.
- **Scene Persistence:** When scenes change, the persistent `UIManager` refreshes its local scene references (buttons, panels) via the `RefreshSceneReferences` pattern.

## 4. Game Systems & Domain Concepts

### Dialogue System
A localization-first system that maps string keys to localized text tables.
- `DialogueManager`: Controls flow, line advancing, and integration with `UnityEngine.Localization`.
- `DialogueEntry`: ScriptableObject containing arrays of `DialogueLine` (speaker and text keys).
- `SpeakerMapping`: Maps localized key suffixes (e.g., `_Ozi`) to specific speaker names or narration rules.
- `TypewriterEffect`: Handles the visual reveal of text over time.
- `DialogueLayoutSettings`: Allows dynamic repositioning of the dialogue UI per interaction.
- **Location:** `Assets/Scripts/Core`, `Assets/Scripts/UI`

### Interaction System
A flexible event-based system for mouse-driven gameplay.
- `InteractableObject`: Uses `IPointer` interfaces to trigger `UnityEvents` for clicks and hovers.
- `CursorManager`: Manages hardware cursor swapping between default and "interact" states.
- `NPCInteraction`: Context-sensitive interaction logic that branches based on the player's held item.
- **Location:** `Assets/Scripts/Interaction`

### Inventory & Item System
Tracks acquired assets and manages player "tools".
- `ItemData`: ScriptableObject defining item names, IDs, and icons.
- `ItemPickup`: Concrete interaction that adds an item to `GameState` and disables the world object.
- `InventoryUI` / `InventoryItemUI`: Manages the display and selection of items.
- **Location:** `Assets/Scripts/Core`, `Assets/Scripts/UI`

### Progress & Save System
Manages chapter progression and persistent data.
- `ChapterManager`: Simple incrementor for the game's chapter-based structure.
- `SaveManager`: Handles JSON serialization of `SaveData` to `Application.persistentDataPath`.
- `SaveData`: A plain C# class representing the serializable snapshot of `GameState`.
- **Location:** `Assets/Scripts/Core`

## 5. Scene Overview
- `TitleScene`: The initial entry point containing the main menu and game initialization logic.
- `OpeningScene`: A dedicated narrative sequence scene. Controlled by `OpeningSceneController`.
- `MainScene`: The primary gameplay environment where most exploration and interactions occur.
- **Scene Flow:** Managed via standard Unity SceneManagement, but global state is preserved by the `GameState` SO and persistent singletons.

## 6. UI System
Built using **uGUI** (Unity UI) with TextMesh Pro for high-quality localized text.
- **Structure:** `UIManager` manages the `dialoguePanel`, `itemPanel`, and `settingsPanel`.
- **Binding:** UI updates are triggered by `GameState` events or manual refreshes when `RefreshSceneReferences` is called.
- **Extension:** To add a new screen, create a prefab, add its reference to `UIManager`, and define a toggle/show method.
- **Localization:** Uses the `UnityEngine.Localization` package. `LocalizedFontAsset` ensures font switching (e.g., Japanese vs. Latin) works across locales.
- **Location:** `Assets/Scripts/UI`

## 7. Asset & Data Model
- **ScriptableObjects (SO):**
    - `GameState`: Found in `Assets/Data/GameState`.
    - `ItemData`: Found in `Assets/Data/Items`.
    - `DialogueEntry`: Found in `Assets/Data/Dialogue`.
- **Localization Tables:** `UIStrings` and `FontAssets` tables managed via the Localization window.
- **Prefabs:** `InventoryItem` and UI panels located in `Assets/Prefabs/UI`.
- **Addressables:** Used for localization assets and potentially for memory-efficient loading of large assets.

## 8. Notes, Caveats & Gotchas
- **Singleton Lifecycle:** Because managers like `UIManager` are `DontDestroyOnLoad`, they must call `RefreshSceneReferences` in `Awake` if a new scene contains its own manager instance to reconnect to the new scene's UI objects.
- **Dialogue Suffixes:** The `DialogueManager` expects localized keys to end in specific suffixes (e.g., `_Protagonist`) to automatically determine the speaker name via `SpeakerMapping`.
- **Automatic Saving:** Saving occurs on `OnApplicationQuit` in `SaveManager`. Be careful with mobile platforms or crashes where this might not trigger; consider adding manual save points.
- **2D Physics:** Interactions rely on `IPointer` handlers; ensure the Main Camera has a `Physics2DRaycaster` (or `GraphicRaycaster` for UI) and objects have appropriate `Collider2D Colliders` with "Query Hit Backfaces" or proper Z-depth.