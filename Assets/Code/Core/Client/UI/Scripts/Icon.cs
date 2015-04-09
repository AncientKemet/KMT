using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Client.UI.Scripts
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Icon : MonoBehaviour
    {

        private Texture2D _oldTexture2D;
        public Texture2D Texture;

        void Update()
        {
            if (_oldTexture2D != Texture)
            {
                RecreateMesh();
            }
        }

        private void RecreateMesh()
        {
            renderer.sharedMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
            renderer.sharedMaterial.mainTexture = Texture;

            Mesh mesh = new Mesh();

            List<Vector3> newVertices = new List<Vector3>();
            List<Vector2> newUV = new List<Vector2>();
            List<int> newTriangles = new List<int>();

            newVertices.Add(new Vector3(0, 0, 0));
            newVertices.Add(new Vector3(0 + 1, 0, 0));
            newVertices.Add(new Vector3(0 + 1, 0 - 1, 0));
            newVertices.Add(new Vector3(0, 0 - 1, 0));

            newTriangles.Add(0);
            newTriangles.Add(1);
            newTriangles.Add(3);
            newTriangles.Add(1);
            newTriangles.Add(2);
            newTriangles.Add(3);

            newUV.Add(new Vector2(0, 0));
            newUV.Add(new Vector2(1, 0));
            newUV.Add(new Vector2(1, 1));
            newUV.Add(new Vector2(0, 1));

            mesh.Clear();
            mesh.vertices = newVertices.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.uv = newUV.ToArray(); // add this line to the code here
            mesh.Optimize();
            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;

            renderer.enabled = Texture != null;

            _oldTexture2D = Texture;
        }
    }
}
