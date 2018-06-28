using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentactleController : MonoBehaviour {

    private bool onTrack;

	public Transform idlePoint;
    public GameObject[] hitPoints;

	public RobotArm arm;

    private float timer;

    public float minTime;
    public float maxTime;

    private int currentHit;

	// Use this for initialization
	void Start () {
        onTrack = false;
        InitialiseTimer();

		arm = GetComponent<RobotArm>();
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;

        if (timer<=0)
        {
            onTrack = !onTrack;
            MoveTentacle(onTrack);

            InitialiseTimer();
        }
	}

    void InitialiseTimer()
    {
        timer = Random.Range(minTime, maxTime);
    }

    void MoveTentacle(bool ontrack)
    {
        if (ontrack)
        {
            currentHit = Random.Range(0, hitPoints.Length);
            hitPoints[currentHit].GetComponent<Renderer>().material.color = Color.red;
			arm.target = hitPoints[currentHit].transform;

        } else
        {
            hitPoints[currentHit].GetComponent<Renderer>().material.color = Color.white;
			arm.target = idlePoint;
        }
    }
}
