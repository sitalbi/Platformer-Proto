using Godot;
using System;

public partial class PlayerCamera : Node3D
{
    public const float sensitivity = 1.0f;


    private Camera3D Camera;
	private Node3D CameraYaw;
	private Node3D CameraPitch;

	public override void _Ready()
	{
        Camera = GetNode<Camera3D>("CameraYaw/CameraPitch/Camera3D");
        CameraYaw = GetNode<Node3D>("CameraYaw"); 
        CameraPitch = GetNode<Node3D>("CameraYaw/CameraPitch");
    }


	public override void _Process(double delta)
	{
        Vector2 cameraInput = Input.GetVector(
            "camera_left",
            "camera_right",
            "camera_up",
            "camera_down"
        );

       CameraYaw.RotateY(-cameraInput.X * sensitivity * (float)delta);
       CameraPitch.RotateX(-cameraInput.Y * sensitivity * (float)delta);

        Vector3 rotation = CameraPitch.Rotation;

        rotation.X = Mathf.Clamp(
            rotation.X,
            Mathf.DegToRad(-80.0f),
            Mathf.DegToRad(80.0f)
        );

        CameraPitch.Rotation = rotation;
    }
}
