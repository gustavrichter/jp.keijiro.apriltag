using UnityEngine;

sealed class TagDrawer : System.IDisposable
{
    Mesh _mesh;
    Material _sharedMaterial;
    Mesh[] axisMeshes;
    Vector3[] axes;
    Color[] axesColors;
    Material[] axesMaterials;
 
    public TagDrawer(Material material)
    {
        axisMeshes = new Mesh[3];
        axes = new Vector3[3];
        axesColors = new Color[3];
        axesMaterials = new Material[3];

        axes[0] = Vector3.right;
        axes[1] = Vector3.up;
        axes[2] = - Vector3.forward;

        axesColors[0] = Color.red;
        axesColors[1] = Color.green;
        axesColors[2] = Color.blue;

        axesMaterials[0] = Resources.Load("Wire 1") as Material;
        axesMaterials[1] = Resources.Load("Wire 2") as Material;
        axesMaterials[2] = Resources.Load("Wire 3") as Material;

        for (int i = 0; i < 3; i++)
        {
            axisMeshes[i] = BuildAxisMesh(axes[i], axesColors[i]);
        }
        _mesh = BuildMesh();
        _sharedMaterial = material;
    }

    public void Dispose()
    {
        Object.Destroy(_mesh);
        _mesh = null;
        _sharedMaterial = null;
    }

    public void Draw(int id, Vector3 position, Quaternion rotation, float scale)
    {
        var xform = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
        for (int i = 0; i < 3; i++)
        {
            Graphics.DrawMesh(axisMeshes[i], xform, axesMaterials[i], 0);
        }
        //Graphics.DrawMesh(_mesh, xform, _sharedMaterial, 0);
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

    static Mesh BuildAxisMesh(Vector3 direction, Color color)
    {
        var axisLength = 50.0f; // Length of the axes
        var axisThickness = 0.1f; // Thickness of the axes

        // Define the vertices for the axis
        var vtx = new Vector3[]
        {
            Vector3.zero,
            direction * axisLength
        };

        // Define the indices for the axis
        var idx = new int[]
        {
            0, 1
        };

        var mesh = new Mesh();
        mesh.vertices = vtx;
        mesh.SetIndices(idx, MeshTopology.Lines, 0);

        // Apply a line thickness to the axis by scaling it
        Vector3[] scaledVertices = new Vector3[vtx.Length];
        for (int i = 0; i < vtx.Length; i++)
        {
            scaledVertices[i] = Vector3.Scale(vtx[i], new Vector3(axisThickness, axisThickness, axisThickness));
        }
        mesh.vertices = scaledVertices;

        // Assign a color to the axis
        Color[] colors = new Color[vtx.Length];
        for (int i = 0; i < vtx.Length; i++)
        {
            colors[i] = color;
        }
        mesh.colors = colors;

        return mesh;
    }
}
