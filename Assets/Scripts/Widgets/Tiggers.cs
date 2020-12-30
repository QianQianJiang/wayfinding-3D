using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiggers : MonoBehaviour
{
    public Transform player;
    public GameSystem gameSystem;

    private bool m_IsPlayerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        bool isGetAllBalls = gameSystem.CheckIsGetAllBalls();
        if (other.transform == player)
        {
            Debug.Log("[Tiggers] Player Touch start: " + isGetAllBalls.ToString());
            if (isGetAllBalls)
            {
                m_IsPlayerInRange = true;
                Debug.Log("[Tiggers] Back to start.");
                gameSystem.SetBackToStart(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInRange)
        {
            gameSystem.SetBackToStart(true);
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
