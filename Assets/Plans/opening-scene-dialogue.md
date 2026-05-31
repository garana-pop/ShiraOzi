# Project Overview
- Game Title: ShiraOzi
- High-Level Concept: 2D narrative-driven puzzle/adventure game.
- Players: Single player.
- Target Platform: Standalone Windows.
- Render Pipeline: URP 2D.
- Input: New Input System.

# Game Mechanics
## Core Gameplay Loop
The player interacts with the environment and NPCs to progress the story. The OpeningScene serves as the introduction, featuring a dialogue sequence between the protagonist and a mysterious man ("Ozi").

## Controls and Input Methods
- Mouse Click: Advances the dialogue.
- Dialogue advances sequentially when the player clicks the interaction area (Advance Button).

# UI
- Dialogue Box: Located at the bottom of the screen (bottom 25%).
- Speaker Name: Displayed above or within the dialogue box.
- Text: Localized dialogue text.

# Key Asset & Context
- **Localization Table**: `UIStrings` (String Table).
- **Dialogue Entry**: `Assets/Data/Dialogue/OpeningDialogue.asset` (ScriptableObject).
- **Controller Script**: `Assets/Scripts/UI/OpeningSceneController.cs`.
- **Sound Effect**: `Assets/Sounds/SE/click_SE.mp3` (Already assigned to `SoundManager.clickClip`).
- **UI Script**: `Assets/Scripts/UI/UISoundTrigger.cs` (Used for click SE).

# Implementation Steps

## 1. Localization Table Setup
Add speaker keys to the `UIStrings` localization table:
- `Speaker_Protagonist`: "主人公" (Japanese), "Protagonist" (English)
- `Speaker_Ozi`: "オジ" (Japanese), "Old Man" (English)
This ensures that the speaker names are correctly localized in the dialogue UI.

## 2. Create Opening Dialogue Data
Create a `DialogueEntry` ScriptableObject at `Assets/Data/Dialogue/OpeningDialogue.asset`.
Configure 21 dialogue lines in the following order:
1.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue01_Protagonist`
2.  Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue02_Ozi`
3.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue03_Protagonist`
4.  Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue04_Ozi`
5.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue05_Protagonist`
6.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue06_Protagonist`
7.  Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue07_Ozi`
8.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue08_Protagonist`
9.  Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue09_Protagonist`
10. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue10_Ozi`
11. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue11_Protagonist`
12. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue12_Ozi`
13. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue13_Protagonist`
14. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue14_Ozi`
15. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue15_Protagonist`
16. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue16_Ozi`
17. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue17_Protagonist`
18. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue18_Ozi`
19. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue19_Protagonist`
20. Speaker: `Speaker_Ozi`, Text: `OpeningScene_dialogue20_Ozi`
21. Speaker: `Speaker_Protagonist`, Text: `OpeningScene_dialogue21_Protagonist`

## 3. Implement OpeningSceneController
Create `Assets/Scripts/UI/OpeningSceneController.cs` to manage the opening sequence:
- Start the dialogue using `DialogueManager.Instance.StartDialogue(openingDialogue)`.
- Monitor `DialogueManager.Instance.IsDisplaying`.
- When `IsDisplaying` becomes false (dialogue finished), load `MainScene`.

## 4. Scene and UI Setup
- **OpeningScene**: 
    - Create a new GameObject named `OpeningController`.
    - Attach `OpeningSceneController` and assign the `OpeningDialogue` asset.
- **UIManager**:
    - Attach `UISoundTrigger` to the `AdvanceButton` in the `UIManager` (on the instance in `TitleScene` or the prefab). This ensures `click_SE` plays whenever the dialogue is advanced.
    - Verify that `AdvanceButton` covers the interaction area as required.

# Verification & Testing
1.  **Start from TitleScene**: Click "Start" and ensure the game transitions to `OpeningScene`.
2.  **Dialogue Sequence**: Verify that dialogues appear at the bottom and match the keys `OpeningScene_dialogue01` to `OpeningScene_dialogue21`.
3.  **Speaker Names**: Verify that "主人公" and "オジ" are displayed correctly.
4.  **Click Interaction**: Click the dialogue area and verify that:
    - The next dialogue line is shown.
    - `click_SE.mp3` is played.
5.  **Scene Transition**: After the 21st line, verify that the game transitions to `MainScene`.
