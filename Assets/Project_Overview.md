# ShiraOzi Project Overview

This technical documentation provides a comprehensive overview of the **ShiraOzi** Unity project, a 2D narrative-driven puzzle/adventure game developed with Unity 6. The game focuses on interaction, item collection, and story progression through dialogue.

---

## 1. Project Description
**ShiraOzi** is a point-and-click adventure experience where players interact with environments to solve puzzles and uncover a narrative.
- **Core Pillars**: Narrative storytelling, item-based interaction, and atmosphere-driven exploration.
- **Target Experience**: A focused 2D experience with a heavy emphasis on localized dialogue and environmental puzzles (e.g., password inputs).
- **Technology**: Built using Unity 6 (6000.4.4f1) with the Universal Render Pipeline (URP) 2D and the New Input System.

## 2. Gameplay Flow / User Loop
1. **Boot**: The game starts in the `TitleScene`, where players can start a new game or continue.
2. **Transition**: If starting fresh, the `OpeningScene` plays (handling intro sequences).
3. **Core Loop**:
   - **Explore**: Navigate the `MainScene`, interacting with objects via `InteractableObject`.
   - **Interact**: Click on NPCs or items. NPCs trigger localized dialogue via `DialogueManager`.
   - **Collect**: Pick up items using `ItemPickup`, which stores them in the `GameState` and displays them in the UI.
   - **Solve**: Use collected items or enter codes in `PasswordPuzzle` to progress.
4. **Persistence**: The `SaveManager` automatically saves the game state on exit, allowing players to resume their progress (Chapter, Inventory, Diary).

## 3. Architecture
The project follows a Manager-based singleton architecture combined with ScriptableObject-based data containers.
- **Data-Centric Design**: `GameState` (ScriptableObject) acts as the "Single Source of Truth" for the game's current state, decoupling logic from scene data.
- **Singleton Managers**: Persistent systems like `SaveManager`, `DialogueManager`, and `UIManager` handle global functionality across scenes.
- **Event-Driven UI**: The UI subscribes to `GameState` events (e.g., `OnItemChanged`) to update automatically without tight coupling.

## 4. Game Systems & Domain Concepts

### Game State & Persistence
- `GameState`: A ScriptableObject containing current chapter, inventory list, active item, and unlocked diary entries.
- `SaveManager`: Handles serialization of `GameState` to a `save.json` file in the persistent data path.
- `SaveData`: A plain C# class used for JSON serialization.
- `Location`: `Assets/Scripts/Core`

### Dialogue System
- `DialogueManager`: Controls the flow of conversation, supporting localized text retrieval and custom UI layouts.
- `DialogueEntry`: A ScriptableObject holding an array of dialogue lines (speaker/text keys).
- `DialogueLayoutSettings`: Allows dynamic repositioning of the dialogue box for specific narrative moments.
- `Location`: `Assets/Scripts/Core` and `Assets/Scripts/UI`

### Interaction System
- `InteractableObject`: A generic component that uses Unity's `IPointer` interfaces to detect mouse clicks/hovers.
- `ItemPickup`: Specific interaction logic for adding items to the inventory and disabling the world object.
- `NPCInteraction`: Triggers dialogue sequences when interacting with world characters.
- `CursorManager`: Changes the mouse cursor visual based on interaction availability.
- `Location`: `Assets/Scripts/Interaction`

### Inventory & Items
- `ItemData`: ScriptableObject defining an item's ID, name, icon, and description.
- `InventoryUI` / `InventoryItemUI`: Manages the display and selection of items within the player's bag.
- `Location`: `Assets/Scripts/Core` and `Assets/Scripts/UI`

## 5. Scene Overview
- `TitleScene`: The entry point. Manages game start, settings, and continuation logic.
- `OpeningScene`: Dedicated to intro narrative/cutscenes.
- `MainScene`: The primary gameplay environment where exploration and puzzles occur.
- **Scene Flow**: Controlled by `ChapterManager` and UI-driven transitions (e.g., `TitleController`).

## 6. UI System
The project uses **UGUI** (Unity UI) for its interface.
- **UIManager**: The central hub for UI panels (Dialogue, Inventory, Settings). It handles the instantiation and visibility of these elements.
- **Localization**: Integration with the `Unity Localization` package. Text is pulled from `UIStrings` tables using keys defined in `DialogueEntry`.
- **Dynamic Layouts**: The dialogue UI can be transformed at runtime using `DialogueLayoutSettings` to fit different scene compositions.
- **Sound**: `UISoundTrigger` components provide audio feedback for hover and click events.
- `Location`: `Assets/Scripts/UI`

## 7. Asset & Data Model
- **ScriptableObjects**: Heavily used for configuration and state:
  - Dialogue: `Assets/Data/Dialogue/*.asset`
  - Items: `Assets/Data/Items/*.asset`
  - Global State: `Assets/Data/GameState/GameState.asset`
- **Addressables**: The project is configured with Addressables, specifically for localization assets (Strings, Locales, Tables).
- **Naming Convention**: Uses clear prefixes like `diag_` for dialogue keys and `item_` for item data.

## 8. Notes, Caveats & Gotchas
- **Auto-Save**: The game saves automatically on `OnApplicationQuit`. Ensure manual saves are triggered if the platform does not guarantee quit calls.
- **Item Search**: The `SaveManager` contains a list of `allItems`. New items created as ScriptableObjects must be added to this list in the `SaveManager` prefab/instance to be restorable from a save file.
- **Localization Keys**: Dialogue doesn't store raw strings; it stores *keys*. If a key is missing from the `UIStrings` table, the UI will display a "Key Not Found" error.
- **Singleton Persistence**: Managers use `DontDestroyOnLoad`. Be careful not to duplicate them when returning to the Title scene.