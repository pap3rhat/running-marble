using System;

[Serializable]
public class SerializedSettings
{
    public int ScreenResolution;
    public int FullscreenMode;
    public float UIScale;
    public float BackgroundmusicVolume;
    public float SFXVolume;

    public SerializedSettings() { }

    public SerializedSettings(int res, int mode, float us, float bv, float sv)
    {
        ScreenResolution = res;
        FullscreenMode = mode;
        UIScale = us;
        BackgroundmusicVolume = bv;
        SFXVolume = sv;
    }
}
