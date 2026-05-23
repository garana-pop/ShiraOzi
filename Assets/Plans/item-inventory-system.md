# プロジェクト概要
- ゲームタイトル: ShiraOzi
- コンセプト: ポイント＆クリック形式の2Dミステリーアドベンチャー。
- プレイヤー: シングルプレイヤー
- ターゲットプラットフォーム: スタンドアロン Windows
- レンダーパイプライン: URP 2D
- UIシステム: uGUI (Canvasベース)

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーはシーンを探索し、オブジェクトを調べ、アイテムを拾います。アイテムはストーリーの進行やパズルの解決に使用されます。
## 操作・入力方法
- マウス操作: オブジェクトにカーソルを合わせるとカーソルが変化します。
- クリック: オブジェクトとのインタラクション、アイテムの取得、インベントリ管理。
- アイテム管理: "ItemPanel"をクリックすると、所持アイテムの一覧が表示されます。一覧からアイテムを選択すると、そのアイテムが「アクティブ」になり、HUDに表示されます。

# UI
- **ItemPanel**: 現在アクティブなアイテムのアイコンを表示するHUD要素。
- **InventoryPanel**: ItemPanelをクリックしたときに表示される、画面中央のオーバーレイパネル。所持しているすべてのアイテムをスクロール可能なリストで表示します。
- **InventoryItem**: リスト内の各アイテム項目（ボタン）。アイテムのアイコンと名前を表示します。

# 主要アセットとコンテキスト
### スクリプト
- `ItemData.cs`: アイテム定義用のScriptableObject。
- `GameState.cs`: 複数のアイテム所持とアクティブアイテムを追跡するように更新。
- `ItemPickup.cs`: `ItemData`を扱うように更新。
- `InventoryUI.cs`: インベントリリストの表示を管理。
- `InventoryItemUI.cs`: リスト内の各アイテム要素を制御。
- `UIManager.cs`: HUDとインベントリの橋渡し役として更新。

### シーンオブジェクト
- `ItemPanel`: 既存のオブジェクト。Buttonコンポーネントの追加と、Imageの更新ロジックが必要。
- `InventoryPanel`: アイテムリスト用の新規プレハブ/オブジェクト。

# 実装ステップ
## 1. データ構造の更新
1. `ItemData.cs` (ScriptableObject) を作成。フィールド: `string itemID`, `string itemName`, `Sprite icon`。
2. `GameState.cs` を修正:
    - `List<ItemData> acquiredItems` を追加。
    - `ItemData activeItem` を追加。
    - `AddItem(ItemData item)` メソッドを実装: リストに追加し、`activeItem` が空なら設定。
    - `SetActiveItem(ItemData item)` メソッドを実装: `activeItem` を更新。
    - **依存関係**: なし。

## 2. インタラクションの更新
1. `ItemPickup.cs` を修正:
    - `string itemID` を `ItemData itemData` に変更。
    - `PickUp()` 内で `gameState.AddItem(itemData)` を呼び出すように変更。
2. **依存関係**: ステップ 1。

## 3. UIロジック - インベントリリスト
1. `InventoryItemUI.cs` を作成:
    - アイテムのアイコンと名前を表示。
    - クリック時に自身をアクティブアイテムとして設定するイベント。
2. `InventoryUI.cs` を作成:
    - `Open()` メソッド: `GameState.acquiredItems` からリストを生成。
    - `Close()` メソッド: パネルを非表示にする。
    - `SelectItem(ItemData item)` メソッド: GameStateを更新し、表示をリフレッシュ。
3. **依存関係**: ステップ 1。

## 4. UIロジック - UIManagerとの統合
1. `UIManager.cs` を更新:
    - `InventoryUI` への参照を追加。
    - `ItemPanel` 上のアクティブアイテム表示用 `Image` への参照を追加。
    - `OnItemPanelClicked()` メソッドを実装: `InventoryUI` の表示/非表示を切り替え。
    - `RefreshItemDisplay()` メソッドを実装: `GameState.activeItem` に基づいてHUDのアイコンを更新。
2. **依存関係**: ステップ 3。

## 5. シーンセットアップ
1. `Canvas` 内に `InventoryPanel` を作成。
    - `ScrollView` を追加。
    - `CloseButton` を追加。
2. `InventoryItem` プレハブを作成。
3. `ItemPanel` に `Button` コンポーネントを追加し、`UIManager.OnItemPanelClicked` にリンク。
4. `UIManager`, `InventoryUI`, `ItemPanel` の各インスペクター参照を設定。
5. **依存関係**: ステップ 1-4。

# 検証とテスト
1. **取得テスト**: `ItemPickup` を持つオブジェクトをクリックし、内部リストに追加されるか確認。
2. **自動アクティブ化テスト**: 最初のアイテムを拾った際、即座に `ItemPanel` にアイコンが表示されるか確認。
3. **リスト表示テスト**: `ItemPanel` をクリックし、`InventoryPanel` が開き、取得したアイテムが表示されるか確認。
4. **アイテム切り替えテスト**: 2つのアイテムを取得し、インベントリで2番目を選択。`ItemPanel` のアイコンが更新されるか確認。
5. **空の状態テスト**: インベントリが空の状態でクリックしてもエラーが発生しないことを確認。
