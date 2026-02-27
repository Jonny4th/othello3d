using UnityEngine;

public class CellVisualController_3d : CellVisualController
{
    [SerializeField]
    private MeshRenderer m_PieceRenderer;

    [SerializeField]
    private MeshRenderer m_CellRenderer;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private Vector2 m_Size = Vector2.one;

    [SerializeField]
    private Material m_DefaultColor;

    [SerializeField]
    private Material m_HintColor;

    private void Show() => m_PieceRenderer.enabled = true;

    private void Hide() => m_PieceRenderer.enabled = false;

    public override Vector2 GetSize() => m_Size;
    
    public override void ShowHintVisual()
    {
        m_CellRenderer.material = m_HintColor;
    }

    public override void HideHintVisual()
    {
        m_CellRenderer.material = m_HintColor;
    }

    protected override void SetWhiteToken()
    {
        Show();
    }

    protected override void SetBlackToken()
    {
        Show();
    }

    protected override void SetEmptyCell()
    {
        Hide();
    }
}