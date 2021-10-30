using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreateEditor : OdinEditor
{
    private void OnSceneGUI()
    {
        var levelCreator = (LevelCreator) target;
        if (Event.current.type == EventType.MouseMove)
        {
            SceneView.currentDrawingSceneView.Repaint();
        }

        levelCreator.CreateObject();
    }
}
