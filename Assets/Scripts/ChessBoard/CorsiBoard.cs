using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CorsiBoard : MonoBehaviour
{
    //public GameObject LeftTop;
    //public GameObject RightBottom;
    public GameSettings gameSettings;

    public GameObject blockObj;
    public Transform parentBoard;

    [HideInInspector] public int blockCount = 9;
    [HideInInspector] public float blockDisplayDuration = 1f;
    [HideInInspector] public float blockSwitchDuration = 0.5f;

    Vector3[] worldCorners = new Vector3[4];
    float m_blockScale;
    float m_bias = 20f;

    int m_probeCount = 0;
    int m_isCorrect = 1;
    bool m_isShined = false;

    List<GameObject> m_blocks = new List<GameObject>();
    List<int> m_correctIdxAnswer = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        m_blockScale = blockObj.GetComponent<RectTransform>().rect.width;
        Debug.Log("[CorsiBoard] Block Scale: " + m_blockScale);

        blockCount = gameSettings.GetCorsiCount("block count");
        Debug.Log("[CorsiBoard] Block Count from settings: " + blockCount);
        blockDisplayDuration = gameSettings.GetCorsiDuration("display duration");
        blockSwitchDuration = gameSettings.GetCorsiDuration("switch duration");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            PrintPoint();
        }
        if (gameSettings.GetVersion() && Input.GetKeyDown(KeyCode.P))
        {
            for(int i=0; i<blockCount; ++i)
            {
                m_blocks[i].GetComponent<BlockButtonController>().ShowIdxText();
            }
        }
    }

    public void BuildGameBoard()
    {
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        GenerateRandomBlock(blockCount);
        Debug.Log("[CorsiBoard] Blocks count: " + m_blocks.Count);
    }

    public void ShowCorrectShining(int count)
    {
        m_correctIdxAnswer = GetRandomAnswer(count);
        StartCoroutine(ShiningBlock(m_correctIdxAnswer));
    }

    public bool CheckIsShiningEnd()
    {
        return m_isShined;
    }

    public void DestroyBlocks()
    {
        for(int i=0; i<m_blocks.Count; ++i)
        {
            Destroy(m_blocks[i]);
        }
        m_blocks.Clear();
        m_correctIdxAnswer.Clear();
        m_probeCount = 0;
        m_isCorrect = 1;
        m_isShined = false;
    }

    public void SetProbeAnswer(int clickIdx)
    {
        if (m_probeCount < m_correctIdxAnswer.Count &&
            m_correctIdxAnswer[m_probeCount] == clickIdx)
        {
            Debug.Log("[CorsiBoard] Click Correct");
        }
        else
        {
            m_isCorrect = 0;
            Debug.Log("[CorsiBoard] Click Wrong");
        }
        m_probeCount += 1;
    }

    public int GetProbeCount()
    {
        return m_probeCount;
    }

    public int GetCorrectState()
    {
        if (m_probeCount != m_correctIdxAnswer.Count)
        {
            m_isCorrect = 0;
        }
        return m_isCorrect;
    }

    public bool CheckIsCorrect()
    {
        return m_isCorrect == 1;
    }

    IEnumerator ShiningBlock(List<int> idxList)
    {
        yield return new WaitForSeconds(blockDisplayDuration);
        foreach (int idx in idxList)
        {
            Debug.Log("[CorsiBoard] Correct answer: " +
                idx + "-" + idxList.Count);
            m_blocks[idx].GetComponent<Button>().interactable = false;
            yield return new WaitForSeconds(blockDisplayDuration);
            m_blocks[idx].GetComponent<Button>().interactable = true;
            yield return new WaitForSeconds(blockSwitchDuration);
        }
        m_isShined = true;
    }

    void GenerateRandomBlock(int count)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Vector3 m_LBPos = worldCorners[0];
        Vector3 m_RTPos = worldCorners[2];
        Vector3 tmp = new Vector3();
        for (int i = 1; i <= count; ++i)
        {
            do
            {
                tmp.Set(Random.Range(m_LBPos.x + m_blockScale + m_bias, m_RTPos.x - m_blockScale - m_bias),
                    Random.Range(m_LBPos.y + m_blockScale + m_bias, m_RTPos.y - m_blockScale - m_bias), 0);
                Debug.Log("[Block " + i + "] (" + tmp.x + "," + tmp.y + ")");
            } while (Physics.OverlapSphere(tmp, m_blockScale + m_bias, 1 << LayerMask.NameToLayer("Target")).Length > 0);

            GameObject newBlock = Instantiate(blockObj, new Vector3(tmp.x, tmp.y,0),
                Quaternion.identity, parentBoard) as GameObject;
            newBlock.GetComponentInChildren<Text>().text = i.ToString();
            newBlock.GetComponentInChildren<Text>().enabled = false;
            m_blocks.Add(newBlock);
        }
    }

    List<int> GetRandomAnswer(int count)
    {
        List<int> indexs = new List<int>();
        for (int i=0; i<blockCount; ++i)
        {
            indexs.Add(i);
        }

        List<int> res = new List<int>();
        for (int i = 0; i < count; ++i)
        {
            
            int index = Random.Range(0, indexs.Count-1);
            res.Add(indexs[index]);
            indexs.Remove(indexs[index]);
        }
        return res;
    }

    void PrintPoint()
    {
        for(int i=0; i < 4; ++i)
        {
            Vector3 tmp = worldCorners[i];
            Debug.Log(string.Format("[Generator Corner] {0}: ({1},{2},{3})",i,tmp.x,tmp.y,tmp.z));
        }
        Debug.Log("[CorsiBoard] Block Scale: " + blockObj.GetComponent<RectTransform>().rect.width);
    }
}
