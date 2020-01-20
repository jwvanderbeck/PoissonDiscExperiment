using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// References:
// http://devmag.org.za/2009/05/03/poisson-disk-sampling/
// http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
// http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
// Code released into the public domain

public class Poisson : MonoBehaviour
{
    public int density = 30;
    public int maxPoints = 1000;
    public Vector2 size;
    public Texture2D image;
    public float minRadius = 10f;
    public float maxRadius = 20f;
    public float blackCutoff = 0.1f;
    public float whiteCutoff = 0.7f;
    public int seed;
    public GameObject prefab;
    public Color gizmoColor = Color.green;

    private List<Vector2> processList;
    private List<Vector2> samplePoints;
    private Grid2 grid;
    private float radius;
    private float radius2;

    public void CreatePoints()
    {
        Debug.Log("Create Points");
        Random.InitState((int)DateTime.Now.Ticks);
        processList = new List<Vector2>();
        samplePoints = new List<Vector2>();

        radius = minRadius;
        radius2 = radius * radius;
        
        // build a grid to work with
        if (image != null)
        {
            size.x = image.width;
            size.y = image.height;
            grid = new Grid2(size.x, size.y, maxRadius/ Mathf.Sqrt(2));
        }
        else
        {
            grid = new Grid2(size.x, size.y, radius/ Mathf.Sqrt(2));
        }
        
        // Start with an initial random point
        var initialPoint = new Vector2(Random.Range(0, size.x), Random.Range(0, size.y));
        Debug.Log($"Initial Point {initialPoint.x},{initialPoint.y}");
        processList.Add(initialPoint);
        samplePoints.Add(initialPoint);
        grid.Add(initialPoint);
        
        // Generate more points
        while (processList.Count > 0 && processList.Count < maxPoints)
        {
            // pick a random point to start from
            Vector2 point = RandomPointFromProcessList();
            
            // generate new points around that one, based on density
            for (int i = 0; i < density; i++)
            {
                Vector2 newPoint = GenerateRandomPointAroundPoint(point);
                // Check if that point is not too close to another (and is inside out bounds)
                if (grid.InsideBounds(newPoint) && IsFarEnoughFromAllPoints(newPoint))
                {
                    // This is a good point, so add it to our grid and lists
                    grid.Add(newPoint);
                    processList.Add(newPoint);
                    samplePoints.Add(newPoint);
                }
            }

            processList.Remove(point);
        }
        Debug.Log($"count {processList.Count}");
        // samplePoints now contains our final points
        if (prefab != null)
        {
            foreach (Vector2 samplePoint in samplePoints)
            {
                GameObject go = Instantiate(prefab, samplePoint, Quaternion.identity, gameObject.transform);
                go.name = $"Point {(int)samplePoint.x},{(int)samplePoint.y}";
            }
        }
    }

    public void Clear()
    {
        int numChildren = transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private Vector2 GenerateRandomPointAroundPoint(Vector2 point)
    {
        float r1 = Random.value;
        float r2 = Random.value;
        float r = maxRadius * (r1 + 1);
        float angle = 2 * Mathf.PI * r2;
        float newX = point.x + r * Mathf.Cos(angle);
        float newY = point.y + r * Mathf.Sin(angle);
        
        return new Vector2(newX, newY);
    }

    private bool IsFarEnoughFromAllPoints(Vector2 point)
    {
        Vector2 cell = grid.PointToCell(point);
        int columnMin = Mathf.Max((int)cell.x - 2, 0);
        int rowMin = Mathf.Max((int)cell.y - 2, 0);
        int columnMax = Mathf.Min((int)cell.x + 2, grid.MaxColumn());
        int rowMax = Mathf.Min((int)cell.y + 2, grid.MaxRow());
        
        
        for (int y = rowMin; y <= rowMax; y++) {
            for (int x = columnMin; x <= columnMax; x++)
            {
                List<Vector2> samples = grid.GetPointsInCell(new Vector2(x, y));
                if (samples != null)
                {
                    foreach (Vector2 sample in samples)
                    {
                        float distance = Vector2.Distance(sample, point);
                        float minDistance;
                        if (image != null)
                        {
                            Color pixelColor = image.GetPixel((int)sample.x, (int)sample.y);
                            if (pixelColor.grayscale <= blackCutoff) pixelColor = Color.black;
                            if (pixelColor.grayscale >= whiteCutoff) pixelColor = Color.white;
                            float greyscale = 1 - pixelColor.grayscale;
                            minDistance = minRadius + greyscale * (maxRadius - minRadius);
                        }
                        else minDistance = minRadius;
                        if (distance < minDistance) return false;
                    }
                }
            }
        }

        return true;
    }

    private Vector2 RandomPointFromProcessList()
    {
        int index = Random.Range(0, processList.Count);

        Vector2 point = processList[index];
        processList.RemoveAt(index);

        return point;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Color c = Gizmos.color;
        Gizmos.color = gizmoColor;
        var center = new Vector3(size.x / 2f, size.y / 2f);
        center = transform.TransformPoint(center);
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y));
        Gizmos.color = c;
    }
}
