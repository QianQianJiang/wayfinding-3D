using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuideSystem : MonoBehaviour
{
    public Image[] checkedImage;
    public Image[] finishImages;

    public Button startBtn;
    public Button backBtn;

    public Text descriptionText;

    private int m_type;
    private int m_level;

    public DataManger dataManger;
    public GameSettings gameSettings;

    private List<int> m_sceneSettings;
    private List<int> m_audioSettings;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startBtn.onClick.AddListener(OnStartClick);
        backBtn.onClick.AddListener(OnBackClick);
    }

    void Start()
    {
        m_type = PlayerPrefs.GetInt("type");
        Debug.Log("[GuideSystem] Get type:" + m_type);
        m_sceneSettings = gameSettings.GetTypeSettings(m_type, "scenes");
        m_audioSettings = gameSettings.GetTypeSettings(m_type, "audios");

        m_level = PlayerPrefs.GetInt("curLevel");
        Debug.Log("[GuideSystem] CurLevel:" + m_level);
        SetCheckedImageAndButton();
        SetFinishImage();
        SetDescription();
    }

    void Update()
    {
        if (gameSettings.GetVersion())
        {
            BackDoor();
        }
    }


    public void OnStartClick()
    {
        PlayerPrefs.SetInt("curScene", m_sceneSettings[m_level]);
        PlayerPrefs.SetInt("isWayFinding", 0);
        Debug.Log("[GuideSystem] Cur map: " + m_sceneSettings[m_level] +
            "; Cur audio: " + m_audioSettings[m_level]);
        if (m_level == 0)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            gameSettings.SetExploringTimes();
            SceneManager.LoadScene(4);
        }

    }

    public void OnBackClick()
    {
        if (m_level == 0)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            // PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(5);
        }
    }

    private void SetFinishImage()
    {
        // Debug.Log("[GuideSystem] finish image: " + finishImages.Length);
        for (int i = 0; i < m_level; ++i)
        {
            // Debug.Log("Name:" + finishImages[i].gameObject.name);
            finishImages[i].gameObject.SetActive(true);
        }
    }

    private void SetCheckedImageAndButton()
    {
        // Debug.Log("[GuideSystem] Checked image: " + checkedImage.Length);
        if (m_level < 3)
        {
            checkedImage[m_level].gameObject.SetActive(true);
        }
        else
        {
            startBtn.enabled = false;
            //backBtn.gameObject.SetActive(false);
        }
    }

    private void SetDescription()
    {
        if (m_level == 0)
        {
            descriptionText.text = "\t\t在本实验中，您将会经历3个阶段。在练习阶段，您需要找到地图中" +
                "的一个小球并回到起点。找到小球后，您需要靠近小球，直至屏幕上呈现\"你找到了小球\"。" +
                "随后，您需要返回起点。在起点有\"Start\"字样漂浮在空中。" +
                "您需要靠近\"Start\"，直到屏幕上出现提示，随后会回到主界面。\n" +
                "\t\t您可以通过键盘上W\\A\\S\\D四个键控制前\\左\\后\\右四个方向移动，" +
                "移动鼠标可以转换视角，点击\"Start\"开始练习。";
        }
        else if (m_level == 1 || m_level == 2)
        {
            if (m_audioSettings[m_level] == 0)
            {
                descriptionText.text = "\t\t在这一阶段，您需要完成自由探索、寻路和指向三个任务。首先是自由" +
                    "探索阶段，您可以自由探索当前地图。为了帮助您更好地探索，" +
                    "您需要在地图中找到三个不同颜色的小球并返回起点。" +
                    "小球颜色为红、绿、蓝，寻找顺序不限。找到小球后，" +
                    "您需要靠近小球直到屏幕上呈现提示。找到三个小球后，" +
                    "您需要返回起点并靠近\"Start\"字样直到屏幕上呈现提示。\n"+
                    "在实验过程中，会有音频不断播放数字。" +
                    "【您不需要关注这个声音信息。】\n" +
                    "请注意，自由探索过程会重复三次，每次上限5分钟，您可以尽可能多地记住地图相关的信息。" +
                    "【请点击\"Start\"开始自由探索】";
            }
            else
            {
                descriptionText.text = "\t\t在这一阶段，您需要完成自由探索、寻路和指向三个任务。首先是自由" +
                    "探索阶段，您可以自由探索当前地图。为了帮助您更好地探索，" +
                    "您需要在地图中找到三个不同颜色的小球并返回起点。" +
                    "小球颜色为红、绿、蓝，寻找顺序不限。找到小球后，" +
                    "您需要靠近小球直到屏幕上呈现提示。找到三个小球后，" +
                    "您需要返回起点并靠近\"Start\"字样直到屏幕上呈现提示。\n" +
                    "在实验过程中，会有音频不断播放数字。" +
                    "【您需要在“滴——”声出现后，口述“滴——”声之前出现的数字的奇偶性】\n" +
                    "请注意，自由探索过程会重复三次，每次上限5分钟，您可以尽可能多地记住地图相关的信息。" +
                    "【请点击\"Start\"开始自由探索】";
            }

        }
        else
        {
            descriptionText.text = "\n\nTask 1已完成，请点击下方Back按钮继续进行Task 2。";
        }
    }


    private void BackDoor()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("curLevel", m_level + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerPrefs.SetInt("curLevel", m_level - 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
