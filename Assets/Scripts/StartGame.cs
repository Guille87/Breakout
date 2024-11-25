using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] Transform paddle;
    [SerializeField] GameObject ball;
    [SerializeField] TextMeshProUGUI tmpStart;
    [SerializeField] float duration;
    
    public void StartGameTransition(int mode)
    {
        // Ocultar elementos iniciales
        ball.SetActive(false);
        tmpStart.enabled = false;

        // Iniciar la transición y cargar la escena según el modo seleccionado
        StartCoroutine(TransitionAndLoadScene(mode));
    }

    private IEnumerator TransitionAndLoadScene(int mode)
    {
        GameController.SetMode(mode); // Configurar el modo de juego
        Vector3 scaleStart = paddle.localScale;
        Vector3 scaleEnd = new Vector3(0, scaleStart.y, scaleStart.z);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            paddle.localScale = Vector3.Lerp(scaleStart, scaleEnd, t / duration);
            yield return null;
        }

        SceneManager.LoadScene(1);
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
