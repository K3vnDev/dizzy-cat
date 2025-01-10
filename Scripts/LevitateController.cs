using System.Collections;
using UnityEngine;

public class LevitateController : MonoBehaviour
{
    [SerializeField] float range, period;

    void Start()
    {
        StartCoroutine(FloatingAnimation());    
    }
    
    private IEnumerator FloatingAnimation()
    {
        float positionY = transform.localPosition.y;

        while (true)
        {
            yield return Levitate(positionY - range / 2);
            yield return Levitate(positionY + range / 2);
        }
    }

    IEnumerator Levitate(float targetY)
    {
        Vector2 start = transform.localPosition, 
            target = new(transform.localPosition.x, targetY);

        float elapsed = 0, time = period / 4;

        while (elapsed < time)
        {
            transform.localPosition = Vector2.Lerp(start, target, elapsed / time);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
    }
}
