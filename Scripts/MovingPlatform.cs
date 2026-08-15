using Godot;
using System;
using static Godot.CameraFeed;

public partial class MovingPlatform : StaticBody3D
{
    [Export]
    public float Speed = 1.0f;

    [Export]
    public Vector3 EndOffset;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _time;

    public override void _Ready()
	{
        _startPosition = Position;
        _endPosition = _startPosition + EndOffset;
    }


	public override void _Process(double delta)
	{
        _time += (float)delta;

        float t = (1.0f - Mathf.Cos(_time * Speed)) * 0.5f;

        Position = _startPosition.Lerp(_endPosition, t);
    }
}
