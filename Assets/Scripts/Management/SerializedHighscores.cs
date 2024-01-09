using System;
using System.Collections.Generic;

[Serializable]
public class SerializedHighscores
{
    public List<Highscore> Highscores = new();

    //public SerializedHighscores(List<Highscore> scores)
    //{
    //    Highscores = scores;
    //}

}

[Serializable]
public struct Highscore
{
    public int Score;
    public string Name;

    public Highscore(int score, string name)
    {
        Score = score;
        Name = name;
    }
}