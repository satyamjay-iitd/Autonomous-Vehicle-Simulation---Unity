using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1IndexScript : MonoBehaviour
{
    public void LoadSceneLevel1ObjDetect()
   {
   	SceneManager.LoadScene("Level1-Driver Assisted1");

   }
   public void LoadSceneLevel1ColourDetect()
   {
   	SceneManager.LoadScene("Level1-Driver Assisted2");

   }
    public void BacktoWelcomeScreen()
   {
   	SceneManager.LoadScene("WelcomeScreen");

   }
}
