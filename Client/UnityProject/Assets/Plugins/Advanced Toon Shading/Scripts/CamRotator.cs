using UnityEngine;
using System.Collections;

public class CamRotator : MonoBehaviour
{
	
	public bool isActive = false;
	public float speed = 0.33f;
	
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		if ( isActive )
			transform.Rotate( Vector3.up, speed * Time.deltaTime );
	}
}
