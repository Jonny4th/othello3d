using UnityEngine;

public class CellVisualController_2d : CellVisualController
{
    [SerializeField]
    SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    Color m_HintColor = Color.yellow;

    public override Vector2 GetSize() => m_SpriteRenderer.bounds.size;

    public override void ShowHintVisual()
    {
        m_SpriteRenderer.gameObject.SetActive(true);
        m_SpriteRenderer.color = m_HintColor;
    }

    public override void HideHintVisual()
    {
        m_SpriteRenderer.gameObject.SetActive(false);
    }

    public override void SetWhiteToken()
    {
        m_SpriteRenderer.gameObject.SetActive(true);
        m_SpriteRenderer.color = Color.white;
    }

    public override void SetBlackToken()
    {
        m_SpriteRenderer.gameObject.SetActive(true);
        m_SpriteRenderer.color = Color.black;
    }

    public override void SetEmptyCell()
    {
        m_SpriteRenderer.gameObject.SetActive(false);
    }
}
