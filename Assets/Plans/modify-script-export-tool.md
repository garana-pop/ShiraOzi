# プロジェクト概要
- ゲームタイトル: 知らないおじぃちゃんが住みついて（仮） (ShiraOzi)
- 概要: ポイント＆クリック形式の2Dナラティブパズルアドベンチャーゲーム。
- プレイヤー: シングルプレイヤー
- ターゲットプラットフォーム: PC (Standalone Windows)
- レンダーパイプライン: URP

# ゲームメカニクス
- ポイント＆クリック操作、インベントリ管理、分岐ダイアログ、パズル解決。
- 開発者ツール `ScriptExportTool` を使用して、スクリプトの書き出しとGitへのプッシュを管理。

# UI
- エディタウィンドウ (`ScriptExportTool`)。
- 「GitにPush」タブを修正し、コミットの件名と本文を分けて入力できるようにする。

# 主要アセットとコンテキスト
- `Assets/Editor/ScriptExportTool.cs`: スクリプト出力およびGit連携を管理するスクリプト。

# 実装手順

## 1. `ScriptExportTool.cs` の変数を修正
- `gitCommitMessage` を `gitCommitSubject` と `gitCommitBody` に置き換える。
- ファイル: `Assets/Editor/ScriptExportTool.cs`

## 2. `DrawGitPushSection` のUIを更新
- コミットの「件名」（TextField）と「本文」（TextArea）を別々のフィールドとして表示するように変更。
- それぞれに分かりやすいラベルを追加。
- ファイル: `Assets/Editor/ScriptExportTool.cs`

## 3. コミットメッセージの整形ロジックを実装
- ヘルパーメソッド `GetFormattedCommitMessage()` を作成し、以下のロジックを実装：
    - 本文のみ入力され、件名が空の場合は、ポップアップを表示して `null` を返す。
    - 件名のみ入力されている場合は、件名のみを返す。
    - 件名と本文の両方が入力されている場合は、件名と本文の間に空行を挟んで整形した文字列を返す。
- ファイル: `Assets/Editor/ScriptExportTool.cs`

## 4. Gitコミット処理の更新
- 「② git commit」ボタンの処理で `GetFormattedCommitMessage()` を使用するように更新。
- 「▶ add → commit → push を一括実行」ボタンの処理（`RunGitAddCommitPush`）で `GetFormattedCommitMessage()` を使用し、戻り値が `null` の場合は処理を中断するように修正。
- ファイル: `Assets/Editor/ScriptExportTool.cs`

# 検証とテスト
- 手動検証：
    1. `Tools > Script Export Tool` からウィンドウを開く。
    2. 「GitにPush」タブへ移動。
    3. テストケース1：件名のみ入力して「② git commit」を実行。ログに件名のみが表示されることを確認。
    4. テストケース2：件名と本文を入力して「② git commit」を実行。ログで件名と本文の間に空行があることを確認。
    5. テストケース3：本文のみ入力して「② git commit」を実行。エラーポップアップが表示され、コミットが行われないことを確認。
    6. テストケース4：一括実行ボタンについても同様のテストを行い、正しく動作することを確認。
