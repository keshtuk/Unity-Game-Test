using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class GameManager : MonoBehaviour
{
	class Save{
		public int Level;
	}
	
	Save save;
	
	// AudioClip audio_found_1;
	AudioSource audioSource;
	public int gameLevel = 1;
	public float startMinute = 2;
	public float startsecond = 0;
	public float minute = 0;
	public float second = 0;
	public bool timerActive = true;
	public bool gameStarted = false;
	
	public bool showBayTime = false;
	public float bayTimeDefault = 1;
	public float bayTime = 1;
	
	public bool showAdd = false;
	public float showAddTimeDefault = 2;
	public float showAddTime = 2;
	
	GameObject backgroundMenu;
	GameObject WinMessage;
	GameObject LossMessage;
	GameObject RestartGameButton;
	GameObject StartGameButton;
	GameObject advertisingPanel;
	GameObject bayTimePanel;
	GameObject GameText;
	GameObject[] targetGOArr;
	string dataFileName = "gamedata.json";

    // Start is called before the first frame update
    void Start()
    {
		audioSource = gameObject.GetComponent<AudioSource>();
		targetGOArr = GameObject.FindGameObjectsWithTag("Target");
		
		minute = startMinute;
		second = startsecond;
		
		
		//�������� ���������� ���������� ����
		string saveFilePath = Application.persistentDataPath + "/" +dataFileName;
		if (File.Exists(saveFilePath)){
			save = JsonUtility.FromJson<Save>(File.ReadAllText(Application.persistentDataPath + "/gamedata.json"));
		}else{
			File.WriteAllText(saveFilePath, "{\"Level\":1}");
			save = JsonUtility.FromJson<Save>(File.ReadAllText(Application.persistentDataPath + "/gamedata.json"));
		}
		
		//�������� ��� ����
		backgroundMenu = GameObject.Find("background menu");
		// backgroundMenu.SetActive(false);
		//�������� ��������� � ������
		WinMessage = GameObject.Find("WinMessage");
		WinMessage.SetActive(false);
		//�������� ������ �������
		advertisingPanel = GameObject.Find("advertisingPanel");
		advertisingPanel.SetActive(false);
		//�������� ������ ������� �������
		bayTimePanel = GameObject.Find("bayTimePanel");
		bayTimePanel.SetActive(false);
		//�������� ��������� � ���������
		LossMessage = GameObject.Find("LossMessage");
		LossMessage.SetActive(false);
		//�������� ������ ����� ����
		RestartGameButton = GameObject.Find("RestartGame");
		RestartGameButton.SetActive(false);
		//�������� ������ ������� ���
		StartGameButton = GameObject.Find("StartGame");
		// StartGameButton.SetActive(false);
		//�������� ������� ����
		GameText = GameObject.Find("GameText");
		// GameText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		//����� ��������� � ������� �������
		if(showBayTime){
			bayTime -= 1f  * Time.deltaTime;
			
			if(bayTime <= 0f){
				showBayTime = false;
				bayTime = bayTimeDefault;
				bayTimePanel.SetActive(false);
			}
		}
		//����� �������
		if(showAdd){
			showAddTime -= 1f  * Time.deltaTime;
			
			if(showAddTime <= 0f){
				timerActive = true;
				gameStarted = true;
				showAdd = false;
				showAddTime = showAddTimeDefault;
				advertisingPanel.SetActive(false);
			}
		}
		
		if(gameStarted){
			if(timerActive){			
				if(second <= 0f){
					minute--;
				}
				
				if(second <= 0f){
					second = 59f;
				}

				second -= 1f  * Time.deltaTime;
				
				if(minute == 0f && second <= 0f){
					second = 0;
					timerActive = false;
				}
				
				//�������� �� ������, ���� ����� ��� �������� ��� ������
				if(CheckWin()){
					gameStarted = false;
					
					backgroundMenu.SetActive(true);
					WinMessage.SetActive(true);
					RestartGameButton.SetActive(true);
					audioSource.PlayOneShot(Resources.Load("Sound and musics/win", typeof(AudioClip)) as AudioClip);
				}
			}else{
				//�������� �� ������ ���� ����� �����������
				if(CheckWin()){
					gameStarted = false;
					
					backgroundMenu.SetActive(true);
					WinMessage.SetActive(true);
					RestartGameButton.SetActive(true);
					audioSource.PlayOneShot(Resources.Load("Sound and musics/win", typeof(AudioClip)) as AudioClip);
				}else{
					gameStarted = false;					
					backgroundMenu.SetActive(true);
					LossMessage.SetActive(true);
					RestartGameButton.SetActive(true);
					audioSource.PlayOneShot(Resources.Load("Sound and musics/loss", typeof(AudioClip)) as AudioClip);
				}
			}
			
			//����������� ���������� �������
			GameObject.Find("TimerText").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minute, second);
			//����������� ������ ����
			GameObject.Find("LevelText").GetComponent<Text>().text = "������� "+save.Level;
		}
    }
	
	//�������� �� ������
	bool CheckWin(){
		int availableTargets = GameObject.FindGameObjectsWithTag("Target").Length;
		if(availableTargets == 0){
			return true;
		}else{
			return false;
		}
	}
	
	public void PlayFoundSound(GameObject targetGO){	
		GameObject StarParticle = Resources.Load("Effects/StarParticle", typeof(GameObject)) as GameObject;
		GameObject StarParticleObject = Instantiate(StarParticle, targetGO.transform.position, transform.rotation);
		
		audioSource.PlayOneShot(Resources.Load("Sound and musics/finde"+UnityEngine.Random.Range(1, 4), typeof(AudioClip)) as AudioClip);
	}
	
	//���������� ����
	public void RestartGame(){
		minute = startMinute;
		second = startsecond;
		advertisingPanel.SetActive(true);
		showAdd = true;
		
		backgroundMenu.SetActive(false);
		WinMessage.SetActive(false);
		LossMessage.SetActive(false);
		RestartGameButton.SetActive(false);
		StartGameButton.SetActive(false);
		GameText.SetActive(false);
		
		save.Level++;
		//���������� ���������� ����
		string saveFilePath = Application.persistentDataPath + "/" +dataFileName;
		File.WriteAllText(saveFilePath, JsonUtility.ToJson(save));
		
		//����������� ����� ��� ������� � ����
		foreach(GameObject gO in targetGOArr){
			gO.SetActive(true);
		}
	}
	
	//������� �������
	public void bayTimeNow(){
		showBayTime = true;
		bayTimePanel.SetActive(true);
		float tmpSecond = second;
		tmpSecond += 10f;
		
		if(tmpSecond > 59f){
			second = tmpSecond - 59f;
			minute++;
		}else{
			second += 10f;
		}
		
		audioSource.PlayOneShot(Resources.Load("Sound and musics/money", typeof(AudioClip)) as AudioClip);
	}
	
	//������ ����
	public void StartGame(){
		gameStarted = true;
		backgroundMenu.SetActive(false);
		WinMessage.SetActive(false);
		LossMessage.SetActive(false);
		RestartGameButton.SetActive(false);
		StartGameButton.SetActive(false);
		GameText.SetActive(false);
	}
	
	//������� ����
	public void CloseGame(){
		Application.Quit();
	}
}
