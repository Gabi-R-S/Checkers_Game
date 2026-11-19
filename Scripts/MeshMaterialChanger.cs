using Godot;
using System;

public partial class MeshMaterialChanger : MeshInstance3D, MaterialChanger
{
    [Export]
    public Material NormalMaterial {get; protected set;}

    [Export]
    public Material HighlightedMaterial {get; protected set;}

    [Export]
    public Material HoveredMaterial {get; protected set;}

    [Export]
    public Material SelectedMaterial {get; protected set;}


    public 
    void SetMaterial(MaterialChangerMaterial material)
    {
        switch (material)
        {
            case MaterialChangerMaterial.NORMAL:
            SetSurfaceOverrideMaterial(0,NormalMaterial);
            break;

            case MaterialChangerMaterial.HIGHLIGHTED:
            SetSurfaceOverrideMaterial(0,HighlightedMaterial);
            break;

            case MaterialChangerMaterial.HOVERED:
            SetSurfaceOverrideMaterial(0,HoveredMaterial);
            break;

            case MaterialChangerMaterial.SELECTED:
            SetSurfaceOverrideMaterial(0,SelectedMaterial);
            break;

            default:
            break;
        }
    }

}
