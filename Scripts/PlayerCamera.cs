using Godot;
using System;
using System.Diagnostics;

public partial class PlayerCamera : Node3D
{
    public const float sensitivity = 2.0f;

    [Export]
    public Node3D Player;

    [Export]
    public Camera3D Camera;

    [Export]
    public float Distance = 5.0f;

    public float TargetDistance;

    [Export]
    public float Height = 1.0f;

    [Export]
    public float TransitionSpeed;

    public float TargetHeight;

    private float _yaw;
    private float _pitch;

    public override void _Ready()
    {
        TargetDistance = Distance;
        TargetHeight = Height;
    }

    public override void _Process(double delta)
	{
        Vector2 cameraInput = Input.GetVector(
            "camera_left",
            "camera_right",
            "camera_up",
            "camera_down"
        );

        _yaw -= cameraInput.X * sensitivity * (float)delta;
        _pitch += cameraInput.Y * sensitivity * (float)delta;

        _pitch = Mathf.Clamp(
            _pitch,
            Mathf.DegToRad(-80.0f),
            Mathf.DegToRad(80.0f)
        );

        Distance = Mathf.Lerp(Distance, TargetDistance, TransitionSpeed * (float)delta);

        Height = Mathf.Lerp(Height, TargetHeight, TransitionSpeed * (float)delta);

        Vector3 target = Player.GlobalPosition + Vector3.Up * Height;

        Vector3 orbitDirection = new Vector3(Mathf.Sin(_yaw) * Mathf.Cos(_pitch), Mathf.Sin(_pitch), Mathf.Cos(_yaw) * Mathf.Cos(_pitch));

        Camera.GlobalPosition = target + orbitDirection * Distance;

        Camera.LookAt(target, Vector3.Up);
    }
}
