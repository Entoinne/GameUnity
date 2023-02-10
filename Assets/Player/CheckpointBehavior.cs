using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    	void OnCollisionEnter(Collision collision)
    	{
        	//Output the Collider's GameObject's name
        	Debug.Log("I got touched by : " + collision.collider.name);
        	Object.Destroy(this.gameObject);
    	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
