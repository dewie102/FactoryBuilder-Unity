using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] Grid grid;
    [SerializeField] public Tilemap[] tilemaps;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {

    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return tilemaps[0].WorldToCell(worldPos);
    }

    public Vector3 CellToWorld(Vector3Int cellPos)
    {
        return tilemaps[0].CellToWorld(cellPos);
    }

    public bool IsCellValid(Vector3Int cellPos)
    {
        return true;
    }

    public void GetNeightbors()
    {
        
    }
}