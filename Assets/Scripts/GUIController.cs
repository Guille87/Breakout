using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpScore;
    [SerializeField] TextMeshProUGUI tmpLifes;

    private void OnGUI() {
        tmpScore.text = string.Format("{0,3:D3}", GameController.score);
        tmpLifes.text = GameController.lifes.ToString();
    }
}
