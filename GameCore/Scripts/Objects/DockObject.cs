using Godot;
using System;

public partial class DockObject : Node
{
    [Export]
    public Node3D RightBlock;
    [Export]
    public Node3D LeftBlock;
    [Export]
    public Node3D BackBlock;

    [Export]
	public Node3D PlayerBorderForwardLeft;
	[Export]
	public Node3D PlayerBorderForwardRight;

    public Vector3 PlayerBorderCenterLeft => PlayerBorderBackgroundLeft.GlobalPosition.MoveTowardDistance(PlayerBorderForwardLeft.GlobalPosition, 0.5f);
    public Vector3 PlayerBorderCenterRight => PlayerBorderBackgroundRight.GlobalPosition.MoveTowardDistance(PlayerBorderForwardRight.GlobalPosition, 0.5f);
    [Export]
    public Node3D PlayerBorderBackgroundLeft;
    [Export]
    public Node3D PlayerBorderBackgroundRight;
    [Export]
	public Node3D PlayerCameraBinding;
    [Export]
    public Node3D CubeSpace;
    [Export] public Node3D Table;
}
