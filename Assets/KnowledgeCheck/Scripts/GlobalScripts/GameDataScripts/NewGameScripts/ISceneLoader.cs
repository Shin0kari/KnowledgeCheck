using UnityEngine;

public interface ISceneLoader
{
    public AsyncOperation LoadSceneAsync(string sceneName);
}
