using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class StartSystem : MonoBehaviour
{ 
    public InputField idInput;
    public InputField ageInput;
    
    public Toggle[] sexToggles;
    public Toggle[] handToggles;
    public Toggle[] typeToggles;

    public Button startBtn;
    public Button exitBtn;

    public Text mistakeMsg;
    public int mistakeShowTime = 5;
    public DataManger dataManger;

    public Button settingButton;

    void Start()
    {
        startBtn.onClick.AddListener(OnStartClick);
        exitBtn.onClick.AddListener(OnExitClick);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            settingButton.gameObject.SetActive(!settingButton.gameObject.activeInHierarchy);
        }
    }

    // Start button click
    public void OnStartClick()
    {
        string idx = idInput.text;
        string age = ageInput.text;
        int sex = GetToggleValue(sexToggles);
        int hand = GetToggleValue(handToggles);
        int type = GetToggleValue(typeToggles);

        if (idx == "" || age == "" || !Regex.IsMatch(idx, @"^[+-]?\d*$") || 
            !Regex.IsMatch(age, @"^[+-]?\d*$"))
        {
            StartCoroutine(ShowMistake(mistakeShowTime));
        }
        else
        {
            Debug.Log("[StartSystem] ID:" + idx + " Age:" + age + " Sex:" + sex
                + " Hand:" + hand + " Type:" + type);
            if(sex == 0 || hand == 0 || type == 0)
            {
                Debug.LogError("[StartSystem] Toggle value error.");
            }
            PlayerPrefs.SetInt("curLevel", 0);
            PlayerPrefs.SetInt("TargetBaseIdx", int.Parse(idx)%3);
            dataManger.SetPersonInfos(idx, age, sex, hand, type);
            dataManger.SavePersonXml();
            SceneManager.LoadScene(5);  
        }
    }

    public void OnExitClick()
    {
        Debug.Log("[StartSystem] Exit.");
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    // Toggle Select
    public int GetToggleValue(Toggle[] tGroup)
    {
        for (int i = 0; i < tGroup.Length; ++i) 
        {
            if (tGroup[i].isOn)
            {
                return i + 1;
            }
        }
        return 0;
    }

    IEnumerator ShowMistake(int time)// 协程
    {
        mistakeMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        mistakeMsg.gameObject.SetActive(false);
    }

}
