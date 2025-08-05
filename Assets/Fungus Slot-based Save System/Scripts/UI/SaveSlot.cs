using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.IO;
using CGT.Globalization;

namespace CGTUnity.Fungus.SaveSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class SaveSlot : MonoBehaviour
    {
        #region Fields
        [SerializeField] protected Text numDisplay = null;
        [SerializeField] protected Text descDisplay = null;
        [SerializeField] protected Text dateDisplay = null;
        [SerializeField] protected StandardFormat dateFormat = StandardFormat.fullLongDate;
        [SerializeField] protected bool refreshContinuously = true;
        [SerializeField] protected RawImage Screenshot;
        [SerializeField] protected int saveSlotNumber;

        protected GameSaveData saveData = null;
        protected Texture2D cachedTexture;

        #endregion

        #region Properties and helpers
        public RectTransform rectTransform { get; private set; }

        public virtual GameSaveData SaveData
        {
            get { return saveData; }
            set
            {
                if (value == null)
                {
                    WarnForNullSaveDataAssignment();
                    return;
                }

                saveData = value;
                SyncWithSaveData();
                UpdateDisplays();
                Signals.SaveSlotUpdated.Invoke(this, value);
            }
        }

        void WarnForNullSaveDataAssignment()
        {
            Debug.LogWarning("Cannot assign null GameSaveData to a Save Slot. Use Clear() instead.");
        }

        protected virtual void SyncWithSaveData()
        {
            //Number = saveData.SlotNumber;
            Description = saveData.Description;
            Date = saveData.LastWritten;
            ScreenPath = saveData.ScreenshotPath;
        }
        
        public virtual int Number
        {
            get => saveSlotNumber;
            set => saveSlotNumber = value;
        }
        public virtual string Description { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string ScreenPath { get; set; }

        public virtual void UpdateDisplays()
        {
            UpdateNumberDisplay();
            UpdateDateDisplay();
            UpdateDescriptionDisplay();
            LoadScreenshot();
        }

        protected virtual void UpdateNumberDisplay()
        {
            numDisplay.text = "Save #" + Number;
        }

        protected virtual void UpdateDateDisplay()
        {
            if (Date == default(DateTime))
                dateDisplay.text = "";
            else
                dateDisplay.text = Date.ToString(StandardFormatValues.vals[dateFormat], userLocale);
        }

        protected readonly CultureInfo userLocale = CultureInfo.CurrentUICulture;

        protected virtual void UpdateDescriptionDisplay()
        {
            descDisplay.text = Regex.Replace(Description, "[^а-яА-Я ]", "");
        }

        private void LoadScreenshot()
        {
            if (Screenshot == null) return; // Prevent errors if not assigned

            if (!File.Exists(ScreenPath))
            {
                Screenshot.enabled = false; // Hide if no screenshot is found
                return;
            }

            byte[] imageBytes = File.ReadAllBytes(ScreenPath);
            if (cachedTexture == null)
                cachedTexture = new Texture2D(100, 100, TextureFormat.RGBA32, false);

            if (cachedTexture.LoadImage(imageBytes))
            {
                Screenshot.texture = cachedTexture;
                Screenshot.enabled = true;
            }
            else
            {
                Debug.LogWarning("Failed to load screenshot from " + ScreenPath);
            }
        }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Description = "";
            clickReceiver = GetComponent<Button>();
            clickReceiver.onClick.AddListener(OnClick);
            UpdateDisplays();
        }

        protected Button clickReceiver = null;

        protected virtual void OnClick()
        {
            Signals.SaveSlotClicked.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            clickReceiver.onClick.RemoveListener(OnClick);
        }

        public virtual void Clear()
        {
            saveData = null;
            Description = "";
            Date = default;
            ScreenPath = "";
            Screenshot.texture = null;
            Screenshot.enabled = false;
            UpdateDisplays();
            Signals.SaveSlotUpdated.Invoke(this, SaveData);
        }
        #endregion
    }
}
