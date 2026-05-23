# プロジェクト概要
- ゲームタイトル: ShiraOzi
- コンセプト: ローカライズされたダイアログとインタラクティブなオブジェクトを備えた、物語重視の2Dミステリー/アドベンチャーゲーム。
- プレイヤー: シングルプレイヤー
- ターゲットプラットフォーム: スタンドアロン Windows
- レンダリングパイプライン: URP (2D)
- 画面の向き: 横向き (Landscape)
- UIシステム: uGUI (Unity UI)

# ゲームメカニクス
## コアゲームプレイループ
探索と環境（NPC、アイテム）とのインタラクションを通じてダイアログを発生させ、ストーリーを進める。
## 操作と入力方法
マウスによるカーソルシステム。`InteractableObject` をクリックすることでインタラクションロジックが実行される。

# UI
`DialoguePanel` は `UIManager` と `DialogueManager` によって管理される永続的なUI要素。話者の名前とダイアログテキストを表示する。

# 主要アセットとコンテキスト
- `UIManager.cs`: UIパネルの表示/非表示を管理。
- `DialogueManager.cs`: ダイアログの流れとローカライズを管理。
- `NPCInteraction.cs`: NPCのダイアログ発生を制御。
- `DialogueLayoutSettings.cs` (新規): レイアウトの上書き設定を保持するコンポーネント。

# 実装ステップ

## 1. DialogueLayoutSettings コンポーネントの作成
レイアウト情報を保持するための新しいスクリプト `Assets/Scripts/UI/DialogueLayoutSettings.cs` を作成します。

```csharp
using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    public class DialogueLayoutSettings : MonoBehaviour
    {
        [Header("RectTransform Settings")]
        public Vector2 anchorMin = new Vector2(0, 0);
        public Vector2 anchorMax = new Vector2(1, 0.25f);
        public Vector2 pivot = new Vector2(0.5f, 0);
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector2 sizeDelta = Vector2.zero;

        public void Apply()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.SetTemporaryLayout(this);
            }
        }
    }
}
```

## 2. UIManager.cs の更新
`DialoguePanel` のレイアウトを適用・リセットするメソッドを追加します。また、`Awake` 時にデフォルト値を保存するようにします。

- **ファイル**: `Assets/Scripts/UI/UIManager.cs`
- **変更内容**:
    - デフォルトの `RectTransform` 値を保持するフィールドを追加。
    - `SetDialogueLayout(DialogueLayoutSettings settings)` メソッドを追加。
    - `ResetDialogueLayout()` メソッドを追加。

## 3. DialogueManager.cs の更新
一時的なレイアウトの上書きを処理し、ダイアログ終了時に確実にリセットされるようにロジックを追加します。

- **ファイル**: `Assets/Scripts/Core/DialogueManager.cs`
- **変更内容**:
    - 一時的なレイアウト設定を保持するプライベートフィールドを追加。
    - `SetTemporaryLayout(DialogueLayoutSettings layout)` メソッドを追加。
    - `StartDialogue` メソッドでレイアウトが設定されていれば適用するように変更。
    - `EndDialogue` メソッドでレイアウトをリセットするように変更。

## 4. NPCInteraction との統合
NPCに `DialogueLayoutSettings` コンポーネントがある場合、`Interact()` 時に自動的に適用されるように更新します。

- **ファイル**: `Assets/Scripts/Interaction/NPCInteraction.cs`
- **変更内容**:
    - `Interact()` 内で、自身のGameObjectに `DialogueLayoutSettings` があれば `Apply()` を呼び出すように変更。

## 5. 検証とテスト
1. シーン内のNPCに `DialogueLayoutSettings` コンポーネントを追加。
2. インスペクターで `anchorMax` や `anchoredPosition` を変更（例：画面上部に移動）。
3. そのNPCとインタラクトし、ダイアログボックスが新しい位置に表示されることを確認。
4. ダイアログを終了し、デフォルトの位置（画面下部）に戻ることを確認。
5. アイテムについても、`InteractableObject.onClick` の UnityEvent に `Apply()` 呼び出しを追加して確認。

# 検証とテスト項目
- **手動確認**:
    - 設定を変更したNPCとのインタラクション時に `DialoguePanel` が指定の位置に移動するか。
    - ダイアログ終了後に `DialoguePanel` がデフォルトの位置にリセットされるか。
    - 複数のNPCで異なるレイアウト設定が正しく機能するか。
    - レイアウト設定がないオブジェクトの場合はデフォルトのレイアウトが使用されるか。
