using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Level3 : MonoBehaviour
{
    [SerializeField] private Transform ball;
    public Transform start;
    public Transform end;
    // Start is called before the first frame update
    bool start_level = false;

    public void StartLevel()
    {
        start_level = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;
    }

    void Start()
    {
        ball.position = start.position;
        ball.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!start_level) return;
        if (Vector3.Distance(ball.position, end.position) < 1)
        {
            ball.position = end.position;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Passed Level 3!");
            start_level = false;
            SceneManager.LoadScene("Level4");
        }
    }
}
