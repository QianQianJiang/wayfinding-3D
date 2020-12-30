using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    private int m_level;

    public GameObject player;
    public CanvasGroup endBackgroundImageCanvasGroup;
    public CanvasGroup getballImageCanvasGroup;

    public Text[] coverTexts;
    
    public float gameDuration = 300f;
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;

    float m_Timer = 0f;
    bool m_IsGetBall = false;  //MARKER R\G\B balls
    int m_missionCount = 1;
    bool m_IsBackToStart = false;

    // Start is called before the first frame update
    void Start()
    {
        m_level = PlayerPrefs.GetInt("curLevel");
        endBackgroundImageCanvasGroup.transform.GetChild(m_level)
            .gameObject.SetActive(true);

        m_IsGetBall = false;
        Debug.Log("[GameSystem] Cur Level: " + m_level);

        SetCoverText(0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGettingBall();
        Exploring();
    }

    // Processing
    void CheckGettingBall()
    {
        if (m_IsGetBall)
        {
            EndLevel(getballImageCanvasGroup, false);
        }
    }

    void Exploring()
    {
        if (m_missionCount == 0 && m_IsBackToStart)
        {
            Debug.Log("[GameSystem] Game Completed.");
            EndLevel(endBackgroundImageCanvasGroup, true);

        }
    }

    void EndLevel(CanvasGroup canvasGroup, bool isEnd)
    {
        m_Timer += Time.deltaTime;
        canvasGroup.alpha = m_Timer / fadeDuration;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (isEnd)
            {
                PlayerPrefs.SetInt("curLevel", m_level + 1);
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
            else
            {
                m_IsGetBall = false;
                SetCoverText(1);
                m_Timer = 0f;
                canvasGroup.alpha = 0;
            }
        }
    }

    // Game and map Setting
    void SetCoverText(int curIdx)
    {
        for(int i=0; i < coverTexts.Length; ++i)
        {
            if (i == curIdx)
            {
                coverTexts[i].gameObject.SetActive(true);
            }
            else
            {
                coverTexts[i].gameObject.SetActive(false);
            }
        }
    }

    // Public API
    public virtual void GetBall(int ball_idx)
    {
        m_IsGetBall = true;
        m_missionCount -= 1;
    }

    public virtual bool CheckIsGetAllBalls()
    {
        return m_missionCount == 0;
    }

    public virtual void SetBackToStart(bool key)
    {
        m_IsBackToStart = key;
    }

}
