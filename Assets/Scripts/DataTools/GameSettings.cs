using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using System.IO;
using LitJson;

[Serializable]
public class Target
{
    public int id { get; set; }
    public string color { get; set; }
    public Vector3 real { get; set; }
    public List<float> map { get; set; }
}

[Serializable]
public class Scene
{
    public int id { get; set; }
    public Vector3 start { get; set; }
    public List<Target> targets { get; set; }
}

[Serializable]
public class Type
{ 
    public int typeid { get; set; }
    public List<int> scenes { get; set; }
    public List<int> audios { get; set; }
}


public class GameSettings : MonoBehaviour
{
    private StreamReader settingsReader;
    private JsonData settings;

    private JsonData sceneSettings;
    private JsonData typeSettings;
    private JsonData basicSettings;
    private JsonData corsiSettings;

    // Load gameSettings.json file
    void Awake()
    {
        settingsReader = new StreamReader(Application.streamingAssetsPath + "/Settings.json");
        settings = JsonMapper.ToObject(new JsonReader(settingsReader));
        Debug.Log("[Settings] Setting text: " + settings.Count);

        sceneSettings = settings["scenes"];
        typeSettings = settings["type"];
        basicSettings = settings["basic"];
        corsiSettings = settings["corsi"];
    }

    // GET "scenes" and "audios" in "type"
    public List<int> GetTypeSettings(int typeIdx, string key)
    { 
        Debug.Log("[TypeSettings] Cur Type id: "
            + typeSettings[typeIdx - 1]["typeid"] + "; Input id: " + typeIdx);
        List<int> res = new List<int>();
        foreach (JsonData s in typeSettings[typeIdx - 1][key])
        {
            res.Add((int)s);
        }
        return res;
    }

    public void SetExploringTimes()
    {
        PlayerPrefs.SetInt("ExploringTime", (int)basicSettings["exploring count"]);
        Debug.Log("[BasicSettings] Set exploring time.");
    }

    public int GetExploringCount()
    {
        return (int)basicSettings["exploring count"];
    }

    public float GetRecordDuration()
    {
        return float.Parse(((double)basicSettings["record duration"]).ToString());
    }

    public float GetGameDuration()
    {
        return float.Parse(((double)basicSettings["game duration"]).ToString());
    }

    public object GetSceneSettings(int sceneIdx, string key)
    {
        Debug.Log("[SceneSettings] Cur Scene id: " +
            sceneSettings[sceneIdx]["id"] + "; Input id: " + sceneIdx);
        return sceneSettings[sceneIdx][key];
    }

    public Vector3 GetSceneStart(int sceneIdx)
    {
        List<float> start = new List<float>();
        foreach (JsonData pos in sceneSettings[sceneIdx]["start"])
        {
            start.Add(float.Parse(((double)pos).ToString()));
        }
        Debug.Log("[SceneSettings] Start pos: " + start[0] + ", " + start[1] + ", " + start[2]);
        return new Vector3(start[0], start[1], start[2]);
    }

    public Target GetTargetFromScene(int sceneIdx, int targetIdx)
    {
        JsonData curTarget = sceneSettings[sceneIdx]["targets"][targetIdx];
        Debug.Log("[TargetSettings] Cur Target color: " + curTarget["color"]);
        Target tmpTarget = new Target();
        tmpTarget.id = (int)curTarget["id"];
        tmpTarget.color = (string)curTarget["color"];
        tmpTarget.real = new Vector3(
            float.Parse(((double)curTarget["real"][0]).ToString()),
            float.Parse(((double)curTarget["real"][1]).ToString()),
            float.Parse(((double)curTarget["real"][2]).ToString()));
        tmpTarget.map = new List<float> {
            float.Parse(((double)curTarget["map"][0]).ToString()),
            float.Parse(((double)curTarget["map"][1]).ToString())};
        return tmpTarget;
    }

    public int GetCorsiCount(string key)
    {
        return (int)corsiSettings[key];
    }

    public float GetCorsiDuration(string key)
    {
        return float.Parse(((double)corsiSettings[key]).ToString());
    }

    public bool GetVersion()
    {
        return (int)settings["version"] == 0;
    }
}
