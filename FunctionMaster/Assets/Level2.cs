using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2 : MonoBehaviour
{
    [SerializeField] private Transform ball;
    public Transform start;
    public Transform target;
    public Transform end;
    // Start is called before the first frame update
    bool p1 = false, p2 = false;

    void Start()
    {
        ball.position = start.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(ball.position, target.position) < 0.5)
        {
            p1 = true;
        }
        if (Vector3.Distance(ball.position, end.position) < 0.5)
        {
            p2 = true;
        }
        if (p1 && p2)
        {
            ball.position = end.position;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Passed Level 2!");
        }
    }
}
