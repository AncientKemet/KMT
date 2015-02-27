#if SERVER
using System;
using Code.Libaries.Generic.Managers;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Server.Model.Content.Spawns
{
    [ExecuteInEditMode]
    public class ObjectSpawn : SpawnMB
    {

        public int ModelID = 1;

        private int _stackedModel = -1;

        private Mesh _mesh;
        private MeshFilter _filter;
        private Renderer _renderer;
#if UNITY_EDITOR
        private void OnRenderObject()
        {

            try
            {
                if (_mesh == null || _filter == null || _renderer == null || ModelID != _stackedModel)
                {
                    _stackedModel = ModelID;
                    _filter = ContentManager.I.Models[_stackedModel].GetComponent<MeshFilter>();
                    if (_filter != null)
                        _renderer = ContentManager.I.Models[_stackedModel].GetComponent<Renderer>();
                    if (_filter != null)
                        _mesh = _filter.mesh;
                }

                if (_renderer != null && _mesh != null)
                {
                    for (int i = 0; i < _renderer.sharedMaterial.passCount; i++)
                    {
                        if(_renderer.material.SetPass(i))
                            Graphics.DrawMeshNow(_mesh, transform.localToWorldMatrix);
                    }
                }

            }
            catch (Exception e)
            {
                
            }
            SceneView.lastActiveSceneView.Repaint();

        }

#endif
    }
}

#endif