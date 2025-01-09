using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins;

    public int currentBgColor = 7;
    public bool fsActive = true;

    public int currentLevel; // one-indexed

    [Header ("Character")]
    public int selectedCharacter = 0;
    public Sprite[] characterSprites;

    private void Awake()
    {
        if (Ins == null) 
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        QualitySettings.vSyncCount = 1;
    }
}
