# プロジェクト概要
- ゲームタイトル: ShiraOzi
- コンセプト: チャプターベースの進行システムを備えた、2Dストーリー主導のアドベンチャー/パズルゲーム。
- プレイヤー: シングルプレイヤー。
- ターゲットプラットフォーム: Windows (PC)。
- レンダリングパイプライン: UniversalRP (URP)。

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーはオブジェクトやNPCとインタラクトして物語を進め、パズルを解きます。会話（ダイアログ）がゲームの核となる要素です。

## 操作方法と入力
- マウスによるインタラクション。
- UIボタンによる会話の進行。

# UI
UIにはUGUI（Unity UI）を使用し、テキスト表示にはTextMeshProを採用しています。会話ボックスは `UIManager` と `DialogueManager` によって管理されています。

# 主要アセットとコンテキスト
- `Assets/Scripts/UI/TypewriterEffect.cs`: テキストの一文字ずつ表示するアニメーションを処理する新しいスクリプト。
- `Assets/Scripts/UI/UIManager.cs`: タイプライター演出の統合と、スキップロジックの処理のために修正が必要です。
- `Assets/Scripts/Core/DialogueManager.cs`: 会話行の進行を制御します。

# 実装ステップ
## 1. TypewriterEffect コンポーネントの作成
- **説明**: `Assets/Scripts/UI/TypewriterEffect.cs` を作成します。このコンポーネントは `TMP_Text.maxVisibleCharacters` を使用してタイプライターアニメーションを処理します。
- **担当ロール**: developer
- **依存関係**: なし
- **並列実行**: 可能

## 2. UIManager への TypewriterEffect の統合
- **説明**: 
    - `UIManager` に `TypewriterEffect` フィールドを追加します。
    - `UIManager.ShowDialogue` を修正し、テキストを直接設定するのではなくタイプライターアニメーションを開始するようにします。
    - `UIManager.OnAdvanceClicked` を修正し、「タイピングのスキップ」ロジックを実装します。タイピング中の場合は即座に全文字を表示し、完了している場合は次の行へ進みます。
    - `RefreshSceneReferences` が新しいコンポーネントの参照を処理するように更新します。
- **担当ロール**: developer
- **依存関係**: ステップ 1
- **並列実行**: 不可

## 3. シーン内でのUI設定
- **説明**: 
    - `TitleScene`、`OpeningScene`、`MainScene`（またはそれらで使用されているプレハブ）を開きます。
    - `DialogueText` オブジェクト（`TextMeshProUGUI` が付いているもの）に `TypewriterEffect` コンポーネントをアタッチします。
    - `UIManager` コンポーネントの `TypewriterEffect` 参照を割り当てます。
    - インスペクターで `TypeSpeed`（表示速度）を設定します。
- **担当ロール**: developer
- **依存関係**: ステップ 2
- **並列実行**: 不可

# 検証とテスト
- **テスト 1**: ゲームを開始し、オープニングの会話が一文字ずつ表示されることを確認します。
- **テスト 2**: テキストのタイピング中に「進む」ボタンをクリックし、即座に全表示されることを確認します。
- **テスト 3**: テキストが表示完了した後に「進む」ボタンをクリックし、次の行に進むことを確認します。
- **テスト 4**: 設定で言語を変更し、ローカライズされたテキストでもタイプライター演出が正しく動作することを確認します。
