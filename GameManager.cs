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
		
		
		//Загрузка сохранённых параметров игры
		string saveFilePath = Application.persistentDataPath + "/" +dataFileName;
		if (File.Exists(saveFilePath)){
			save = JsonUtility.FromJson<Save>(File.ReadAllText(Application.persistentDataPath + "/gamedata.json"));
		}else{
			File.WriteAllText(saveFilePath, "{\"Level\":1}");
			save = JsonUtility.FromJson<Save>(File.ReadAllText(Application.persistentDataPath + "/gamedata.json"));
		}
		
		//Получить фон меню
		backgroundMenu = GameObject.Find("background menu");
		// backgroundMenu.SetActive(false);
		//Получить сообщение о победе
		WinMessage = GameObject.Find("WinMessage");
		WinMessage.SetActive(false);
		//Получить панель рекламы
		advertisingPanel = GameObject.Find("advertisingPanel");
		advertisingPanel.SetActive(false);
		//Получить панель покупки времени
		bayTimePanel = GameObject.Find("bayTimePanel");
		bayTimePanel.SetActive(false);
		//Получить сообщение о проигрыше
		LossMessage = GameObject.Find("LossMessage");
		LossMessage.SetActive(false);
		//Получить кнопку новой игры
		RestartGameButton = GameObject.Find("RestartGame");
		RestartGameButton.SetActive(false);
		//Получить кнопку запуска игы
		StartGameButton = GameObject.Find("StartGame");
		// StartGameButton.SetActive(false);
		//Получить надпись игры
		GameText = GameObject.Find("GameText");
		// GameText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		//Показ сообщения о покупке времени
		if(showBayTime){
			bayTime -= 1f  * Time.deltaTime;
			
			if(bayTime <= 0f){
				showBayTime = false;
				bayTime = bayTimeDefault;
				bayTimePanel.SetActive(false);
			}
		}
		//Показ рекламы
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
				
				//Проверка на победу, если время ещё доступно для игрока
				if(CheckWin()){
					gameStarted = false;
					
					backgroundMenu.SetActive(true);
					WinMessage.SetActive(true);
					RestartGameButton.SetActive(true);
					audioSource.PlayOneShot(Resources.Load("Sound and musics/win", typeof(AudioClip)) as AudioClip);
				}
			}else{
				//Проверка на победу если время закончилось
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
			
			//Отображение доступного времени
			GameObject.Find("TimerText").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minute, second);
			//Отображение уровня игры
			GameObject.Find("LevelText").GetComponent<Text>().text = "Уровень "+save.Level;
		}
    }
	
	//Проверка на победу
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
	
	//Перезапуск игры
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
		//Сохранение параметров игры
		string saveFilePath = Application.persistentDataPath + "/" +dataFileName;
		File.WriteAllText(saveFilePath, JsonUtility.ToJson(save));
		
		//Активизация целей для нажатия в игре
		foreach(GameObject gO in targetGOArr){
			gO.SetActive(true);
		}
	}
	
	//Покупка времени
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
	
	//Запуск игры
	public void StartGame(){
		gameStarted = true;
		backgroundMenu.SetActive(false);
		WinMessage.SetActive(false);
		LossMessage.SetActive(false);
		RestartGameButton.SetActive(false);
		StartGameButton.SetActive(false);
		GameText.SetActive(false);
	}
	
	//Закрыть игру
	public void CloseGame(){
		Application.Quit();
	}
}
