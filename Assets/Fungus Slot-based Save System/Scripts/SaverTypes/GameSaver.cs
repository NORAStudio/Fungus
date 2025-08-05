using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using System.IO;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// This encodes game objects into save data for a whole playthrough, so their state 
    /// can be restored upon loading the game.
    /// Handles Flowcharts and NarrativeLogs by default.
    /// To extend this to handle other data types, just modify or subclass this component.
    /// </summary>
    public class GameSaver: DataSaver<GameSaveData>, ISaveCreator<GameSaveData>
    {
        protected List<DataSaver> subsavers = new List<DataSaver>();
        [SerializeField] Camera mainCamera;

        #region Methods
        protected virtual void Awake()
        {
           subsavers.AddRange(GetComponents<DataSaver>());
           subsavers.RemoveAll(saver => saver == this); // This can't be its own subsaver!
        }

        public override IList<SaveDataItem> CreateItems()
        {
            // Create GameSaveData, and encode it into a SaveDataItem
            var gameSave = CreateSave();
            var jsonSave = JsonUtility.ToJson(gameSave, true);
            var newItem = new SaveDataItem(saveType.Name, jsonSave);

            // The array has only one element, since we only made one GameSaveData
            return new SaveDataItem[1] {newItem};
        }

        /// <summary>
        /// Creates and returns save data for the whole game.
        /// </summary>
        public virtual GameSaveData CreateSave()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var newGameSave = new GameSaveData(sceneName, -1);
            EncodeInto(ref newGameSave);
            newGameSave.UpdateTime();
            if (ProgressMarker.latestExecuted != null)
                newGameSave.ProgressMarkerKey = ProgressMarker.latestExecuted.Key;

            // It's common to make VN save file descs be the text that was in the textbox, 
            // at the time of the save being made.
            var description = "";
            var sayDialog = SayDialog.ActiveSayDialog;

            if (sayDialog != null && !string.IsNullOrEmpty(sayDialog.StoryText))
                description = sayDialog.StoryText;
           
            newGameSave.Description = description;
            
            var narrativeLog = FungusManager.Instance.NarrativeLog;
            if (narrativeLog != null)
                newGameSave.narrativeLogJson = narrativeLog.GetJsonHistory();
            
            return newGameSave;
        }

        /// <summary>
        /// Creates and returns save data with the passed slot number.
        /// </summary>
        public virtual GameSaveData CreateSave(int slotNumber)
        {
            var newGameSave = CreateSave();
            newGameSave.SlotNumber = slotNumber;

            RenderTexture rt = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);
            mainCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

            mainCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            screenShot.Apply();

            mainCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Path.Combine(Application.persistentDataPath, "screen_" + slotNumber + ".png");
            File.WriteAllBytes(filename, bytes);
            
            var narrativeLog = FungusManager.Instance.NarrativeLog;
            if (narrativeLog != null)
                newGameSave.narrativeLogJson = narrativeLog.GetJsonHistory();

            newGameSave.ScreenshotPath = filename;
            return newGameSave;
        }

        /// <summary>
        /// Creates and returns save data for the whole game, set for the passed save slot.
        /// </summary>
        public virtual GameSaveData CreateSave(SaveSlot saveSlot)
        {
            return CreateSave(saveSlot.Number);
        }

        #region Helpers

        protected virtual void EncodeInto(ref GameSaveData saveData)
        {
            for (int i = 0; i < subsavers.Count; i++)
            {
                var subsaver = subsavers[i];
                var saveDataItems = subsaver.CreateItems();
                saveData.Items.AddRange(saveDataItems);
            }
        }

        #endregion

        #endregion


    }
}