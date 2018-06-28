using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour {

    [Range(1,5)]
    public int health;

    public GameObject cartPrefab;

	// Use this for initialization
	void Start () {
        for (int i=1; i<health; i++)
        {
            //Instantiate new carts
            GameObject cart = Instantiate(cartPrefab);
            cart.transform.SetParent(this.transform);
            cart.transform.position = transform.position - new Vector3(0,0,i);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
