using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Countdown : MonoBehaviour
{
    [SerializeField] private Image countdownCircleTimer;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float startTime = 3.0f;
    [SerializeField] public GameObject home;
    [SerializeField] public GameObject time;

     private float currentTime;
     private bool updateTime;

     private void Start()
   {
      StartCoroutine(CountdownTimer());
   }
   private IEnumerator CountdownTimer()
   {
      
      float duration = startTime;  
      //to whatever you want
      float normalizedTime = 0;

      while(normalizedTime <= 1f)
      {
      currentTime += Time.deltaTime;
      countdownCircleTimer.fillAmount = normalizedTime;
      normalizedTime += Time.deltaTime / duration;
      countdownText.text = (int)currentTime+ "";
      yield return null;
      }
      yield return new WaitForSeconds(1.0f);
      showHome();

   }

   void showHome()
   {
      time.SetActive(false);
      home.SetActive(true);
      //Return to Main Menu Scene
   }
}
