using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ShiraOzi.Interaction
{
    /// <summary>
    /// パスワード入力を判定するパズル要素を管理するコンポーネント。
    /// </summary>
    public class PasswordPuzzle : MonoBehaviour
    {
        public string correctPassword = "4771"; // 正解のパスワード文字列
        public TMP_InputField inputField;       // 入力用のUIフィールド
        public UnityEvent onCorrectPassword;    // 正解時に発行されるイベント
        public UnityEvent onWrongPassword;      // 不正解時に発行されるイベント

        /// <summary>
        /// 入力されたパスワードをチェックする。
        /// </summary>
        public void SubmitPassword()
        {
            // 入力内容と正解を照合
            if (inputField != null && inputField.text == correctPassword)
            {
                Debug.Log("Password Correct!");
                onCorrectPassword?.Invoke();
            }
            else
            {
                Debug.Log("Password Wrong!");
                onWrongPassword?.Invoke();
            }
        }
    }
}
