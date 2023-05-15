
#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class FabriqueTools : EditorWindow
{
    [MenuItem("Window/FabriqueTools")]
    public static void ShowWindow()
    {
        GetWindow<FabriqueTools>("FabriqueTools");
    }

    private void OnGUI()
    {
        CreateChainAction();
    }

    #region CreateChainAction

    private Type[] _chainActionTypes;
    private int _selectedChainActionTypeIndex = 0;

    private void CreateChainAction()
    {
        if (_chainActionTypes == null)
        {
            // Find all types in the project that inherit from ChainAction
            _chainActionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(ChainAction)))
                .ToArray();
        }

        if (_chainActionTypes.Length > 0)
        {
            string[] chainActionTypeNames = _chainActionTypes.Select(x => x.Name).ToArray();
            _selectedChainActionTypeIndex =
                EditorGUILayout.Popup("Chain Action Type", _selectedChainActionTypeIndex, chainActionTypeNames);
        }

        if (GUILayout.Button("Create ChainAction"))
        {
            GameObject chainActionObject = new GameObject();

            if (_chainActionTypes != null && _selectedChainActionTypeIndex >= 0 &&
                _selectedChainActionTypeIndex < _chainActionTypes.Length)
            {
                Type selectedChainActionType = _chainActionTypes[_selectedChainActionTypeIndex];

                chainActionObject.AddComponent(selectedChainActionType);
                chainActionObject.name = selectedChainActionType.Name;

                // Get selected object in hierarchy
                GameObject selectedObject = Selection.activeGameObject;
                if (selectedObject != null)
                {
                    chainActionObject.transform.SetParent(selectedObject.transform);
                }
            }
        }

    }

    #endregion
}

#endif