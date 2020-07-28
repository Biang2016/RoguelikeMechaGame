using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Client
{
    public static class ClientUtils
    {
        public static Color ChangeColorToWhite(Color color, float whiteRatio)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, g, b);

            if (max - r < 0.2f && max - g < 0.2f && max - b < 0.2f) //本来就是灰色
            {
                max = max + 0.3f;
                Color res = Color.Lerp(color, new Color(max, max, max, color.a), 1f);
                return res;
            }
            else
            {
                max = max + 0.3f;
                Color res = Color.Lerp(color, new Color(max, max, max, color.a), whiteRatio);
                return res;
            }
        }

        public static Color HSL_2_RGB(float H, float S, float L)
        {
            //H, S and L input range = 0 ÷ 1.0
            //R, G and B output range = 0 ÷ 255
            float R;
            float G;
            float B;
            if (S.Equals(0))
            {
                R = L;
                G = L;
                B = L;
            }
            else
            {
                float var_1 = 0;
                float var_2 = 0;
                if (L < 0.5)
                {
                    var_2 = L * (1 + S);
                }
                else
                {
                    var_2 = (L + S) - (S * L);
                }

                var_1 = 2 * L - var_2;

                R = Hue_2_RGB(var_1, var_2, H + (1.0f / 3));
                G = Hue_2_RGB(var_1, var_2, H);
                B = Hue_2_RGB(var_1, var_2, H - (1.0f / 3));
            }

            return new Color(R, G, B);
        }

        static float Hue_2_RGB(float v1, float v2, float vH) //Function Hue_2_RGB
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
            if ((2 * vH) < 1) return (v2);
            if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2.0f / 3.0f) - vH) * 6);
            return v1;
        }

        public static Vector3 GenerateRandomPosInsideCollider(BoxCollider bc)
        {
            float x = Random.Range(bc.center.x - bc.size.x * 0.5f, bc.center.x + bc.size.x * 0.5f);
            float y = Random.Range(bc.center.y - bc.size.y * 0.5f, bc.center.y + bc.size.y * 0.5f);
            float z = Random.Range(bc.center.z - bc.size.z * 0.5f, bc.center.z + bc.size.z * 0.5f);
            Vector3 localPos = new Vector3(x, y, z);
            Vector3 worldPos = bc.transform.TransformPoint(localPos);
            return worldPos;
        }

        public static SortedDictionary<int, List<T>> GetRank<T>(this IEnumerable<T> value, IComparer<T> comparer)
        {
            List<T> list = value.ToList<T>();
            list.Sort(comparer); // 排序  Sort方法排序后的结果是升序
            list.Reverse(); //反转List中的数据  就变成降序了

            SortedDictionary<int, List<T>> rank = new SortedDictionary<int, List<T>>();

            int order = 0;
            int pcount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!rank.ContainsKey(order))
                {
                    rank.Add(order, new List<T> { });
                }

                if (i < list.Count && i > 0)
                {
                    int result = comparer.Compare(list[i], list[i - 1]);
                    if (result != 0)
                    {
                        if (pcount == 0)
                        {
                            order++;
                        }
                        else
                        {
                            order = order + 1 + pcount;
                            pcount = 0;
                        }
                    }
                    else
                    {
                        pcount++;
                    }
                }

                if (!rank.ContainsKey(order))
                {
                    rank.Add(order, new List<T> { });
                }

                rank[order].Add(list[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                if (!rank.ContainsKey(i))
                {
                    rank.Add(i, new List<T> { });
                }
            }

            return rank;
        }

        public static float DistanceBetweenPointToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 S_P = point - segmentStart;
            Vector3 S_E = segmentEnd - segmentStart;
            Vector3 E_P = point - segmentEnd;
            Vector3 E_S = -S_E;

            if (Vector3.Dot(S_P, S_E) <= 0)
            {
                return S_P.magnitude;
            }

            if (Vector3.Dot(E_P, E_S) <= 0)
            {
                return E_P.magnitude;
            }

            return (Vector3.Dot(S_P, S_E) / S_E.magnitude * S_E.normalized - S_P).magnitude;
        }

        public static List<T> GetRandomFromList<T>(List<T> OriList, int number, List<T> exceptList = null)
        {
            if (OriList == null || OriList.Count == 0) return new List<T>();

            List<T> ori = OriList.ToArray().ToList();
            if (exceptList != null)
            {
                List<T> remove = new List<T>();
                foreach (T t in ori)
                {
                    if (exceptList.Contains(t))
                    {
                        remove.Add(t);
                    }
                }

                foreach (T removeT in remove)
                {
                    ori.Remove(removeT);
                }
            }

            if (number > ori.Count) number = ori.Count;

            HashSet<int> indices = new HashSet<int>();
            while (indices.Count < number)
            {
                int index = Random.Range(0, ori.Count);
                if (!indices.Contains(index))
                {
                    indices.Add(index);
                }
            }

            List<T> res = new List<T>();
            foreach (int i in indices)
            {
                res.Add(ori[i]);
            }

            return res;
        }

        public static string TimeToString(float timeTick)
        {
            return Mathf.FloorToInt(timeTick / 60f) + ":" + Mathf.FloorToInt(timeTick % 60).ToString().PadLeft(2, '0');
        }

        public static string TimeToString_Milisecond(float timeTick)
        {
            return "." + Mathf.CeilToInt(timeTick % 1 * 10f);
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float GetClipLength(Animator animator, string clip)
        {
            if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
                return 0;
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            AnimationClip[] tAnimationClips = ac.animationClips;
            if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
            AnimationClip tAnimationClip;
            for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
            {
                tAnimationClip = ac.animationClips[tCounter];
                if (null != tAnimationClip && tAnimationClip.name == clip)
                    return tAnimationClip.length;
            }

            return 0F;
        }

        public static GridPos ConvertGridPosToMatrixIndex(this GridPos gp)
        {
            return new GridPos(gp.z + ConfigManager.EDIT_AREA_HALF_SIZE, gp.x + ConfigManager.EDIT_AREA_HALF_SIZE);
        }

        public static GridPos ConvertMatrixIndexToGridPos(this GridPos gp_matrix)
        {
            return new GridPos(gp_matrix.z - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.x - ConfigManager.EDIT_AREA_HALF_SIZE);
        }

        public static string GetIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        public static void GetStateCallbackFromContext(this ButtonState state, InputAction action)
        {
            ControlManager.Instance.ButtonStateDict.Add(state.ButtonName, state);
            action.performed += context =>
            {
                ButtonControl bc = (ButtonControl) context.control;
                state.Down = !state.LastPressed;
                state.Pressed = bc.isPressed;
                state.Up = bc.wasReleasedThisFrame;
                if (bc.wasReleasedThisFrame)
                {
                    state.Down = false;
                    state.Pressed = false;
                }
            };

            action.canceled += context => { state.Pressed = false; };
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float Remap(this int value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static GridPosR.OrientationFlag ToFlag(this GridPosR.Orientation ori)
        {
            switch (ori)
            {
                case GridPosR.Orientation.Up:
                {
                    return GridPosR.OrientationFlag.Up;
                }
                case GridPosR.Orientation.Down:
                {
                    return GridPosR.OrientationFlag.Down;
                }
                case GridPosR.Orientation.Left:
                {
                    return GridPosR.OrientationFlag.Left;
                }
                case GridPosR.Orientation.Right:
                {
                    return GridPosR.OrientationFlag.Right;
                }
            }

            return 0;
        }
    }
}