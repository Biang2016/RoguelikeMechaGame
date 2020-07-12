using System.Collections.Generic;
using BiangStudio.Singleton;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackManager : TSingletonBaseManager<BackpackManager>
    {
        private Dictionary<string, Backpack> BackpackDict = new Dictionary<string, Backpack>();

        private Dictionary<string, Sprite> BackpackItemSpriteDict;

        private void LoadBackpackItemConfigs()
        {
            GameObject[] prefabs = Resources.LoadAll<GameObject>("/Prefabs/UI/Backpack/Items");
            foreach (GameObject prefab in prefabs)
            {
                BackpackItemDesignerHelper helper = prefab.GetComponent<BackpackItemDesignerHelper>();
                //BackpackItemSpriteDict.Add( helper.BackpackItemSprite);
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