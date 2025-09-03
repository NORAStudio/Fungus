using UnityEngine;

namespace CGTUnity.Fungus.SaveSystem
{
    public class SaveSlotHighlighter : MonoBehaviour
    {
        public static SaveSlotHighlighter Instance { get; private set; }

        private SaveSlot currentlySelectedSlot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetSelectedSlot(SaveSlot slot)
        {
            if (currentlySelectedSlot != null)
            {
                currentlySelectedSlot.SetSelected(false);
            }

            currentlySelectedSlot = slot;
            currentlySelectedSlot.SetSelected(true);
        }
    }
}