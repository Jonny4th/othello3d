using UnityEngine;

public class CellVisualController_3d : CellVisualController
{
    [SerializeField]
    private MeshRenderer m_Renderer;

    [SerializeField]
    private Vector2 Size = Vector2.one;

    private void Show() => m_Renderer.enabled = true;

    private void Hide() => m_Renderer.enabled = false;

    public override Vector2 GetSize() => Size;

    public override void ShowHintVisual()
    {
    }

    public override void HideHintVisual()
    {
    }

    protected override void SetWhiteToken()
    {
    }

    protected override void SetBlackToken()
    {
    }

    protected override void SetEmptyCell()
    {
    }
}