using Godot;
using System;

public partial class Piece : Area3D
{
    public uint ID { get; set; }

    Board board;

    public Board Board
    {
        get => board; set
        {
            board = value;
        }
    }

    public Vector2I BoardPosition
    {
        get
        {
            if (board != null)
            {
                if (board.Pieces.ContainsKey(ID))
                {
                    (var x, var y, var _) = board.Pieces[ID];

                    return new Vector2I((int)x, (int)y);
                }
            }

            return new Vector2I(0, 0);
        }
    }

    public Player OwnerPlayer
    {
        get
        {

            if (board != null)
            {
                if (board.Pieces.ContainsKey(ID))
                {
                    (var x, var y, var p) = board.Pieces[ID];
                    return p;
                }
            }

            return Player.PLAYER1;
        }
    }

    bool hovered;

    public event Action<uint> Hover;
    public event Action<uint> Unhover;
    public event Action<uint> Clicked;


    public override void _Ready()
    {
        var entered = () => OnMouseEntered();

        var exited = () => OnMouseExited();

        InputEventEventHandler clicked = (a, @event, c, d, e) =>
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && hovered)
            {
                OnClicked();
            }
        };

        MouseEntered += entered;
        MouseExited += exited;
        InputEvent += clicked;
        TreeExited += () =>
            {
                MouseEntered -= entered;
                MouseExited -= exited;
                InputEvent -= clicked;
            };

    }

    void OnMouseEntered()
    {
        if (!hovered)
        {
            hovered = true;
            Hover?.Invoke(ID);
        }
    }
    void OnMouseExited()
    {
        if (hovered)
        {
            hovered = false;
            Unhover?.Invoke(ID);
        }
    }

    void OnClicked()
    {
        Clicked?.Invoke(ID);
    }







    private Tween _movementTween;

    public void MoveInArc(
        Vector3 destination,
        float duration = 1.0f,
        float arcHeight = 3.0f,
        Action onCompleted = null)
    {
        // Cancel existing movement.
        if (_movementTween != null && _movementTween.IsRunning())
            _movementTween.Kill();

        Vector3 start = GlobalPosition;

        // Calculate control point.
        Vector3 control = (start + destination) / 2.0f;
        control.Y += arcHeight;

        _movementTween = CreateTween();

        _movementTween.SetTrans(Tween.TransitionType.Sine);
        _movementTween.SetEase(Tween.EaseType.InOut);

        // Interpolate t from 0 to 1.
        _movementTween.TweenMethod(
            Callable.From<float>(t =>
            {
                float invT = 1.0f - t;

                GlobalPosition =
                    invT * invT * start +
                    2.0f * invT * t * control +
                    t * t * destination;
            }),
            0.0f,
            1.0f,
            duration
        );

        // Completion callback.
        if (onCompleted != null)
        {
            _movementTween.TweenCallback(
                Callable.From(onCompleted)
            );
        }
    }


    public void Disappear(float duration = 0.5f, Action onCompleted = null)
    {
        Tween tween = CreateTween();

        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.In);

        tween.TweenProperty(this, "scale", Vector3.Zero, duration);

        if (onCompleted != null)
        {
            tween.TweenCallback(Callable.From(onCompleted));
        }
    }

}
