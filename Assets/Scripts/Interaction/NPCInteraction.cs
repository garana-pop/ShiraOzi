using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
    public class NPCInteraction : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private DialogueEntry defaultDialogue;
        [SerializeField] private DialogueEntry[] itemSpecificDialogues;
        [SerializeField] private string[] itemIDs;

        public void Interact()
        {
            DialogueEntry toPlay = defaultDialogue;
            
            if (gameState != null && !string.IsNullOrEmpty(gameState.heldItemID))
            {
                for (int i = 0; i < itemIDs.Length; i++)
                {
                    if (gameState.heldItemID == itemIDs[i])
                    {
                        if (i < itemSpecificDialogues.Length)
                        {
                            toPlay = itemSpecificDialogues[i];
                        }
                        break;
                    }
                }
            }

            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.StartDialogue(toPlay);
            }
        }
    }
}
