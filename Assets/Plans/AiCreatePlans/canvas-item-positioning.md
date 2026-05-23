# プロジェクト概要
- ゲームタイトル: ShiraOzi
- コンセプト: 2Dアドベンチャー/ミステリー。NPCとの会話やアイテム収集を通じてストーリーを進行させ、日記を埋めていく。
- プレイヤー: シングルプレイヤー
- レンダリングパイプライン: URP
- ターゲットプラットフォーム: スタンドアロン (Windows)
- 入力システム: New Input System + UI EventSystem

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーはシーンを探索し、オブジェクトやNPCとインタラクトしてアイテムを収集し、ストーリーを進行させて日記のエントリを解放する。
## 操作と入力方法
- マウスによるポイント＆クリック操作。
- インタラクト可能なオブジェクトにホバーするとカーソルが変化。
- クリックで会話開始またはアイテム取得。

# UI
- **DialoguePanel**: 話者の名前とテキストを表示。
- **ItemPanel**: 現在保持しているアイテムを表示。
- **Canvas**: ScreenSpaceOverlayを使用するメインUIコンテナ。

# 主要アセットとコンテキスト
- `InteractableObject.cs`: インタラクションのためのポインターイベント（ホバー、クリック）を処理。
- `ItemPickup.cs`: アイテムの取得ロジックを担当。
- `GameState.cs`: 進行状況（現在の章、所持アイテムID）を保持するScriptableObject。
- `UIManager.cs`: UIの表示/非表示と内容を管理。

# 実施ステップ
## 1. ItemPickupの強化（Canvas対応と永続化）
`ItemPickup.cs` を修正して、UI要素として動作し、かつ `GameState` に基づいて自身の表示状態を自動管理するようにします。

### `ItemPickup.cs` の変更点:
- `Start` メソッドを追加し、`gameState.heldItemID` を確認。もし自身の `itemID` と一致すれば、そのオブジェクトを無効化する（取得済みなら表示しない）。
- `PickUp` メソッド内で、アイテム取得時に `UIManager.Instance.UpdateHeldItem()` を呼び出すようにし、UIが即座に更新されるようにする。
- `[RequireComponent(typeof(InteractableObject))]` を追加し、設定漏れを防ぐ。

## 2. アイテムをCanvas内へ移行
アイテムをCanvas内で配置するための手順を提供します。
- `SpriteRenderer` の代わりに `UnityEngine.UI.Image` を使用。
- `Image` の `Raycast Target` が有効であることを確認。
- `InteractableObject` と `ItemPickup` をUIオブジェクトにアタッチ。

## 3. UIManagerのフィードバック改善
アイテム取得時の状態変化が即座にUIに反映されるようにします。

# 検証とテスト
- **ビジュアル確認**: 新しいアイテムをCanvas内に `Image` として配置。ゲームビューで意図した位置に表示されることを確認。
- **インタラクション確認**: アイテムにホバーした際にカーソルが変化し、クリックするとアイテムが消え、`ItemPanel` にアイテムIDが表示されることを確認。
- **永続化確認**: アイテム取得後、シーンを再読み込みしてもそのアイテムが表示されないことを確認。
- **エッジケース**: `GraphicRaycaster` を通じて、クリックイベントが正しく発行されることを確認。
