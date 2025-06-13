using System.IO;
using UnityEngine;

public class SettingsData
{
    #region Service
    private readonly static string filepath = Application.persistentDataPath + "//settings.json";

    private static SettingsData _instance;
    public static SettingsData Instance
    {
        get
        {
            if (_instance == null)
                Load();
            return _instance;
        }
        private set { _instance = value; }
    }
    static void Load()
    {
        if (!File.Exists(filepath))
        {
            _instance = new SettingsData();
            _instance.resolution = new Resolution {width = _instance.width, height = _instance.height};
            Save();
            return;
        }
        _instance = JsonUtility.FromJson<SettingsData>(File.ReadAllText(filepath));
        _instance.resolution = new Resolution {width = _instance.width, height = _instance.height};
    }
    public static void Save()
    {
        _instance.width = _instance.resolution.width;
        _instance.height = _instance.resolution.height;
        File.WriteAllText(filepath, JsonUtility.ToJson(_instance));
    }
    #endregion

    public float masterVolume = 0.5f, voiceVolume = 0.5f;
    public float autoForwardDelay = 1f, writingSpeed = 60f;
    public Resolution resolution;
    [SerializeField] private int width = 1366;
    [SerializeField] private int height = 768;
    public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;

    public bool autoForward = false, skip = false;

    public float WritingSpeed
    {
        get
        {
            if (skip)
                return 1000f;
            return writingSpeed;
        }
    }
}
