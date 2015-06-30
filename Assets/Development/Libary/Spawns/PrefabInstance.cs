﻿using Client.Enviroment;
using Development.Libary.Spawns;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;

[ExecuteInEditMode]
public class PrefabInstance : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField] private bool _rebuild = true;
    public int UnitId = -1;

#if UNITY_EDITOR
    // Struct of all components. Used for edit-time visualization and gizmo drawing
    public struct Thingy
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public List<Material> materials;
    }

    [System.NonSerializedAttribute]
    public List<Thingy> things = new List<Thingy>();

    void Start()
    {
        if (Application.isPlaying)
        {
            /*var o = Instantiate(prefab, transform.position, transform.rotation) as GameObject;*/
            BakeInstance(this);
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        things.Clear();
        prefab.transform.localRotation = Quaternion.identity;
        if (enabled)
            Rebuild(prefab, Matrix4x4.identity);
    }

    void OnEnable()
    {
        things.Clear();
        prefab.transform.localRotation = Quaternion.identity;
        if (enabled)
            Rebuild(prefab, Matrix4x4.identity);
    }

    void Rebuild(GameObject source, Matrix4x4 inMatrix)
    {
        CheckInstanceId();
        _rebuild = false;
        if (!source)
            return;

        Matrix4x4 baseMat = inMatrix * Matrix4x4.TRS(-source.transform.position, Quaternion.identity, Vector3.one);

// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (Renderer mr in source.GetComponentsInChildren(typeof(Renderer), true))
        {
            things.Add(new Thingy()
            {
                mesh = mr.GetComponent<MeshFilter>().sharedMesh,
                matrix = baseMat * mr.transform.localToWorldMatrix,
                materials = new List<Material>(mr.sharedMaterials)
            });
        }

// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (PrefabInstance pi in source.GetComponentsInChildren(typeof(PrefabInstance), true))
        {
            if (pi.enabled && pi.gameObject.activeSelf)
                Rebuild(pi.prefab, baseMat * pi.transform.localToWorldMatrix);
        }
    }

    private void CheckInstanceId()
    {   var parent = transform.parent;
        KemetMap map = parent.GetComponent<KemetMap>();
        while (map == null)
        {
            parent = parent.parent;
            map = parent.GetComponent<KemetMap>();
        }
        if (map.PrefabInstances[UnitId] != this)
            UnitId = -1;
        if (UnitId == -1 )
        {
                
                    for (int i = 0; i < map.PrefabInstances.Count; i++)
                    {
                        if (map.PrefabInstances[i] == null)
                        {
                            map.PrefabInstances[i] = this;
                            UnitId = i;
                            break;
                        }
                    }
                    if (UnitId == -1)
                    {
                        UnitId = map.PrefabInstances.Count;
                        map.PrefabInstances.Add(this);
                    }
            
        }
    }

    // Editor-time-only update: Draw the meshes so we can see the objects in the scene view
    void Update()
    {
        if (EditorApplication.isPlaying)
            return;
        if(_rebuild)
            Rebuild(prefab, Matrix4x4.identity);
        Matrix4x4 mat = transform.localToWorldMatrix;
        foreach (Thingy t in things)
            for (int i = 0; i < t.materials.Count; i++)
                Graphics.DrawMesh(t.mesh, mat * t.matrix, t.materials[i], gameObject.layer, null, i);
    }

    // Picking logic: Since we don't have gizmos.drawmesh, draw a bounding cube around each thingy
    void OnDrawGizmos() { DrawGizmos(new Color(0, 0, 0, 0)); }
    void OnDrawGizmosSelected() { DrawGizmos(new Color(0, 0, 1, .2f)); }
    void DrawGizmos(Color col)
    {
        if (EditorApplication.isPlaying)
            return;
        Gizmos.color = col;
        Matrix4x4 mat = transform.localToWorldMatrix;
        foreach (Thingy t in things)
        {
            Gizmos.matrix = mat * t.matrix;
            Gizmos.DrawCube(t.mesh.bounds.center, t.mesh.bounds.size);
        }
    }

    // Baking stuff: Copy in all the referenced objects into the scene on play or build
    [PostProcessScene(-2)]
    public static void OnPostprocessScene()
    {
// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (PrefabInstance pi in UnityEngine.Object.FindObjectsOfType(typeof(PrefabInstance)))
            BakeInstance(pi);
    }

    public static void BakeInstance(PrefabInstance pi)
    {
        if (!pi.prefab || !pi.enabled)
            return;
        var go = Instantiate(pi.prefab, pi.transform.position, pi.transform.rotation) as GameObject;
        foreach (PrefabInstance childPi in go.GetComponentsInChildren<PrefabInstance>())
            BakeInstance(childPi);

        var _in = go.GetComponent<StaticObjectInstance>();
        if(_in != null)
            _in.OnBake((ushort) pi.UnitId);

    }

#endif
}
