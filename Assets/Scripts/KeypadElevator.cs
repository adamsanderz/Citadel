﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadElevator : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public DataTab dataTabResetter;
	public GameObject elevatorControl;
	public GameObject[] elevatorButtonHandlers;
	public GameObject[] targetDestination;
	public bool[] buttonsEnabled;
	public bool[] buttonsDarkened;
	public string[] buttonText;
	public int currentFloor;
	public AudioClip SFX;
	private float disconnectDist;
	public float useDist = 2f;
	private AudioSource SFXSource;
	private bool padInUse;
	private GameObject levelManager;
	private GameObject playerCapsule;
	private GameObject playerCamera;

	void Awake () {
		levelManager = GameObject.Find("LevelManager");
		if (levelManager == null)
			Const.sprint("Warning: Could not find object 'LevelManager' in scene",playerCapsule.transform.parent.gameObject);

		if (targetDestination == null)
			Const.sprint("Warning: Could not find target destination for elevator keypad" + gameObject.name,playerCapsule.transform.parent.gameObject);
	}

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
		for (int i=0;i<8;i++) {
			elevatorButtonHandlers[i].GetComponent<ElevatorButton>().SetTooFarFalse();
		}
	}

	void Use (GameObject owner) {
		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			Const.sprint("Blocked by SHODAN level Security.",owner);
			MFDManager.a.BlockedBySecurity();
			return;
		}

		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		dataTabResetter.Reset();
		elevatorControl.SetActive(true);
		playerCapsule = owner.GetComponent<PlayerReferenceManager>().playerCapsule; // Get player capsule of player using this pad
		playerCamera = owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
		for (int i=0;i<8;i++) {
			elevatorControl.GetComponent<ElevatorKeypad>().buttonsEnabled[i] = buttonsEnabled[i];
			elevatorControl.GetComponent<ElevatorKeypad>().buttonsDarkened[i] = buttonsDarkened[i];
			elevatorControl.GetComponent<ElevatorKeypad>().buttonText[i] = buttonText[i];
			elevatorControl.GetComponent<ElevatorKeypad>().targetDestination[i] = targetDestination[i];
		}
		elevatorControl.GetComponent<ElevatorKeypad>().currentFloor = currentFloor;
		elevatorControl.GetComponent<ElevatorKeypad>().activeKeypad = gameObject;
		if (playerCamera.GetComponent<MouseLookScript>().inventoryMode == false)
			playerCamera.GetComponent<MouseLookScript>().ToggleInventoryMode();

		MFDManager.a.OpenTab(4,true,MFDManager.TabMSG.Elevator,0);
	}

	void Update () {
		if (padInUse) {
			if (Vector3.Distance(playerCapsule.transform.position, gameObject.transform.position) > disconnectDist) {
				padInUse = false;
				MFDManager.a.TurnOffElevatorPad();
				//elevatorControl.SetActive(false);
			} else {
				if (Vector3.Distance(playerCapsule.transform.position, gameObject.transform.position) < useDist) {
					// Elevator keypad functions normally
					for(int i=0;i<8;i++) {
						if (MFDManager.a.GetElevatorControlActiveState() /*elevatorControl.activeInHierarchy*/)
							
							elevatorButtonHandlers[i].GetComponent<ElevatorButton>().SetTooFarFalse();
					}
				} else {
					// Elevator keypad tells you that you are too far away
					for(int i=0;i<8;i++) {
						if (MFDManager.a.GetElevatorControlActiveState()/*elevatorControl.activeInHierarchy*/)
							elevatorButtonHandlers[i].GetComponent<ElevatorButton>().SetTooFarTrue();
					}
				}
			}
		}
	}
}
