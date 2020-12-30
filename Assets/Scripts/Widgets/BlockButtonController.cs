using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockButtonController : MonoBehaviour
{
    public CorsiBoard corsiBoard;

    int m_buttonIdx;

    // Start is called before the first frame update
    void Start()
    {
        m_buttonIdx = int.Parse(GetComponentInChildren<Text>().text);
        // Debug.Log("[BlockButton]" + m_buttonIdx);
    }

    public void ShowIdxText()
    {
        GetComponentInChildren<Text>().enabled = true;
    }

    // Update is called once per frame
    public void OnBlockClick()
    {
        GetComponent<Button>().interactable = false;
        Debug.Log("[BlockButton] Clicked: " + m_buttonIdx);
        corsiBoard.SetProbeAnswer(m_buttonIdx-1);
    }
}
