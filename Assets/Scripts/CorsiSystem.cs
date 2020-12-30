using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CorsiSystem : MonoBehaviour
{
    public CorsiBoard corsiBoard;
    public CorsiRecorder corsiRecorder;
    public GameSettings gameSettings;

    public GameObject guideBackground;
    public GameObject focusBackground;

    public CanvasGroup endImageCanvasGroup;

    public Button doneButton;
    public Button restartButton;

    public Text levelText;
    public GameObject[] ansFeedback;

    [HideInInspector] public int trialCount = 4;
    [HideInInspector] public int levelCount = 6;
    [HideInInspector] public float focusDuration = 2f;
    [HideInInspector] public float wrongMsgDuration = 1f;

    Button[] blockButtons;

    int m_level = 1;
    int m_trial = 1;
    int m_state = 0;

    bool m_isFoucs = false;
    bool m_isBulit = false;
    bool m_isPlay = true;
    bool m_isSet = false;
    bool m_isCorrect = false;
    bool m_isTrialCompleted = false;
    bool m_isDisappearEnd = true;

    float m_Timer = 0f;
    float m_totalScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelText.gameObject.SetActive(false);
        guideBackground.gameObject.SetActive(true);
        doneButton.onClick.AddListener(OnDoneClick);
        if (gameSettings.GetVersion())
        {
            restartButton.gameObject.SetActive(true);
        }
        restartButton.onClick.AddListener(OnRestartClick);

        trialCount = gameSettings.GetCorsiCount("trial count");
        levelCount = gameSettings.GetCorsiCount("level count");
        focusDuration = gameSettings.GetCorsiDuration("focus duration");
        wrongMsgDuration = gameSettings.GetCorsiDuration("wrong Msg duration");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameSettings.GetVersion())
        {
            SetLevelText();
        }

        if (!m_isDisappearEnd)
        {
            StartCoroutine(MsgDisappear(wrongMsgDuration));
        }

        // Debug.Log("[CorsiSystem] Cur State: " + m_state);
        if (m_state == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                guideBackground.gameObject.SetActive(false);
                m_state = 1;
            }
        }
        else if (m_state == 1 && !m_isFoucs && m_isDisappearEnd)
        {
            if (!corsiBoard.CheckIsShiningEnd())
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_isSet = false;
            }
            StartCoroutine(ShowfocusBackground(focusDuration));
            m_isFoucs = true;
        }
        else if (m_state == 2 && !m_isBulit)
        {
            BulidGame();    
            m_isBulit = true;
            m_state = 3;
        }
        else if (m_state == 3 && m_isPlay)
        {
            if (corsiBoard.CheckIsShiningEnd() && !m_isSet)
            {
                m_Timer = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_isSet = true;
            }
            m_Timer += Time.deltaTime;
        }
        else if (m_state == 4 && m_isDisappearEnd)
        {
            ShowCanvas(endImageCanvasGroup);
        }
    }

    private void BulidGame()
    {
        corsiBoard.BuildGameBoard();
        blockButtons = corsiBoard.transform.Find("Map").GetComponentsInChildren<Button>();
        Debug.Log("[CorsiSystem] Block count:" + blockButtons.Length);
        corsiBoard.ShowCorrectShining(m_level+1);
    }

    IEnumerator ShowfocusBackground(float time)
    {
        focusBackground.SetActive(true);
        yield return new WaitForSeconds(time);
        focusBackground.SetActive(false);
        m_state = 2;
    }

    public void OnDoneClick()
    {
        int probeCount = corsiBoard.GetProbeCount();
        int isCorrect = corsiBoard.GetCorrectState();
        Debug.Log("[CorsiSystem] " + m_level + "-" + m_trial + " Count: " + probeCount
            + "; Correct: " + isCorrect + "; Time Cost: " + m_Timer);
        corsiRecorder.SetRecord(m_level, m_trial, probeCount, isCorrect, m_Timer);
        CheckAnswerAndSetParms(isCorrect==1);
        m_isDisappearEnd = false;

        if (m_level > levelCount)
        {
            m_isPlay = false;
            m_state = 4;
        }
        else
        {
            ResetParms();
        }
    }

    public void OnRestartClick()
    {
        corsiBoard.DestroyBlocks();
        ResetParms();
    }

    // Widgets
    void CheckAnswerAndSetParms(bool isCorrect)
    {
        m_trial += 1;
        if (isCorrect)
        {
            m_isTrialCompleted = true;
            m_isCorrect = true;
            if(m_trial > trialCount)
            {
                m_level += 1;
                m_trial = 1;
                m_isTrialCompleted = false;
            }
        }
        else
        {
            m_isCorrect = false;
            if (m_trial > trialCount)
            {
                if (m_isTrialCompleted)
                {
                    m_level += 1;
                    m_trial = 1;
                    m_isTrialCompleted = false;
                }
                else
                {
                    m_level = levelCount + 1;
                }
            }
        }
    }

    void ResetParms()
    {
        m_isFoucs = false;
        m_isBulit = false;
        m_isPlay = true;
        m_state = 1;
    }

    void ShowCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        if (Input.anyKeyDown)
        {
            PlayerPrefs.SetInt("curLevel", 4);
            SceneManager.LoadScene(5);
        }
    }

    IEnumerator MsgDisappear(float time)
    {
        if (m_isCorrect)
        {
            SetFeedbackText(1);
        }
        else
        {
            SetFeedbackText(0);
        }       
        yield return new WaitForSeconds(time);
        corsiBoard.DestroyBlocks();
        SetFeedbackText(-1);
        m_isDisappearEnd = true;
    }

    void SetLevelText()
    {
        if (levelText.gameObject.activeInHierarchy)
        {
            levelText.text = string.Format("Lv:" + m_level + "-" + m_trial);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (levelText.gameObject.activeInHierarchy)
            {
                levelText.gameObject.SetActive(false);
            }
            else
            {
                levelText.gameObject.SetActive(true);   
            }
        }
    }

    void SetFeedbackText(int state)
    {
        ansFeedback[0].gameObject.SetActive(false);
        ansFeedback[1].gameObject.SetActive(false);
        if (state == 0 || state == 1)
        {
            ansFeedback[state].gameObject.SetActive(true);
        }
    }
}
