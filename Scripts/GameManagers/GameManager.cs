using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public int currentLevel;

    [Header("Character")]
    public int selectedCharacter = 0;
    public Sprite[] characterSprites;

    [Header ("Settings")]
    public bool IsOnFullscreen = true;
    public AnimationCurve soundCurve;

    public bool openingAlreadyShown = false;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 1;
    }
}
