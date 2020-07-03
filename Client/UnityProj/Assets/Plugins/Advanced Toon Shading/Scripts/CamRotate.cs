using UnityEngine;
using System.Collections;

public class CamRotate : MonoBehaviour {
	private bool roate;
	private float RoatedSpeed = 1000.0F;
	void Start () {
		roate = false;
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButton(0))
		{
			float y = 0;

			y = Input.GetAxis("Mouse X")*RoatedSpeed*Time.deltaTime;
			if(roate)
			{
				gameObject.transform.Rotate(new Vector3(0,y,0));

			}

		}

	}
	void OnMouseDown()
	{

		roate =true;
		Debug.Log("collider");
	}
	void OnMouseUp()
	{
		roate = false;
		Debug.Log("Out of collider");
	}

}