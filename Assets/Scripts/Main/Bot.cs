using Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bot
{
    private GameRules m_GameRule;

    public event Action<Coordinates> OnBotMoveMade;

    public void SetGameRules(GameRules ruleBook)
    {
        m_GameRule = ruleBook;
    }

    public async void MakeDecision(BoardState boardState, Faction botColor)
    {
        var selectedMove = await Deciding(boardState, botColor);

        // Place the disc at the selected move
        OnBotMoveMade?.Invoke(new Coordinates(selectedMove.Item1, selectedMove.Item2));
        Debug.Log($"Bot placed a disc at {selectedMove}");
    }

    private async Task<(int,int)> Deciding(BoardState boardState, Faction botColor)
    {
        var legalMoves = m_GameRule.FindLegalMoves(boardState, botColor);

        if(legalMoves.Length == 0)
        {
            Debug.Log("No legal moves available for the bot.");
            OnBotMoveMade?.Invoke(new Coordinates(-1, -1));
        }

        // Randomly select a move from the legal moves
        var randomIndex = Random.Range(0, legalMoves.Length);
        return legalMoves[randomIndex];
    }
}
