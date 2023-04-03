using UnityEngine;

public struct Settings
{
    public enum TerrainQuality { Low, High }
    public enum ShadowQuality { Disabled, Low, High }

    // Graphics
    public Vector2Int screenSize;
    public bool isFullscreen;
    public bool vsyncEnabled;
    public TerrainQuality terrainQuality;
    public ShadowQuality shadowQuality;

    // Audio / Language
    public string languageID;
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    // Load settings from prefs
    public static Settings LoadSavedSettings()
    {
        Settings settings = new Settings();
        // Graphics
        settings.vsyncEnabled = PlayerPrefs.GetInt(nameof(vsyncEnabled), defaultValue: 1) == 1;
        // Note: since Unity remembers screen size / fullscreen mode automatically, just get current screen size
        settings.screenSize = new Vector2Int(Screen.width, Screen.height);
        settings.isFullscreen = Screen.fullScreen;
        settings.terrainQuality = (TerrainQuality)PlayerPrefs.GetInt(nameof(terrainQuality), defaultValue: (int)TerrainQuality.High);
        settings.shadowQuality = (ShadowQuality)PlayerPrefs.GetInt(nameof(shadowQuality), defaultValue: (int)ShadowQuality.High);

        // Audio / Language
        settings.languageID = PlayerPrefs.GetString(nameof(languageID));
        settings.masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), defaultValue: 0.75f);
        settings.musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), defaultValue: 0.75f);
        settings.sfxVolume = PlayerPrefs.GetFloat(nameof(sfxVolume), defaultValue: 0.75f);
        return settings;
    }

    public static void Save(Settings settings)
    {
        // --- Graphics
        // Note: Unity remembers screen size / fullscreen mode automatically, so don't need to save these
        PlayerPrefs.SetInt(nameof(vsyncEnabled), (settings.vsyncEnabled) ? 1 : 0);
        PlayerPrefs.SetInt(nameof(terrainQuality), (int)settings.terrainQuality);
        PlayerPrefs.SetInt(nameof(shadowQuality), (int)settings.shadowQuality);

        // Audio / Language
        PlayerPrefs.SetString(nameof(languageID), settings.languageID);
        PlayerPrefs.SetFloat(nameof(masterVolume), settings.masterVolume);
        PlayerPrefs.SetFloat(nameof(musicVolume), settings.musicVolume);
        PlayerPrefs.SetFloat(nameof(sfxVolume), settings.sfxVolume);

        // Write
        PlayerPrefs.Save();
    }


}