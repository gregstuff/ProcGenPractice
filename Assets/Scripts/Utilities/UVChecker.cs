using UnityEngine;
namespace DungeonGeneration.Utilities
{
    public class UVChecker : MonoBehaviour
    {
        public GameObject prefab; // Assign a tile prefab here
        void Start()
        {
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Mesh mesh = mf.sharedMesh;
                Debug.Log($"Mesh {mesh.name}: UV0={(mesh.uv != null ? mesh.uv.Length : 0)}, UV2={(mesh.uv2 != null ? mesh.uv2.Length : 0)}");
            }
        }
    }
}