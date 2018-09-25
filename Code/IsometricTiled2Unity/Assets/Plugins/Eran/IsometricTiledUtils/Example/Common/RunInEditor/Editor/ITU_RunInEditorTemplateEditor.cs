using UnityEditor;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Common.RunInEditor.Editor
{
    [CustomEditor(typeof(ITU_RunInEditorTemplate), true)]
    public class ITU_RunInEditorTemplateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var sp = (ITU_RunInEditorTemplate) target;
            if (GUILayout.Button("Run"))
            {
                sp.OnClickRunBtn();
            }
        }
    }
}