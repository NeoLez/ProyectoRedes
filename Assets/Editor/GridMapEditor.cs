using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
    private GridMap grid;

    private void OnEnable()
    {
        grid = (GridMap)target;
    }

    private void OnSceneGUI()
    {
        DrawGrid();
        Event e = Event.current;

        // Only react to mouse click
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPos = hit.point;

                Vector2Int tilePos = grid.GetTilePosition(worldPos);

                Undo.RecordObject(grid, "Toggle Tile");

                Debug.Log(tilePos);
                grid.ToggleTile(tilePos);

                EditorUtility.SetDirty(grid);

                e.Use();
            }
        }
    }

    private void DrawGrid()
    {
        Handles.color = Color.red;
        
        foreach (var p in grid._posStates)
        {
            if (p.state == TileState.Free) continue;
            Vector3 pos = new Vector3(p.position.x, 0, p.position.y);
            Handles.DrawWireCube(pos, Vector3.one);
        }
    }
}