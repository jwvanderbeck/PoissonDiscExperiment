using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateProjects.Editor
{
    [CustomEditor(typeof(Poisson))]
    public class PoissonEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/PoissonInspector.uxml");
            var root = visualTree.CloneTree();
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/PoissonInspector.uss"));

            root.Query<Button>("sample").First().clicked += SamplePoints;
            root.Query<Button>("clear").First().clicked += ClearPoints;
            return root;
        }

        private void ClearPoints()
        {
            Poisson p = target as Poisson;

            if (p != null)
            {
                p.Clear();
            }
        }

        private void SamplePoints()
        {
            Poisson p = target as Poisson;

            if (p != null)
            {
                p.CreatePoints();
            }
        }
    }
}