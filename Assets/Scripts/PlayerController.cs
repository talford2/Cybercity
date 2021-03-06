﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region Public Variables

	public float Speed = 1.4f;

	public float RunSpeed = 2.0f;

	public Camera HeadCam;

	public float MouseSpeed = 30f;

	public float PlayerHeight = 1.8f;

	public float EyeLevel = 1.7f;

	public float PlayerRadius = 0.5f;

	#endregion

	#region Private Variables

	#endregion

	void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;

		HeadCam.transform.position = transform.position + Vector3.up * EyeLevel;
	}

	void Update()
	{
		var speed = Speed;

		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = RunSpeed;
		}

		// Movement
		transform.position += transform.forward * speed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime;
		transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * MouseSpeed * Time.deltaTime, Vector3.up);
		HeadCam.transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * MouseSpeed * Time.deltaTime, Vector3.left);


		if (Input.GetKeyUp(KeyCode.Space))
		{
			Debug.Break();
		}

	    if (Input.GetKeyUp(KeyCode.Escape))
	    {
	        Application.Quit();
	    }
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(transform.position + Vector3.up * PlayerHeight / 2, new Vector3(PlayerRadius * 2, PlayerHeight, PlayerRadius * 2));
		Gizmos.DrawLine(transform.position + Vector3.up * EyeLevel, transform.position + Vector3.up * EyeLevel + transform.forward * 2);
	}
}
