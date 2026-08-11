using Godot;

enum AnimationState
{
    Idle,
    Move,
    Jump,
    Fall
};

public partial class AnimationController : Node
{
    [Export]
    public CharacterBody3D Player { get; set; }

    [Export]
    public Node3D Model { get; set; }

    [Export]
    public float MaxRunSpeed { get; set; } = 5.0f;

    private AnimationState _currentState = AnimationState.Idle;

    public override void _PhysicsProcess(double delta)
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Vector3 velocity = Player.Velocity;

        float horizontalSpeed = new Vector2(velocity.X, velocity.Z).Length();

        AnimationState desiredState;

        if (Player.IsOnFloor())
        {
            if (horizontalSpeed > 0.1f)
            {
                desiredState = AnimationState.Move;

                float blend = Mathf.Clamp(horizontalSpeed / MaxRunSpeed, 0.0f, 1.0f);

                Model.Set("walk_run_blending", blend);
            } 
            else
            {
                desiredState = AnimationState.Idle;
            }
        } 
        else
        {
            if (velocity.Y > 0.0f)
            {
                desiredState = AnimationState.Jump;
            } 
            else
            {
                desiredState = AnimationState.Fall;
            }
        }

        SetAnimationState(desiredState);
    }

    private void SetAnimationState(AnimationState newState)
    {
        if (_currentState == newState)
        {
            return;
        }

        _currentState = newState;

        switch (_currentState)
        {
            case AnimationState.Idle:
                Model.Call("idle");
                break;

            case AnimationState.Move:
                Model.Call("move");
                break;

            case AnimationState.Jump:
                Model.Call("jump");
                break;

            case AnimationState.Fall:
                Model.Call("fall");
                break;
        }
    }
}