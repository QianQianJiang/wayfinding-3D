using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    public GameSystem gameSystem;

    public int ballID;     // R/G/B = 0/1/2
    bool m_IsPlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
            Debug.Log("[Observer] Get ball in: " + this.transform.name);
            gameSystem.GetBall(ballID);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        m_IsPlayerInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInRange)
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
