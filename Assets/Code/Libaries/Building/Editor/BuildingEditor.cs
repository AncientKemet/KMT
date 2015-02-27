using Code.Libaries.Building;
using Code.Code.Libaries.Building;
using UnityEditor;
using UnityEngine;

namespace Code.Code.Libaries.Building
{
    [CustomEditor(typeof(BuildingGenerator))]
    public class BuildingEditor : Editor
    {

        BuildingGenerator gen
        {
            get { return (BuildingGenerator)target; }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PrefixLabel("Nodes: " + gen.nodes.Count);
            base.DrawDefaultInspector();
        }

        public void OnSceneGUI()
        {
            gen.nodes.RemoveAll(item => item == null);

            Event e = Event.current;
            if (e.type == EventType.keyDown)
            {
                if (e.keyCode == KeyCode.A)
                {
                    Vector3 v = e.mousePosition;
                    v.y = Screen.height - v.y;
                    Ray ray = Camera.current.ScreenPointToRay(v);
                    Debug.DrawRay(ray.origin, ray.direction, Color.green);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        BuildingNode node = CreateBuildingNode();
                        node.transform.position = hit.point;
                        node.transform.parent = gen.transform;
                        gen.nodes.Add(node);

                        gen.ForceBuild();
                    }
                }
            }
        }

        private static BuildingNode CreateBuildingNode()
        {
            GameObject go = new GameObject("Node");
            go.AddComponent<BuildingNode>();
            return go.GetComponent<BuildingNode>();
        }

        [MenuItem("Kemet/Buildings/New Builing")]
        public static void NewBuilding()
        {
            GameObject go = new GameObject("Building");
            go.AddComponent<BuildingGenerator>();
            Selection.activeGameObject = go;
        }
    }
}
