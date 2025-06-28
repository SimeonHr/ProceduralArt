using UnityEngine;
using System.Collections.Generic;

namespace Demo
{
    public class SimpleBuilding : Shape
    {
        public int buildingHeight = -1;
        public float stockHeight = 1;
        public int maxHeight = 5;
        public int minHeight = 1;

        public GameObject[] stockPrefabs;
        public GameObject[] roofPrefabs;
        public GameObject[] neonPrefabs;

        public int maxNeonCount = 2;

        int stockNumber = 0;
        int[] neonTargetStocks;
        bool hasNeon = false;

        public void Initialize(
            int pBuildingHeight,
            float pStockHeight,
            int pStockNumber,
            GameObject[] pStockPrefabs,
            GameObject[] pRoofPrefabs,
            GameObject[] pNeonPrefabs,
            int[] pNeonTargets = null,
            bool pHasNeon = false
        )
        {
            buildingHeight = pBuildingHeight;
            stockHeight = pStockHeight;
            stockNumber = pStockNumber;
            stockPrefabs = pStockPrefabs;
            roofPrefabs = pRoofPrefabs;
            neonPrefabs = pNeonPrefabs;
            neonTargetStocks = pNeonTargets;
            hasNeon = pHasNeon;
        }

        GameObject ChooseRandom(GameObject[] choices)
        {
            int index = Random.Range(0, choices.Length);
            return choices[index];
        }

        protected override void Execute()
        {
            if (buildingHeight < 0)
            {
                buildingHeight = Random.Range(minHeight, maxHeight + 1);

                
                if (neonPrefabs != null && neonPrefabs.Length > 0)
                {
                    hasNeon = true;
                    int count = Mathf.Min(maxNeonCount, buildingHeight);
                    HashSet<int> uniqueTargets = new HashSet<int>();

                    // Ensure at least one
                    uniqueTargets.Add(Random.Range(0, buildingHeight));

                    // Try adding more unique neon targets
                    while (uniqueTargets.Count < count)
                    {
                        uniqueTargets.Add(Random.Range(0, buildingHeight));
                    }

                    neonTargetStocks = new int[uniqueTargets.Count];
                    uniqueTargets.CopyTo(neonTargetStocks);
                }
            }

            if (stockNumber < buildingHeight)
            {
                GameObject newStock = SpawnPrefab(ChooseRandom(stockPrefabs));

                // === NEON SIGN ATTACHMENT ===
                if (hasNeon && neonTargetStocks != null && System.Array.Exists(neonTargetStocks, n => n == stockNumber) && neonPrefabs.Length > 0)
                {
                    GameObject neon = Instantiate(ChooseRandom(neonPrefabs), newStock.transform);

                    // Position the neon on a random wall side
                    int side = Random.Range(0, 4); // 0=+Z, 1=-Z, 2=+X, 3=-X
                    Vector3 localPos = Vector3.zero;
                    Quaternion localRot = Quaternion.identity;

                    float wallOffset = 1.01f;
                    float xOffset = Random.Range(-0.3f, 0.3f);
                    float yOffset = Random.Range(0.2f, 0.8f);

                    switch (side)
                    {
                        case 0: localPos = new Vector3(xOffset, yOffset, wallOffset); localRot = Quaternion.Euler(90, 0, 0); break;
                        case 1: localPos = new Vector3(xOffset, yOffset, -wallOffset); localRot = Quaternion.Euler(90, 180, 0); break;
                        case 2: localPos = new Vector3(wallOffset, yOffset, xOffset); localRot = Quaternion.Euler(90, 90, 0); break;
                        case 3: localPos = new Vector3(-wallOffset, yOffset, xOffset); localRot = Quaternion.Euler(90, -90, 0); break;
                    }

                    neon.transform.localPosition = localPos;
                    neon.transform.localRotation = localRot;
                    neon.transform.localScale = Vector3.one * Random.Range(0.15f, 0.3f);
                }

                // Continue with the next floor
                SimpleBuilding next = CreateSymbol<SimpleBuilding>("stock", new Vector3(0, stockHeight, 0));
                next.Initialize(buildingHeight, stockHeight, stockNumber + 1,
                    stockPrefabs, roofPrefabs, neonPrefabs,
                    neonTargetStocks, hasNeon);
                next.Generate(buildDelay);
            }
            else
            {
                SpawnPrefab(ChooseRandom(roofPrefabs));
            }
        }
    }
}
