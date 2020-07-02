﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

namespace Client
{
    public static class ClientUtils
    {
        public static Color HTMLColorToColor(string htmlColor)
        {
            Color cl = new Color();
            ColorUtility.TryParseHtmlString(htmlColor, out cl);
            return cl;
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

        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
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

        public static GridPos GetGridPosByMousePos(Transform parentTransform, Vector3 planeNormal, int gridSize)
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Common_MousePosition);
            Vector3 intersect = ClientUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, planeNormal, parentTransform.position);

            Vector3 rot_intersect = parentTransform.InverseTransformPoint(intersect);
            GridPos local_GP = GridPos.GetGridPosByPoint(rot_intersect + Vector3.one * gridSize / 2f, 1);

            int x = Mathf.FloorToInt(local_GP.x / gridSize) * gridSize;
            int z = Mathf.FloorToInt(local_GP.z / gridSize) * gridSize;
            return new GridPos(x, z);
        }

        public static string GridPositionListToString(this List<GridPos> gridPositions)
        {
            string res = "";
            foreach (GridPos gp in gridPositions)
            {
                res += gp + ", ";
            }

            return res;
        }

        public static GridPos ConvertGridPosToMatrixIndex(this GridPos gp)
        {
            return new GridPos(gp.z + ConfigManager.EDIT_AREA_SIZE, gp.x + ConfigManager.EDIT_AREA_SIZE);
        }

        public static GridPos ConvertMatrixIndexToGridPos(this GridPos gp_matrix)
        {
            return new GridPos(gp_matrix.z - ConfigManager.EDIT_AREA_SIZE, gp_matrix.x - ConfigManager.EDIT_AREA_SIZE);
        }

        public static int CalculateModifiers(this List<Modifier> modifiers, int value)
        {
            int res = value;
            foreach (Modifier modifier in modifiers)
            {
                res = modifier.Calculate(res);
            }

            return res;
        }

        public static float CalculateModifiers(this List<Modifier> modifiers, float value)
        {
            float res = value;
            foreach (Modifier modifier in modifiers)
            {
                res = modifier.Calculate(res);
            }

            return res;
        }

        public static void GetStateCallbackFromContext(this ControlManager.ButtonState state, InputAction action)
        {
            ControlManager.Instance.ButtonStateList.Add(state);
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
    }
}