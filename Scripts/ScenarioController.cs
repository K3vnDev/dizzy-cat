using System.Collections;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private AnimationCurve easeCurve;
    private float rotationTime = .8f;
    private float zRotation = 0;
    public bool rotating = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetRotation(float rawDirection)
    {
        int direction = (int) Mathf.Sign(rawDirection);

        rotating = true;
        zRotation -= 90 * direction;

        Quaternion targetRot = Quaternion.Euler(0, 0, zRotation );
        StartCoroutine(Rotate(transform.rotation, targetRot, rotationTime));
    }

    private IEnumerator Rotate(Quaternion start, Quaternion target, float t)
    {
        float currentTime = 0;
        player.transform.SetParent(transform);

        while (currentTime < t)
        {
            transform.rotation = Quaternion.Lerp(start, target, 
                easeCurve.Evaluate(currentTime/t));

            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = target;

        rotating = false;
        player.transform.SetParent(null);
    }
}
