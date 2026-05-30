# プロジェクト概要
- ゲームタイトル: ShiraOzi
- 高レベルコンセプト: 2Dアドベンチャー/パズルゲーム。ポイント＆クリック形式の探索と対話が中心。
- ターゲットプラットフォーム: Standalone Windows
- レンダリングパイプライン: URP

# ゲームメカニクス
## コアゲームプレイループ
探索、アイテム収集、ダイアログ進行、パズル解決。
## コントロールと入力方法
マウスによるポイント＆クリック（New Input System使用）。

# UI
タイトル画面、設定パネル、インベントリ、ダイアログボックス。

# 主要アセットとコンテキスト
- `SoundManager.cs`: SE再生を管理するシングルトンクラス。`PlayHoverSound()` と `PlayClickSound()` を持つ。
- `click_SE.mp3`: クリック時の効果音。
- `Hove_SE.mp3`: ホバー時の効果音。
- `TitleScene`: 今回の対象となるシーン。

# 実装手順
## 1. UI音響制御用スクリプトの作成
UI要素（Button, Toggle, Slider）のインタラクションを検知してSEを鳴らす汎用スクリプト `UISoundTrigger.cs` を作成します。

- **ファイル名**: `Assets/Scripts/UI/UISoundTrigger.cs`
- **機能**:
    - `IPointerEnterHandler`: マウスが乗った時に `SoundManager.Instance.PlayHoverSound()` を呼び出す（Button, Toggleのみ）。
    - `IPointerClickHandler` または各コンポーネントのイベント:
        - `Button`: クリック時に `PlayClickSound()`。
        - `Toggle`: クリック（値変更）時に `PlayClickSound()`。
        - `Slider`: 操作開始（PointerDown）時に `PlayClickSound()`。

## 2. TitleSceneのUI要素へのアタッチ
TitleScene内の以下のオブジェクトに `UISoundTrigger` コンポーネントを追加します。

- **Buttons**:
    - `StartButton`
    - `SettingsButton`
    - `GameEndButton`
    - `Close Button` (設定パネル内)
    - `Yes, Reset Button` (リセット確認パネル内)
    - `Cancel Button` (リセット確認パネル内)
    - `Reset Data Button` (設定パネル内)
- **Toggles**:
    - `Toggle` (フルスクリーン設定用)
- **Sliders**:
    - `bgmSlider`
    - `sfxSlider`

## 3. SoundManagerの設定確認
TitleSceneに配置されている `SoundManager` オブジェクトの `Hover Clip` と `Click Clip` フィールドに、指定されたMP3ファイルが割り当てられていることを確認（または設定）します。
- `Click Clip`: `Assets/Sounds/SE/click_SE.mp3`
- `Hover Clip`: `Assets/Sounds/SE/Hove_SE.mp3`

# 検証とテスト
- **動作確認**:
    - 各ボタンの上にマウスを乗せた時にホバー音が鳴ること。
    - 各ボタンをクリックした時にクリック音が鳴ること。
    - 設定パネルのトグルをクリックした時に音が鳴ること。
    - ボリュームスライダーをクリック（またはドラッグ開始）した時に音が鳴ること。
- **エッジケース**:
    - 設定パネルが非表示の状態でも、表示された瞬間に正しく音が鳴るか確認。
    - 音量設定（SFX）が0の時に音が鳴らないことを確認。
