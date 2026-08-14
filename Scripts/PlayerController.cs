using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	public const float TurnSpeed = 8.0f;

	private float _gravityMultiplier = 1.0f;
	private bool _jumpRelease = false;

    private float _jumpBufferTimer;

    [Export]
	public Camera3D Camera { get; set; }

    [Export]
    public Node3D Model { get; set; }

    [Export]
    public float MaxSpeed = 5.0f;

    [Export]
    public float GroundedAcceleration = 15.0f;

    [Export]
    public float GroundedTurnAcceleration = 30.0f;

    [Export]
    public float GroundedDeceleration = 15.0f;

    [Export]
    public float AirAcceleration = 6.0f;

    [Export]
    public float AirDeceleration = 2.0f;

    [Export]
    public float JumpVelocity { get; set; }

    [Export]
    public float JumpGravityMultiplier { get; set; }
    [Export]
    public float FallGravityMultiplier { get; set; }
    [Export]
    public float JumpBufferDuration { get; set; }

    public float HorizontalSpeed => new Vector2(Velocity.X, Velocity.Z).Length();

    public float NormalizedHorizontalSpeed => Mathf.Clamp(HorizontalSpeed / MaxSpeed, 0.0f, 1.0f);

    private Vector2 _inputDir;

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


        Vector3 direction = GetMovementDirection();
        if (IsOnFloor())
        {
            GroundedMovement(ref velocity, direction, delta);
        } else
        {
            AirMovement(ref velocity, direction, delta);
        }

        GravityModifier(ref velocity, delta);

        Velocity = velocity;
		MoveAndSlide();
	}

	private void Jump(ref Vector3 velocity)
	{
        velocity.Y = JumpVelocity;
    }


    private void GroundedMovement(ref Vector3 velocity, Vector3 direction, double delta)
    {

        Vector2 horizontalVelocity = new Vector2(velocity.X, velocity.Z);

        Vector2 targetVelocity = new Vector2(direction.X * MaxSpeed, direction.Z * MaxSpeed);

        if (direction != Vector3.Zero)
        {
            Vector2 currentDir = horizontalVelocity.Normalized();
            Vector2 desiredDir = new Vector2(direction.X, direction.Z).Normalized();

            float alignment = currentDir.Dot(desiredDir);

            float acceleration = GroundedAcceleration;

            if (horizontalVelocity.Length() > 0.1f && alignment < 0.0f)
            {
                acceleration = GroundedTurnAcceleration;
            }

            horizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, acceleration * (float)delta);

            float targetYaw = Mathf.Atan2(direction.X, direction.Z);

            Vector3 modelRotation = Model.Rotation;

            modelRotation.Y = Mathf.LerpAngle(
                modelRotation.Y,
                targetYaw,
                TurnSpeed * (float)delta
            );

            Model.Rotation = modelRotation;
        } 
        else
        {
            horizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, GroundedDeceleration * (float)delta);
        }

        velocity.X = horizontalVelocity.X;
        velocity.Z = horizontalVelocity.Y;
    }

    private void AirMovement(ref Vector3 velocity, Vector3 direction, double delta)
    {
        Vector2 horizontalVelocity = new Vector2(velocity.X, velocity.Z);

        Vector2 targetVelocity = new Vector2(direction.X * MaxSpeed, direction.Z * MaxSpeed);
        
        horizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, AirAcceleration * (float)delta);

        float targetYaw = Mathf.Atan2(direction.X, direction.Z);

        if(direction == Vector3.Zero)
        {
            horizontalVelocity = horizontalVelocity.MoveToward(Vector2.Zero, AirDeceleration * (float)delta);
        }
        else
        {
            Vector3 modelRotation = Model.Rotation;

            modelRotation.Y = Mathf.LerpAngle(
                modelRotation.Y,
                targetYaw,
                TurnSpeed * (float)delta
            );

            Model.Rotation = modelRotation;
        }


            velocity.X = horizontalVelocity.X;
        velocity.Z = horizontalVelocity.Y;
    }

    private void GravityModifier(ref Vector3 velocity, double delta)
    {
        if (!IsOnFloor())
        {
            Vector3 gravity = GetGravity();
            if (velocity.Y > 0.1f)
            {
                if (_jumpRelease)
                {
                    _gravityMultiplier = FallGravityMultiplier;
                } else
                {
                    _gravityMultiplier = JumpGravityMultiplier;
                }
            } else
            {
                _gravityMultiplier = FallGravityMultiplier;
            }
            velocity += gravity * _gravityMultiplier * (float)delta;
        }
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 forward = -Camera.GlobalBasis.Z;
        Vector3 right = Camera.GlobalBasis.X;

        forward.Y = 0;
        right.Y = 0;

        forward = forward.Normalized();
        right = right.Normalized();

        return (right * _inputDir.X + forward * -_inputDir.Y).Normalized();
    }
}
