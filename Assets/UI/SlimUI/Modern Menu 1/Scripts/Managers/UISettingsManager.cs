using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UISettingsManager : MonoBehaviour {




		// sliders
		[Header("VOLUME SLIDERS")]

		[SerializeField]
    private AudioManager audioManager;

		[SerializeField]
		private GameObject globalSlider;
		[SerializeField]
		private GameObject uiSlider;
		[SerializeField]
		private GameObject sfxSlider;
		[SerializeField]
		private GameObject enemySlider;
		[SerializeField]
		private GameObject musicSlider;
		
		

		public void  Start (){
			

      audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
			// check slider values
			globalSlider.GetComponent<Slider>().value = audioManager.getGlobalVolume();
			uiSlider.GetComponent<Slider>().value = audioManager.getUIVolume();
			sfxSlider.GetComponent<Slider>().value = audioManager.getSFXVolume();
			enemySlider.GetComponent<Slider>().value = audioManager.getEnemyVolume();
			musicSlider.GetComponent<Slider>().value = audioManager.getMusicVolume();


			audioManager.setGlobalVolume(globalSlider.GetComponent<Slider>().value);
			audioManager.setUIVolume(uiSlider.GetComponent<Slider>().value);
			audioManager.setSFXVolume(sfxSlider.GetComponent<Slider>().value);
			audioManager.setEnemyVolume(enemySlider.GetComponent<Slider>().value);
			audioManager.setMusicVolume(musicSlider.GetComponent<Slider>().value);
		}











		


		

		public void MusicSlider (){
    audioManager.setMusicVolume(musicSlider.GetComponent<Slider>().value);
		}

		public void GlobalSlider (){
			audioManager.setGlobalVolume(globalSlider.GetComponent<Slider>().value);
		}

		public void UISlider (){
			audioManager.setUIVolume(uiSlider.GetComponent<Slider>().value);
		}

		public void SFXSlider (){
			audioManager.setSFXVolume(sfxSlider.GetComponent<Slider>().value);
		}

		public void EnemySlider (){
			audioManager.setEnemyVolume(enemySlider.GetComponent<Slider>().value);
		}
		
}