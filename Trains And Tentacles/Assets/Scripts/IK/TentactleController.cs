using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentactleController : MonoBehaviour {

    public bool onTrack;

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
            hitPoints[currentHit].GetComponent<BoxCollider>().enabled = true;

        } else
        {
            hitPoints[currentHit].GetComponent<Renderer>().material.color = Color.white;
            hitPoints[currentHit].GetComponent<BoxCollider>().enabled = false;
            arm.target = idlePoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onTrack)
        {
            Debug.Log("Tentacle Hit");
        }
    }

    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }

    private HitDirection ReturnDirection(GameObject Object, GameObject ObjectHit)
    {

        HitDirection hitDirection = HitDirection.None;
        RaycastHit MyRayHit;
        Vector3 direction = (Object.transform.position - ObjectHit.transform.position).normalized;
        Ray MyRay = new Ray(ObjectHit.transform.position, direction);
        Debug.Log(MyRay);
        if (Physics.Raycast(MyRay, out MyRayHit))
        {
            Debug.Log("raycast");
            if (MyRayHit.collider != null)
            {
                Debug.Log("not null");
                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up) { hitDirection = HitDirection.Top; }
                if (MyNormal == -MyRayHit.transform.up) { hitDirection = HitDirection.Bottom; }
                if (MyNormal == MyRayHit.transform.forward) { hitDirection = HitDirection.Forward; }
                if (MyNormal == -MyRayHit.transform.forward) { hitDirection = HitDirection.Back; }
                if (MyNormal == MyRayHit.transform.right) { hitDirection = HitDirection.Right; }
                if (MyNormal == -MyRayHit.transform.right) { hitDirection = HitDirection.Left; }
            }
        }
        return hitDirection;
    }
}
