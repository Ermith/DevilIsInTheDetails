using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public UDictionary<Item, int> ItemSpawnWeights;

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
                Random.Range(-camera.orthographicSize * camera.aspect, camera.orthographicSize * camera.aspect - 1),
                Random.Range(-camera.orthographicSize, camera.orthographicSize - 1),
                0);
            item.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        } while ((item.OverlapsAnyCell() || !item.InCameraView()) && nTries-- > 0);
        item.FixLetterRotation();
        
        // call item.PlayAppearing after a short delay
        item.Invoke("PlayAppearing", 0.1f);
        
        return item;
    }
}