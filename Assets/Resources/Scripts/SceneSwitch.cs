using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

	public string targetSceneName;
	
	public void Switch()
	{
		SceneManager.LoadScene(targetSceneName);
	}
}
