using System;

[Serializable]
public class SerializedState
{
    public int remainingLifes;
    public float remainingTime;
    public int currentLevel;

    public SerializedState() { }

    public SerializedState(int lifes, float time, int level)
    {
        remainingLifes = lifes;
        remainingTime = time;
        currentLevel = level;
    }
}
