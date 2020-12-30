using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DivergeSystem : MonoBehaviour
{
    public GameSettings gameSettings;
    public DataManger dataManger;

    public Button[] gameButtons;
    public Button saveButton;
    public Text[] descriptionTexts;

    int m_level;
    
    void Start()
    {
        m_level = PlayerPrefs.GetInt("curLevel");
        Debug.Log("[DivergeSystem] Cur level: " + m_level);

        gameButtons[0].onClick.AddListener(OnClickOne);
        gameButtons[1].onClick.AddListener(OnClicktwo);
        SetDescription(m_level);
        SetButton(m_level);
    }

    void Update()
    {
        if (gameSettings.GetVersion())
        {
            BackDoor();
        }
    }

    void SetDescription(int level)
    {
        if(level <= 2)
        {
            descriptionTexts[0].gameObject.SetActive(true);
        }
        else if(level ==3)
        {
            descriptionTexts[1].gameObject.SetActive(true);
        }
        else
        {
            descriptionTexts[2].gameObject.SetActive(true);
        }
    }

    void SetButton(int level)
    {
        if (level >= 4)
        {
            gameButtons[0].gameObject.SetActive(false);
            gameButtons[1].gameObject.SetActive(false);
            saveButton.gameObject.SetActive(true);
        }
        else
        {
            saveButton.gameObject.SetActive(false);
        }
    }

    public void OnClickOne()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClicktwo()
    {
        SceneManager.LoadScene(6);
    }

    private void BackDoor()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("curLevel", 3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {

        }
    }
}
