using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using SFB;

public class DialogWindow : MonoBehaviour
{
    public DataManger dataManger;

    private string xmlFilePath;

    void Start()
    {
        xmlFilePath = dataManger.GetFilePath();
    }

    public void OnClick()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save Testing Result.",
            "", "Result","xml");
        if (!string.IsNullOrEmpty(path))
        {
            File.Copy(xmlFilePath, path, true);
        }
        PlayerPrefs.DeleteAll();
        // Application.Quit();
    }
}
