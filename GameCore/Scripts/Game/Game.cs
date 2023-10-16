using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

public partial class Game : Node
{
    InputEx input;

	[Export]
	public Node3D DockObject;
    [Export]
    public DockObject Dock;
    [Export]
    public Node3D DockCubeRoot;

    public CubeObject CubeExample;
    [Export]
    public Camera3D Camera;

    [Export] public float CubeBlockSizeX = 1;
    [Export] public float CubeBlockSizeY = 1;
    [Export] public float CubeBlockSizeZ = 1;

    [Export] bool GameLoaded;
    HashSet<CubeObject> GameCubes = new HashSet<CubeObject>();


    private float MaxCubeMovementBoundLeft;
    private float MaxCubeMovementBoundRight;
    private float MaxCubeMovementBoundDistance => MaxCubeMovementBoundRight - MaxCubeMovementBoundLeft;
    public override void _Ready()
    {
        input = InputEx.Init(this);

        CubeExample = ResourceLoader.Load<PackedScene>("res://GameCore/Prefabs/Cube.tscn").Instantiate() as CubeObject;
        
        ReloadGame();
        Camera.GlobalPosition = Dock.PlayerCameraBinding.GlobalPosition;

        MaxCubeMovementBoundLeft = GetViewport().GetCamera3D().UnprojectPosition(Dock.PlayerBorderForwardLeft.GlobalPosition).X;
        MaxCubeMovementBoundRight = GetViewport().GetCamera3D().UnprojectPosition(Dock.PlayerBorderForwardRight.GlobalPosition).X;
        cacheLeftCubeBoundPositon = Dock.PlayerBorderForwardLeft.GlobalPosition;
        cacheRightCubeBoundPositon = Dock.PlayerBorderForwardRight.GlobalPosition;

        ShowGame();
    }

    public void ReloadGame()
    {
        if(DockObject.GetChildCount() == 0) 
        {
            Dock = ResourceLoader.Load<PackedScene>("res://GameCore/Prefabs/PlayDock.tscn").Instantiate() as DockObject;
            DockObject.AddChild(Dock);
        }
    }

    public void PrepareBorders()
    {
        var leftBorder = Dock.PlayerBorderBackgroundLeft.GlobalPosition.Set(null, 0, 0);
        var leftBlock = Dock.BackBlock.GlobalPosition.Set(null, 0, 0);
        var rightBorder = Dock.PlayerBorderBackgroundRight.GlobalPosition.Set(null, 0, 0);
        var cubeBounds = CubeExample.CubeShape.Size * CubeExample.CubeCollider.Scale;

    }

    public void ShowGame()
    {
        var cubeBounds = CubeExample.CubeShape.Size * CubeExample.CubeCollider.Scale;
        Dock.CubeSpace.Position = Vector3.Zero;
        for (float x = 1; x < CubeBlockSizeX + 1; x++)
        {
            var xmodifier = Dock.PlayerBorderBackgroundLeft.GlobalPosition.MoveTowardDistance(Dock.PlayerBorderBackgroundRight.GlobalPosition, x / (CubeBlockSizeX + 1));
            for (float y = 1; y < CubeBlockSizeY+1; y++)
            {
                var ymodifier = (cubeBounds.Y + cubeBounds.Y/4)*y;
                for (float z = 1; z < CubeBlockSizeZ+1; z++)
                {
                    var zmodifier = Dock.PlayerBorderBackgroundLeft.GlobalPosition.MoveTowardDistance(Dock.PlayerBorderCenterLeft, z / (CubeBlockSizeZ + 1));

                    var newCube = ResourceLoader.Load<PackedScene>("res://GameCore/Prefabs/Cube.tscn").Instantiate() as CubeObject;
                    Dock.CubeSpace.AddChild(newCube);
                    GameCubes.Add(newCube);
                    newCube.GlobalPosition = new Vector3(xmodifier.X, ymodifier, zmodifier.Z);
                }
            }
        }
    }

    private Vector3 cacheLeftCubeBoundPositon;
    private Vector3 cacheRightCubeBoundPositon;
    private Vector3 shootableCubePosition;
    private bool onTimeout;

    private void GameInput()
    {
        if (!GameLoaded)
            return;
        var ShootCube = ResourceLoader.Load<PackedScene>("res://GameCore/Prefabs/Cube.tscn").Instantiate() as CubeObject;
        if (OS.GetName() == "Windows")
        {
            var pressed = Input.Singleton.IsMouseButtonPressed(MouseButton.Left);
            //Input.Singleton.
            if (pressed && ShootCube != null)
            {
                var nowMouse = GetViewport().GetMousePosition();

                var viewportSize = GetViewport().GetVisibleRect().Size;

                var screenWidth = (float)viewportSize.X;
                var screenHeight = (float)viewportSize.Y;

                Vector2 pos = nowMouse;
                if (pos.X >= MaxCubeMovementBoundLeft && pos.X <= MaxCubeMovementBoundRight)
                {
                    float coefDistance = (pos.X - MaxCubeMovementBoundLeft) / MaxCubeMovementBoundDistance;
                    var procentPosition = coefDistance < 0.5f ? (0.5f - coefDistance) * -1 : coefDistance - 0.5f;
                    shootableCubePosition = new Vector3(cacheLeftCubeBoundPositon.Lerp(cacheRightCubeBoundPositon, coefDistance).X, shootableCubePosition.Y, shootableCubePosition.Z);
                }

                ShootCube.GlobalPosition = shootableCubePosition;
            }

            //if (!onTimeout && Input.GetMouseButtonUp(0) && ShootCube != null)
            //{
            //    ShootCube.GetComponent<Rigidbody>().AddForce(Vector3.forward * ShootForce);
            //    ShootCube = null;
            //    onTimeout = true;
            //    pressed = false;
            //    StartCoroutine("TimeoutWaiter");
            //}
        }

        //if(OS.GetName() == "Android")
        //{
        //    if (Input.touchCount > 0 && ShootCube != null)
        //    {
        //        Touch touch = Input.GetTouch(0);

        //        var screenWidth = (float)Screen.width;
        //        var screenHeight = (float)Screen.height;
        //        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
        //        {
        //            var nowMouse = touch.position;

        //            Vector2 pos = nowMouse;
        //            if (pos.x >= MaxCubeMovementBoundLeft && pos.x <= MaxCubeMovementBoundRight)
        //            {
        //                float coefDistance = (pos.x - MaxCubeMovementBoundLeft) / MaxCubeMovementBoundDistance;
        //                var procentPosition = coefDistance < 0.5f ? (0.5f - coefDistance) * -1 : coefDistance - 0.5f;
        //                shootableCubePosition = new Vector3(Vector3.Lerp(cacheLeftCubeBoundPositon, cacheRightCubeBoundPositon, coefDistance).x, shootableCubePosition.y, shootableCubePosition.z);
        //            }

        //            ShootCube.transform.position = shootableCubePosition;
        //        }

        //        if (!Timeout && touch.phase == TouchPhase.Ended)
        //        {
        //            ShootCube.GetComponent<Rigidbody>().AddForce(Vector3.forward * ShootForce);
        //            ShootCube = null;
        //            Timeout = true;
        //            pressed = false;
        //            StartCoroutine("TimeoutWaiter");
        //        }
        //    }
        //}

        

        //if (!Timeout && ShootCube == null)
        //{
        //    ShowNewShootCube();
        //    OnShotEvent();
        //}
    }
}
