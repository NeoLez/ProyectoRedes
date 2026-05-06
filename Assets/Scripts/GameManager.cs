using UnityEngine;

public class GameManager
{
    public static InputActions input { get; private set;}
    public static GridMap gridMap;
    public static Player player;
    public static MatchResult matchResult;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        input = new ();
        input.Enable();
        input.Player.Enable();
    }
    
}