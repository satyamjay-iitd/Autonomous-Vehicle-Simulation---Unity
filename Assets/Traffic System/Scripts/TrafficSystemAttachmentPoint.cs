using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TrafficSystemAttachmentPoint : MonoBehaviour 
{
	void Awake()
	{
		Activate(true);
	}

	public void Activate( bool a_enable )
	{
		if(a_enable)
		{
			if(!GetComponent<Rigidbody>())
			{
				Rigidbody rigidbodyObj  = gameObject.AddComponent<Rigidbody>();
				rigidbodyObj.useGravity = false;
			}

			if(!GetComponent<Collider>())
			{
				BoxCollider colliderObj  = gameObject.AddComponent<BoxCollider>() as BoxCollider;
				colliderObj.isTrigger    = true;
			}
		}
		else
		{
			if(GetComponent<Rigidbody>())
				Destroy(GetComponent<Rigidbody>());

			if(GetComponent<Collider>())
				Destroy(GetComponent<Collider>());
		}
	}

	void OnTriggerEnter( Collider a_obj )
	{
		//print ("a_obj: " + a_obj);
	}

	void OnTriggerExit( Collider a_obj )
	{
		//print ("a_obj: " + a_obj);
	}
}
