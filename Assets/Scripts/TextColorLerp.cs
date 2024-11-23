using System.Collections;
using TMPro;
using UnityEngine;

public class TextColorLerp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI msg;
    [SerializeField] float duration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("ChangeColor");
    }

    IEnumerator ChangeColor()
    {
        /*float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            msg.color = Color.Lerp(Color.black, Color.white, t/duration);
            yield return null;
        }
        // reiniciar la corutina
        StartCoroutine("ChangeColor");*/

        while (true)
        {
            float t = Mathf.PingPong(Time.time / duration, 1);
            msg.color = Color.Lerp(Color.black, Color.white, t);
            yield return null;
        }
    }
}
