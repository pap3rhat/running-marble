using System;

[Serializable]
public class SerializedSettings
{
    public int ScreenResolution;
    public int FullscreenMode;
    public float BackgroundmusicVolume;
    public float SFXVolume;

    public SerializedSettings() { }

    public SerializedSettings(int res, int mode, float bv, float sv)
    {
        ScreenResolution = res;
        FullscreenMode = mode;
        BackgroundmusicVolume = bv;
        SFXVolume = sv;
    }
}
