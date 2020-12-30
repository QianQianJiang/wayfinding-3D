using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public Transform m_transform; // 角色transform
    public float m_speed = 3.0f; // 移动速度
    public float m_gravity = 1.0f; // 重力
    Vector3 m_Movement;

    CharacterController m_characterController; // 控制controller

    public Transform m_camTransform; // 摄像机transform
    Vector3 m_camRotation; // 摄像机角度
    float m_camHeight = 1.4f; // 摄像机高度 

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        // m_audio = GetComponent<AudioSource>();
        m_characterController = GetComponent<CharacterController>();

        //获取主摄像机的transform组件
        m_camTransform = Camera.main.transform;
        m_camTransform.position = m_transform.TransformPoint(0, m_camHeight, 0);
        m_camTransform.rotation = m_transform.rotation;
        m_camRotation = m_transform.eulerAngles;
        //锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * m_speed;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * m_speed;

        m_Movement.Set(horizontal , 0f, vertical);
        m_characterController.Move(m_transform.TransformDirection(m_Movement));

        float camHorizontal = Input.GetAxis("Mouse X");
        float camVertical = Input.GetAxis("Mouse Y");

        m_camRotation.x -= camVertical;
        m_camRotation.y += camHorizontal;
        m_camTransform.eulerAngles = m_camRotation;

        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0;
        camrot.z = 0;
        m_transform.eulerAngles = camrot; //仅仅只需要让主角的面朝向相机的方向就行了，不用旋转别的方向

        //保持主角摄像机在上方
        m_camTransform.position = transform.TransformPoint(0, m_camHeight, 0);
    }
}
