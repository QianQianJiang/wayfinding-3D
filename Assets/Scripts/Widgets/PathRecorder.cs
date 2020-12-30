using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRecorder : MonoBehaviour
{
    public float recordDuration = 2f;
    public DataManger dataManger;
    public GameSettings gameSettings;

    Transform m_player;

    static List<float> m_exploringCost = new List<float>();
    string m_Path = "";
    float m_RecordTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponent<Transform>();
        recordDuration = gameSettings.GetRecordDuration();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_RecordTimer += Time.deltaTime;
        if (m_RecordTimer > recordDuration)
        {
            m_Path = string.Format("{0};({1:F2},{2:F2})",
                m_Path, m_player.position.x, m_player.position.z);
            m_RecordTimer = 0f;
        }
    }

    void OnDestroy()
    {
        m_exploringCost = null;
    }

    public void SetStart()
    {
        m_Path = string.Format("({0:F2},{1:F2})",
                m_player.position.x, m_player.position.z);
    }

    public string GetPath()
    {
        return m_Path;
    }

    public void SaveExploringCost(int sceneNum, float cost, int idx)
    {
        CheckExploringCostCount();
        m_exploringCost.Add(cost);
        Debug.Log("[PathRecorder] Saving exploring " + idx + " cost: " + cost);
        Debug.Log("[Global] Cur Count: " + m_exploringCost.Count);
    }

    public void SaveWayfindingPathAndCost(int sceneNum, float costTime)
    { 
        dataManger.SetGamePathAndCost(sceneNum, m_exploringCost, m_Path, costTime);
        dataManger.SaveGamePathAndCost(sceneNum, PlayerPrefs.GetInt("curLevel"));
        Debug.Log("[PathRecorder] Save path: " + m_Path);
        Debug.Log("[PathRecorder] Save Wayfinding cost: " + costTime);   
    }

    private void CheckExploringCostCount()
    {
        if (m_exploringCost.Count >= gameSettings.GetExploringCount())
        {
            m_exploringCost.Clear();
            Debug.Log("[PathRecorder] Empty exploring cost list.");
        }
    }
}
