using UnityEngine;


sealed class TagDrawer : System.IDisposable
{
    Mesh _mesh;
    Material _sharedMaterial;
    public ScriptObjBucketAssetRef _bucketAssetRef;

    GameObject bucketMeshFile;

    float bucket_rotation_speed = 100.0f;
    float bucket_orientation = 0.0f;
    float log_tick_sec = 1.0f;
    float log_cd = 2.0f;
    bool useBucket = false;
    public TagDrawer(Material material)
    {
        

        bucketMeshFile = Resources.Load("candyBucket") as GameObject;
        bucketMeshFile.transform.Rotate(Vector3.forward, 90.0f);//bringt nichts

        if (!bucketMeshFile)
        {
            Debug.Log("bucketMeshFile not found");
            _mesh = BuildCoordinateAxes();
        }
        else
        {
            _mesh = bucketMeshFile.GetComponent<MeshFilter>().sharedMesh;
            useBucket = true;
        }
        
        //if (!_bucketAssetRef)
        //    Debug.Log("ScriptableObject asset reference invalid");

        //GameObject bucketObject = _bucketAssetRef.bucketAssetReference;
        //if (!bucketObject)
        //    Debug.Log("bucket asset not found");

        //_mesh = bucketObject.GetComponent<MeshFilter>().sharedMesh;
        //if (!_mesh)
        //    Debug.Log("bucket mesh not found");

        //_mesh = BuildMesh();
        _sharedMaterial = material;
    }
    
    public Quaternion rotate_bucket(Quaternion rotation)
    {
        //Debug.Log("Y_Bucket_rotation = " + bucket_orientation);
        Vector3 angles = rotation.eulerAngles;
        bucket_orientation += bucket_rotation_speed * Time.fixedDeltaTime;
        angles.x = bucket_orientation;
        rotation.eulerAngles = angles;
        return rotation;
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
        //Debug.Log(bucketMeshFile.name);
        //Debug.Log("Bucket-Orientation: " + bucketMeshFile.transform.rotation.eulerAngles + ". Bucket-Position: " + bucketMeshFile.transform.position);
        //wenn tag parallel und aufrecht zur kamera, enstpricht orientierung (0,0,0)
        //bucket um +90 Grad um x-achse drehen, damit bucket mit boden auf tag steht
        //angles.x -= 90.0f;
        //rotation.eulerAngles = angles;

        
        if (log_cd <= 0.0f)
        {
            Debug.Log("Tag-Orientation: X=" + angles.x + ", Y =" + angles.y + ", Z = " + angles.z + ". Tag-Position: " + position);
            log_cd = log_tick_sec;
        }
        else
        {
            log_cd -= Time.fixedDeltaTime;
        }

        //rotation.Normalize();
        position.y += 0.02f;
        Quaternion rotation_animated = rotate_bucket(rotation); //rotation quaternion treats axes as global
        var xform = Matrix4x4.TRS(position, rotation_animated, Vector3.one * scale);
        //public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, Rendering.ShadowCastingMode castShadows, bool receiveShadows = true, Transform probeAnchor = null, bool useLightProbes = true); 
        if (useBucket)
        {

            for (int i = 0; i < _mesh.subMeshCount; i++)
            {

                Graphics.DrawMesh(_mesh, xform, bucketMeshFile.GetComponent<MeshRenderer>().sharedMaterials[i], 0, null, i);

            }
        }
        else
        {
            Graphics.DrawMesh(_mesh, xform, _sharedMaterial, 0);

        }

    }
    static Mesh BuildCoordinateAxes()
    {
        var axisLength = 1.0f; // Length of the axes
        var axisThickness = 0.02f; // Thickness of the axes

        // Define the vertices for the axes
        var vtx = new Vector3[]
        {
        // X-axis
        new Vector3(0, 0, 0),
        new Vector3(axisLength, 0, 0),

        // Y-axis
        new Vector3(0, 0, 0),
        new Vector3(0, axisLength, 0),

        // Z-axis
        new Vector3(0, 0, 0),
        new Vector3(0, 0, axisLength)
        };

        // Define the indices for the axes
        var idx = new int[]
        {
        0, 1, // X-axis
        2, 3, // Y-axis
        4, 5  // Z-axis
        };

        var mesh = new Mesh();
        mesh.vertices = vtx;
        mesh.SetIndices(idx, MeshTopology.Lines, 0);

        // Apply a line thickness to the axes by scaling them
        Vector3[] scaledVertices = new Vector3[vtx.Length];
        for (int i = 0; i < vtx.Length; i++)
        {
            scaledVertices[i] = Vector3.Scale(vtx[i], new Vector3(axisThickness, axisThickness, axisThickness));
        }
        mesh.vertices = scaledVertices;

        return mesh;
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
