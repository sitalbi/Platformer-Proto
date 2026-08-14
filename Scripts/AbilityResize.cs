using Godot;
using System;

public partial class AbilityResize : Node
{
	[Export]
	public PlayerController PlayerController;

    [Export]
    public CollisionShape3D CollisionShape;

    [Export]
    public PlayerCamera Camera;


    [Export]
    public Vector3 MiniCameraPosition = new Vector3(0, 0.8f, 2.0f);

    [Export]
    public float CameraZoomSpeed = 5.0f;

    [Export]
    public float MiniSizeMultiplier = 2.0f;

    [Export]
    public float MiniSpeedMultiplier = 1.5f;

    [Export]
    public float MiniJumpMultiplier = 2.0f;

    private bool isMini = false;

    private Vector3 _originalModelScale;
    private float _originalMaxSpeed;
    private float _originalJumpVelocity;
    private float _originalJumpGravityMultiplier;
    private float _originalFallGravityMultiplier;

    private float _originalCapsuleHeight;
    private float _originalCapsuleRadius;

    private CapsuleShape3D _capsule;

    private Vector3 _originalCollisionPosition;

    private Vector3 _normalCameraPosition;


    public override void _Ready()
	{
        _originalModelScale = PlayerController.Model.Scale;
        _originalMaxSpeed = PlayerController.MaxSpeed;
        _originalJumpVelocity = PlayerController.JumpVelocity;
        _originalJumpGravityMultiplier = PlayerController.JumpGravityMultiplier;
        _originalFallGravityMultiplier = PlayerController.FallGravityMultiplier;

        _capsule = CollisionShape.Shape as CapsuleShape3D;

        _originalCapsuleHeight = _capsule.Height;
        _originalCapsuleRadius = _capsule.Radius;

        _originalCollisionPosition = CollisionShape.Position;

        _normalCameraPosition = new Vector3(0, Camera.Height, Camera.Distance);
    }

	public override void _PhysicsProcess(double delta)
	{
        if (Input.IsActionJustPressed("effect"))
        {

            GD.Print("isMini " + isMini);
            if (isMini)
            {

                PlayerController.Model.Scale = _originalModelScale;
                PlayerController.MaxSpeed = _originalMaxSpeed;

                PlayerController.JumpVelocity = _originalJumpVelocity;
                PlayerController.JumpGravityMultiplier = _originalJumpGravityMultiplier;
                PlayerController.FallGravityMultiplier = _originalFallGravityMultiplier;

                _capsule.Height = _originalCapsuleHeight;
                _capsule.Radius = _originalCapsuleRadius;

                CollisionShape.Position = _originalCollisionPosition;

                Camera.TargetHeight = _normalCameraPosition.Y;
                Camera.TargetDistance = _normalCameraPosition.Z;
            } 
            else
            {
                float scale = 1.0f / MiniSizeMultiplier;

                PlayerController.Model.Scale = _originalModelScale * scale;
                PlayerController.MaxSpeed = _originalMaxSpeed * 1.0f / MiniSpeedMultiplier;

                PlayerController.JumpVelocity = _originalJumpVelocity * Mathf.Sqrt(1.0f /MiniJumpMultiplier);
                PlayerController.JumpGravityMultiplier = _originalJumpGravityMultiplier * Mathf.Sqrt(1.0f /MiniJumpMultiplier);
                PlayerController.FallGravityMultiplier = _originalFallGravityMultiplier * Mathf.Sqrt(1.0f /MiniJumpMultiplier);

                _capsule.Height = _originalCapsuleHeight * scale;
                _capsule.Radius = _originalCapsuleRadius * scale;

                float heightDifference = _originalCapsuleHeight - _capsule.Height;

                Vector3 position = _originalCollisionPosition;
                position.Y -= heightDifference * 0.5f;

                CollisionShape.Position = position;

                Camera.TargetHeight = MiniCameraPosition.Y;
                Camera.TargetDistance = MiniCameraPosition.Z;
            }

            isMini = !isMini;
        }
    }
}
