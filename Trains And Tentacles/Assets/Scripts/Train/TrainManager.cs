using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrainManager : MonoBehaviour {
	[Header("Runtime Info")]
	public List<CartController> sections;

	[Header("Config")]
    [Range(1,5)]
    public int health;

    public CartController cartPrefab;

	// Use this for initialization
	void Start () {
        for (int i=1; i<health; i++)
        {
			//Instantiate new carts
			CartController cart = Instantiate(cartPrefab);
            //cart.transform.SetParent(this.transform);
            cart.transform.position = transform.position + new Vector3(0, 0, -i * 1.5f);

			cart.prevCart = sections.Last();
			cart.Forward();
			sections.Add(cart);
        }

	}
}
