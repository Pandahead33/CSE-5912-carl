﻿using UnityEngine;
using UnityEngine.Networking;

public class SpawnController : NetworkBehaviour {

	[SerializeField]
	Behaviour[] disabled;

	Camera LobbyCam; 

	public bool localPlayer = false; 

	void Start()
	{
		if (!isLocalPlayer) {
			for (int i = 0; i < disabled.Length; i++) {
				disabled [i].enabled = false; 
			}
		} else {
			LobbyCam = Camera.main;
			if(LobbyCam != null)
				LobbyCam.gameObject.SetActive (false);
			localPlayer = true; 
		}
	}
		
	void OnDisable()
	{
		if (LobbyCam != null)
			LobbyCam.gameObject.SetActive (true);
	}

}
