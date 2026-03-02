using Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class OthelloBot
{
    public event Action<Coordinates> OnBotMoveMade;
    public abstract Faction Faction { get; }
    public abstract void SetFaction(Faction faction);
    public abstract void SetGameRules(GameRules ruleBook);
    public virtual async void MakeDecision(BoardState boardState)
    {
        await Task.Delay(1000);
    }

    protected void InvokeOnBotMoveMade(Coordinates coordinates)
    {
        OnBotMoveMade?.Invoke(coordinates);
    }
}

public class Bot_RandomMove : OthelloBot
{
    private GameRules m_GameRule;
    public override Faction Faction => m_Faction;
    private Faction m_Faction;

    public override void SetFaction(Faction faction)
    {
        m_Faction = faction; 
    }

    public override void SetGameRules(GameRules ruleBook)
    {
        m_GameRule = ruleBook;
    }

    public override async void MakeDecision(BoardState boardState)
    {
        var selectedMove = await Deciding(boardState);

        // Place the disc at the selected move
        InvokeOnBotMoveMade(new Coordinates(selectedMove.Item1, selectedMove.Item2));
        Debug.Log($"Bot placed a disc at {selectedMove}");
    }

    private async Task<(int,int)> Deciding(BoardState boardState)
    {
        var legalMoves = m_GameRule.FindLegalMoves(boardState, m_Faction);

        if(legalMoves.Length == 0)
        {
            Debug.Log("No legal moves available for the bot.");
            InvokeOnBotMoveMade(new Coordinates(-1, -1));
        }

        // Randomly select a move from the legal moves
        var randomIndex = Random.Range(0, legalMoves.Length);
        return legalMoves[randomIndex];
    }
}
