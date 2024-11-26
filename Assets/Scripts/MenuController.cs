using System.Collections;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] AudioClip sfxNextLevel;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] private TextMeshProUGUI tmp1Player;
    [SerializeField] private TextMeshProUGUI tmp2PlayerPerTurns;
    [SerializeField] private TextMeshProUGUI tmp2PlayerSimultaneous;
    [SerializeField] private GameObject indicator;
    [SerializeField] private StartGame startGame; // Referencia al script StartGame
    private int selectedOption = 0; // 0 = "1 Jugador", 1 = "2 Jugadores por turnos"
    private int totalOptions = 3; // Número total de opciones
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
            AudioManager.PlaySound(sfxWall, 0.2f);
            selectedOption = (selectedOption - 1 + totalOptions) % totalOptions; // Navegar hacia arriba, circular
            UpdateIndicatorPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AudioManager.PlaySound(sfxWall, 0.2f);
            selectedOption = (selectedOption + 1) % totalOptions; // Navegar hacia abajo, circular
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
            indicator.transform.position = tmp1Player.transform.position + Vector3.left * 50f;
        }
        else if (selectedOption == 1)
        {
            indicator.transform.position = tmp2PlayerPerTurns.transform.position + Vector3.left * 50f;
        }
        else if (selectedOption == 2)
        {
            indicator.transform.position = tmp2PlayerSimultaneous.transform.position + Vector3.left * 50f;
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
            tmp2PlayerPerTurns.enabled = false; // Oculta "2 Jugadores por turnos"
            tmp2PlayerSimultaneous.enabled = false; // Oculta "2 Jugadores simultaneos"
            startGame.StartGameTransition(1); // Llamar a la transición para 1 jugador
        }
        else if (selectedOption == 1)
        {
            StartCoroutine(StartFastBlinking(0.1f)); // Acelerar el parpadeo
            tmp1Player.enabled = false; // Oculta "1 Jugador"
            tmp2PlayerSimultaneous.enabled = false; // Oculta "2 Jugadores simultaneos"
            startGame.StartGameTransition(2); // Llamar a la transición para 2 jugadores por turnos
        }
        else if (selectedOption == 2)
        {
            StartCoroutine(StartFastBlinking(0.1f)); // Acelerar el parpadeo
            tmp1Player.enabled = false; // Oculta "1 Jugador"
            tmp2PlayerPerTurns.enabled = false; // Oculta "2 Jugadores por turnos"
            startGame.StartGameTransition(3); // Llamar a la transición para 2 jugadores simultaneos
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
        tmp1Player.enabled = true;
        tmp2PlayerPerTurns.enabled = true;
        tmp2PlayerSimultaneous.enabled = true;
    }

    private void RestartBlinking()
    {
        StopBlinking();
        StartBlinking();
    }

    private IEnumerator BlinkSelectedOption(float blinkSpeed = 0.4f)
    {
        // Detenemos el parpadeo en todas las opciones inicialmente
        tmp1Player.enabled = true;
        tmp2PlayerPerTurns.enabled = true;
        tmp2PlayerSimultaneous.enabled = true;
        
        TextMeshProUGUI selectedText;

        if (selectedOption == 0)
        {
            selectedText = tmp1Player; // Opción 1 seleccionada
        }
        else if (selectedOption == 1)
        {
            selectedText = tmp2PlayerPerTurns; // Opción 2 seleccionada
        }
        else
        {
            selectedText = tmp2PlayerSimultaneous; // Opción 3 seleccionada
        }

        while (true)
        {
            selectedText.enabled = !selectedText.enabled; // Alterna visibilidad
            yield return new WaitForSeconds(blinkSpeed); // Usa el tiempo proporcionado
        }
    }
}
