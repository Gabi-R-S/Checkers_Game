using Godot;
using System;

public partial class Piece : Area3D
{
    public uint ID { get; set; }

    Board board;
    
    public Board Board {get=>board; set
        {
            board = value;
        }
    }
    
    public Vector2I BoardPosition {
        get
        {
            if(board != null)
            {
                if (board.Pieces.ContainsKey(ID))
                {
                    (var x, var y, var _) = board.Pieces[ID];

                    return new Vector2I((int) x,(int) y);
                }
            }

            return new Vector2I(0,0);
        } 
    }
    
    public Player OwnerPlayer { get {
            
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
}
