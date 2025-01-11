using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins;

    public bool fsActive = true;
    public int currentLevel; // one-indexed

    [Header("Character")]
    public int selectedCharacter = 0;
    public Sprite[] characterSprites;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 1;
    }
}
