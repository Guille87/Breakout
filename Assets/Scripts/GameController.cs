using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int score { get; private set; } = 0;
    public static int lifes { get; private set; } = 3;

    private static int baseThreshold = 300; // Puntos iniciales para la primera recompensa
    private static float multiplier = 1.5f; // Incremento porcentual en cada recompensa
    private static int nextReward = 300; // Próximo umbral para ganar una vida
    private static int rewardsGiven = 0; // Número de vidas otorgadas como premio

    public static List<int> totalBricks = new List<int> { 0, 32, 32 };

    public static void UpdateScore(int points) {
        score += points;

        // Verificar si el jugador alcanzó el umbral de puntuación
        if (score >= nextReward)
        {
            lifes++; // Otorgar una vida adicional
            rewardsGiven++; // Incrementar contador de recompensas

            // Calcular el nuevo umbral dinámico
            nextReward = Mathf.CeilToInt(baseThreshold * Mathf.Pow(multiplier, rewardsGiven));

            Debug.Log($"¡Has ganado una vida extra! Nuevo umbral: {nextReward} puntos.");
        }
    }

    public static void UpdateLifes(int numLifes) {
        lifes += numLifes;
    }

    public static void ResetScore()
    {
        score = 0; // Establecer el puntaje al valor inicial
    }

    public static void ResetLifes()
    {
        lifes = 3; // Establecer las vidas al valor inicial
    }
}
