using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    AudioSource sfx;
    [SerializeField] Transform paddle;
    [SerializeField] GameObject ball;
    [SerializeField] TextMeshProUGUI msg;
    [SerializeField] float duration;

    void Start()
    {
        sfx = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.anyKeyDown)
        {
            ball.SetActive(false);
            msg.enabled = false;

            sfx.Play();

            StartCoroutine("StartNextLevel");
        }
    }

    IEnumerator StartNextLevel()
    {
        Vector3 scaleStart = paddle.localScale;
        Vector3 scaleEnd = new Vector3(0, scaleStart.y, scaleStart.z);
        
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            paddle.localScale = Vector3.Lerp(scaleStart, scaleEnd, t/duration);
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
}
