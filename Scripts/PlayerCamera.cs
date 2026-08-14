using Godot;

public partial class PlayerCamera : Node3D
{
    public const float sensitivity = 2.0f;

    [Export]
    public PlayerController Player;

    [Export]
    public Camera3D Camera;

    [Export]
    public float Distance = 5.0f;

    public float TargetDistance;

    [Export]
    public float Height = 1.0f;

    [Export]
    public float ZoomTransitionSpeed = 5.0f;

    [Export]
    public float VerticalTransitionSpeed = 5.0f;

    [Export]
    public float VerticalFollowSpeed = 2.0f;

    [Export]
    public float TargetScreenY = 0.6f;

    [Export]
    public float VerticalDeadzonePixels = 100.0f;

    public float TargetHeight;

    private float _yaw;
    private float _pitch;

    private float _followY;
    private float _targetFollowY;

    public override void _Ready()
    {
        TargetDistance = Distance;
        TargetHeight = Height;

        _followY = Player.GlobalPosition.Y;
        _targetFollowY = Player.GlobalPosition.Y;
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

        _pitch = Mathf.Clamp(_pitch, Mathf.DegToRad(-80.0f), Mathf.DegToRad(80.0f));

        float zoomT = Mathf.Clamp(ZoomTransitionSpeed * (float)delta, 0.0f, 1.0f);

        Distance = Mathf.Lerp(Distance, TargetDistance, zoomT);

        Height = Mathf.Lerp(Height, TargetHeight, zoomT );

        Vector3 target = new Vector3(Player.GlobalPosition.X, _followY + Height, Player.GlobalPosition.Z);

        Vector3 orbitDirection = new Vector3(Mathf.Sin(_yaw) * Mathf.Cos(_pitch), Mathf.Sin(_pitch), Mathf.Cos(_yaw) * Mathf.Cos(_pitch));

        Camera.GlobalPosition = target + orbitDirection * Distance;

        Camera.LookAt(target, Vector3.Up);

        UpdateVerticalFollow(delta);

        float verticalT = Mathf.Clamp(VerticalTransitionSpeed * (float)delta, 0.0f, 1.0f);

        _followY = Mathf.Lerp(_followY, _targetFollowY, verticalT);
    }

    private void UpdateVerticalFollow(double delta)
    {
        Vector2 playerScreenPosition = Camera.UnprojectPosition(Player.GlobalPosition);

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

        float desiredScreenY = viewportSize.Y * TargetScreenY;

        float verticalError = playerScreenPosition.Y - desiredScreenY;

        if (Player.IsOnFloor())
        {
            _targetFollowY = Player.GlobalPosition.Y;
            return;
        }

        if (verticalError < -VerticalDeadzonePixels)
        {
            _targetFollowY += VerticalFollowSpeed * (float)delta;
        } 
        else if (verticalError > VerticalDeadzonePixels)
        {
            _targetFollowY -= VerticalFollowSpeed * (float)delta;
        }
    }
}