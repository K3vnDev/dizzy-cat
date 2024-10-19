using UnityEngine;

public class InfoPlayerSingleton : MonoBehaviour
{
    public static InfoPlayerSingleton Instance;

    //Player Info
    public int selectedCharacter;
    public int currentLevel;
    public int currentBgColor = 7;
    public bool fsActive = true;

    private void Awake()
    {
        if (InfoPlayerSingleton.Instance == null) 
        {
            InfoPlayerSingleton.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        QualitySettings.vSyncCount = 1;
    }
}
