using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormalGameSystem : GameSystem
{
    private int m_level;
    private int m_mapScene;
    private int m_target;
    private int m_audio;
    private int m_isWayFinding;

    // public GameObject player;
    public GameSettings gameSettings;
    // public CanvasGroup endBackgroundImageCanvasGroup;
    // public CanvasGroup getballImageCanvasGroup;
    public CanvasGroup exploringEndImageCanvasGroup;
    public CanvasGroup exploringSwitchImageCanvasGroup;
    
    public GameObject[] maps;
    public GameObject[] backgroundAudios;
    public GameObject[] coverCanvases;
    public Text gainText;

    // public float gameDuration = 300f;
    // public float fadeDuration = 1f;
    // public float displayImageDuration = 1f;
    PathRecorder m_pathRecorder;
    AudioSource m_audioSource;

    float m_gameTimer = 0f;
    float m_Timer = 0f;

    bool m_IsGetBall = false;  //MARKER R\G\B balls
    bool m_IsBackToStart = false;
    bool m_IsSwitchCover = false;
    bool m_IsSavePath = false;

    int m_missionCount;
    int m_exploringTime;
    int m_exploringCount;

    // Start is called before the first frame update
    void Start()
    {
        m_level = PlayerPrefs.GetInt("curLevel");
        Debug.Log("[FormalGameSystem] Cur level: " + m_level);

        m_mapScene = PlayerPrefs.GetInt("curScene");
        Debug.Log("[FormalGameSystem] map Scene: " + m_mapScene);
        SetMap(m_mapScene - 1);
        SetPlayerToStart(m_mapScene);

        m_audio = gameSettings.GetTypeSettings(PlayerPrefs.GetInt("type"), "audios")[m_level];
        backgroundAudios[m_audio].gameObject.SetActive(true);

        m_target = PlayerPrefs.GetInt("TargetBaseIdx");
        Debug.Log("[FormalGameSystem] Cur Level: " + m_level);

        m_isWayFinding = PlayerPrefs.GetInt("isWayFinding");
        SetCoverCanvasSettings((m_isWayFinding == 1));

        m_pathRecorder = player.GetComponent<PathRecorder>();
        Debug.Log("[FormalGameSystem] Init path recorder: "
            + m_pathRecorder.GetPath());

        m_exploringTime = PlayerPrefs.GetInt("ExploringTime");
        m_exploringCount = gameSettings.GetExploringCount();
        m_audioSource = backgroundAudios[m_audio].transform.GetChild(m_exploringCount - m_exploringTime).
            GetComponent<AudioSource>();
        SetAudio();
        
        Debug.Log("[FormalGameSystem] Cur Audio: " + m_audioSource.transform.name);

        gameDuration = gameSettings.GetGameDuration();
        Debug.Log("[FormalGameSystem] Game Duration: " + gameDuration);
    }

    // Update is called once per frame
    void Update()
    {
        m_gameTimer += Time.deltaTime;
        CheckGettingBall();
        if (m_isWayFinding == 1)
        {
            WayFinding();
        }
        else
        {
            if (m_missionCount == 0 && !m_IsSwitchCover)
            {
                coverCanvases[0].SetActive(false);
                coverCanvases[4].SetActive(true);
                m_IsSwitchCover = true;
            }
            Exploring();
        }
        if (gameSettings.GetVersion())
        {
            BackDoor();
        }
    }   

    // Processing
    void CheckGettingBall()
    {
        if (m_IsGetBall)
        {
            EndLevel(getballImageCanvasGroup, 1);
        }
    }

    void Exploring()
    {
        if ((m_missionCount == 0 && m_IsBackToStart) || m_gameTimer > gameDuration)
        {
            if (!m_IsSavePath)
            {
                m_pathRecorder.SaveExploringCost(m_mapScene, m_gameTimer, m_exploringCount - m_exploringTime);
                m_IsSavePath = true;
            }
            if (m_exploringTime <= 1)
            {
                Debug.Log("[FormalGameSystem] Exploring Completed.");
                EndLevel(exploringEndImageCanvasGroup, 2);
            }
            else
            {
                Debug.Log("[FormalGameSystem] Exploring Switch. Last:" + (m_exploringTime - 1));
                EndLevel(exploringSwitchImageCanvasGroup, 3);
            }
        }
    }

    void WayFinding()
    {
        if (m_missionCount == 0 && !m_IsGetBall)
        {
            if (!m_IsSavePath)
            {
                m_pathRecorder.SaveWayfindingPathAndCost(m_mapScene, m_gameTimer);
                m_IsSavePath = true;
            }
            Debug.Log("[FormalGameSystem] Way Finding Completed.");
            EndLevel(endBackgroundImageCanvasGroup, 0);
        }
    }


    void EndLevel(CanvasGroup canvasGroup, int canvasIdx)
    {
        m_Timer += Time.deltaTime;
        canvasGroup.alpha = m_Timer / fadeDuration;

        if (m_audioSource.isPlaying && canvasIdx > 1)
        {
            m_audioSource.Stop();
        }

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            // way finding end
            if (canvasIdx == 0)   
            {
                if (Input.anyKeyDown)
                {
                    PlayerPrefs.SetInt("isWayFinding", 0);
                    SceneManager.LoadScene(2, LoadSceneMode.Single);
                }
            }
            // get ball end
            else if(canvasIdx == 1)
            {
                m_IsGetBall = false;
                canvasGroup.alpha = 0;
                m_Timer = 0f;
            }
            // exploring end && exploring switch
            else 
            {
                if (Input.anyKeyDown)
                {
                    // exploring end
                    if (canvasIdx == 2)
                    {
                        PlayerPrefs.SetInt("isWayFinding", 1);
                    }
                    // exploring switch
                    else if (canvasIdx == 3)
                    {
                        PlayerPrefs.SetInt("ExploringTime", m_exploringTime - 1);
                    }
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }

    void BackDoor()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_missionCount = 0;
            gainText.text = "游戏就应该是这么玩的，朋友！";
        }
    }

    // Game and map Setting
    void SetAudio()
    {
        if (m_isWayFinding == 0 && !m_audioSource.isPlaying)
        {
            m_audioSource.Play();
        }
    }

    void SetMap(int mapIdx)
    {
        for (int i = 0; i < maps.Length; ++i)
        {
            if (i == mapIdx)
            {
                maps[i].gameObject.SetActive(true);
                Debug.Log("[FormalGameSystem] Set Map ID: " + mapIdx);
            }
            else
            {
                maps[i].gameObject.SetActive(false);
            }
        }
        
    }

    void SetPlayerToStart(int sceneIdx)
    {
        Vector3 start = gameSettings.GetSceneStart(sceneIdx);
        Debug.Log("[FormalGameSystem] Set player.");
        // Edit->project Settings->Physics->Auto sysnc transform(turn on);
        // To resolve the conflict between "transform" and "controller"
        player.transform.position = start;
        player.GetComponent<PathRecorder>().SetStart();
    }

    void SetCoverCanvasSettings(bool isWayFinding)
    {
        if (isWayFinding)
        {
            coverCanvases[0].SetActive(false);
            coverCanvases[m_target + 1].SetActive(true);
            m_missionCount = 1;
        }
        else
        {
            coverCanvases[0].SetActive(true);
            coverCanvases[m_target + 1].SetActive(false);
            m_missionCount = 3;
        }
    }

    void SetGainText(int ball_idx)
    {
        Debug.Log("[FormalGameSystem] Set gain text: " + ball_idx);
        string[] ballColor = new string[3] { "红球", "绿球", "蓝球" };
        gainText.text += string.Format(" {0}", ballColor[ball_idx]);
    }

    // Public API
    public override void GetBall(int ball_idx)
    {
        // exploring or target==idx
        if (m_isWayFinding == 0 || (m_isWayFinding == 1 && ball_idx == m_target))
        {
            m_IsGetBall = true;
            SetGainText(ball_idx);
            m_missionCount -= 1;
        }
    }

    public override bool CheckIsGetAllBalls()
    {
        return m_missionCount == 0;
    }

    public override void SetBackToStart(bool key)
    {
        m_IsBackToStart = key;
    }
}
