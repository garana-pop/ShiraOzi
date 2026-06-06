# Project Overview
- **Game Title:** ShiraOzi
- **High-Level Concept:** 2D 物語主導のアドベンチャー/パズルゲーム。章立て進行で、シーン探索・NPC との会話・パズル解きを通じて物語を進める。
- **Players:** シングルプレイヤー
- **Inspiration / Reference Games:** ナラティブ系探索アドベンチャー
- **Tone / Art Direction:** 雰囲気重視・物語中心。多言語（日本語/英語/中国語）対応。
- **Target Platform:** StandaloneWindows64 (PC)
- **Screen Orientation / Resolution:** Landscape
- **Render Pipeline:** URP (Universal Render Pipeline)

# Game Mechanics
本タスクはゲームメカニクスの変更ではなく、**会話（ダイアログ）UI の話者名表示ロジックの修正**です。

## 対象の課題
会話行のローカライズキー（`textKey`）が以下の命名規則に従っている場合、サフィックスに応じて正しい話者名を自動表示したい。

| キー例 | サフィックス | 表示する話者名 |
|---|---|---|
| `SceneName_dialogue(No)_Ozi` | `Ozi` | おじさん |
| `SceneName_dialogue(No)_Protagonist` | `Protagonist` | 主人公 |
| `SceneName_dialogue(No)_Narration` | `Narration` | （話者名は非表示／ナレーション） |

## 現状分析（調査結果）
- `DialogueLine`（`Assets/Scripts/Core/DialogueEntry.cs`）は `speakerKey` と `textKey` の 2 フィールドを持つ。
- `DialogueManager.DisplayCurrentLine()`（`Assets/Scripts/Core/DialogueManager.cs:114`）は `speaker = GetLocalizedString("UIStrings", line.speakerKey)` で話者名を取得している。
- `Assets/Data/Dialogue/OpeningDialogue.asset` では `textKey` が既に `OpeningScene_dialogue01_Protagonist` / `..._Ozi` の命名規則に従っているが、`speakerKey` は `Speaker_Protagonist` / `Speaker_Ozi` を指しており、**これらのキーは `UIStrings` テーブルに存在しない**（`UIStrings Shared Data.asset` に未登録）。よって現状、話者名は正しく表示されていない（空 or 翻訳なし表示）。
- `_Narration` を使うデータはまだ存在しないが、将来対応として要件に含む。
- 他のダイアログ（`IntroDialogue.asset` の `diag_intro_1_speaker`、`OldManClick.asset` の `OldMan_Name`）は命名規則が異なるため、**従来の `speakerKey` フォールバックを維持**する必要がある。

## 採用する設計方針（ユーザー確定事項）
1. **話者名はローカライズ対応**：サフィックスをローカライズキー（`Speaker_Ozi` 等）に対応付け、`UIStrings` テーブルに各言語の値を追加する。
2. **対応付けは ScriptableObject で管理**：`SpeakerMapping` SO を作成し、Inspector でサフィックス→話者を追加・編集可能にする。
3. **Narration は話者ラベルを非表示**：`speakerText` の GameObject を非アクティブにして余白も消す。
4. **サフィックス優先・フォールバック維持**：`textKey` のサフィックスがマッピングに存在すればそれを使用。存在しない場合は従来の `speakerKey` を使用する（後方互換）。

## Controls and Input Methods
変更なし（New Input System。会話送りは既存の `advanceButton` のまま）。

# UI
会話パネル（`UIManager.dialoguePanel`）の構成は維持。
- `speakerText`（TextMeshProUGUI）: 話者名。Narration 時は **GameObject を非アクティブ化**して非表示。
- `dialogueText`（TextMeshProUGUI）: セリフ本文（変更なし）。
- Narration 以外の行に切り替わった際は `speakerText` を再アクティブ化して名前を表示する（連続表示で消えたままにならないように両方向の制御を行う）。

# Key Asset & Context

## 新規作成
### 1. `Assets/Scripts/Core/SpeakerMapping.cs`（新規 ScriptableObject）
```csharp
using System;
using UnityEngine;

namespace ShiraOzi.Core
{
    /// <summary>
    /// textKey のサフィックス（_Ozi, _Protagonist, _Narration 等）と
    /// 話者名（ローカライズキー）の対応を管理する ScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "SpeakerMapping", menuName = "ShiraOzi/SpeakerMapping")]
    public class SpeakerMapping : ScriptableObject
    {
        [Serializable]
        public struct SpeakerEntry
        {
            [Tooltip("textKey の最後の '_' 以降のサフィックス。例: Ozi, Protagonist, Narration")]
            public string suffix;
            [Tooltip("話者名のローカライズキー（UIStrings テーブル）。hideSpeaker が true の場合は無視。")]
            public string speakerNameKey;
            [Tooltip("true の場合、話者ラベルを非表示にする（ナレーション用）。")]
            public bool hideSpeaker;
        }

        public SpeakerEntry[] entries;

        /// <summary>サフィックスから対応エントリを取得（大文字小文字無視）。</summary>
        public bool TryGet(string suffix, out SpeakerEntry result)
        {
            if (!string.IsNullOrEmpty(suffix) && entries != null)
            {
                foreach (var e in entries)
                {
                    if (string.Equals(e.suffix, suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        result = e;
                        return true;
                    }
                }
            }
            result = default;
            return false;
        }
    }
}
```

### 2. `Assets/Data/SpeakerMapping.asset`（新規 SO インスタンス）
エントリ:
| suffix | speakerNameKey | hideSpeaker |
|---|---|---|
| `Ozi` | `Speaker_Ozi` | false |
| `Protagonist` | `Speaker_Protagonist` | false |
| `Narration` | （空） | true |

## 変更
### 3. `Assets/Scripts/Core/DialogueManager.cs`
- フィールド追加: `[SerializeField] private SpeakerMapping speakerMapping;`
- `DisplayCurrentLine()`（114-126行付近）を以下のロジックに変更:
  1. `textKey` の最後の `_` 以降をサフィックスとして抽出。
  2. `speakerMapping != null && speakerMapping.TryGet(suffix, out var entry)` の場合:
     - `entry.hideSpeaker == true` → 話者ラベル非表示で表示。
     - それ以外 → `speaker = GetLocalizedString("UIStrings", entry.speakerNameKey)`、ラベル表示。
  3. マッチしない場合 → 従来通り `speaker = GetLocalizedString("UIStrings", line.speakerKey)`、ラベル表示（フォールバック）。
  4. `UIManager.Instance.ShowDialogue(speaker, text, showSpeaker)` を呼ぶ。
- サフィックス抽出ヘルパー追加:
```csharp
private static string ExtractSuffix(string key)
{
    if (string.IsNullOrEmpty(key)) return null;
    int idx = key.LastIndexOf('_');
    return idx >= 0 && idx < key.Length - 1 ? key.Substring(idx + 1) : null;
}
```
- 変更後の `DisplayCurrentLine()` イメージ:
```csharp
private void DisplayCurrentLine()
{
    var line = currentEntry.lines[currentLineIndex];
    string text = GetLocalizedString("UIStrings", line.textKey);

    string speaker;
    bool showSpeaker = true;

    string suffix = ExtractSuffix(line.textKey);
    if (speakerMapping != null && speakerMapping.TryGet(suffix, out var entry))
    {
        if (entry.hideSpeaker)
        {
            speaker = string.Empty;
            showSpeaker = false;
        }
        else
        {
            speaker = GetLocalizedString("UIStrings", entry.speakerNameKey);
        }
    }
    else
    {
        // フォールバック: 従来の speakerKey を使用
        speaker = GetLocalizedString("UIStrings", line.speakerKey);
    }

    if (UIManager.Instance)
    {
        UIManager.Instance.ShowDialogue(speaker, text, showSpeaker);
    }
}
```

### 4. `Assets/Scripts/UI/UIManager.cs`
- `ShowDialogue` シグネチャを変更（既定値付きで後方互換）:
```csharp
public void ShowDialogue(string speaker, string text, bool showSpeaker = true)
{
    if (dialoguePanel) dialoguePanel.SetActive(true);
    if (speakerText)
    {
        speakerText.gameObject.SetActive(showSpeaker);
        speakerText.text = showSpeaker ? speaker : string.Empty;
    }
    if (dialogueText) dialogueText.text = text;
}
```
（`speakerText.gameObject.SetActive` により Narration 時は余白も消える。次の通常行で `true` が渡され再表示される。）

### 5. ローカライズ：`UIStrings` テーブルへ話者名キーを追加
対象ファイル（IDの整合性が必要なため、Localization API での追加を推奨）:
- `Assets/Localization/UIStrings Shared Data.asset`（キー登録）
- `Assets/Localization/UIStrings_ja.asset`
- `Assets/Localization/UIStrings_en.asset`
- `Assets/Localization/UIStrings_zh-Hans.asset`
- `Assets/Localization/UIStrings_zh-Hant.asset`

追加キーと値（日本語はユーザー指定。EN/ZH は提案値。**必要に応じて調整可**）:
| キー | ja | en | zh-Hans | zh-Hant |
|---|---|---|---|---|
| `Speaker_Ozi` | おじさん | Uncle | 大叔 | 大叔 |
| `Speaker_Protagonist` | 主人公 | Protagonist | 主角 | 主角 |

> 実装メモ: 手動で YAML を編集すると `m_Id` の整合が崩れやすい。エディタ拡張スクリプト（`LocalizationEditorSettings.GetStringTableCollection("UIStrings")` → `SharedData.AddKey(...)` → 各 `StringTable.AddEntry(key, value)`）でプログラム的に追加するのが安全。

### 6. シーンでの SO 紐付け
`DialogueManager` は `DontDestroyOnLoad` シングルトンで、最初に存在するシーン（`TitleScene`）に配置されている想定。
- `DialogueManager` コンポーネントの `speakerMapping` フィールドに `Assets/Data/SpeakerMapping.asset` を割り当てる。
- 配置シーンは実装前に確認（`TitleScene.unity` / `OpeningScene.unity` を調査して特定）。

# Implementation Steps

### Step 1: DialogueManager の配置シーンと既存参照の確認
- **Description:** `DialogueManager` コンポーネントがどのシーン/プレハブに存在するかを特定（`TitleScene.unity`、`OpeningScene.unity`）。`speakerMapping` 紐付け先を確定。`OpeningDialogue.asset` 以外に suffix 命名のダイアログがあるか最終確認。
- **Assigned role:** explorer
- **Dependencies:** None
- **Parallelizable:** Yes

### Step 2: `SpeakerMapping.cs`（ScriptableObject）の作成
- **Description:** 上記コードで新規スクリプトを作成。
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes

### Step 3: `SpeakerMapping.asset` の作成と 3 エントリ設定
- **Description:** `Assets/Data/SpeakerMapping.asset` を作成し、Ozi/Protagonist/Narration の 3 エントリを設定。
- **Assigned role:** developer
- **Dependencies:** Depends on Step 2
- **Parallelizable:** No

### Step 4: `UIManager.ShowDialogue` の更新（話者ラベル非表示対応）
- **Description:** `showSpeaker` 引数を追加し、`speakerText.gameObject.SetActive` 制御を実装。
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes

### Step 5: `DialogueManager` のロジック更新
- **Description:** `speakerMapping` フィールド追加、`ExtractSuffix` 追加、`DisplayCurrentLine()` をサフィックス優先＋フォールバックに変更。`ShowDialogue` の新シグネチャ呼び出しに合わせる。
- **Assigned role:** developer
- **Dependencies:** Depends on Step 2, Step 4
- **Parallelizable:** No

### Step 6: ローカライズ話者名キーの追加
- **Description:** `Speaker_Ozi` / `Speaker_Protagonist` を `UIStrings` テーブルに追加し、ja/en/zh-Hans/zh-Hant の値を設定（エディタ拡張 or Localization API 経由を推奨）。
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes

### Step 7: シーンへの SO 紐付け
- **Description:** `DialogueManager` の `speakerMapping` に `SpeakerMapping.asset` を割り当て（Step 1 で特定したシーン）。
- **Assigned role:** developer
- **Dependencies:** Depends on Step 1, Step 3, Step 5
- **Parallelizable:** No

# Verification & Testing

## コンパイル確認
- 変更後、Unity Console にコンパイルエラーがないこと（`Unity.GetConsoleLogs`）。

## 機能確認（Play Mode）
1. **Ozi 行**: `OpeningScene_dialogue02_Ozi` 等の行で、話者名に「おじさん」（日本語ロケール時）が表示される。
2. **Protagonist 行**: `..._Protagonist` 行で「主人公」が表示される。
3. **Narration 行**: テスト用に `..._Narration` キーを持つ行を作成し、話者ラベル（`speakerText` GameObject）が非アクティブになり余白が消えること。直後に通常行へ進むと話者ラベルが再表示されること。
4. **フォールバック**: `OldManClick.asset`（`OldMan_Name` / `OldMan_Clicked`）など命名規則外のダイアログで、従来通り `speakerKey` ベースの話者名が表示されること。
5. **言語切り替え**: ロケールを en / zh-Hans に切り替え、話者名がローカライズ値（Uncle / 大叔 等）に更新されること（`OnLocaleChanged` → 再描画）。

## エッジケース
- `textKey` に `_` が無い／末尾が `_` のキー → `ExtractSuffix` が null を返し、フォールバック動作になること（例外を出さない）。
- `speakerMapping` 未割り当て（null）→ 全行フォールバック動作になり、例外を出さないこと。
- マッピングに無いサフィックス（例: `_Clicked`）→ フォールバック動作。
