using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap; 
using Leap.Unity;

public class CollisionDetection : MonoBehaviour {



	/*LeapProvider provider;

	void start()
	{
		provider = FindObjectOfType<LeapProvider> () as LeapProvider;
	}

	void update()
	{
		Frame frame = provider.CurrentFrame; 
		Debug.Log ("frame ID: "+frame.Id+",hands in frame: "+frame.Hands.Count);
	}*/

	void OnCollisionEnter(Collision collision) 
	{
		//Debug.Log ("Gameobject name: "+collision.gameObject.name);
		//Debug.Log ("Colider name: "+collision.collider.name);
		//ContactPoint contact; 
		foreach (ContactPoint contact in collision.contacts) 
		{
			//Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name+" at position :"+contact.point*100+"This collider position: "+contact.thisCollider.transform.position*100);
			//Debug.Log ("This collider position: "+contact.thisCollider.transform.position);
			transform.parent.GetComponent<FilterData>().CollisionDetected(this, contact.thisCollider, contact.otherCollider, contact.point);

		}
		//transform.parent.GetComponent<FilterData>().CollisionDetected(this);
	}
}
