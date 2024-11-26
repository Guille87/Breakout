using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]  TextMeshProUGUI tmpGameOver;
    [SerializeField] TextMeshProUGUI tmpCurrentPlayer;

    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxPaddle;
    [SerializeField] AudioClip sfxBrick;
    [SerializeField] AudioClip sfxWall;
    [SerializeField] AudioClip sfxFail;
    [SerializeField] AudioClip sfxNextLevel;

    [Header("Settings")]
    [SerializeField] float force;
    [SerializeField] float forceInc;
    [SerializeField] float delay;
    [SerializeField] float hitOffset;

    public static event Action OnBrickDestroyed; // Evento para destrucción de ladrillos
    public static event Action OnBallLost;       // Evento para pérdida de la bola

    Rigidbody2D rb;

    int hitCount;
    int brickCount;
    int sceneId;

    GameObject paddle1;
    GameObject paddle2;
    bool halved;

    Dictionary<string, int> bricks = new Dictionary<string, int>
    {
        {"brick-y", 10},
        {"brick-g", 15},
        {"brick-a", 20},
        {"brick-r", 25},
        {"brick-pass", 25}
    };

    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;
        rb = GetComponent<Rigidbody2D>();
        paddle1 = GameObject.FindWithTag("paddle1");
        paddle2 = GameObject.FindWithTag("paddle2");
        Invoke(nameof(LaunchBall), delay);

        // Configurar visibilidad de las palas
        ConfigurePaddlesVisibility();

        // Mostrar el jugador actual al iniciar
        UpdateCurrentPlayerText();
    }

    private void LaunchBall()
    {
        ResetBall();
        Vector2 randomDir = GetRandomDirection();
        ApplyForce(randomDir);
    }

    private void ConfigurePaddlesVisibility()
{
    if (GameController.IsSinglePlayer || GameController.IsTwoPlayerTurnBased)
    {
        paddle1.SetActive(true);
        paddle2.SetActive(false);
    }
    else if (GameController.IsTwoPlayerSimultaneous)
    {
        paddle1.SetActive(true);
        paddle2.SetActive(true);
    }
}

    private void UpdateCurrentPlayerText()
    {
        if (tmpCurrentPlayer != null)
        {
            if (GameController.IsTwoPlayerTurnBased)
            {
                tmpCurrentPlayer.text = $"JUGADOR {GameController.currentPlayer}";
                tmpCurrentPlayer.gameObject.SetActive(true); // Muestra el texto
            }
            else
            {
                tmpCurrentPlayer.gameObject.SetActive(false); // Oculta el texto en otros modos
            }
        }
    }

    // Actualiza el texto al cambiar de jugador
    void OnEnable()
    {
        GameController.OnPlayerSwitched += UpdatePaddleVisibility;
        GameController.OnPlayerSwitched += UpdateCurrentPlayerText;
    }

    void OnDisable()
    {
        GameController.OnPlayerSwitched -= UpdatePaddleVisibility;
        GameController.OnPlayerSwitched -= UpdateCurrentPlayerText;
    }

    private void UpdatePaddleVisibility()
    {
        if (GameController.IsTwoPlayerTurnBased)
        {
            paddle1.SetActive(GameController.currentPlayer == 1);
            paddle2.SetActive(GameController.currentPlayer == 2);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;

        if (bricks.ContainsKey(tag))
        {
            DestroyBrick(other.gameObject);
        }
        else if (tag == "paddle1" || tag == "paddle2")
        {
            HandlePaddleCollision(other);
        }
        else if (tag == "wall-top" || tag == "wall-lateral" || tag == "brick-rock")
        {
            AudioManager.PlaySound(sfxWall, 0.2f);

            if (!halved && tag == "wall-top")
            {
                // Reduce the size of the paddle
                HalvePaddle(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "wall-bottom")
        {
            AudioManager.PlaySound(sfxFail, 0.2f);

            // Notificar que la bola se perdió
            OnBallLost?.Invoke();
            if (GameController.IsSinglePlayer || GameController.IsTwoPlayerSimultaneous)
            {
                GameController.UpdateLifes(-1, GameController.currentPlayer);

                if (GameController.player1Lifes <= 0)
                {
                    GameOver();
                }
                else
                {
                    ResetPaddle();
                    Invoke(nameof(LaunchBall), delay);
                }
            }
            else if (GameController.IsTwoPlayerTurnBased)
            {
                GameController.UpdateLifes(-1, GameController.currentPlayer);
                
                // Verificar si el jugador actual ha perdido todas las vidas
                if (GameController.currentPlayer == 1 && GameController.player1Lifes <= 0)
                {
                    // Si el jugador 1 ha perdido todas las vidas
                    // Verificar si el jugador 2 tiene vidas
                    if (GameController.player2Lifes > 0)
                    {
                        // Cambiar al jugador 2
                        GameController.SwitchPlayer();
                    }
                    else
                    {
                        // Si ambos jugadores han perdido todas las vidas, fin del juego
                        GameOver();
                    }
                }
                else if (GameController.currentPlayer == 2 && GameController.player2Lifes <= 0)
                {
                    // Si el jugador 2 ha perdido todas las vidas
                    // Verificar si el jugador 1 tiene vidas
                    if (GameController.player1Lifes > 0)
                    {
                        // Cambiar al jugador 1
                        GameController.SwitchPlayer();
                    }
                    else
                    {
                        // Si ambos jugadores han perdido todas las vidas, fin del juego
                        GameOver();
                    }
                }
                else
                {
                    // Si el jugador activo no ha perdido todas sus vidas, verificamos el turno
                    if (GameController.currentPlayer == 1 && GameController.player1Lifes > 0 && GameController.player2Lifes <= 0)
                    {
                        // Si el jugador 1 sigue con vidas y el jugador 2 no tiene, continúa jugando el jugador 1
                    }
                    else if (GameController.currentPlayer == 2 && GameController.player2Lifes > 0 && GameController.player1Lifes <= 0)
                    {
                        // Si el jugador 2 sigue con vidas y el jugador 1 no tiene, continúa jugando el jugador 2
                    }
                    else
                    {
                        // Si ambos jugadores tienen vidas, cambiamos de turno
                        GameController.SwitchPlayer();
                    }
                }

                // Verificar si ambos jugadores han perdido todas las vidas
                if (!GameController.IsGameOver())
                {
                    // Si aún quedan vidas, reiniciamos la bola
                    ResetPaddle();
                    Invoke(nameof(LaunchBall), delay);
                }
            }
        }
        else if (other.tag == "brick-pass")
        {
            DestroyBrick(other.gameObject);
        }
    }

    private void GameOver()
    {
        // Detener la pelota
        rb.linearVelocity = Vector2.zero;

        tmpGameOver.gameObject.SetActive(true);

        // Reinicia el juego después de 3 segundos
        Invoke(nameof(RestartGame), 3f);
    }

    private void RestartGame()
    {
        // Recargar la escena principal para reiniciar el juego
        SceneManager.LoadScene(0);

        // Restablecer las vidas y el puntaje antes de comenzar el juego
        GameController.ResetScore();
        GameController.ResetLifes();
    }

    private void ResetBall()
    {
        float randomX = UnityEngine.Random.Range(-2f, 2f); // Cambia -5 y 5 por el rango deseado
        transform.position = new Vector3(randomX, 0, 0);   // Coloca la pelota en X aleatoria
        rb.linearVelocity = Vector2.zero;
    }

    private Vector2 GetRandomDirection()
    {
        float dirX = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        return new Vector2(dirX, -1).normalized;
    }

    private void ApplyForce(Vector2 direction)
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void DestroyBrick(GameObject obj)
    {
        AudioManager.PlaySound(sfxBrick, 0.2f);

        // Actualizar la puntuación del jugador actual
        GameController.UpdateScore(bricks[obj.tag], GameController.currentPlayer);

        Destroy(obj);

        ++brickCount;

        OnBrickDestroyed?.Invoke(); // Notificar que se destruyó un ladrillo

        if (brickCount == GameController.totalBricks[sceneId])
        {
            GoToNextLevel();
        }
    }

    private void HandlePaddleCollision(Collision2D other)
    {
        AudioManager.PlaySound(sfxPaddle, 0.2f);
        
        // paddle position
        /*Vector3 paddle = other.gameObject.transform.position;

        // get the contact point
        Vector2 contact = other.GetContact(0).point;

        if ((rb.linearVelocity.x < 0 && contact.x > (paddle.x + hitOffset)) || 
            (rb.linearVelocity.x > 0 && contact.x < (paddle.x - hitOffset)))
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }*/

        // Posición de la pala y cálculo del desplazamiento
        Vector2 paddlePosition = other.transform.position;
        float offset = transform.position.x - paddlePosition.x;
        float width = other.collider.bounds.size.x;
        float normalizedOffset = offset / (width / 2);

        // Dirección calculada con un mínimo de velocidad en X
        float minXVelocity = 0.2f; // Velocidad mínima en X
        float directionX = Mathf.Clamp(normalizedOffset, -1, 1);

        // Aseguramos que no sea completamente vertical
        if (Mathf.Abs(directionX) < minXVelocity)
        {
            directionX = directionX > 0 ? minXVelocity : -minXVelocity;
        }

        float directionY = Mathf.Sqrt(1 - directionX * directionX); // Mantener magnitud del vector

        // Corregir el vector para asegurar que siempre vaya hacia arriba con un mínimo en Y
        float minYVelocity = 0.3f; // Mínimo componente vertical
        if (directionY < minYVelocity)
        {
            directionY = minYVelocity;
            directionX = Mathf.Sqrt(1 - directionY * directionY) * Mathf.Sign(directionX); // Recalcular X
        }

        Vector2 direction = new Vector2(directionX, directionY).normalized;

        // Aplicar nueva dirección con la velocidad actual
        rb.linearVelocity = direction * rb.linearVelocity.magnitude;

        // Incremento de velocidad por impactos
        hitCount++;
        if (hitCount % 4 == 0)
        {
            rb.AddForce(direction * forceInc, ForceMode2D.Impulse);
        }
    }

    void HalvePaddle(bool halve)
    {
        halved = halve;

        Vector3 scale = paddle1.transform.localScale = paddle2.transform.localScale;

        paddle1.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z):
            new Vector3(scale.x * 2f, scale.y, scale.z);
        
        paddle2.transform.localScale = halved ?
            new Vector3(scale.x * 0.5f, scale.y, scale.z):
            new Vector3(scale.x * 2f, scale.y, scale.z);
    }

    private void ResetPaddle()
    {
        if (halved) HalvePaddle(false);
    }

    private void GoToNextLevel()
    {
        AudioManager.PlaySound(sfxNextLevel, 0.2f);
        rb.linearVelocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke(nameof(NextScene), 3);
    }

    void NextScene()
    {
        int nextId = sceneId + 1;
        if (nextId == GameController.totalBricks.Count)
        {
            nextId = 0;
            GameController.ResetScore();
            GameController.ResetLifes();
        }
        SceneManager.LoadScene(nextId);
    }
}
