using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    public UDictionary<Item, int> ItemSpawnWeights;

    public bool SpawningItems
    {
        get => !GameDirector.GameDirectorInstance.IsPaused;
    }

    public int LooseItems = 0;

    public int MaxLooseItems = 3;

    public double PerSecondExpectedNewItems = 0.5f;

    public double InitialNewItemsBoost = 2f;

    public int TotalItemsSpawned { get; private set; }

    private Item PickItem()
    {
        int totalWeight = 0;
        foreach (var item in ItemSpawnWeights)
        {
            totalWeight += item.Value;
        }

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var item in ItemSpawnWeights)
        {
            randomWeight -= item.Value;
            if (randomWeight <= 0)
            {
                return item.Key;
            }
        }

        return null;
    }

#if DEBUG
    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 0, 100, 20), "Spawn Item"))
        {
            SpawnItem();
        }
    }
#endif

    public Item SpawnItem()
    {
        Item item = Instantiate(PickItem());
        Camera camera = Camera.main;
        int nTries = 30;
        do
        {
            // random position in the camera view
            item.transform.position = new Vector3(
                Random.Range(-camera.orthographicSize * camera.aspect, camera.orthographicSize * camera.aspect),
                Random.Range(-camera.orthographicSize, camera.orthographicSize),
                0);
            item.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        } while ((item.OverlapsAnyCell() || !item.InCameraView()) && nTries-- > 0);
        item.FixLetterRotation();
        
        // call item.PlayAppearing after a short delay
        item.Invoke("PlayAppearing", 0.1f);

        TotalItemsSpawned++;
        
        return item;
    }

    public void Update()
    {
        if (GameDirector.GameDirectorInstance.IsPaused)
            return;
        double expected = PerSecondExpectedNewItems;
        if (TotalItemsSpawned < MaxLooseItems)
            expected += (InitialNewItemsBoost - expected) * (MaxLooseItems - TotalItemsSpawned) / MaxLooseItems;
        double prob = expected * Time.deltaTime;
        if (SpawningItems && LooseItems < MaxLooseItems && Random.Range(0f, 1f) < prob)
        {
            SpawnItem();
        }
    }
}