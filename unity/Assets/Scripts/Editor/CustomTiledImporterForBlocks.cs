using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
class CustomTiledImporterForBlocks : Tiled2Unity.ICustomTiledImporter
{

    public void HandleCustomProperties(UnityEngine.GameObject gameObject,
        IDictionary<string, string> props)
    {
        // Does this game object have a spawn property?
        if (!props.ContainsKey("spawn"))
            return;

        // Are we spawning an Appearing Block?
        if (props["spawn"] != "AppearingBlock")
            return;

        // Load the prefab assest and Instantiate it
        string prefabPath = "Assets/Prefabs/AppearingBlock.prefab";
        UnityEngine.Object spawn = 
            AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        if (spawn != null)
        {
            // Remove old tile object
            Transform oldTileObject = gameObject.transform.Find("TileObject");
            if (oldTileObject != null)
            {
                GameObject.DestroyImmediate(oldTileObject.gameObject);
            }

            // Replace with new spawn object
            GameObject spawnInstance = 
                (GameObject)GameObject.Instantiate(spawn);
            spawnInstance.name = spawn.name;

            // Use the position of the game object we're attached to
            spawnInstance.transform.parent = gameObject.transform;
            spawnInstance.transform.localPosition = Vector3.zero;
        }
    }

    public void CustomizePrefab(UnityEngine.GameObject prefab)
    {
        // Do nothing
    }
}

