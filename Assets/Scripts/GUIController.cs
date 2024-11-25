using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpScorePlayer1;
    [SerializeField] TextMeshProUGUI tmpScorePlayer2;
    [SerializeField] TextMeshProUGUI tmpLifesPlayer1;
    [SerializeField] TextMeshProUGUI tmpLifesPlayer2;

    private void OnGUI()
    {
        // Mostrar la puntuación y vidas de cada jugador
        if (GameController.currentPlayer == 1)
        {
            tmpScorePlayer1.gameObject.SetActive(true);
            tmpLifesPlayer1.gameObject.SetActive(true);
            tmpScorePlayer2.gameObject.SetActive(false);
            tmpLifesPlayer2.gameObject.SetActive(false);
        }
        else
        {
            tmpScorePlayer1.gameObject.SetActive(false);
            tmpLifesPlayer1.gameObject.SetActive(false);
            tmpScorePlayer2.gameObject.SetActive(true);
            tmpLifesPlayer2.gameObject.SetActive(true);
        }
        
        // Actualizar la puntuación y vidas de cada jugador
        tmpScorePlayer1.text = string.Format("{0,3:D3}", GameController.player1Score);
        tmpLifesPlayer1.text = GameController.player1Lifes.ToString();

        tmpScorePlayer2.text = string.Format("{0,3:D3}", GameController.player2Score);
        tmpLifesPlayer2.text = GameController.player2Lifes.ToString();
    }
}
