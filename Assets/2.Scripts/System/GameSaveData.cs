using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int playTime;
    public string sceneName;
    public Vector2 playerPos;
    public List<Vector2> foundMaps = new List<Vector2>();
    public List<string> deadBosses = new List<string>();
    public List<string> seenTutorials = new List<string>();

    public GameSaveData(int playTime,
                        string sceneName,
                        Vector2 playerPos,
                        List<Vector2> foundMaps,
                        List<string> deadBosses,
                        List<string> seenTutorials)
    {
        this.playTime = playTime;
        this.sceneName = sceneName;
        this.playerPos = playerPos;
        this.foundMaps = foundMaps;
        this.deadBosses = deadBosses;
        this.seenTutorials = seenTutorials;
    }
}
