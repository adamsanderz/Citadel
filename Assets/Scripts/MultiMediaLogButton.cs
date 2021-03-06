using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogButton : MonoBehaviour {
	public int logButtonIndex;
	public GameObject textReader;
	public GameObject multiMediaTab;
	public GameObject dataTabAudioLogContainer;
	public int logReferenceIndex;

	void LogButtonClick() {
		textReader.SetActive(true);
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		dataTabAudioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(logReferenceIndex);
		textReader.GetComponent<LogTextReaderManager>().SendTextToReader(logReferenceIndex);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogButtonClick(); });
	}
}
