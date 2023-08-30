using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] Vector2Int gridSize;

    [Header("Tile Settings")]
    [SerializeField] float outerSize = 1f;
    [SerializeField] float innerSize = 1f;
    [SerializeField] float maxHeight = 1f;
    [SerializeField] bool isFlatTopped;
    [SerializeField] Material[] materials;

    [SerializeField] float gridOffset;
    [SerializeField] float noiseDetail;

    [Header("Water")]
    [SerializeField] float waterLevel = 0.5f;
    [SerializeField] float maskRadius = 20f;
    [SerializeField] bool mask = false;
    [SerializeField] Material waterMaterial;
    

    public void UpdateGrid()
    {
        Debug.Log("Update Grid");
        ClearHexagons();
        LayoutGrid();
    }

    private void LayoutGrid()
    {
        // Create mask

        GameObject hexMask = new GameObject($"Mask", typeof(HexTile));
        HexTile maskTile = hexMask.GetComponent<HexTile>();
        maskTile.transform.parent = transform;

        if (mask)
        {
            maskTile.outerSize = maskRadius;
            maskTile.innerSize = 0f;
            maskTile.isFlatTopped = isFlatTopped;
            maskTile.height = waterLevel;
            maskTile.material = waterMaterial;
            maskTile.DrawMesh();
            maskTile.AddCollider();
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                
                GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexTile));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y)) 
                    + new Vector3(-((maskRadius) + 0),
                    0,
                    (maskRadius) + 0);

                if (mask)
                {
                    if (!IsPointInsideMask(tile.transform.position))
                    {
                        DestroyImmediate(tile);
                        continue;
                    }
                }


                HexTile tileRenderer = tile.GetComponent<HexTile>();
                tileRenderer.isFlatTopped = isFlatTopped;
                tileRenderer.outerSize = outerSize;
                tileRenderer.innerSize = innerSize;

                // Add a random value to height
                //float randomHeight = Random.Range(0, maxHeight);

                // Random value with Perlin Noise

                float randomHeight = GenerateHeight(x, y, noiseDetail, tile) * maxHeight ;
                Debug.Log($"{x}, {y} tile: Height: {randomHeight}");

                tileRenderer.height = randomHeight;

                // Asign material based on height.
                // Mapping the range.
                int materialIndex = Mathf.FloorToInt( Mathf.Lerp(0, materials.Length, randomHeight/maxHeight) );

                Debug.Log($"Material Index {materialIndex}");
                tileRenderer.material = materials[materialIndex];
                //tileRenderer.material = material;

                tileRenderer.DrawMesh();

                tile.transform.parent = transform;
            }
        }

        // Adjust mask size to embrace all tiles
        maskTile.transform.localScale = new Vector3(1.05f, 1.0f, 1.05f);
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize; // radius.

        if (!isFlatTopped)
        {
            // offset the hexagons on odd row.
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * size; //The height of an equilateral triangle with sides of length 2 equals the square root of 3.
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f); // 3/4 of the height.

            offset = shouldOffset ? width / 2 : 0;

            // Place tile based on the currentPosition
            xPosition = (column * horizontalDistance) + offset;
            // Place tile based on the currentRow
            yPosition = row * verticalDistance;

            
        }
        else
        {
            shouldOffset = column % 2 == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2 : 0;

            xPosition = column * horizontalDistance;
            yPosition = (row * verticalDistance) - offset;


        }
        return new Vector3(xPosition, 0, -yPosition);
        //return new Vector3();
    }

    private void ClearHexagons()
    {
        HexTile[] hexagons = GetComponentsInChildren<HexTile>();
        foreach (HexTile tile in hexagons)
        {
            if (Application.isPlaying)
            {
                Destroy(tile.gameObject);
            }
            else
            {
                DestroyImmediate(tile.gameObject);
            }
            
        }
    }

    private float GenerateHeight(int x, int y, float detailScale, GameObject tile)
    {
        float xNoise = (x + tile.transform.position.x) / detailScale;
        float yNoise = (y + tile.transform.position.y) / detailScale;
        
        return Mathf.PerlinNoise(xNoise, yNoise);

    }

    private bool IsPointInsideMask(Vector3 pos)
    {
        Ray ray = new Ray(new Vector3(pos.x, waterLevel + 1, pos.z), Vector3.down);

        if (Physics.Raycast(ray))
        {
            Debug.Log("THis point hit something");
            return true;
        }
        Debug.Log("Not THis point hit something");
        return false;
    }
    
    
}
