using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessSystem : MonoBehaviour
{ 
    public GameObject Middle;
    public Image pointPic;

    Vector3[] worldCorners = new Vector3[4];

    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;
    Vector3 OPos;

    Vector3 pointPos;
    float pointX, pointY;

    int m_screenWidth = 0;
    int m_screenHight = 0;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("IsMouseClicked", 0);

        SetOposColor(PlayerPrefs.GetInt("TargetBaseIdx"));
    }

    // Update is called once per frame
    void Update()
    {
        if (IsScreenSizeChanged())
        {
            SetCorner();
        }
        if (Input.GetMouseButtonDown(0))
        {
            pointPos = Input.mousePosition;
            if (CheckInRange(pointPos))
            {
                pointX = pointPos.x - OPos.x;
                pointY = pointPos.y - OPos.y;
                DrawPoint(pointPos);
                PlayerPrefs.SetInt("IsMouseClicked", 1);
                PlayerPrefs.SetFloat("PointX", pointX);
                PlayerPrefs.SetFloat("PointY", pointY);
            }
        }
        if (PlayerPrefs.GetInt("IsInputField")==1)
        {
            pointX = PlayerPrefs.GetFloat("PointX");
            pointY = PlayerPrefs.GetFloat("PointY");
            pointPos.x = pointX + OPos.x;
            pointPos.y = pointY + OPos.y;
            DrawPoint(pointPos);
            PlayerPrefs.SetInt("IsInputField", 0);
        }
    }

    public void DrawPoint(Vector3 pointPos)
    {
        pointPic.transform.position = pointPos;
        pointPic.gameObject.SetActive(true);
        Debug.Log("[ChessSystem] Clicked in: " +
            "(" + pointPos.x + "," + pointPos.y + "); " +
            "Draw Point in: (" + pointX + "," + pointY + ")");
    }

    private bool CheckInRange(Vector3 pointPos)
    {
        float x = pointPos.x;
        float y = pointPos.y;
        if (LTPos.x<=x && x<=RTPos.x && LTPos.y>=y && y >= LBPos.y)
        {
            return true;
        }
        return false;
    }

    private void SetCorner()
    {
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        LBPos = worldCorners[0];
        LTPos = worldCorners[1];
        RTPos = worldCorners[2];
        RBPos = worldCorners[3];
        OPos = new Vector3((LTPos.x + RTPos.x) / 2, (LTPos.y + LBPos.y) / 2, 0);
        Middle.transform.position = OPos;
        PlayerPrefs.SetFloat("blockScale", (RTPos.x - LTPos.x) / 20);

        Debug.Log("[ChessSystem] LTPos: x=" + LTPos.x + "; y=" + LTPos.y + "; z=" + LTPos.z);
        Debug.Log("[ChessSystem] RTPos: x=" + RTPos.x + "; y=" + RTPos.y + "; z=" + RTPos.z);
        Debug.Log("[ChessSystem] LBPos: x=" + LBPos.x + "; y=" + LBPos.y + "; z=" + LBPos.z);
        Debug.Log("[ChessSystem] RBPos: x=" + RBPos.x + "; y=" + RBPos.y + "; z=" + RBPos.z);
        Debug.Log("[ChessSystem] OPos: x=" + OPos.x + "; y=" + OPos.y + "; z=" + OPos.z);
        Debug.Log("[ChessSystem] Scale x: " + (RTPos.x - LTPos.x) / 20);
    }

    private void SetOposColor(int ballIdx)
    {
        Color color;
        switch (ballIdx)
        {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = Color.green;
                break;
            case 2:
                color = Color.blue;
                break;
            default:
                color = Color.grey;
                break;

        }
        Middle.GetComponentInChildren<Image>().color = color;
    }

    private bool IsScreenSizeChanged()
    {
        if(Screen.height!=m_screenHight || Screen.width != m_screenWidth)
        {
            Debug.Log("[ChessSystem] Screen Size: " + Screen.width + "*" + Screen.height);
            m_screenHight = Screen.height;
            m_screenWidth = Screen.width;
            return true;
        }
        return false;
    }
}
