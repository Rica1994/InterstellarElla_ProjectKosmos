using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    [CustomEditor(typeof(BezierCurveDrawer))]
    public class BezierCurveDrawerEditor : Editor
    {
        private BezierCurveDrawer curveDrawer;

        private void OnSceneGUI()
        {
            curveDrawer = (BezierCurveDrawer)target;

            Handles.color = Color.white;
            Handles.DrawBezier(curveDrawer.startPoint.position, curveDrawer.GetCurveEndPoint(), curveDrawer.startPoint.position + Vector3.forward, curveDrawer.GetCurveEndPoint() + Vector3.forward, Color.white, null, 2f);
        }
    }
