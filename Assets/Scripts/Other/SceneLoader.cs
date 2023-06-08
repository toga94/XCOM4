using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] int loadSceneIndex;
    [SerializeField] bool OnClickOnly;

    private void Start()
    {
        if (!OnClickOnly)
        {
            LoadSceneAsync();
        }
    }

    public void OnClick()
    {
        LoadSceneAsync();
    }

    private async void LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneIndex, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }
    }
}
