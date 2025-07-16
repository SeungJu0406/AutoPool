#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AutoPool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.

    [CustomEditor(typeof(MainAutoPool))]
    public class ObjectPoolEditor : Editor
    {
        enum SortType { Name, ActiveCount }

        private SortType sortType = SortType.ActiveCount;

        private string searchQuery = "";
        private const int MaxDisplayCount = 10;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DrawInspectorObjectPool();
            DrawInspectorGenericPool();
        }
        

        private void DrawInspectorObjectPool()
        {
            MainAutoPool pool = (MainAutoPool)target;

            var infos = pool.GetAllPoolInfos();

            if(infos == null || infos.Count == 0)
            {
                EditorGUILayout.HelpBox("No pools currently active", MessageType.Info);
                return;
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Active Pools", EditorStyles.boldLabel);

            // 정렬 방식 선택
            sortType = (SortType)EditorGUILayout.EnumPopup("Sort by", sortType);

            // 검색 입력 필드
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(EditorGUIUtility.IconContent("Search Icon"), GUILayout.Width(20), GUILayout.Height(20));
            searchQuery = EditorGUILayout.TextField($"Search", searchQuery);
            EditorGUILayout.EndHorizontal();


            // 활성 풀만 추림
            infos = infos.Where(info => info.IsActive).ToList();

            // 검색 필터 적용
            if (!string.IsNullOrEmpty(searchQuery))
            {
                infos = infos.Where(info => info.Prefab.name.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            // 정렬 적용
            switch (sortType)
            {
                case SortType.Name:
                    infos.Sort((a, b) => string.Compare(a.Prefab.name, b.Prefab.name));
                    break;
                case SortType.ActiveCount:
                    infos.Sort((a, b) => b.ActiveCount.CompareTo(a.ActiveCount));
                    break;
            }


            if (infos.Count == 0)
            {
                EditorGUILayout.HelpBox("No pools currently active", MessageType.Info);
            }
            else
            {
                int count = 0;
                int maxCount = MaxDisplayCount;

                foreach (var info in infos)
                {
                    if (!info.IsActive) continue;

                    if (++count > maxCount)
                        break;

                    string key = info.Prefab.name;


                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField($"{info.Prefab.name}");
                    EditorGUILayout.ObjectField("Prefab", info.Prefab, typeof(GameObject), false);
                    EditorGUILayout.LabelField($"Active: {info.ActiveCount} / Total: {info.PoolCount}");

                    // 현재 사용량 바
                    float ratio = (float)info.ActiveCount / Mathf.Max(1, info.PoolCount);
                    Rect r = GUILayoutUtility.GetRect(100, 5);
                    EditorGUI.DrawRect(r, Color.gray);
                    EditorGUI.DrawRect(new Rect(r.x, r.y, r.width * ratio, r.height), Color.white);

                    if (GUILayout.Button("Log Pool Info"))
                    {
                        Debug.Log($"[{info.Prefab.name}] Active: {info.ActiveCount} / {info.PoolCount}");
                    }

                    if (info.Pool.Contains(null))
                    {
                        EditorGUILayout.HelpBox("Null Object remains in the pool!", MessageType.Warning);
                    }

                    EditorGUILayout.EndVertical();

                }
            }
        }
        private void DrawInspectorGenericPool()
        {
            MainAutoPool pool = (MainAutoPool)target;

            var infos = pool.GetAllGenericPoolInfos();

            if(infos == null || infos.Count == 0)
            {
                EditorGUILayout.HelpBox("No Generic pools currently active", MessageType.Info);
                return;
            }   

            GUILayout.Space(30);
            EditorGUILayout.LabelField("Active Generic Pools", EditorStyles.boldLabel);

            // 정렬 방식 선택
            sortType = (SortType)EditorGUILayout.EnumPopup("Sort by", sortType);

            // 검색 입력 필드
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(EditorGUIUtility.IconContent("Search Icon"), GUILayout.Width(20), GUILayout.Height(20));
            searchQuery = EditorGUILayout.TextField($"Search", searchQuery);
            EditorGUILayout.EndHorizontal();
            // 활성 풀만 추림
            infos = infos.Where(info => info.IsActive).ToList();

            // 검색 필터 적용
            if (!string.IsNullOrEmpty(searchQuery))
            {
                infos = infos.Where(info => info.Type.Name.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            // 정렬 적용
            switch (sortType)
            {
                case SortType.Name:
                    infos.Sort((a, b) => string.Compare(a.Type.Name, b.Type.Name));
                    break;
                case SortType.ActiveCount:
                    infos.Sort((a, b) => b.ActiveCount.CompareTo(a.ActiveCount));
                    break;
            }


            if (infos.Count == 0)
            {
                EditorGUILayout.HelpBox("No pools currently active", MessageType.Info);
            }
            else
            {
                int count = 0;
                int maxCount = MaxDisplayCount;

                foreach (var info in infos)
                {
                    if (!info.IsActive) continue;

                    if (++count > maxCount)
                        break;

                    string key = info.Type.Name;


                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField($"{info.Type.Name}");
                    EditorGUILayout.LabelField($"Active: {info.ActiveCount} / Total: {info.PoolCount}");

                    // 현재 사용량 바
                    float ratio = (float)info.ActiveCount / Mathf.Max(1, info.PoolCount);
                    Rect r = GUILayoutUtility.GetRect(100, 5);
                    EditorGUI.DrawRect(r, Color.gray);
                    EditorGUI.DrawRect(new Rect(r.x, r.y, r.width * ratio, r.height), Color.white);

                    if (GUILayout.Button("Log Pool Info"))
                    {
                        Debug.Log($"[{info.Type.Name}] Active: {info.ActiveCount} / {info.PoolCount}");
                    }

                    if (info.Pool.Contains(null))
                    {
                        EditorGUILayout.HelpBox("Null Object remains in the pool!", MessageType.Warning);
                    }

                    EditorGUILayout.EndVertical();

                }
            }
        }
    }
}
#endif
