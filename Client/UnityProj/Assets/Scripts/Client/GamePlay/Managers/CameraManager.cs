using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public Camera MainCamera;
        public Camera BattleUICamera;
        public CameraFollow MainCameraFollow;
    }
}