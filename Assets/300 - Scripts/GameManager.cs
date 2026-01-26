using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameManager Instance;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
