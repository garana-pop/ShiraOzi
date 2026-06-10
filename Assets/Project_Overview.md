# Project Overview: ShiraOzi

## 1. Project Description
**ShiraOzi** is a 2D narrative-driven adventure/puzzle game developed in Unity 6. The experience focuses on exploration, dialogue-heavy storytelling, and point-and-click interaction. Players progress through chapters, interacting with objects and NPCs to uncover a story, while managing an inventory and unlocking diary entries. The core pillars of the project are immersive localized storytelling, atmospheric 2D visuals using URP, and a robust state-driven progression system.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title**: The user starts in the `TitleScene`, where they can begin a new game or resume from a saved state.
2.  **Introduction**: New games trigger the `OpeningScene`, which uses a specialized controller to play an introductory `DialogueEntry` before transitioning to the main game.
3.  **Exploration Loop**: In `MainScene`, the player interacts with the environment using a point-and-click interface.
    *   **Interact**: Hovering over objects changes the cursor and triggers audio feedback.
    *   **Collect**: Players pick up items (stored in `GameState`) which appear in the UI.
    *   **Solve**: Use acquired items or solve puzzles (like the `PasswordPuzzle`) to advance the narrative.
4.  **Dialogue & Choice**: Interacting with NPCs or specific objects triggers the `DialogueManager`, showing localized text via a typewriter effect.
5.  **Progression**: The `SaveManager` automatically persists the `GameState` (chapter, inventory, diary) to a JSON file on exit or specific triggers.

## 3. Architecture
The project follows a **Manager-based architecture** with **ScriptableObject-driven data**.

*   **Global Singletons**: Critical systems like `DialogueManager`, `UIManager`, `SaveManager`, and `SoundManager` use the Singleton pattern and `DontDestroyOnLoad` to persist across scenes.
*   **State Management**: `GameState` (a `ScriptableObject`) serves as the "Single Source of Truth" for the game's runtime data. Components observe this state via C# events (e.g., `OnItemChanged`).
*   **Decoupled Interaction**: The `InteractableObject` component uses `UnityEvents` to decouple the physical interaction (clicking/hovering) from the specific logic (picking up an item vs. starting a conversation).
*   **Localization**: The project integrates the **Unity Localization Package**, using string tables for all dialogue and UI elements.

`Location: Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Dialogue System
A key narrative engine that sequences lines from `DialogueEntry` assets.
*   `DialogueManager`: Coordinates the flow of conversation and UI display.
*   `DialogueEntry`: A ScriptableObject containing an array of `DialogueLine` (keys for localization).
*   `TypewriterEffect`: Handles the visual presentation of text over time.
*   `SpeakerMapping`: Dynamically determines the speaker name and UI layout based on string key suffixes (e.g., `_Ozi`, `_Narration`).
`Location: Assets/Scripts/Core`, `Assets/Scripts/UI`

### Interaction System
Handles user input and world feedback.
*   `InteractableObject`: Provides a generic wrapper for mouse events (`OnPointerClick`, etc.).
*   `CursorManager`: Changes the mouse cursor based on the current interaction state.
*   `ItemPickup`: A specialized component for adding `ItemData` to the player's inventory.
`Location: Assets/Scripts/Interaction`

### Inventory & Item System
Manages player possessions and their active use.
*   `ItemData`: ScriptableObject defining an item's ID, Name, and Icon.
*   `InventoryUI`: Manages the display of the grid and handles item selection.
*   `InventoryItemUI`: Represents a single item slot in the UI.
`Location: Assets/Scripts/UI`, `Assets/Scripts/Core`

## 5. Scene Overview
*   **TitleScene**: The entry point. Handles game initialization and provides access to settings/start options.
*   **OpeningScene**: A narrative scene dedicated to the intro cinematic/dialogue. Controlled by `OpeningSceneController`.
*   **MainScene**: The primary gameplay environment where exploration and puzzles occur.
*   **0.unity (_Recovery)**: A recovery/backup scene.

`Location: Assets/Scenes`

## 6. UI System
The project uses **UGUI** (Unity UI) for its interface, managed primarily through the `UIManager`.

*   **Dialogue Panel**: Centrally managed; supports dynamic layout changes via `DialogueLayoutSettings`.
*   **Inventory Overlay**: A toggleable panel that populates dynamically based on `GameState.acquiredItems`.
*   **Typewriter Rendering**: Uses TextMeshPro for high-quality localized text rendering.
*   **Binding**: The UI listens to `GameState` events to update icons and text automatically when items are picked up or selected.
*   **UI Sounds**: `UISoundTrigger` components are used to attach audio feedback to UI interactions.

`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
*   **ScriptableObjects**:
    *   `GameState`: Runtime state (Chapter, Inventory, Diary).
    *   `ItemData`: Static item definitions.
    *   `DialogueEntry`: Narrative content.
    *   `SpeakerMapping`: Configuration for UI labels.
*   **Persistence**:
    *   `SaveData`: A plain C# class used to serialize `GameState` into `save.json` via `JsonUtility`.
*   **Localization**:
    *   Addressables-based localized assets (String Tables, Asset Tables).
    *   Supports multiple locales: `en`, `ja`, `zh-Hans`, `zh-Hant`.

`Location: Assets/Data`, `Assets/Scripts/Core`

## 8. Notes, Caveats & Gotchas
*   **Singleton Refreshing**: `UIManager` has a `RefreshSceneReferences` method. When a new scene loads, the persistent singleton updates its references to the new scene's UI objects to avoid null references.
*   **Dialogue Suffixes**: The speaker name is often derived from the `textKey` suffix. Ensure all localization keys for dialogue follow the `KEY_Name` format if they are to be mapped to a specific speaker.
*   **Save/Load Timing**: The `SaveManager` loads data on `Awake`. If you modify `GameState` in the editor, it may be overwritten by the existing `save.json` upon entering Play Mode. Use the `SaveResetTool` in the Editor to clear state.
*   **Layer/Input Caveat**: Interactivity depends on `IPointer` handlers; ensure world objects have Colliders and a `Physics2DRaycaster` is present on the Camera.