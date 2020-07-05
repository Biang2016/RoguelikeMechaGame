using UnityEngine;
using System.Collections;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.GridBackpack
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BackpackItemDesignerHelper))]
    public class BackpackGridSnapper : MonoBehaviour
    {
        private BackpackItemDesignerHelper helper;
        void Awake()
        {
            helper = GetComponent<BackpackItemDesignerHelper>();
        }
        void LateUpdate()
        {
            Vector3 localPosition = transform.localPosition;
            //GridPos gp = GridPos.GetGridPosByLocalTrans(transform, helper.Data.GridSize);
            //transform.localPosition = new Vector3(gp.x, localPosition.y, gp.z);
        }
    }
}