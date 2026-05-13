using UnityEngine;

namespace SystemicOverload.Interaction
{
    public sealed class NpcInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "[E] 대화";
        [SerializeField] private string npcName = "NPC";
        [SerializeField] [TextArea(2, 5)] private string[] dialogueLines = { "안녕하세요!" };

        private int dialogueIndex;

        public string GetPrompt() => promptText;

        public void Interact(GameObject actor)
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                return;
            }

            string line = dialogueLines[dialogueIndex];
            Debug.Log($"[{npcName}] {line}");
            dialogueIndex = (dialogueIndex + 1) % dialogueLines.Length;
        }
    }
}
