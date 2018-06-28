using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentactleController : MonoBehaviour {

    public bool onTrack;

    public GameObject[] hitPoints;

    private float timer;

    public float minTime;
    public float maxTime;

    private int currentHit;

	// Use this for initialization
	void Start () {
        onTrack = false;
        InitialiseTimer();
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

        } else
        {
            hitPoints[currentHit].GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
