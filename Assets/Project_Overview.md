# ShiraOzi Project Overview

## 1. Project Description
**ShiraOzi** is a 2D narrative-driven puzzle adventure game built in Unity. It features a point-and-click interaction style where players explore environments, collect items, and engage in branching dialogues with NPCs to progress through chapters. The project places a heavy emphasis on localization and immersive UI, utilizing the Universal Render Pipeline (URP) and the official Localization package.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title**: The game starts in the `TitleScene` where players can adjust settings or start a new game.
2.  **Exploration**: Players interact with objects in the environment (using `InteractableObject`) to trigger events or dialogues.
3.  **Inventory & Item Use**: Players collect items (via `ItemPickup`) which are stored in the `GameState`. Carrying specific items changes the outcome of interactions with NPCs.
4.  **Puzzle Solving**: Progression is often gated by logical puzzles (e.g., `PasswordPuzzle`) or by possessing the correct item for a specific interaction.
5.  **Progression**: The `ChapterManager` and `GameState` track the story's progress, unlocking diary entries and transitioning through scenes.

## 3. Architecture
The project follows a **Manager-Centric** architecture with a focus on **ScriptableObject-driven data**.
-   **Global State**: `GameState` (ScriptableObject) acts as the single source of truth for items, progress, and diary unlocks.
-   **Singleton Managers**: Core systems like `DialogueManager`, `UIManager`, `SoundManager`, and `CursorManager` use the Singleton pattern to provide global access while persisting across scene loads via `DontDestroyOnLoad`.
-   **Event-Driven UI**: The UI observes changes in the `GameState` (e.g., `OnItemChanged`) to update the display reactively.
-   **Decoupled Interaction**: The `InteractableObject` uses UnityEvents to trigger logic, allowing designers to hook up interactions (like picking up items or starting dialogues) in the inspector without writing new code.

## 4. Game Systems & Domain Concepts

### Interaction System
A point-and-click framework that handles hover states, cursor changes, and click events.
-   `InteractableObject`: The base component for all clickable/hoverable objects in the scene.
-   `CursorManager`: Manages visual feedback by changing the cursor texture during interaction.
-   `ItemPickup`: Specialized interaction to add items to the inventory.
-   `NPCInteraction`: Triggers dialogues, with logic to branch based on the player's held item.
`Location: Assets/Scripts/Interaction/`

### Dialogue System
A data-driven system for displaying localized text and managing conversation flow.
-   `DialogueEntry`: A ScriptableObject containing a sequence of conversation lines.
-   `DialogueManager`: Controls the flow of conversation and handles localization lookups.
-   `DialogueLayoutSettings`: Allows per-interaction customization of the dialogue UI's position and size.
`Location: Assets/Scripts/Core/` and `Assets/Scripts/UI/`

### Inventory & Item System
Tracks acquired items and manages the "active" item used for world interactions.
-   `ItemData`: ScriptableObject defining an item's ID, name, and icon.
-   `GameState`: Stores the list of `acquiredItems` and the currently `activeItem`.
-   `InventoryUI`: Manages the display of the inventory grid.
`Location: Assets/Scripts/Core/` and `Assets/Scripts/UI/`

## 5. Scene Overview
-   **TitleScene**: The entry point, containing the main menu and global settings.
-   **OpeningScene**: Dedicated to the introductory narrative/cutscene.
-   **MainScene**: The primary gameplay environment where exploration and puzzles take place.
-   **_Recovery/0**: A fallback or recovery scene used during development.

## 6. UI System
The project uses **UGUI** (Unity UI) combined with **TextMesh Pro** for high-quality text rendering.
-   **Localization**: Integrated with the Unity Localization package. Text is retrieved via keys (e.g., `UIStrings`) to support multiple languages (EN, JA, ZH).
-   **UIManager**: The central hub for UI panels (Dialogue, Inventory, Settings). It handles the layouting of the dialogue box dynamically.
-   **Diary System**: Managed by `DiaryManager`, it tracks story milestones and allows players to review unlocked lore.
-   **LocalizedFontAsset**: A utility to ensure correct font assets are used based on the active language.
`Location: Assets/Scripts/UI/`

## 7. Asset & Data Model
-   **ScriptableObjects**: Extensively used for data definitions.
    -   `DialogueEntry`: Stores conversation trees.
    -   `ItemData`: Stores item metadata.
    -   `GameState`: Stores runtime save-ready data.
-   **Addressables**: Used for managing assets, specifically for localization tables and shared resources.
-   **Input Actions**: Uses the New Input System with a defined `InputActions` asset for cross-platform compatibility.
-   **Prefabs**: UI elements like `InventoryItem` are prefabs to ensure consistency across different screens.

## 8. Notes, Caveats & Gotchas
-   **Dialogue Layout Overrides**: The `DialogueManager` can have its UI layout temporarily overridden by `DialogueLayoutSettings` attached to specific interactables. If a new interactable needs a unique dialogue box position, add this component.
-   **Item IDs**: Interactions in `NPCInteraction` rely on string-based `itemIDs`. Ensure the ID in `ItemData` matches the ID entered in the NPC's interaction list.
-   **Singleton Persistence**: Since Managers use `DontDestroyOnLoad`, ensure that scene-specific references are cleared or updated when transitioning between scenes to avoid null references.
-   **Localization Refresh**: The `DialogueManager` listens for `SelectedLocaleChanged`. If adding new UI elements that display localized text, ensure they either use `LocalizeStringEvent` components or manually subscribe to locale changes.