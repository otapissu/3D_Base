using TMPro;
using UnityEngine;

namespace SystemicOverload.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private TpsRayInteractor interactor;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private GameObject promptPanel;

        private void Awake()
        {
            if (interactor == null)
            {
                interactor = FindFirstObjectByType<TpsRayInteractor>();
            }

            SetVisible(false);
        }

        private void Update()
        {
            string prompt = interactor != null ? interactor.CurrentPrompt : string.Empty;
            bool hasPrompt = !string.IsNullOrEmpty(prompt);

            SetVisible(hasPrompt);

            if (hasPrompt && promptText != null)
            {
                promptText.text = prompt;
            }
        }

        private void SetVisible(bool visible)
        {
            if (promptPanel != null)
            {
                promptPanel.SetActive(visible);
            }
            else if (promptText != null)
            {
                promptText.gameObject.SetActive(visible);
            }
        }
    }
}
