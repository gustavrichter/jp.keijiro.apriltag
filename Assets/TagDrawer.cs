using UnityEngine;


sealed class TagDrawer : System.IDisposable
{
    Mesh _mesh;
    Material _sharedMaterial;
    public ScriptObjBucketAssetRef _bucketAssetRef;

    GameObject bucketMeshFile; //draw mesh
    GameObject bucketObject; // or instantiate a gameobject

    float log_tick_sec = 3.0f;
    float log_cd = 0.0f;
    int it = 0;
    bool useBucket = true;
    public TagDrawer(Material material)
    {
        if (useBucket)
        {
            bucketMeshFile = Resources.Load("candyBucket") as GameObject;
            bucketObject = GameObject.Instantiate(bucketMeshFile, Vector3.zero, Quaternion.identity);
         
            if (!bucketMeshFile)
            {
                Debug.Log("bucketMeshFile not found");
                
            }
            else
            {
                _mesh = bucketMeshFile.GetComponent<MeshFilter>().sharedMesh;
            }

        }
        else
        {
            _mesh = BuildMesh();
        }

        //if (!_bucketAssetRef)
        //    Debug.Log("ScriptableObject asset reference invalid");

        //GameObject bucketObject = _bucketAssetRef.bucketAssetReference;
        //if (!bucketObject)
        //    Debug.Log("bucket asset not found");

        //_mesh = bucketObject.GetComponent<MeshFilter>().sharedMesh;
        //if (!_mesh)
        //    Debug.Log("bucket mesh not found");

        _sharedMaterial = material;
    }
    
    public void Dispose()
    {
        if (_mesh)
        {
        //Object.DestroyImmediate(_mesh, true); //actually deletes the mesh from the prefab
        }
        else
        {
            Debug.Log("TagDrawer::Dispose() is disposing of nothing");
        }
        _mesh = null;
        _sharedMaterial = null;
    }

    public void Draw(int id, Vector3 position, Quaternion rotation, float scale)
    {
        Vector3 angles = rotation.eulerAngles;
        Vector3 offset_pos = new Vector3(0.0f, 0.33f, 0.0f) * scale;
        Vector3 corrected_pos = position + offset_pos;
        Quaternion corrected_rot = rotation * Quaternion.Euler(-90,0,0);
        Vector3 corrected_angles = corrected_rot.eulerAngles;

        //debug message every lock_tick_sec seconds
        if (log_cd <= 0.0f)
        {
            Debug.Log( ++it + ". ---------TagvsBucket--------------");
            Debug.Log("X= " + angles.x + " vs " + corrected_angles.x);
            Debug.Log("Y= " + angles.y + " vs " + corrected_angles.y);
            Debug.Log("Z= " + angles.z + " vs " + corrected_angles.z);
            Debug.Log("Pos= " + position + " vs " + corrected_pos);
            log_cd = log_tick_sec;
        }
        else
        {
            log_cd -= Time.fixedDeltaTime;
        }
        var corrected_xform = Matrix4x4.TRS(corrected_pos, corrected_rot, Vector3.one * scale);
        var xform = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
        //public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, Rendering.ShadowCastingMode castShadows, bool receiveShadows = true, Transform probeAnchor = null, bool useLightProbes = true); 
        if (useBucket)
        {
            bucketObject.transform.localScale = Vector3.one * scale;
            bucketObject.transform.position = corrected_pos;
            bucketObject.transform.rotation = corrected_rot;

            //for (int i = 0; i < _mesh.subMeshCount; i++)
            //{
            //    Graphics.DrawMesh(_mesh, corrected_xform, bucketMeshFile.GetComponent<MeshRenderer>().sharedMaterials[i], 0, null, i);
            //}
        }
        else
        {
            Graphics.DrawMesh(_mesh, xform, _sharedMaterial, 0);

        }

    }
 
    static Mesh BuildMesh()
    {
        var vtx = new Vector3 [] { new Vector3(-0.5f, -0.5f, 0),
                                   new Vector3(+0.5f, -0.5f, 0),
                                   new Vector3(+0.5f, +0.5f, 0),
                                   new Vector3(-0.5f, +0.5f, 0),
                                   new Vector3(-0.5f, -0.5f, -1),
                                   new Vector3(+0.5f, -0.5f, -1),
                                   new Vector3(+0.5f, +0.5f, -1),
                                   new Vector3(-0.5f, +0.5f, -1),
                                   new Vector3(-0.2f, 0, 0),
                                   new Vector3(+0.2f, 0, 0),
                                   new Vector3(0, -0.2f, 0),
                                   new Vector3(0, +0.2f, 0),
                                   new Vector3(0, 0, 0),
                                   new Vector3(0, 0, -1.5f) };

        var idx = new int [] { 0, 1, 1, 2, 2, 3, 3, 0,
                               4, 5, 5, 6, 6, 7, 7, 4,
                               0, 4, 1, 5, 2, 6, 3, 7,
                               8, 9, 10, 11, 12, 13 };

        var mesh = new Mesh();
        mesh.vertices = vtx;
        mesh.SetIndices(idx, MeshTopology.Lines, 0);
        return mesh;
    }
}
