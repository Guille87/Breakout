using UnityEngine;

public class PaddleController : MonoBehaviour
{
    const float MAX_X = 3.1f;
    const float MIN_X = -3.1f;

    [SerializeField] float speed;
    [SerializeField] bool isPlayer1;

    void Start()
    {
        // Configura la posición inicial dependiendo del jugador y el modo de juego
        if (GameController.IsTwoPlayerSimultaneous)
        {
            if (isPlayer1)
            {
                transform.position = new Vector3(MIN_X / 2, transform.position.y, transform.position.z); // Jugador 1 a la izquierda
            }
            else
            {
                transform.position = new Vector3(MAX_X / 2, transform.position.y, transform.position.z); // Jugador 2 a la derecha
            }
        }
        else if (GameController.IsSinglePlayer || GameController.IsTwoPlayerTurnBased)
        {
            if (isPlayer1)
            {
                transform.position = new Vector3(0, transform.position.y, transform.position.z); // Jugador 1 centrado
            }
        }
    }

    void Update()
    {
        float x = transform.position.x;
        
        if (isPlayer1) // Controles del jugador 1
        {
            if (x > MIN_X && Input.GetKey(KeyCode.LeftArrow))
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            else if (x < MAX_X && Input.GetKey(KeyCode.RightArrow))
                transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else // Controles del jugador 2
        {
            if (x > MIN_X && Input.GetKey(KeyCode.A))
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            else if (x < MAX_X && Input.GetKey(KeyCode.D))
                transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        
        // control más suave aunque no da una sensación retro
        /*float horizontal = Input.GetAxis("Horizontal");
        float newX = Mathf.Clamp(transform.position.x + horizontal * speed * Time.deltaTime, MIN_X, MAX_X);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);*/
    }
}
