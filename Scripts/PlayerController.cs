using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float TurnSpeed = 8.0f;

	private float _gravityMultiplier = 1.0f;
	private bool _jumpRelease = false;

    private float _jumpBufferTimer;

    [Export]
	public Node3D Camera { get; set; }

    [Export]
    public Node3D Model { get; set; }

    [Export]
    public float JumpVelocity { get; set; }

    [Export]
    public float jumpGravityMultiplier { get; set; }
    [Export]
    public float fallGravityMultiplier { get; set; }
    [Export]
    public float JumpBufferDuration { get; set; }

    private Vector2 _inputDir;
	private bool _canJump;

    public override void _Process(double delta)
    {
        _inputDir = Input.GetVector("left", "right", "forward", "backward");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

        // Handle Jump
        if (Input.IsActionJustPressed("jump"))
        {
            _jumpBufferTimer = JumpBufferDuration;
            _jumpRelease = false;
        }

        if (Input.IsActionJustReleased("jump"))
        {
			_jumpRelease = true;
        }

        if (_jumpBufferTimer > 0.0f)
        {
            _jumpBufferTimer = Math.Max(0, _jumpBufferTimer - (float)delta);
        }

        if (_jumpBufferTimer > 0.0f && IsOnFloor())
        {
            Jump(ref velocity);
            _jumpBufferTimer = 0.0f;
        }


        Vector3 direction = (Camera.GlobalBasis * new Vector3(_inputDir.X, 0, _inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;

			// Rotate to face direction
            float targetYaw = Mathf.Atan2(direction.X, direction.Z);

            Vector3 modelRotation = Model.Rotation;

            modelRotation.Y = Mathf.LerpAngle(modelRotation.Y, targetYaw, TurnSpeed * (float)delta);

            Model.Rotation = modelRotation;
        }
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

        if (!IsOnFloor())
        {
            Vector3 gravity = GetGravity();
            if (Velocity.Y > 0.1f)
            {
                if (_jumpRelease)
                {
                    _gravityMultiplier = fallGravityMultiplier;
                } 
                else
                {
                    _gravityMultiplier = jumpGravityMultiplier;
                }
            } 
            else
            {
                _gravityMultiplier = fallGravityMultiplier;
            }
            velocity += gravity * _gravityMultiplier * (float)delta;
        }

        Velocity = velocity;
		MoveAndSlide();
	}

	private void Jump(ref Vector3 velocity)
	{
        velocity.Y = JumpVelocity;
    }
}
