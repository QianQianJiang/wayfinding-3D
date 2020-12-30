using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PosDirectingSystem : MonoBehaviour
{
    public DataManger dataManger;
    public GameSettings gameSettings;

    public InputField xInputField;
    public InputField yInputField;
    public Button checkButton;
    public Button confirmButton;
    public Button settingButton;

    public CanvasGroup[] endBackgroundImageCanvasGroups;

    [HideInInspector] public float fadeDuartion = 1f;
    [HideInInspector] public float displayImageDuration = 2f;

    private int m_level;
    private int m_curScene;
    private float m_timer = 0f;

    float posX = 0f;
    float posY = 0f;
    float pointScale = 1f;

    bool m_isSelected = false;
    bool m_isEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
        m_level = PlayerPrefs.GetInt("curLevel");
        m_curScene = PlayerPrefs.GetInt("curScene");
        Debug.Log("[PosDirecting] " +
            "Cur Level: " + m_level + "; " +
            "Cur Scene: " + m_curScene + "; ");

        PlayerPrefs.SetInt("IsInputField", 0);

        checkButton.onClick.AddListener(OnCheckClick);
        confirmButton.onClick.AddListener(OnConfirmClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            settingButton.gameObject.SetActive(!settingButton.gameObject.activeInHierarchy);
        }
        if (PlayerPrefs.GetInt("IsMouseClicked") == 1)
        {
            m_isSelected = true;
            pointScale = PlayerPrefs.GetFloat("blockScale");
            posX = PlayerPrefs.GetFloat("PointX");
            posY = PlayerPrefs.GetFloat("PointY");
            xInputField.text = (posX / pointScale).ToString("0.0");
            yInputField.text = (posY / pointScale).ToString("0.0");
            PlayerPrefs.SetInt("IsMouseClicked", 0);
        }
        if (m_isEnd)
        {
            ShowConfirmCanvas(endBackgroundImageCanvasGroups[m_level-1]);
        }
    }

    public void OnCheckClick()
    {
        pointScale = PlayerPrefs.GetFloat("blockScale");
        posX = float.Parse(xInputField.text) * pointScale;
        posY = float.Parse(yInputField.text) * pointScale;

        m_isSelected = true;
        PlayerPrefs.SetInt("IsInputField", 1);
        PlayerPrefs.SetFloat("PointX", posX);
        PlayerPrefs.SetFloat("PointY", posY);
        Debug.Log("[PosDirecting] Checked in: (" + posX + "," + posY + ")");
    }

    public void OnConfirmClick()
    {
        pointScale = PlayerPrefs.GetFloat("blockScale");
        if (m_isSelected)
        {
            Debug.Log("[PosDirecting] Confirming..");
            int targetIdx = PlayerPrefs.GetInt("TargetBaseIdx");
            int audioIdx = GetCurAudioIdx(m_curScene);
            Debug.Log("[PosDirecting] Audio idx: " + audioIdx);
            List<float> realPos = gameSettings.GetTargetFromScene(m_curScene, targetIdx).map;
            dataManger.SetGameInfos(m_curScene, audioIdx, targetIdx,
                posX / pointScale, posY / pointScale, realPos[0], realPos[1]);
            dataManger.SaveGameInfosXml(m_curScene, m_level - 1);
            PlayerPrefs.SetInt("curLevel", m_level + 1);
            PlayerPrefs.SetInt("TargetBaseIdx", (targetIdx + 1)%3);
            m_isEnd = true;
        }   
    }

    private int GetCurAudioIdx(int sceneIdx)
    {
        int typeIdx = PlayerPrefs.GetInt("type");
        return gameSettings.GetTypeSettings(typeIdx, "audios")[sceneIdx];
    }

    private void ShowConfirmCanvas(CanvasGroup canvasGroup)
    {
        if(!canvasGroup.gameObject.activeInHierarchy)
        {
            canvasGroup.gameObject.SetActive(true);
        }
        
        m_timer += Time.deltaTime;
        canvasGroup.alpha = m_timer / fadeDuartion;
        
        if (m_timer > fadeDuartion + displayImageDuration)
        { 
            SceneManager.LoadScene(1);
        }
    }
}

