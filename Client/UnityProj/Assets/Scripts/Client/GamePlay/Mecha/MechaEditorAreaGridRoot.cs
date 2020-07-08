using UnityEngine;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using GameCore;

namespace Client
{
    public class MechaEditorAreaGridRoot : MonoBehaviour
    {
        private List<MechaEditorAreaGrid> MechaEditorAreaGrids = new List<MechaEditorAreaGrid>();

        public void Init()
        {
            for (int col = -ConfigManager.EDIT_AREA_HALF_SIZE; col < ConfigManager.EDIT_AREA_HALF_SIZE; col++)
            {
                for (int row = -ConfigManager.EDIT_AREA_HALF_SIZE; row < ConfigManager.EDIT_AREA_HALF_SIZE; row++)
                {
                    MechaEditorAreaGrid grid = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechaEditorAreaGrid].AllocateGameObject<MechaEditorAreaGrid>(transform);
                    grid.transform.localPosition = new Vector3(col * ConfigManager.GridSize, 0, row * ConfigManager.GridSize);
                    grid.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    grid.Init(new GridPos(col + ConfigManager.EDIT_AREA_HALF_SIZE, row + ConfigManager.EDIT_AREA_HALF_SIZE));
                    MechaEditorAreaGrids.Add(grid);
                }
            }
        }
    }
}