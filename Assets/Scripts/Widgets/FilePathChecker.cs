using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilePathChecker : MonoBehaviour
{
    public DataManger dataManger;
    public InputField filePathInputField;
    public GameSettings gameSettings;

    string m_filePath;

    // Start is called before the first frame update
    void Start()
    {
        m_filePath = dataManger.GetFilePath();
        this.gameObject.SetActive(gameSettings.GetVersion());
    }

    // Update is called once per frame
    public void OnSettingButtonClick()
    {
        if (filePathInputField.gameObject.activeInHierarchy)
        {
            filePathInputField.gameObject.SetActive(false);
        }
        else
        {
            filePathInputField.gameObject.SetActive(true);
            filePathInputField.text = dataManger.GetFilePath() + ";";
        }
    }
}
