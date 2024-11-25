using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool IsSinglePlayer { get; private set; }
    public static bool IsTwoPlayerTurnBased { get; private set; }
    public static bool IsTwoPlayerSimultaneous { get; private set; }

    public static int player1Score { get; private set; } = 0;
    public static int player2Score { get; private set; } = 0;

    public static int player1Lifes { get; private set; } = 3;
    public static int player2Lifes { get; private set; } = 3;

    public static int currentPlayer = 1; // 1: Player 1, 2: Player 2

    private static int baseThreshold = 300; // Puntos iniciales para la primera recompensa
    private static float multiplier = 1.5f; // Incremento porcentual en cada recompensa
    private static int nextReward = 300; // Próximo umbral para ganar una vida
    private static int rewardsGiven = 0; // Número de vidas otorgadas como premio
    public static event Action OnPlayerSwitched; // Evento para notificar cambio de turno

    public static List<int> totalBricks = new List<int> { 0, 32, 32 };

    public static void SetMode(int mode)
    {
        IsSinglePlayer = mode == 1;
        IsTwoPlayerTurnBased = mode == 2;
        IsTwoPlayerSimultaneous = mode == 3;
    }

    public static void UpdateScore(int points, int player)
    {
        if (IsSinglePlayer || IsTwoPlayerSimultaneous)
        {
            player1Score += points;

            // Verificar si se alcanza el umbral de puntuación para ganar una vida
            if (player1Score >= nextReward)
            {
                player1Lifes++;
                rewardsGiven++;
                nextReward = Mathf.CeilToInt(baseThreshold * Mathf.Pow(multiplier, rewardsGiven));
            }
        }
        else if (IsTwoPlayerTurnBased)
        {
            // Puntos individuales por jugador
            if (player == 1)
            {
                player1Score += points;
            }
            else
            {
                player2Score += points;
            }

            // Verificar si el jugador alcanzó el umbral de puntuación
            if ((player == 1 && player1Score >= nextReward) || (player == 2 && player2Score >= nextReward))
            {
                if (player == 1)
                {
                    player1Lifes++; // Otorgar una vida adicional al jugador 1
                }
                else
                {
                    player2Lifes++; // Otorgar una vida adicional al jugador 2
                }

                rewardsGiven++; // Incrementar contador de recompensas

                // Calcular el nuevo umbral dinámico
                nextReward = Mathf.CeilToInt(baseThreshold * Mathf.Pow(multiplier, rewardsGiven));
            }
        }
    }

    public static void UpdateLifes(int numLifes, int player)
    {
        if (IsSinglePlayer || IsTwoPlayerSimultaneous)
        {
            player1Lifes += numLifes;
        }
        else if (IsTwoPlayerTurnBased)
        {
            if (player == 1)
            {
                player1Lifes += numLifes;
            }
            else
            {
                player2Lifes += numLifes;
            }
        }
        
    }

    public static void SwitchPlayer()
    {
        // Cambiar jugador
        currentPlayer = (currentPlayer == 1) ? 2 : 1;

        // Notificar cambio de jugador
        OnPlayerSwitched?.Invoke();
    }

    public static void ResetScore()
    {
        player1Score = 0; // Restablecer puntaje de jugador 1
        player2Score = 0; // Restablecer puntaje de jugador 2
        rewardsGiven = 0;
        nextReward = 0;
    }

    public static void ResetLifes()
    {
        player1Lifes = 3; // Restablecer vidas de jugador 1
        player2Lifes = 3; // Restablecer vidas de jugador 2
    }

    public static bool IsGameOver()
    {
        if (IsSinglePlayer || IsTwoPlayerSimultaneous)
        {
            return player1Lifes <= 0;
        }
        else if (IsTwoPlayerTurnBased)
        {
            return player1Lifes <= 0 && player2Lifes <= 0;
        }
        return true;
    }
}
