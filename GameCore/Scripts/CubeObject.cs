using Godot;
using System;
using Godot.Collections;
using System.Linq;

public partial class CubeObject : Node
{
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
}
