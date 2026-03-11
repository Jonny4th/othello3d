using Core;
using Main.Models;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameplayManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private BoardStateEvent m_EndGameEvent;

    [SerializeField]
    private GameParametersEvent m_StartGameEvent;

    [SerializeField]
    private GameplayUiDataEvent m_HudDataEvent;

    [SerializeField]
    private GameplayScoreEvent m_ScoreEvent;

    [SerializeField]
    private FactionEvent m_TurnSwitchEvent;

    [SerializeField]
    private BooleanEventChannel m_HudActivateEvent;

    [Header("Gameplay")]
    [SerializeField]
    private Cell m_CellPrototype;

    [SerializeField]
    private int m_BoardWidth = 8;

    [SerializeField]
    private int m_BoardHeight = 8;

    [SerializeField]
    private Transform m_BoardParent;

    [Header("Debug")]
    [SerializeField]
    private bool m_IsDebug;
    [SerializeField]
    private GameParameters m_DebugConfig;

    private Cell[,] m_Cells;
    private bool m_IsPlaying = false;

    private Coroutine m_Game;
    private GameRules m_GameRules = new();
    (int, int)[] m_CurrentLegalMoves;

    private OthelloBot m_Bot = null;
    private bool m_IsBotTurn = false;

    public bool IsBlackTurn = true; // Track whose turn it is.

#if UNITY_EDITOR
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 20), "Start Game"))
        {
            StartGame();
        }

        if(GUI.Button(new Rect(0, 25, 100, 20), "End Game"))
        {
            EndGame();
        }
    }

    [ContextMenu("Debug/Start Game")]
    public void StartGame()
    {
        StartGame(m_DebugConfig);
    }

    [ContextMenu("Debug/End Game")]
    public void EndGame()
    {
        BeginEndGameProcess(new()
        {
            Cells = ConvertCellsToTokenMap(m_Cells)
        });
    }
#endif

    private void Awake()
    {
        m_StartGameEvent.AddListener(StartGame);
    }

    public async void StartGame(GameParameters parameters)
    {
        if(m_Game != null || m_IsPlaying)
        {
            Debug.LogWarning("Cannot start another game while a session is running.");
            return;
        }

        //reset
        m_IsPlaying = true;
        if(m_Cells != null)
        {
            foreach(var cell in m_Cells)
            {
                cell.SetToken(Faction.None);
            }
        }

        //m_Game = StartCoroutine(RunGameLogic(parameters));

        if(parameters.IsBotUsed)
        {
            m_Bot = new Bot_RandomMove();
            m_Bot.SetFaction(parameters.PlayerFaction == Faction.Black ? Faction.White : Faction.Black);
            m_Bot.SetGameRules(m_GameRules);
            m_Bot.OnBotMoveMade += HandleBotMoveMade;
        }

        Faction playerFaction = parameters.PlayerFaction;
        //set UIs
        GameplayUIData uiData = new()
        {
            BlackPlayerName = parameters.BlackPlayer,
            WhitePlayerName = parameters.WhitePlayer,
        };

        m_HudDataEvent.Invoke(uiData);
        m_ScoreEvent.Invoke((2, 2));
        m_TurnSwitchEvent.Invoke(Faction.Black);
        m_HudActivateEvent.Invoke(true);

        await CreateBoard();
        IsBlackTurn = true;

        TurnPhase();
    }

    public void TurnPhase()
    {
        var currentPlayer = IsBlackTurn ? Faction.Black : Faction.White;
        Debug.Log($"Turn Phase: {currentPlayer} starts");
        m_TurnSwitchEvent.Invoke(currentPlayer);

        m_IsBotTurn = m_Bot != null && currentPlayer == m_Bot.Faction && m_IsPlaying;

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = new(-1, -1),
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        m_CurrentLegalMoves = m_GameRules.FindLegalMoves(boardState, currentPlayer);

        if(m_CurrentLegalMoves.Length == 0) SwitchTurn();
        
        if(m_Bot != null && m_IsBotTurn && m_IsPlaying)
        {
            Debug.Log($"Turn Phase: Bot Making Decision");
            m_Bot.MakeDecision(boardState);
            return;
        }

        Debug.Log($"Turn Phase: Player Making Decision");
        foreach((int x, int y) in m_CurrentLegalMoves)
        {
            m_Cells[x, y].ShowHintVisual();
        }
    }

    public void BeginEndGameProcess(BoardState boardState)
    {
        Debug.Log($"Turn Phase: Game ends.");

        m_IsPlaying = false;
        m_IsBlockingPlayerInput = false;

        if(m_Bot != null) m_Bot.OnBotMoveMade -= HandleBotMoveMade;

        (int a, int b) = m_GameRules.CountTokens(boardState);
        var winner = a > b ? Faction.Black : Faction.White;
        if(a == b) { winner = Faction.None; }

        m_EndGameEvent.Invoke(boardState);

        m_HudActivateEvent.Invoke(false);
    }

    private IEnumerator RunGameLogic(GameParameters parameters)
    {
        //pre-game
        if(parameters.IsBotUsed)
        {
            m_Bot = new Bot_RandomMove();
            m_Bot.SetFaction(parameters.PlayerFaction == Faction.Black ? Faction.White : Faction.Black);
            m_Bot.SetGameRules(m_GameRules);
            m_Bot.OnBotMoveMade += HandleBotMoveMade;
        }

        yield return CreateBoard();

        IsBlackTurn = true;

        Faction playerFaction = parameters.PlayerFaction;
        //set UIs
        GameplayUIData uiData = new()
        {
            BlackPlayerName = parameters.BlackPlayer,
            WhitePlayerName = parameters.WhitePlayer,
        };

        m_HudDataEvent.Invoke(uiData);
        m_ScoreEvent.Invoke((2, 2));
        m_TurnSwitchEvent.Invoke(Faction.Black);
        m_HudActivateEvent.Invoke(true);

        //run game
        while(m_IsPlaying)
        {
            yield return null;
        }

        //post game
        BeginEndGameProcess(new());
    }

    private async Task CreateBoard()
    {
        if(m_Cells == null)
        {
            BoardCreator<Cell> creator = new();
            creator.SetParent(m_BoardParent)
                .SetPrototype(m_CellPrototype)
                .SetDimension(m_BoardWidth, m_BoardHeight);

            m_Cells = creator.CreateBoard(BuildMode.D3);
        }

        var initialState = new BoardStateCreator().Build();

        for(int i = 0; i < m_BoardWidth; i++)
        {
            for(int j = 0; j < m_BoardHeight; j++)
            {
                m_Cells[i, j].SetToken(initialState.Cells[i, j]);
            }
        }

        await Awaitable.WaitForSecondsAsync(1);

        foreach(var cell in m_Cells)
        {
            cell.OnCellClicked += OnCellClicked;
        }
    }

    private void ShowHint(BoardState boardState, Faction player)
    {
        m_CurrentLegalMoves = m_GameRules.FindLegalMoves(boardState, player);

        if(m_CurrentLegalMoves.Length == 0) SwitchTurn();

        foreach((int x, int y) in m_CurrentLegalMoves)
        {
            m_Cells[x, y].ShowHintVisual();
        }
    }

    private void OnCellClicked(ICell cell)
    {
        Debug.Log("click");
        if(m_Bot != null && m_IsBotTurn) return; //block player from clicking while bot is making a move
        if(m_IsBlockingPlayerInput) return;
        ProcessMove(cell);
    }

    private bool m_IsBlockingPlayerInput = false;

    private async void ProcessMove(ICell cell)
    {
        Debug.Log($"Turn Phase: Process Move");

        if(cell.CurrentToken != Faction.None)
        {
            Debug.LogWarning("Occupied tile cannot be processed.");
            return;
        }

        if(!m_CurrentLegalMoves.Contains(cell.Coordinates.ToTuple()))
        {
            Debug.LogWarning("This tile is not a legal move.");
            return;
        }

        m_IsBlockingPlayerInput = true;

        foreach(var c in m_Cells) c.HideHintVisual();

        var token = IsBlackTurn ? Faction.Black : Faction.White;
        m_Cells[cell.Coordinates.X, cell.Coordinates.Y].SetToken(token);
        await Awaitable.WaitForSecondsAsync(1f);

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = cell.Coordinates,
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        var updatedBoardState = await Resolve(boardState);

        if(m_GameRules.IsGameOver(updatedBoardState))
        {
            BeginEndGameProcess(updatedBoardState);
            return;
        }

        m_IsBlockingPlayerInput = false;
        SwitchTurn();
    }

    private void HandleBotMoveMade(Coordinates coordinates)
    {
        Debug.Log($"Bot made a move at {coordinates}");
        if(coordinates.X < 0 || coordinates.Y < 0 || coordinates.X >= m_BoardWidth || coordinates.Y >= m_BoardHeight)
        {
            Debug.LogWarning("Bot made an invalid move. Ignoring.");
            SwitchTurn();
            return;
        }

        ICell cell = m_Cells[coordinates.X, coordinates.Y];
        ProcessMove(cell);
    }

    private void SwitchTurn()
    {
        Debug.Log($"Turn Phase: Switch turn.");
        IsBlackTurn = !IsBlackTurn;
        m_IsBotTurn &= false;
        TurnPhase();
    }

    private async Task<BoardState> Resolve(BoardState boardState)
    {
        Debug.Log($"Turn Phase: Resolving");

        foreach((int x, int y) in m_GameRules.GetAllOutflankedTokens(boardState))
        {
            Debug.Log($"{x}{y}");
            m_Cells[x, y].SetToken(boardState.LastPlacedDisc);
            boardState.Cells = ConvertCellsToTokenMap(m_Cells);
            (int a, int b) = m_GameRules.CountTokens(boardState);
            m_ScoreEvent.Invoke((a,b));

            await Awaitable.WaitForSecondsAsync(0.25f);
        }

        await Awaitable.WaitForSecondsAsync(0.5f);

        return boardState;
    }

    private IEnumerator ResolvePlayerAction(BoardState boardState)
    {
        yield return null;
    }

    public Faction[,] ConvertCellsToTokenMap(ICell[,] cells)
    {
        var lenX = cells.GetLength(0);
        var lenY = cells.GetLength(1);

        var result = new Faction[lenX, lenY];

        foreach(var cell in cells)
        {
            result[cell.Coordinates.X, cell.Coordinates.Y] = cell.CurrentToken;
        }

        return result;
    }
}
