using DG.Tweening;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    public bool isRotating = false;

    GameObject player;
    readonly float ROTATION_TIME = 0.85f;
    float currrentRotation = 0;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void Rotate(float rawDirection)
    {
        isRotating = true;
        player.transform.SetParent(transform);

        int direction = (int) Mathf.Sign(rawDirection);
        currrentRotation -= 90 * direction;
        Vector3 targetRot = new (0, 0, currrentRotation );

        transform
            .DORotate(targetRot, ROTATION_TIME)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                isRotating = false;
                player.transform.SetParent(null);
            });
    }
}
