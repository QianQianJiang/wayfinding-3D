using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;

[Serializable]
public class Person
{
    public string id; 
    public string age;
    public int sex;
    public int hand;
    public int type;
}

[Serializable]
public class GameInfos
{
    public int no;
    public int audio;
    public string target;
    public float posX;
    public float posY;
    public float realX;
    public float realY;
}

[Serializable]
public class GameCost
{
    public int no;
    public List<float> exploringCost;
    public string path;
    public float findingCost;
}

public class DataManger : MonoBehaviour
{
    
    [HideInInspector] public string m_filePath;
    private Person m_player;
    private GameInfos[] m_gameInfos;
    private GameCost[] m_gameCost;

    public int mapCount = 2;

    bool m_isChanged = false;
    string[] BallColor = new string[3] { "Red", "Green", "Blue" };

    void Awake()
    {
        m_filePath = Application.streamingAssetsPath + "/dataXMl.xml";
        Debug.Log("[DataManger] File Path: " + m_filePath);

        m_gameInfos = new GameInfos[mapCount];
        m_gameCost = new GameCost[mapCount];
    }

    public void SetPersonInfos(string id, string age,
        int sex, int hand, int type)
    {
        m_isChanged = true;
        m_player = new Person();
        m_player.id = id;
        m_player.age = age;
        m_player.sex = sex;
        m_player.hand = hand;
        m_player.type = type;
        
        PlayerPrefs.SetInt("type", m_player.type);
        PlayerPrefs.SetInt("curLevel", 0);
        Debug.Log("[DataManger] ID:" + m_player.id + " Age:" + m_player.age + " Sex:" + m_player.sex
            + " Hand:" + m_player.hand + " Type:" + m_player.type);
    }

    public void SetGamePathAndCost(int sceneNum, List<float> exploringCost, string path, float cost)
    {
        m_isChanged = true;
        int curIdx = sceneNum - 1;
        Debug.Log("[DataManger] Cur idx:" + curIdx);
        m_gameCost[curIdx] = new GameCost();
        m_gameCost[curIdx].no = sceneNum;
        m_gameCost[curIdx].exploringCost = new List<float>(exploringCost);
        m_gameCost[curIdx].path = path;
        m_gameCost[curIdx].findingCost = cost;
        Debug.Log("[DataManger] Set Wayfinding cost - " + m_gameCost[curIdx].no
            + " Path Writed");
    }

    public void SetGameInfos(int sceneNum, int audio, int target,
        float x, float y, float rx, float ry)
    {
        m_isChanged = true;
        int curIdx = sceneNum - 1;
        m_gameInfos[curIdx] = new GameInfos();
        m_gameInfos[curIdx].no = sceneNum;
        m_gameInfos[curIdx].audio = audio;
        m_gameInfos[curIdx].target = BallColor[target];
        m_gameInfos[curIdx].posX = x;
        m_gameInfos[curIdx].posY = y;
        m_gameInfos[curIdx].realX = rx;
        m_gameInfos[curIdx].realY = ry;
        Debug.Log("[DataManger] Seting GameInfos for scene: " + m_gameInfos[curIdx].no +
            " PosX: " + m_gameInfos[curIdx].posX + "; PosY: " + m_gameInfos[curIdx].posY);
        // SaveGameInfosXml(curIdx);
    }


    public void SaveGamePathAndCost(int sceneNum, int level)
    {
        int gameIdx = sceneNum - 1;

        if (!File.Exists(m_filePath))
        {
            Debug.LogError("[DataManger] Game Infos: XMl File not found.");
            return;
        }
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(m_filePath);

        XmlNode root = xmlDocument.SelectSingleNode("Object");

        XmlElement game = xmlDocument.CreateElement("Wayfinding");
        game.SetAttribute("Game_ID", level.ToString());

        XmlElement gameno = xmlDocument.CreateElement("Map");
        gameno.InnerText = m_gameCost[gameIdx].no.ToString();
        game.AppendChild(gameno);

        XmlElement exploringTimes = xmlDocument.CreateElement("Exploring_Times");
        exploringTimes.SetAttribute("Count", m_gameCost[gameIdx].exploringCost.Count.ToString());
        for (int i = 0; i < m_gameCost[gameIdx].exploringCost.Count; ++i)
        {
            XmlElement cur = xmlDocument.CreateElement("Times_"+(i + 1).ToString());
            cur.InnerText = m_gameCost[gameIdx].exploringCost[i].ToString();
            exploringTimes.AppendChild(cur);
        }
        game.AppendChild(exploringTimes);

        XmlElement gamePath = xmlDocument.CreateElement("Path");
        gamePath.InnerText = m_gameCost[gameIdx].path;
        game.AppendChild(gamePath);

        XmlElement gameCost = xmlDocument.CreateElement("Wayfinding_Time");
        gameCost.InnerText = m_gameCost[gameIdx].findingCost.ToString();
        game.AppendChild(gameCost);

        root.AppendChild(game);

        xmlDocument.AppendChild(root);
        xmlDocument.Save(m_filePath);

        Debug.Log("[DataManger] Saving Game " + gameIdx
            + " Cost and Path to xmlFile:" + m_filePath);
    }

    public void SaveGameInfosXml(int sceneNum, int level)
    {
        int gameIdx = sceneNum - 1;

        if (!File.Exists(m_filePath))
        {
            Debug.LogError("[DataManger] Game Infos: XMl File not found.");
            return;
        }
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(m_filePath);

        // #region AddXMl elements

        XmlNode root = xmlDocument.SelectSingleNode("Object");

        XmlNode game = xmlDocument.SelectNodes(@"Object/Wayfinding")[level];
        // game.SetAttribute("Game_ID", m_gameInfos[gameIdx].no.ToString());

        XmlElement gameAudio = xmlDocument.CreateElement("Audio");
        gameAudio.InnerText = m_gameInfos[gameIdx].audio.ToString();
        game.AppendChild(gameAudio);

        XmlElement gameTarget = xmlDocument.CreateElement("Target");
        gameTarget.InnerText = m_gameInfos[gameIdx].target;
        game.AppendChild(gameTarget);

        XmlElement gameX = xmlDocument.CreateElement("Pos_X");
        gameX.InnerText = m_gameInfos[gameIdx].posX.ToString();
        game.AppendChild(gameX);

        XmlElement gameY = xmlDocument.CreateElement("Pos_Y");
        gameY.InnerText = m_gameInfos[gameIdx].posY.ToString();
        game.AppendChild(gameY);

        XmlElement realposX = xmlDocument.CreateElement("Real_X");
        realposX.InnerText = m_gameInfos[gameIdx].realX.ToString();
        game.AppendChild(realposX);

        XmlElement realposY = xmlDocument.CreateElement("Real_Y");
        realposY.InnerText = m_gameInfos[gameIdx].realY.ToString();
        game.AppendChild(realposY);

        root.AppendChild(game);

        // #endregion

        xmlDocument.AppendChild(root);
        xmlDocument.Save(m_filePath);

        Debug.Log("[DataManger] Saving Game " + gameIdx
            + " to xmlFile:" + m_filePath);
    }

    public void SavePersonXml()
    { 
        XmlDocument xmlDocument = new XmlDocument();

        #region CreateXMl elements

        XmlElement root = xmlDocument.CreateElement("Object");

        XmlElement player = xmlDocument.CreateElement("Player");
        player.SetAttribute("Player_ID", m_player.id);

        XmlElement playerId = xmlDocument.CreateElement("ID");
        playerId.InnerText = m_player.id;
        player.AppendChild(playerId);

        XmlElement playerAge = xmlDocument.CreateElement("Age");
        playerAge.InnerText = m_player.age;
        player.AppendChild(playerAge);

        XmlElement playerSex = xmlDocument.CreateElement("Sex");
        playerSex.InnerText = m_player.sex.ToString();
        player.AppendChild(playerSex);

        XmlElement playerHand = xmlDocument.CreateElement("Hand");
        playerHand.InnerText = m_player.hand.ToString();
        player.AppendChild(playerHand);

        XmlElement playerType = xmlDocument.CreateElement("Type");
        playerType.InnerText = m_player.type.ToString();
        player.AppendChild(playerType);

        root.AppendChild(player);

        #endregion

        xmlDocument.AppendChild(root);
        xmlDocument.Save(m_filePath);
        
        Debug.Log("[DataManger] Saving Person "+ m_player.id
            +" to xmlFile:" + m_filePath);
    } 

    public void LoadPersonXml()
    {
        if (!File.Exists(m_filePath))
        {
            // File.Create(m_filePath);
            Debug.Log("[DataManger] File NOT FOUND.");
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(m_filePath);

        XmlNodeList idNode = xmlDocument.GetElementsByTagName("ID");
        string id = idNode[0].InnerText;

        XmlNodeList ageNode = xmlDocument.GetElementsByTagName("Age");
        string age = ageNode[0].InnerText;

        XmlNodeList sexNode = xmlDocument.GetElementsByTagName("Sex");
        int sex = int.Parse(sexNode[0].InnerText);

        XmlNodeList handNode = xmlDocument.GetElementsByTagName("Hand");
        int hand = int.Parse(handNode[0].InnerText);

        XmlNodeList typeNode = xmlDocument.GetElementsByTagName("Type");
        int type = int.Parse(typeNode[0].InnerText);

        SetPersonInfos(id, age, sex, hand, type);
        
        Debug.Log("[DataManger] Loading Xml File from:" + m_filePath);
    }

    public virtual string GetFilePath()
    {
        return m_filePath;
    }
}
