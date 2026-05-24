# プロジェクト概要
- ゲームタイトル: ShiraOzi
- ハイレベルコンセプト: 探索、アイテムの使用、多言語対応のミステリーに焦点を当てたナラティブ駆動型アドベンチャー/パズルゲーム。
- プレイヤー: シングルプレイヤー
- インスピレーション / 参照ゲーム: ポイント＆クリック型アドベンチャーゲーム
- トーン / アートディレクション: 雰囲気のあるミステリー
- ターゲットプラットフォーム: スタンドアロン (Windows)
- 画面の向き / 解像度: 横向き (Landscape)
- レンダーパイプライン: URP

# ゲームメカニクス
## コアゲームプレイループ
- 2Dシーンの探索、環境やNPCとのインタラクション、インベントリ管理、およびチャプター進行のためのパズル解決。

## コントロールと入力方法
- マウスによるポイント＆クリック（New Input System / EventSystem）。

# UI
## タイトルシーン (Title Scene)
- **StartButton**: 進行状況に応じて OpeningScene と MainScene を分岐させるロジック。
- **SettingsButton**: 音量、画面、言語、データ初期化オプションを備えた設定パネルを表示。
- **GameEndButton**: アプリケーションを終了。

## 設定パネル (Settings Panel)
- **BGM/SE スライダー**: オーディオ音量を調整。
- **画面モード**: フルスクリーン/ウィンドウ表示の切り替え（トグルまたはドロップダウン）。
- **言語設定**: 英語、日本語、中国語（簡体字/繁体字）を選択するドロップダウン。
- **データ初期化**: 確認ポップアップを表示した上で GameState をクリアするボタン。

# 主要アセットとコンテキスト
- `Assets/Scripts/Core/GameState.cs`: `hasSeenOpening` フラグを追加。
- `Assets/Scripts/Core/SoundManager.cs`: 音量制御ロジックを追加。
- `Assets/Scripts/UI/SettingsManager.cs`: ローカライズ、サウンド、リセット処理を処理するように拡張。
- `Assets/Scripts/UI/TitleController.cs`: タイトルシーンのボタンイベントを処理する新しいスクリプト。
- `TitleScene`: 設定パネルと確認ポップアップのUIアップデート。

# 実装ステップ
## 1. GameState の更新
- **ファイル**: `Assets/Scripts/Core/GameState.cs`
- **変更内容**: 
  - `public bool hasSeenOpening = false;` を追加。
  - `ResetState()` を更新し、`hasSeenOpening = false;` に戻すようにする。
- **依存関係**: なし。

## 2. SoundManager の拡張
- **ファイル**: `Assets/Scripts/Core/SoundManager.cs`
- **変更内容**: 
  - `public void SetBGMVolume(float volume)` と `public void SetSFXVolume(float volume)` を追加。
  - AudioSource の音量を調整するロジックを実装（BGM用の AudioSource がない場合は追加）。
- **依存関係**: なし。

## 3. SettingsManager の拡張
- **ファイル**: `Assets/Scripts/UI/SettingsManager.cs`
- **変更内容**:
  - BGM/SEスライダー、言語ドロップダウン、リセット確認パネルの参照を追加。
  - `SetBGMVolume(float)`, `SetSFXVolume(float)` を実装。
  - `LocalizationSettings` を使用して `SetLanguage(int index)` を実装。
  - `ShowResetConfirmation()` と `ConfirmReset()` を実装。
- **依存関係**: ステップ1、ステップ2。

## 4. TitleController の実装
- **ファイル**: `Assets/Scripts/UI/TitleController.cs` (新規作成)
- **内容**:
  - `OnStartClicked()`: `gameState.hasSeenOpening` を確認。false の場合は `OpeningScene` をロード（フラグを立てる）、それ以外は `MainScene` をロード。
  - `OnSettingsClicked()`: 設定パネルを表示/切り替え。
  - `OnGameEndClicked()`: `Application.Quit()` を実行。
- **依存関係**: ステップ1。

## 5. TitleScene でのUI構築
- **シーン**: `Assets/Scenes/TitleScene.unity`
- **項目**:
  - 以下の要素を含む `SettingsPanel` (UI GameObject) を作成:
    - BGMスライダー、SEスライダー。
    - フルスクリーン切り替え（既存の SettingsManager のロジックを使用）。
    - 言語ドロップダウン。
    - リセットボタン。
  - `ResetConfirmationPopup` (UI GameObject, 初期状態は非アクティブ) を作成:
    - 「本当に初期化しますか？」というテキスト。
    - 「はい」と「いいえ」のボタン。
  - `SettingsPanel` に `SettingsManager` をアタッチ。
  - 新しい GameObject `TitleController` を作成し、スクリプトをアタッチ。
  - `StartButton`, `SettingsButton`, `GameEndButton` を `TitleController` のメソッドに接続。
- **依存関係**: ステップ3、ステップ4。

# 検証とテスト
- **ゲーム開始**:
  - セーブデータを削除/リセット -> Start をクリック -> `OpeningScene` がロードされることを確認。
  - タイトルに戻る -> Start をクリック -> `MainScene` がロードされることを確認。
- **設定**:
  - BGM/SE音量を変更 -> 音が変わることを確認。
  - 言語を変更 -> UIテキストが更新されることを確認。
  - フルスクリーンを切り替え -> 画面モードが変わることを確認。
- **データ初期化**:
  - 初期化をクリック -> 確認ポップアップが表示されることを確認。
  - 「はい」をクリック -> `hasSeenOpening` やインベントリがクリアされることを確認。
- **ゲーム終了**:
  - 終了をクリック -> アプリケーションが終了することを確認（ビルド環境）。
