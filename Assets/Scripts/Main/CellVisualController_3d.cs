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

    private const string FactionTag = "IsWhite";
    private const string OccupiedTag = "IsOccupied";
    private const string PlaceTriggerTag = "Place";

    private bool m_IsOccupied = false;

    private void Awake()
    {
        m_Animator.SetBool(OccupiedTag, m_IsOccupied);
    }

    public override void ShowHintVisual()
    {
        m_CellRenderer.material = m_HintColor;
    }

    public override void HideHintVisual()
    {
        m_CellRenderer.material = m_DefaultColor;
    }

    protected override void SetWhiteToken()
    {
        SetFaction(true);
    }

    protected override void SetBlackToken()
    {
        SetFaction(false);
    }

    protected override void SetEmptyCell()
    {
        m_IsOccupied = false;
        m_Animator.SetBool(OccupiedTag, m_IsOccupied);
        Hide();
    }

    private void SetFaction(bool isWhite)
    {
        HideHintVisual();
        Show();

        m_Animator.SetBool(FactionTag, isWhite);

        if(!m_IsOccupied)
        {
            m_Animator.SetTrigger(PlaceTriggerTag);
        }

        m_IsOccupied = true;
        m_Animator.SetBool(OccupiedTag, m_IsOccupied);
    }
}