using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

[SerializeField]
public class GameRecord
{
    public int level;
    public int trial;
    public int probeCount;
    public int isCorrect;
    public float costTime;
}

public class CorsiRecorder : DataManger
{
    // [HideInInspector] public new string m_filePath;

    // Start is called before the first frame update
    void Awake()
    {
        m_filePath = Application.streamingAssetsPath + "/dataXMl.xml";
        Debug.Log("[CorsiRecorder] File Path: " + m_filePath);
    }

    public void SetRecord(int level, int trial,
        int probeCount, int isCorrect, float costTime)
    {
        GameRecord tmp = new GameRecord();
        tmp.level = level;
        tmp.trial = trial;
        tmp.probeCount = probeCount;
        tmp.isCorrect = isCorrect;
        tmp.costTime = costTime;
        SaveRecordXml(tmp);
    }

    public void SaveRecordXml(GameRecord gameRecord)
    {
        if (!File.Exists(m_filePath))
        {
            Debug.LogError("[CorsiRecorder] Corsi Record: XMl File not found.");
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(m_filePath);

        XmlNode root = xmlDocument.SelectSingleNode("Object");

        XmlNode corsi;
        if (xmlDocument.SelectSingleNode(@"Object/Corsi") == null)
        {
            corsi = xmlDocument.CreateElement("Corsi");
        }
        else
        {
            corsi = xmlDocument.SelectSingleNode(@"Object/Corsi");
        }

        XmlElement record = xmlDocument.CreateElement("Record");
        record.SetAttribute("Record_ID",
            gameRecord.level.ToString()+"-"+gameRecord.trial.ToString());

        XmlElement levelXml = xmlDocument.CreateElement("Real_Count");
        levelXml.InnerText = (gameRecord.level+1).ToString();
        record.AppendChild(levelXml);

        XmlElement trialXml = xmlDocument.CreateElement("Trial");
        trialXml.InnerText = gameRecord.trial.ToString();
        record.AppendChild(trialXml);

        XmlElement countXml = xmlDocument.CreateElement("Probe_Count");
        countXml.InnerText = gameRecord.probeCount.ToString();
        record.AppendChild(countXml);

        XmlElement correctXml = xmlDocument.CreateElement("Is_Correct");
        correctXml.InnerText = gameRecord.isCorrect.ToString();
        record.AppendChild(correctXml);

        XmlElement costXml = xmlDocument.CreateElement("Cost_Time");
        costXml.InnerText = gameRecord.costTime.ToString();
        record.AppendChild(costXml);

        corsi.AppendChild(record);
        root.AppendChild(corsi);

        xmlDocument.AppendChild(root);
        xmlDocument.Save(m_filePath);

        Debug.Log("[CorsiRecorder] Saving Game: "
            + gameRecord.level + "-" + gameRecord.trial);
            
    }

    public void SaveTotalScore(float score)
    {
        if (!File.Exists(m_filePath))
        {
            Debug.LogError("[CorsiRecorder] Corsi Record: XMl File not found.");
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(m_filePath);

        XmlNode root = xmlDocument.SelectSingleNode("Object");

        XmlNode corsi = xmlDocument.SelectSingleNode(@"Object/Corsi");

        XmlElement scoreXml = xmlDocument.CreateElement("Total_Score");
        scoreXml.InnerText = score.ToString();

        corsi.AppendChild(scoreXml);
        root.AppendChild(corsi);

        xmlDocument.AppendChild(root);
        xmlDocument.Save(m_filePath);

        Debug.Log("[CorsiRecorder] Saving Score: " + score);
    }
}
