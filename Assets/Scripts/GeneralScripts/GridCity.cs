using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    [Serializable]
    public class GridCity : MonoBehaviour
    {
        [Serializable]
        public class CityRegion
        {
            [Tooltip("Name of this region for your reference")]
            public string regionName;

            [Tooltip("Define the bounds of this region with a BoxCollider (editable in Scene view)")]
            public BoxCollider bounds;

            [Tooltip("Number of rows in this region's grid")]
            public int rows = 5;

            [Tooltip("Number of columns in this region's grid")]
            public int columns = 5;

            [Tooltip("Building prefabs to spawn in this region")]
            public GameObject[] buildingPrefabs;
        }

        [Header("City Regions (at least two by default)")]
        public List<CityRegion> regions = new List<CityRegion> {
            new CityRegion { regionName = "Region 1" },
            new CityRegion { regionName = "Region 2" }
        };

        [Tooltip("Delay between building grammar generations")]
        public float buildDelaySeconds = 0.1f;

        void Start()
        {
            Generate();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                DestroyChildren();
                Generate();
            }
        }

        void DestroyChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        void Generate()
        {
            foreach (var region in regions)
            {
                if (region.bounds == null || region.buildingPrefabs == null || region.buildingPrefabs.Length == 0)
                    continue;

                Bounds b = region.bounds.bounds;
                Vector3 min = b.min;

                float xStep = region.columns > 1 ? b.size.x / (region.columns - 1) : 0;
                float zStep = region.rows > 1 ? b.size.z / (region.rows - 1) : 0;

                for (int row = 0; row < region.rows; row++)
                {
                    for (int col = 0; col < region.columns; col++)
                    {
                        int idx = UnityEngine.Random.Range(0, region.buildingPrefabs.Length);
                        GameObject prefab = region.buildingPrefabs[idx];
                        GameObject newBuilding = Instantiate(prefab, transform);

                        // Position within region bounds
                        Vector3 pos = new Vector3(
                            min.x + col * xStep,
                            0,
                            min.z + row * zStep
                        );
                        newBuilding.transform.position = pos;

                        // Launch grammar if available
                        Shape shape = newBuilding.GetComponent<Shape>();
                        if (shape != null)
                        {
                            shape.Generate(buildDelaySeconds);
                        }
                    }
                }
            }
        }
    }
}
