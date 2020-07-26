using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackManager : TSingletonBaseManager<BackpackManager>
    {
        private Dictionary<string, Backpack> BackpackDict = new Dictionary<string, Backpack>();

        private Dictionary<string, Sprite> BackpackItemSpriteDict = new Dictionary<string, Sprite>();

        public void LoadBackpackItemConfigs(int backpackGridSize)
        {
            GameObject[] prefabs = Resources.LoadAll("Prefabs/UI/Backpack/BackpackItems").Cast<GameObject>().ToArray();
            foreach (GameObject prefab in prefabs)
            {
                BackpackItemDesignerHelper helper = prefab.GetComponent<BackpackItemDesignerHelper>();
                helper.ReadBackpackItemInfo(backpackGridSize, out List<GridPos> occupiedGridPositions, out Sprite sprite);
                BackpackItemSpriteDict.Add(prefab.name, sprite);
            }
        }

        public void AddBackPack(Backpack backPack)
        {
            BackpackDict.Add(backPack.InventoryName, backPack);
        }

        public Backpack GetBackPack(string backPackName)
        {
            BackpackDict.TryGetValue(backPackName, out Backpack backpack);
            return backpack;
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            foreach (KeyValuePair<string, Backpack> kv in BackpackDict)
            {
                kv.Value.Update();
            }
        }

        public Sprite GetBackpackItemSprite(string spriteName)
        {
            BackpackItemSpriteDict.TryGetValue(spriteName, out Sprite sprite);
            return sprite;
        }
    }
}