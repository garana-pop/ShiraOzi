using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ShiraOzi.Interaction
{
    public class PasswordPuzzle : MonoBehaviour
    {
        public string correctPassword = "4771";
        public TMP_InputField inputField;
        public UnityEvent onCorrectPassword;
        public UnityEvent onWrongPassword;

        public void SubmitPassword()
        {
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
