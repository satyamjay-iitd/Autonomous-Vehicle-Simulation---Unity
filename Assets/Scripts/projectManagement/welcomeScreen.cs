using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class welcomeScreen : MonoBehaviour
{
   public void LoadSceneLevel0()
   {
   	SceneManager.LoadScene("Level0Index");

   }
   public void LoadSceneLevel1()
   {
   	SceneManager.LoadScene("Level1Index");

   }
    public void LoadSceneLevel2()
   {
   	SceneManager.LoadScene("Level2-PartiallyAutomated");

   }
     public void LoadSceneLevel2NodeBased()
   {
    SceneManager.LoadScene("Level2NodeLaneDetection");

   }
    public void ObjectDetection()
   {
   	SceneManager.LoadScene("Object Detection");

   }
    public void ObjectTracking()
   {
   	SceneManager.LoadScene("Object Tracking");

   }
   public void LaneDetection()
   {
   	SceneManager.LoadScene("Lane Detection");

   }
    public void WeatherEffects()
   {
   	SceneManager.LoadScene("WeatherEffects");

   }
    public void Credits()
   {
   	SceneManager.LoadScene("CreditsScreen");

   }

}
