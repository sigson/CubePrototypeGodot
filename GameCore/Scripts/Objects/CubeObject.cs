using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Drawing;

public partial class CubeObject : RigidBody3D
{
    [Export] public MeshInstance3D CubeMesh;
    [Export] public CollisionShape3D CubeCollider;
    public BoxShape3D CubeShape => CubeCollider.Shape as BoxShape3D;
    [Export]
	public Array<Label3D> labels = new Array<Label3D>();
	private int _value;
	[Export]
	public int Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			labels.ForEach(x => x.Text = value.ToString());
		}
	}

    private Dictionary<int, float> countNumToTextSizeComparsion = new Dictionary<int, float> {
        {1, 1f },
        {2, 0.7f },
        {3, 0.5f },
        {4, 0.4f },
        {5, 0.3f },
        {6, 0.25f },
        {7, 0.2f },
        {8, 0.18f },
        {9, 0.17f },
        {10, 0.15f },
        {11, 0.14f },
        {12, 0.13f },
        {13, 0.12f },
        {14, 0.11f },
        {15, 0.10f },
    };

    public void UpdateCube(Godot.Color color)
    {
        this.CubeMesh.MaterialOverride._Set("color", color);
        //var boxColl = this.GetComponent<BoxCollider>();
        //boxColl.enabled = false;
        //boxColl.enabled = true;
        //cubeMaterial.SetColor("_Color", color);
        //textArray.ForEach((text) =>
        //{
        //    text.text = value.ToString();
        //    text.characterSize = countNumToTextSizeComparsion[value.ToString().Length];
        //});
    }
}
