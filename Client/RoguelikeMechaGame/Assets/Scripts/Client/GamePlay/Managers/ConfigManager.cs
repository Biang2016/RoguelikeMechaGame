using System.Collections.Generic;
using System.IO;
using GameCore;
using Newtonsoft.Json;
using UnityEngine;

namespace Client
{
    public class ConfigManager : MonoSingleton<ConfigManager>
    {
        public const int EDIT_AREA_SIZE = 9;
    }
}