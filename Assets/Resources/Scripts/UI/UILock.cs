using UnityEngine;
using System.Collections;

public class UILock : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y);
	}
}
