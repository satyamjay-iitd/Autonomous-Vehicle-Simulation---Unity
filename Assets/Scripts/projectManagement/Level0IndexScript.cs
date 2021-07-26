using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level0IndexScript : MonoBehaviour
{
   public void LoadSceneLevel0SkyCar()
   {
   	SceneManager.LoadScene("Level0-No Automation1");

   }
   public void LoadSceneLevel0HuracanCar()
   {
   	SceneManager.LoadScene("Level0-No Automation2");

   }
    public void BacktoWelcomeScreen()
   {
   	SceneManager.LoadScene("WelcomeScreen");

   }
   
}

