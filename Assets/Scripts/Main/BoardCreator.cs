using UnityEngine;

public enum BuildMode
{
    D2 , D3
}

public class BoardCreator<T> where T : MonoBehaviour, ICell
{
    private T? m_Prototype;
    private int? m_Width;
    private int? m_Height;
    private Transform? m_Parent;

    public BoardCreator<T> SetDimension(int width, int height)
    {
        m_Width = width;
        m_Height = height;
        return this;
    }

    public BoardCreator<T> SetPrototype(T protptype)
    {
        m_Prototype = protptype;
        return this;
    }

    public BoardCreator<T> SetParent(Transform parent)
    {
        m_Parent = parent;
        return this;
    }

    public T[,] CreateBoard(BuildMode buildMode)
    {
        if(m_Prototype == null)
        {
            throw new MissingComponentException($"Prototyp not set. Use {nameof(SetPrototype)} to set prototype.");
        }

        if(m_Parent == null)
        {
            throw new MissingComponentException($"Parent transform not set. Use {nameof(SetParent)} to set parent.");
        }

        if(m_Height == null || m_Width == null)
        {
            throw new MissingComponentException($"Dimension not set. Use {nameof(SetDimension)} to set dimension.");
        }

        var cells = new T[m_Width.Value, m_Height.Value];

        Vector2 size = m_Prototype.GetSize();

        float startPosX = size.x * m_Width.Value / 2f - 0.5f;
        float startPosY = size.y * m_Height.Value / 2f - 0.5f;

        for(int x = 0; x < m_Width; x++)
        {
            for(int y = 0; y < m_Height; y++)
            {
                var parameter = new InstantiateParameters()
                {
                    parent = m_Parent,
                    worldSpace = false
                };

                var posX = x * size.x - startPosX;
                var posY = y * size.y - startPosY;
                Vector3 pos = buildMode == BuildMode.D2 ? new Vector3(posX, posY, 0) : new Vector3(posX, 0, posY);
                var cell = Object.Instantiate(m_Prototype, pos, Quaternion.identity, parameter);

                cell.SetCoordinates(x, y);
                cell.name = $"Cell_{x}_{y}";
                cells[x, y] = cell;
            }
        }

        return cells;
    }
}
