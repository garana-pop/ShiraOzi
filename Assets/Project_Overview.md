# Project Overview: ShiraOzi

## 1. Project Description
**ShiraOzi** is a 2D narrative-driven adventure/puzzle game built with Unity. It features a chapter-based progression system where players explore scenes, interact with NPCs, and solve environmental puzzles to advance the story. The project targets Windows (PC) and emphasizes a localized experience across multiple languages including Japanese, English, and Chinese. Key pillars include atmospheric storytelling through localized dialogue, item-based interaction, and a persistent save system that tracks player progress and diary unlocks.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title**: The user starts at the `TitleScene`. They can adjust settings or start the game.
2.  **Scene Logic**: On "Start", the `TitleController` checks `GameState.hasSeenOpening`.
    *   If false: Transition to `OpeningScene` for narrative introduction via `DialogueManager`.
    *   If true: Transition directly to `MainScene`.
3.  **Exploration Loop**:
    *   **Interact**: Players click on objects using the `InteractableObject` component.
    *   **Dialogue**: Interacting with NPCs or specific triggers starts dialogue sequences managed by `DialogueManager`.
    *   **Collection**: Players find and collect items (`ItemPickup`), which are added to the `GameState` inventory.
    *   **Puzzles**: Players use collected items or solve logic puzzles (e.g., `PasswordPuzzle`) to unlock new areas or chapters.
4.  **Progression**: Using `ChapterManager`, the game advances through narrative milestones.
5.  **Shutdown**: Progress is automatically saved to `save.json` via `SaveManager` upon quitting.

## 3. Architecture
The project follows a **Manager-Singleton Pattern** for core systems to ensure global accessibility and persistence across scenes.
*   **Central State**: The `GameState` ScriptableObject acts as the single source of truth for the player's current status, inventory, and unlocked content.
*   **Scene Management**: Uses standard Unity `SceneManager` logic wrapped in controllers (`TitleController`, `OpeningSceneController`) to handle state-dependent transitions.
*   **Persistence**: `SaveManager` serializes the `GameState` data into JSON. It uses a `SaveData` DTO to bridge the gap between runtime ScriptableObjects and disk storage.
*   **Communication**: Systems often use C# Events (e.g., `GameState.OnItemChanged`) to notify the UI or other systems of state changes without tight coupling.

## 4. Game Systems & Domain Concepts

### Narrative & Dialogue System
The system handles multi-line, localized conversations with configurable UI layouts.
*   `DialogueManager`: Singleton that controls the flow, line by line, and triggers UI updates.
*   `DialogueEntry`: ScriptableObject containing an array of dialogue lines (keys for localization).
*   `DialogueLayoutSettings`: ScriptableObject used to dynamically reposition the dialogue box for specific narrative needs.
*   `Localization`: Integrated with Unity's Localization package; `DialogueManager` listens for locale changes to refresh text.
*   **Extension**: Create new `DialogueEntry` assets and call `DialogueManager.Instance.StartDialogue(entry)`.
`Location: Assets/Scripts/Core`

### Interaction System
A mouse-driven system for environmental interaction and cursor feedback.
*   `InteractableObject`: Base component for all clickable/hoverable objects in the world.
*   `CursorManager`: Changes the mouse sprite when hovering over interactable elements.
*   `NPCInteraction`: A specialized interaction that triggers a specific `DialogueEntry`.
*   `ItemPickup`: Interaction that adds an `ItemData` to the player's inventory.
*   **Extension**: Attach `InteractableObject` to any GameObject with a Collider2D and hook into its `UnityEvents` (onHover, onClick).
`Location: Assets/Scripts/Interaction`

### Inventory & Item System
Manages the acquisition and usage of game items.
*   `ItemData`: ScriptableObject defining an item's ID, name, and icon.
*   `GameState`: Stores the list of `acquiredItems` and the currently `activeItem`.
*   `InventoryUI` / `InventoryItemUI`: Handles the visual representation of the player's bag.
*   **Extension**: Create new `ItemData` ScriptableObjects and assign them to `ItemPickup` components in the scene.
`Location: Assets/Scripts/Core` & `Assets/Scripts/UI`

### Progression & Save System
Handles chapter transitions and data persistence.
*   `ChapterManager`: Tracks and increments the current chapter index.
*   `SaveManager`: Handles `Save()` on quit and `Load()` on awake.
*   `SaveData`: A plain C# class for JSON serialization.
*   **Extension**: Add new fields to `SaveData` and update `SaveManager.Save/Load` methods to persist new types of progress.
`Location: Assets/Scripts/Core`

## 5. Scene Overview
*   **TitleScene**: The entry point. Handles game start logic and global settings access.
*   **OpeningScene**: A narrative-heavy scene that plays the intro dialogue before handing off to the main gameplay.
*   **MainScene**: The primary gameplay area where exploration and puzzles take place.
*   **0.unity (_Recovery)**: A recovery/backup scene.
*   **Scene Loading Rules**: The `TitleController` determines the flow based on `GameState`. Managers are `DontDestroyOnLoad`, ensuring they persist from the Title onwards.

## 6. UI System
The project uses **UGUI (Unity UI)** for its interface, managed primarily through the `UIManager`.
*   **Dialogue UI**: A panel with text for speaker and content, and an "Advance" button.
*   **Inventory UI**: A grid-based system that displays `InventoryItem` prefabs.
*   **Binding Logic**: `UIManager` subscribes to `GameState.OnItemChanged` to update the "Active Item" display in the HUD automatically.
*   **Localization**: Uses `LocalizedFontAsset` to swap fonts based on the selected language (e.g., using MSGothic for Japanese).
*   **UI Sound**: `UISoundTrigger` components are attached to buttons to provide audio feedback via `SoundManager`.
`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
*   **ScriptableObjects**: Heavily utilized for data-driving the game.
    *   `DialogueEntry`: Narrative content.
    *   `ItemData`: Item definitions.
    *   `GameState`: Global runtime state (one instance exists as an asset).
    *   `DialogueLayoutSettings`: UI configuration.
*   **Addressables**: The project is configured for Addressables, primarily used for managing localized assets and potentially for memory-efficient loading of scene content.
*   **Save Data**: Stored in `Application.persistentDataPath/save.json`.
*   **Organization**: Assets are grouped by type (`Data/`, `Prefabs/UI/`, `Scripts/Core/`).

## 8. Notes, Caveats & Gotchas
*   **UIManager Singleton Pattern**: The `UIManager` uses a pattern where the "Old" singleton (from a previous scene) takes over the references of the "New" singleton in a newly loaded scene via `RefreshSceneReferences`. This prevents losing the `DontDestroyOnLoad` instance while still allowing scene-specific UI assignment.
*   **Scene Transition Delay**: `TitleController` uses a 1-frame delay (`yield return null`) before loading scenes to avoid `MissingReferenceException` in the Editor when clicking UI buttons.
*   **Localization Sync**: Dialogue text is fetched from `StringTables` using keys. Ensure keys in `DialogueEntry` assets exactly match the keys in the `UIStrings` localization table.
*   **Save Logic**: Note that `SaveManager` saves automatically on `OnApplicationQuit`. During development, use the `SaveResetTool` (Editor) if you need to clear the state.