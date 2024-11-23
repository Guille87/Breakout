using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int score { get; private set; } = 0;
    public static int lifes { get; private set; } = 3;

    public static List<int> totalBricks = new List<int> { 0, 32, 32 };

    public static void UpdateScore(int points) {
        score += points;
    }

    public static void UpdateLifes(int numLifes) {
        lifes += numLifes;
    }
}
