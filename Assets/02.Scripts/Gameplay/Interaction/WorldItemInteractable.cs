using UnityEngine;
using UnityEngine.Events;

namespace SystemicOverload.Interaction
{
    public sealed class WorldItemInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string itemName = "아이템";
        [SerializeField] private string promptText = "[E] 획득";
        [SerializeField] private UnityEvent<GameObject> onPickedUp;

        private bool pickedUp;

        public string GetPrompt() => pickedUp ? string.Empty : promptText;

        public void Interact(GameObject actor)
        {
            if (pickedUp)
            {
                return;
            }

            pickedUp = true;
            Debug.Log($"[WorldItem] {itemName} 획득 / Actor: {actor?.name}");
            onPickedUp?.Invoke(actor);
            gameObject.SetActive(false);
        }
    }
}
