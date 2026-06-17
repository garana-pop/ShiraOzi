# プロジェクト概要
- ゲームタイトル: ShiraOzi
- ハイレベルコンセプト: 探索・アイテム使用・多言語対応のナラティブ駆動型ポイント＆クリックアドベンチャー。
- プレイヤー: シングルプレイヤー
- インスピレーション / 参照ゲーム: ポイント＆クリック型アドベンチャー
- トーン / アートディレクション: 雰囲気のあるミステリー
- ターゲットプラットフォーム: スタンドアロン (Windows)
- 画面の向き / 解像度: 横向き (Landscape)
- レンダーパイプライン: URP

# 本タスクの目的
TitleScene の `SettingsPanel > Screen Container` / `WindowSize Container` を改修し、以下を実現する。

1. **Screen Container**: フルスクリーン / ウィンドウ をラジオボタン（2つの Toggle + ToggleGroup）で排他選択できるようにする。
2. **ウィンドウ選択時**: `WindowSize Dropdown` を操作可能（interactable=true）にし、画面サイズを選択できる。
   - 選択候補: **1920×1080 / 1280×720 / 960×540 / 640×360**
     - ※ 要件の「1980×1080」はタイプミスと判断し **1920×1080** を採用。
3. **WindowSize Dropdown**: 上記4候補をドロップダウンで選択 → 選択時に `Screen.SetResolution` を適用。
4. **フルスクリーン選択時**: `WindowSize Dropdown` をクリック不可（interactable=false）にし、Unity の Disabled カラーによりグレースケール表示にする。

## 仕様上の解釈（重要）
- 要件2「WindowSize Container がアクティブになる」と要件4「フルスクリーン時はグレースケールで表示（＝非表示ではない）」を両立させるため、
  **Container 自体は常に表示したまま、`WindowSize Dropdown` の `interactable` を切り替える**方式を採用する。
  - ウィンドウ選択 → `interactable = true`（通常表示・操作可）
  - フルスクリーン選択 → `interactable = false`（グレー表示・操作不可）
- この解釈で問題ないか、承認時にご確認ください。`SetActive` で完全に消す方式が希望であれば変更可能です。

# ゲームメカニクス
本タスクは設定 UI のみで、コアゲームプレイループには影響しない。

## コントロールと入力方法
- マウス操作（New Input System / EventSystem, `InputSystemUIInputModule`）。
- Toggle / TMP_Dropdown いずれも既存の `GraphicRaycaster` + `EventSystem` で動作。

# UI

## 現状の階層（実測）
```
SettingsPanel [SettingsManager, VerticalLayoutGroup]
  BGM Volume Container (Text + Slider)
  SFX Volume Container (Text + Slider)
  Screen Container (RectTransform)
    Text (TMP)
    Toggle [Toggle, UISoundTrigger]      ← 現状フルスクリーン用の単一トグル
      Background / Checkmark / Label
  WindowSize Container (RectTransform)
    Text (TMP)
    WindowSize Dropdown [TMP_Dropdown]   ← SettingsManager 未接続
  Language Container (Text + Language Dropdown)
  Reset Data Button / Close Button
```

## 改修後の階層（目標）
```
Screen Container [RectTransform, (任意)Horizontal/水平配置]
  Text (TMP)  … 見出し「画面モード」
  ToggleGroup は Screen Container にアタッチ（allowSwitchOff = false）
  Fullscreen Toggle [Toggle, UISoundTrigger]  (group=ScreenContainer ToggleGroup, Label「フルスクリーン」)
    Background / Checkmark / Label
  Windowed Toggle [Toggle, UISoundTrigger]    (group=ScreenContainer ToggleGroup, Label「ウィンドウ」)
    Background / Checkmark / Label
WindowSize Container (変更なしの構造)
  Text (TMP)  … 見出し「ウィンドウサイズ」
  WindowSize Dropdown [TMP_Dropdown]  ← SettingsManager に接続、固定4候補
```

## 動作（ワイヤーフレーム的説明）
- 起動時: 現在の `Screen.fullScreenMode` を見て、フルスクリーンなら `Fullscreen Toggle` を ON、それ以外なら `Windowed Toggle` を ON。
- `Fullscreen Toggle` ON → 画面をフルスクリーン化、`WindowSize Dropdown` グレーアウト（操作不可）。
- `Windowed Toggle` ON → 画面をウィンドウ化＋ドロップダウンで選択中のサイズを適用、`WindowSize Dropdown` 操作可。
- ドロップダウンでサイズ変更（ウィンドウ時のみ有効）→ 即時 `Screen.SetResolution` 適用。

# 主要アセットとコンテキスト

## 変更ファイル
- `Assets/Scripts/UI/SettingsManager.cs`（改修）
- `Assets/Scenes/TitleScene.unity`（`SettingsPanel > Screen Container` / `WindowSize Container` の UI 構築・参照接続）

## SettingsManager 現状の関連メンバ
```csharp
public UnityEngine.UI.Toggle fullscreenToggle; // 単一トグル（→ Toggle に接続済み）
public TMP_Dropdown resolutionDropdown;        // 現状 NULL（未接続）
...
private void Start() { fullscreenToggle.isOn = Screen.fullScreen; ... InitializeResolutionDropdown(); }
public void SetFullscreen(bool isFullscreen) { Screen.fullScreen = isFullscreen; }
public void SetResolution(int resolutionIndex) { /* Screen.resolutions[] から適用 */ }
```

## SettingsManager 改修後の想定シグネチャ
```csharp
[Header("Screen")]
public UnityEngine.UI.Toggle fullscreenToggle;   // 「フルスクリーン」ラジオ
public UnityEngine.UI.Toggle windowedToggle;     // 「ウィンドウ」ラジオ（任意：初期化用）
public TMP_Dropdown windowSizeDropdown;          // 旧 resolutionDropdown を置換／改名

// 固定のウィンドウサイズ候補（ドロップダウン表示順と一致）
private static readonly Vector2Int[] WindowSizes =
{
    new Vector2Int(1920, 1080),
    new Vector2Int(1280, 720),
    new Vector2Int(960, 540),
    new Vector2Int(640, 360),
};

private void InitializeScreenControls();          // ラジオ初期化＋リスナー登録＋初期interactable設定
private void InitializeWindowSizeDropdown();       // 固定4候補を投入、現在値選択、リスナー登録
public void SetFullscreen(bool isFullscreen);      // モード切替＋dropdown.interactable更新＋ウィンドウ時サイズ適用
public void SetWindowSize(int index);              // ウィンドウ時のみ Screen.SetResolution 適用
private void UpdateWindowSizeInteractable(bool isFullscreen); // dropdown.interactable = !isFullscreen
```

# 実装ステップ

## ステップ1: SettingsManager の改修
- **内容**:
  - `resolutionDropdown` を `windowSizeDropdown` に改名（または新規追加し旧フィールドは削除）。`windowedToggle` フィールドを追加。`[Header("Screen")]` を付与。
  - `WindowSizes` 固定配列（1920×1080 / 1280×720 / 960×540 / 640×360）を定義。
  - `InitializeResolutionDropdown()` を `InitializeWindowSizeDropdown()` に置換:
    - `ClearOptions()` → `"1920 x 1080"` 等の固定4文字列を `AddOptions()`。
    - 現在の `Screen.width/height` に最も近い候補を初期選択（一致が無ければ index 0）。
    - `onValueChanged += SetWindowSize`。
  - `Start()` を更新:
    - `bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;`
    - `fullscreenToggle.isOn = isFullscreen; windowedToggle.isOn = !isFullscreen;`
    - `fullscreenToggle.onValueChanged += SetFullscreen;`（windowedToggle はグループで連動するため必須リスナーは1つで可。必要なら両方で `RefreshState` を呼ぶ）
    - `InitializeWindowSizeDropdown();` の後 `UpdateWindowSizeInteractable(isFullscreen);`
  - `SetFullscreen(bool isFullscreen)`:
    - `Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;`
    - 非フルスクリーン時は選択中サイズを適用（`ApplyWindowSize(windowSizeDropdown.value)`）。
    - `UpdateWindowSizeInteractable(isFullscreen);`
  - `SetWindowSize(int index)`:
    - フルスクリーン中は無視（または何もしない）。範囲チェックの上 `Screen.SetResolution(WindowSizes[index].x, WindowSizes[index].y, FullScreenMode.Windowed);`
  - `UpdateWindowSizeInteractable(bool isFullscreen)`:
    - `if (windowSizeDropdown != null) windowSizeDropdown.interactable = !isFullscreen;`
    - これにより Unity Selectable の Disabled カラーで自動的にグレー表示（要件4）。
  - 旧 `SetResolution(int)` は削除（または互換のため残す場合は明記）。
- **担当ロール**: developer
- **依存関係**: なし
- **並列実行可能**: Yes（ステップ2と並行可能だが、最終的な参照接続はコード確定後が安全）

## ステップ2: Screen Container にラジオボタン2つ + ToggleGroup を構築
- **シーン**: `Assets/Scenes/TitleScene.unity` → `SettingsPanel > Screen Container`
- **内容**:
  - `Screen Container` に `ToggleGroup` を追加（`Allow Switch Off = false`）。
  - 既存 `Toggle` を `Fullscreen Toggle` にリネーム、`Label` を「フルスクリーン」に設定、`Toggle.group` に Screen Container の ToggleGroup を割当。
  - `Fullscreen Toggle` を複製して `Windowed Toggle` を作成、`Label`「ウィンドウ」、同じ ToggleGroup を割当。
  - 2トグルが重ならないよう `RectTransform` の anchoredPosition を調整（既存コンテナは手動配置のため、`Fullscreen Toggle` と `Windowed Toggle` を横並び or 縦並びに配置）。任意で `HorizontalLayoutGroup` を Screen Container に付与して自動整列してもよい。
  - 両トグルとも既存の `UISoundTrigger` を維持（複製で引き継がれる）。
- **担当ロール**: developer
- **依存関係**: なし
- **並列実行可能**: Yes

## ステップ3: SettingsManager への参照接続
- **シーン**: `Assets/Scenes/TitleScene.unity` → `SettingsPanel` の `SettingsManager`
- **内容**:
  - `fullscreenToggle` ← `Fullscreen Toggle`
  - `windowedToggle` ← `Windowed Toggle`
  - `windowSizeDropdown` ← `WindowSize Container > WindowSize Dropdown`（現状未接続を解消）
  - 既存の `bgmSlider / sfxSlider / languageDropdown / gameState / resetConfirmationPanel` は維持。
- **担当ロール**: developer
- **依存関係**: ステップ1（コード確定）、ステップ2（オブジェクト作成）
- **並列実行可能**: No

## ステップ4（任意）: グレースケール強化・ローカライズ
- **内容**:
  - グレー表示を強める場合: `WindowSize Dropdown` の TMP_Dropdown / 子 Image の Disabled カラーを明示設定、または `WindowSize Container` に `CanvasGroup` を付け interactable 連動で `alpha` を下げる。
  - トグルラベル「フルスクリーン」「ウィンドウ」を `UIStrings` ローカライズテーブルに追加し `LocalizeStringEvent` で多言語化（本プロジェクトはローカライズ優先方針）。
- **担当ロール**: developer
- **依存関係**: ステップ2/3
- **並列実行可能**: Yes

# 検証とテスト
- **ラジオ排他**: `Fullscreen Toggle` と `Windowed Toggle` が同時に ON にならない（ToggleGroup, allowSwitchOff=false で常にどちらか1つ ON）。
- **フルスクリーン選択**:
  - 画面がフルスクリーン化する。
  - `WindowSize Dropdown` がグレーアウトし、クリックしても開かない（`interactable=false`）。
- **ウィンドウ選択**:
  - 画面がウィンドウ化する。
  - `WindowSize Dropdown` が通常表示・操作可能になる。
  - 候補が **1920×1080 / 1280×720 / 960×540 / 640×360** の4件のみ表示される。
  - 各サイズ選択でウィンドウ解像度が即時変更される（`Screen.SetResolution`）。
- **初期化**:
  - 起動時、現在の画面モードに応じて正しい方のトグルが ON、ドロップダウンの interactable 状態も一致。
- **回帰**:
  - 設定パネルの BGM/SE スライダー・言語ドロップダウン・データ初期化が従来どおり動作。
  - Console にエラー/警告が出ないこと（特に旧 `SetResolution` 参照が UnityEvent に残っていないか確認）。
- **手動確認手順**: TitleScene 再生 → SettingsButton → SettingsPanel 表示 → 上記各項目を確認。
