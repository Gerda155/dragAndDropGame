using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void City(string scene)
    {
        SceneManager.LoadScene(scene);
    }

   public void Exit()
   {
        Application.Quit();
   } 
}
