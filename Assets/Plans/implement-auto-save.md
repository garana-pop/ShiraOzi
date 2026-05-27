# プロジェクト概要
- ゲームタイトル: ShiraOzi
- コンセプト: ポイント＆クリック形式の2Dナラティブパズルアドベンチャーゲーム。
- プレイヤー: シングルプレイヤー
- ターゲットプラットフォーム: PC (StandaloneWindows64)
- レンダーパイプライン: URP

# ゲームメカニクス
## コアゲームプレイループ
環境を探索し、アイテムを収集し、分岐するダイアログを通じてチャプターを進める。
## 操作・入力方法
ポイント＆クリック（新Input Systemを使用）。

# UI
- スタート、設定、終了ボタンを備えたタイトル画面。
- スタート時のロジックはセーブデータのフラグに基づいて分岐。

# 主要アセットとコンテキスト
- `GameState.cs`: ゲームの状態を保持するScriptableObject。`hasSeenOpening` フラグを含む。
- `SaveData.cs` (新規): JSONシリアライズ用のデータ構造。
- `SaveManager.cs` (新規): セーブ/ロード処理および終了時の自動セーブを管理。
- `TitleController.cs`: タイトル画面のインタラクション（シーン遷移）を制御。

# 実装ステップ
## 1. SaveDataクラスの作成
- `Assets/Scripts/Core/SaveData.cs` を作成。
- `hasSeenOpening`（オープニング遷移済みフラグ）を含む、`GameState` の永続化が必要なフィールドを定義。

## 2. SaveManagerの実装
- `Assets/Scripts/Core/SaveManager.cs` をシングルトンとして作成。
- **ロード処理**: 起動時に `save.json` を読み込み、`hasSeenOpening` などの値を `GameState` に反映させる。
- **セーブ処理**: ゲーム終了時（`OnApplicationQuit`）に、現在の `GameState`（フラグ状況を含む）をJSON形式で `Application.persistentDataPath/save.json` に保存する。
- アイテムIDから `ItemData` への解決ロジックも実装。

## 3. TitleControllerの遷移ロジックの調整・確認
- `TitleController.OnStartClicked` では、`SaveManager` によってロードされた `GameState.hasSeenOpening` の値を参照する。
- **フラグが false の場合**: 初めてのプレイと判断し、`OpeningScene` に遷移する。この際、フラグを `true` に更新する。
- **フラグが true の場合**: すでにオープニングを通過済みと判断し、直接 `MainScene` に遷移する。

## 4. TitleSceneでの設定
- `Managers` オブジェクトに `SaveManager` をアタッチ。
- `GameState` と `ItemData` のリストを適切に設定。

# 検証とテスト
1. **初期状態**: セーブファイルがない状態でスタート。`OpeningScene` に遷移することを確認。
2. **自動セーブ**: ゲームを終了し、`save.json` に `hasSeenOpening: true` が書き込まれていることを確認。
3. **ロード遷移**: 再起動後にスタート。直接 `MainScene` に遷移することを確認。
4. **データの整合性**: JSONファイルが正しく生成・読み込みされていることを確認。
