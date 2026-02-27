using Core;
using UnityEngine;

public abstract class CellVisualController : MonoBehaviour
{
    public abstract Vector2 GetSize();
    public abstract void ShowHintVisual();

    public abstract void HideHintVisual();

    public virtual void SetToken(Faction token)
    {
        Debug.Log($"Placing token: {token} at cell: {name}");

        switch(token)
        {
            case Faction.None:
                SetEmptyCell();
                break;
            case Faction.Black:
                SetBlackToken();
                break;
            case Faction.White:
                SetWhiteToken();
                break;
            default:
                Debug.LogWarning($"Unknown token type: {token}");
                break;
        }
    }

    protected abstract void SetWhiteToken();

    protected abstract void SetBlackToken();

    protected abstract void SetEmptyCell();
}
