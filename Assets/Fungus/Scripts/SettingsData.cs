using System.IO;
using UnityEngine;

public class SettingsData
{
    #region Service
    private static readonly string filepath = Path.Combine(Application.persistentDataPath, "settings.json");

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
        try
        {
            if (File.Exists(filepath))
            {
                _instance = JsonUtility.FromJson<SettingsData>(File.ReadAllText(filepath));
            }
            else
            {
                _instance = new SettingsData();
            }
        }
        catch
        {
            Debug.LogWarning("Failed to load settings, resetting to defaults.");
            _instance = new SettingsData();
        }
    }

    public static void Save()
    {
        File.WriteAllText(filepath, JsonUtility.ToJson(_instance));
    }
    #endregion

    public float musicVolume = 1f;
    public float soundVolume = 1f;
    public float autoForwardDelay = 1f;
    public float writingSpeed = 60f;

    [SerializeField] private int width = 1366;
    [SerializeField] private int height = 768;
    private Resolution _resolution;
    public Resolution resolution
    {
        get
        {
            if (_resolution.width == 0 || _resolution.height == 0)
            {
                _resolution.width = width;
                _resolution.height = height;
            }
            return _resolution;
        }
        set
        {
            _resolution = value;
            width = value.width;
            height = value.height;
        }
    }

    public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    public bool autoForward = false;
    public bool skip = false;

    public float WritingSpeed => skip ? 1000f : writingSpeed;
}
