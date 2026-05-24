# Project ShiraOzi Technical Documentation

## 1. Project Description
**ShiraOzi** is a 2D mystery/adventure game developed in Unity, focusing on point-and-click exploration, dialogue progression, and puzzle-solving. The game uses a chapter-based structure where players interact with the environment, collect items, and engage with NPCs to uncover the narrative. It leverages modern Unity features such as the Universal Render Pipeline (URP 2D), Addressables for asset management, and the Localization package for multi-language support.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title**: The user starts at `TitleScene`, where they can manage settings or start a new game/continue.
2.  **Introduction**: Transitions to `OpeningScene` for narrative setup (controlled by `hasSeenOpening` in `GameState`).
3.  **Exploration & Interaction**: The core loop takes place in `MainScene`. The player interacts with `InteractableObject` components using mouse input.
    -   **Hovering**: Triggers visual feedback via `CursorManager` and SFX via `SoundManager`.
    -   **Clicking**: Triggers narrative events via `DialogueManager` or item acquisition via `ItemPickup`.
4.  **Inventory & Progression**: Collected items are stored in the `GameState`. Players can select an "Active Item" from the `InventoryUI` to use in puzzles or specific interactions.
5.  **Chapter Transition**: Significant narrative milestones trigger `ChapterManager.NextChapter()`, updating the world state.

## 3. Architecture
The project follows a **Manager-Pattern** combined with **ScriptableObject-based State Management**.

-   **Global State**: `GameState` (ScriptableObject) acts as the "Single Source of Truth" for player progress, inventory, and unlocked diary entries.
-   **Service Managers**: Persistent Singletons (`DialogueManager`, `UIManager`, `SoundManager`, `ChapterManager`) handle cross-scene logic and UI orchestration.
-   **Event-Driven UI**: `UIManager` and `InventoryUI` subscribe to events in `GameState` (e.g., `OnItemChanged`) to refresh the display without tight coupling.
-   **Localization Integration**: The `DialogueManager` fetches strings from `LocalizationSettings` using keys defined in `DialogueEntry` assets.

## 4. Game Systems & Domain Concepts

### Dialogue System
A data-driven system for displaying localized text and speaker names.
-   `DialogueEntry`: ScriptableObject containing an array of `DialogueLine` (speaker/text keys).
-   `DialogueManager`: Controls the flow, line by line, and handles locale change events to refresh text instantly.
-   `DialogueLayoutSettings`: ScriptableObject allowing temporary UI layout overrides (position/size) for specific cinematic moments.
Location: `Assets/Scripts/Core`

### Interaction System
A standard interface for mouse-driven gameplay using Unity's EventSystem.
-   `InteractableObject`: The base component for all clickable/hoverable world objects.
-   `ItemPickup`: Attaches to world objects to handle `GameState` integration when clicked.
-   `CursorManager`: Changes the mouse cursor sprite based on interaction state (Default vs. Interact).
-   `PasswordPuzzle`: A specialized interaction component for code-entry logic.
Location: `Assets/Scripts/Interaction`

### Inventory & Item System
Manages the lifecycle of items from world pickup to active usage.
-   `ItemData`: ScriptableObject defining item attributes (ID, name, icon).
-   `InventoryUI`: Dynamically generates `InventoryItemUI` elements based on `GameState.acquiredItems`.
-   **Active Item**: The player "holds" one item at a time, reflected in the `UIManager`'s bottom-left panel.
Location: `Assets/Scripts/Core` & `Assets/Scripts/UI`

## 5. Scene Overview
-   **TitleScene**: Initial entry point. Handles game initialization and global settings.
-   **OpeningScene**: Narrative-focused scene for intro sequences.
-   **MainScene**: The primary gameplay environment where exploration and interaction occur.
-   **_Recovery / バックアップ**: Utility scenes for development and backup purposes.

Scene loading is primarily handled via direct scene transitions, with managers marked as `DontDestroyOnLoad` to maintain persistence.

## 6. UI System
The project uses **UGUI (Unity GUI)** with **TextMesh Pro** for text rendering.
-   **Structure**: The `UIManager` holds references to several root panels: `dialoguePanel`, `itemPanel` (Active Item), and `settingsPanel`.
-   **Binding**: Logic is bound via code (e.g., `Button.onClick.AddListener`) rather than Inspector events where possible to maintain control.
-   **Localization**: Integrated via the Unity Localization package. `LocalizedFontAsset` helps switch fonts (e.g., MS Gothic for Japanese vs. Liberation Sans for English).
-   **Extension**: To add a new screen, create a prefab, add its reference to `UIManager`, and define a `Toggle` or `Show` method.
Location: `Assets/Scripts/UI`

## 7. Asset & Data Model
-   **ScriptableObjects**: Extensively used for data:
    -   `DialogueEntry`: Conversation data.
    -   `ItemData`: Item definitions.
    -   `GameState`: Runtime session data.
-   **Addressables**: Used for organizing localized assets and potentially for memory-efficient loading of chapter-specific content (refer to `AddressableAssetsData`).
-   **Naming Convention**: 
    -   Scripts use PascalCase.
    -   Data assets are grouped in `Assets/Data/`.
    -   UI Prefabs live in `Assets/Prefabs/UI/`.

## 8. Notes, Caveats & Gotchas
-   **Singleton Persistence**: Most Managers use `DontDestroyOnLoad`. Ensure that when returning to the `TitleScene`, these managers are either reset properly or handled to avoid duplicates.
-   **GameState Reset**: The `GameState` ScriptableObject persists its values in the editor during play sessions. Use the `ResetState()` method or the `SaveResetTool` in the `Editor` folder to clear progress.
-   **Localization Keys**: Ensure that every `speakerKey` and `textKey` in a `DialogueEntry` exactly matches a key in the `UIStrings` localization table, or the system will return a "Key Not Found" error.
-   **Input**: The project uses the **New Input System** (InputActions), though many interactions are currently handled via `IPointer` interfaces from the EventSystem.