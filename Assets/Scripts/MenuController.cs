using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] AudioClip sfxNextLevel;
    [SerializeField] private TextMeshProUGUI option1Text;
    [SerializeField] private TextMeshProUGUI option2Text;
    [SerializeField] private GameObject indicator;
    [SerializeField] private StartGame startGame; // Referencia al script StartGame
    private int selectedOption = 0; // 0 = "1 Jugador", 1 = "2 Jugadores por turnos"
    private Coroutine blinkCoroutine;

    void Start()
    {
        UpdateIndicatorPosition();
        StartBlinking(); // Iniciar parpadeo al arrancar
    }

    void Update()
    {
        // Navegación con las flechas
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedOption = Mathf.Max(0, selectedOption - 1); // No permite valores negativos
            UpdateIndicatorPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedOption = Mathf.Min(1, selectedOption + 1); // Máximo es 1
            UpdateIndicatorPosition();
        }

        // Selección con Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleSelection();
        }
    }

    private void UpdateIndicatorPosition()
    {
        // Ajustar la posición del indicador según la opción seleccionada
        if (selectedOption == 0)
        {
            indicator.transform.position = option1Text.transform.position + Vector3.left * 50f;
        }
        else if (selectedOption == 1)
        {
            indicator.transform.position = option2Text.transform.position + Vector3.left * 50f;
        }

        // Reinicia el parpadeo en la nueva opción seleccionada
        RestartBlinking();
    }

    private void HandleSelection()
    {
        // Reproduce el sonido antes de iniciar la transición
        AudioManager.PlaySound(sfxNextLevel, 0.2f);

        if (selectedOption == 0)
        {
            StartCoroutine(StartFastBlinking(0.1f)); // Acelerar el parpadeo
            option2Text.enabled = false; // Oculta "2 Jugadores"
            startGame.StartGameTransition(1); // Llamar a la transición para 1 jugador
        }
        else if (selectedOption == 1)
        {
            StartCoroutine(StartFastBlinking(0.1f)); // Acelerar el parpadeo
            option1Text.enabled = false; // Oculta "1 Jugador"
            startGame.StartGameTransition(2); // Llamar a la transición para 2 jugadores
        }
    }

    private IEnumerator StartFastBlinking(float fastBlinkSpeed)
    {
        // Detiene cualquier parpadeo previo
        StopAllCoroutines();

        // Inicia el parpadeo rápido
        yield return StartCoroutine(BlinkSelectedOption(fastBlinkSpeed));
    }

    private void StartBlinking()
    {
        // Inicia el parpadeo en la opción seleccionada
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkSelectedOption());
    }

    private void StopBlinking()
    {
        // Detiene el parpadeo y muestra ambos textos
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        option1Text.enabled = true;
        option2Text.enabled = true;
    }

    private void RestartBlinking()
    {
        StopBlinking();
        StartBlinking();
    }

    private IEnumerator BlinkSelectedOption(float blinkSpeed = 0.4f)
    {
        // Detenemos el parpadeo en todas las opciones inicialmente
        option1Text.enabled = true;
        option2Text.enabled = true;
        
        TextMeshProUGUI selectedText = selectedOption == 0 ? option1Text : option2Text;

        while (true)
        {
            selectedText.enabled = !selectedText.enabled; // Alterna visibilidad
            yield return new WaitForSeconds(blinkSpeed); // Usa el tiempo proporcionado
        }
    }
}
