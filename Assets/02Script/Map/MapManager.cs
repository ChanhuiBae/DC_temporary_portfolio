using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D.Animation;
public enum ChestType
{
    None,
    Wooden,
    Silver,
    Golden
}
public class MapManager : MonoBehaviour
{
    private Map data => GameManager.Inst.Exploration.map;
    private ExplorationManager manager;
    private Camera camera;

    private MapPainter mapPainter;

    private Vector2Int playerPosition;

    private CharacterController merchant;
    private ParticleSystem portal;
    private ParticleSystem red;
    private ParticleSystem blue;


    private ChestType chest;
    public ChestType CurrentChest
    {
        set => chest = ChestType.None;

        get => chest;
    }

    public void SetChest()
    {
        int range = Random.Range(0, 100);
        if(range < 10)
            chest = ChestType.Golden;
        else if(range < 50)
            chest = ChestType.Silver;
        else
            chest = ChestType.Wooden;
        mapPainter.DrawChest((int)chest);
    }

    private Animator monsterAnim;
    private SpriteLibrary monsterSprite;

    private ParticleSystem reconnaissance;

    private void Awake()
    {
        GameObject cam = GameObject.Find("MapCamera");
        if (cam == null || !cam.transform.TryGetComponent<Camera>(out camera))
        {
            Debug.Log("DragMap - Awake -  Camera");
        }
        if (!TryGetComponent<MapPainter>(out mapPainter))
        {
            Debug.Log("MapManager - Awake - mapPainter");
        }
        GameObject character = GameObject.Find("Merchant");
        if(character == null || !character.transform.TryGetComponent<CharacterController>(out merchant))
        {
            Debug.Log("MapManager - Awake - CharacterController");
        }
        else
        {
            merchant.gameObject.SetActive(false);
        }
        character = GameObject.Find("Monster");
        if(character == null || !character.TryGetComponent<Animator>(out monsterAnim))
        {
            Debug.Log("MapManager - Awake - Animator");
        }
        if(character == null || !character.transform.GetChild(0).TryGetComponent<SpriteLibrary>(out monsterSprite))
        {
            Debug.Log("MapManager - Awake - SpriteLibrary");
        }
        monsterAnim.gameObject.SetActive(false);
        GameObject p = GameObject.Find("Reconnaissance");
        if(p == null || !p.TryGetComponent<ParticleSystem>(out reconnaissance))
        {
            Debug.Log("MapManager - Awake - ParticleSystem");
        }
        else
        {
            reconnaissance.Stop();
        }
        p = GameObject.Find("WhitePortal");
        if(p == null || !p.transform.TryGetComponent<ParticleSystem>(out portal))
        {
            Debug.Log("MapManager - Awake - ParticleSystem");
        }
        p = GameObject.Find("RedPortal");
        if (p == null || !p.transform.TryGetComponent<ParticleSystem>(out red))
        {
            Debug.Log("MapManager - Awake - ParticleSystem");
        }
        p = GameObject.Find("BluePortal");
        if (p == null || !p.transform.TryGetComponent<ParticleSystem>(out blue))
        {
            Debug.Log("MapManager - Awake - ParticleSystem");
        }
        portal.Stop();
        red.Stop();
        blue.Stop();    
    }

    private void Start()
    {
        manager = (ExplorationManager)GameManager.Inst.manager;
        chest = ChestType.None;
        if (GameManager.Inst.CheckNewMap())
        {
            if (GameManager.Inst.CreateMap())
            {
                DrawMap();
            }
            else
            {
                StartCoroutine(CheckFinishMap());
            }
        }
        else
        {
            DrawMap();
        }
    }

    private void DrawMap()
    {
        mapPainter.DrawMap();
        playerPosition = data.Mapdata.position;
        camera.transform.position = new Vector3(playerPosition.x * 12 + 12, playerPosition.y * 12 + 6, camera.transform.position.z);
        GameManager.Inst.player.transform.position = new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, 0);
        manager.UseCameraController(true);
        manager.FadeOut(1f);
        GameManager.Inst.player.Control = true;
    }

    private IEnumerator CheckFinishMap()
    {
        while(data == null)
        {
            while(GameManager.Inst.Exploration == null)
            {
                yield return null;
            }
        }
        while (!data.Access)
        {
            yield return null;
        }

        DrawMap();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        Reconnaissance();
    }

    private bool Reconnaissance()
    {
        if (data.GetCurrentGone() == 1)
        {
            if (Random.Range(0, 100) < GameManager.Inst.Exploration.player.reconnaissance)
            {
                data.UpdateKnownMap();
                reconnaissance.transform.position = GameManager.Inst.player.transform.position;
                StartCoroutine(ShowIcons());
                return true;
            }
        }        
        return false;
    }

    private IEnumerator ShowIcons()
    {
        reconnaissance.Play();
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        mapPainter.DrawIcon();
    }

    public bool CheckSearched()
    {
        return data.CheckSearched();
    }

    public SearchEventType Search()
    {
        SearchEventType result = data.Search();
        if(result == SearchEventType.FindSecrect)
        {
            mapPainter.DrawMap();
        }
        return result;
    }

    public void BlownUp()
    {
        data.BlownUp();
        mapPainter.DrawMap();
    }

    public void Campfire(bool use)
    {
        if (use)
            mapPainter.props.DrawCampfire(data.GetPosition() * 12);
        else
            mapPainter.props.EraseCampfire();
    }

    private void SetMonster()
    {
        monsterAnim.gameObject.SetActive(true);
        Vector2Int position = data.GetPosition() * 12;
        monsterAnim.transform.position = new Vector3(position.x + 6, position.y + 6, 0);
        switch ((TileType)data.GetCurrentMap())
        {
            case TileType.Monster:
                monsterAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/Character");
                monsterSprite.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("SpriteLibrary/Monster");
                break;
            case TileType.MiddleBoss:
                monsterAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/Character");
                monsterSprite.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("SpriteLibrary/MiddleBoss");
                break;
            case TileType.Boss:
                Entity_Monster boss = GameManager.Inst.Exploration.GetBoss();
                monsterAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + boss.Animator);
                monsterSprite.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("SpriteLibrary/" + boss.Path);
                break;
        }
    }

    public void DeleteMonster()
    {
        monsterAnim.gameObject.SetActive(false);
    }

    public void AppearPortal()
    {
        Vector2Int position = data.GetPosition() * 12;
        portal.transform.position = new Vector3(position.x + 3, position.y + 7, 0);
        portal.Play();
        GameManager.Inst.player.Look = Direction.Left;
        StartCoroutine(AppearMerchant(portal.transform.position + new Vector3(0, -1, 0)));
    }
    public void AppearPortal(Color color)
    {
        Vector2Int position = data.GetPosition() * 12;
        if (color == Color.red)
        {
            red.transform.position = new Vector3(position.x + 4, position.y + 9, 0);
            red.Play();
        }
        else
        {
            blue.transform.position = new Vector3(position.x + 4, position.y + 5, 0);
            blue.Play();
        }
        MoveToPortal(color);
    }

    private void MoveToPortal(Color color)
    {
        if (color == Color.red)
        {
            GameManager.Inst.player.Move(red.transform.position, 1);
        }
        else
        {
            GameManager.Inst.player.Move(blue.transform.position, 1);
        }
    }

    private IEnumerator AppearMerchant(Vector3 position)
    {
        yield return YieldInstructionCache.WaitForSeconds(1);
        merchant.gameObject.SetActive(true);
        merchant.transform.position = position;
        merchant.transform.LeanMoveX(position.x + 2, 2);
        merchant.PlayAnimation(CharacterStateDC.Run);
        yield return YieldInstructionCache.WaitForSeconds(2);
        merchant.PlayAnimation(CharacterStateDC.Idle);
        ExplorationManager manager = (ExplorationManager)GameManager.Inst.manager;
        manager.MeetMerchant();
    }

    public void DrawMerchant(Vector2Int point)
    {
        merchant.gameObject.SetActive(true);
        merchant.transform.position = new Vector3(point.x + 4, point.y + 6, 0);
    }

    public void ReturnPlayerToPrevious()
    {
        data.SetGonefalse();
        switch (data.GetDirection())
        {
            case Direction.Left:
                MovePlayer(Direction.Right);
                break;
            case Direction.Right:
                MovePlayer(Direction.Left);
                break;
            case Direction.Up:
                MovePlayer(Direction.Down);
                break;
            case Direction.Down:
                MovePlayer(Direction.Up);
                break;
        }
    }

    public void MovePlayer(float time)
    {
        StartCoroutine(GoPlayerToCenter(time));
    }
    public void MovePlayer(Direction direction)
    {
        if(direction !=  Direction.None)
            StartCoroutine(SetPlayerMove(direction));
    }
    private IEnumerator GoPlayerToCenter(float time)
    {
        GameManager.Inst.player.Control = false;
        manager.UseCameraController(false);
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Run);
        GameManager.Inst.player.Move(new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 6, 0), time);
        yield return YieldInstructionCache.WaitForSeconds(time);
        Reconnaissance();
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Idle);
        manager.UseCameraController(true);
        GameManager.Inst.player.Control = true;
    }
    private IEnumerator SetPlayerMove(Direction direction)
    {
        if (data.MovePlayer(direction))
        {
            GameManager.Inst.player.Control = false;
            playerPosition = data.GetPosition();

            mapPainter.DrawMap();

            if ((direction == Direction.Left || direction == Direction.Right) 
                && direction != GameManager.Inst.player.Look)
            {
                GameManager.Inst.player.Look = direction;
            }

            manager.AddTurn();

            if (data.GetCurrentGone() == 1)
            {
                TileType type = (TileType)data.GetCurrentMap();
                switch (type)
                {
                    case TileType.Chest:
                        SetChest(); 
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.1f);
                        manager.Notify(type);
                        break;
                    case TileType.Boss:
                        GameManager.Inst.Exploration.SetSquadID(TileType.Boss);
                        yield return null;
                        SetMonster();
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.5f);
                        manager.Notify(type);
                        break;
                    case TileType.MiddleBoss:
                        GameManager.Inst.Exploration.SetSquadID(TileType.MiddleBoss);
                        SetMonster();
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.5f);
                        manager.Notify(type);
                        break;
                    case TileType.Monster:
                        GameManager.Inst.Exploration.SetSquadID(TileType.Monster);
                        SetMonster();
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.5f);
                        manager.Notify(type);
                        break;
                    case TileType.PositiveEvent:
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.5f);
                        manager.Notify(type);
                        break;
                    case TileType.NegativeEvent:
                        yield return null;
                        EnterPlayer(direction);
                        yield return YieldInstructionCache.WaitForSeconds(0.5f);
                        manager.Notify(type);
                        break;
                    default:
                        MovePlayer(0.6f); 
                        yield return YieldInstructionCache.WaitForSeconds(0.6f);
                        manager.UseCameraController(true);
                        GameManager.Inst.player.Control = true;
                        break;
                }
            }
            else if ((TileType)data.GetCurrentMap() == TileType.Merchant)
            {
                MovePlayer(0.6f);
                yield return YieldInstructionCache.WaitForSeconds(0.6f);
                manager.MeetMerchant();
            }
            else
            {
                MovePlayer(0.6f);
                yield return YieldInstructionCache.WaitForSeconds(0.6f);
                manager.UseCameraController(true);
                GameManager.Inst.player.Control = true;
            }
        }
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Idle);
    }

    private void EnterPlayer(Direction direction)
    {
        manager.UseCameraController(false);
        GameManager.Inst.player.PlayAnimation(CharacterStateDC.Run);
        switch (direction)
        {
            case Direction.Left:
                GameManager.Inst.player.transform.LeanMove(new Vector3(playerPosition.x * 12 + 9, playerPosition.y * 12 + 6, 0), 0.5f);
                break;
            case Direction.Right:
                GameManager.Inst.player.transform.LeanMove(new Vector3(playerPosition.x * 12 + 4, playerPosition.y * 12 + 6, 0), 0.5f);
                break;
            case Direction.Up:
                GameManager.Inst.player.transform.LeanMove(new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 4, 0), 0.5f);
                break;
            case Direction.Down:
                GameManager.Inst.player.transform.LeanMove(new Vector3(playerPosition.x * 12 + 6, playerPosition.y * 12 + 8, 0), 0.5f);
                break;
        }
    }

    public void DeleteEvent()
    {
        mapPainter.DeleteEvent();
    }

    public void StartMerchantEvent()
    {
        data.ClearMap();
        AppearPortal();
    }
}
