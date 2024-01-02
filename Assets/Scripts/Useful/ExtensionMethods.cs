public static class ExtensionMethods
{

    public static float Remap(this float t, float a, float b, float c, float d)
    {
        return (t - a) * ((d - c) / (b - a)) + c;
    }

}