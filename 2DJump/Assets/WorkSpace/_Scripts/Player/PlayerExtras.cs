using UnityEngine;

public struct FrameInput {
    public float X,Y;
    public bool JumpDown;
    public bool JumpUp;
    public bool Dash;
}

public interface IPlayerController 
{
    public Vector3 Velocity { get; }
    public FrameInput Input { get; }
    public bool JumpingThisFrame { get; }
    public bool FallingThisFrame { get; }
    public bool LandingThisFrame { get; }
    public bool DashingThisFrame { get; set; }
    public bool AirDashingThisFrame { get; set; }
    public bool AttackingThisFrame { get; set; }
    public bool LaidoingThisFrame { get; set; }
    public bool SwordDancingThisFrame { get; set; }
    public bool FuryCutteringThisFrame { get; set; }
    public bool HammeringThisFrame { get; set; }
    public bool DrillingThisFrame { get; set; }
    public bool UpwardDeriveThisFrame { get; set; }
    public bool DownDeriveThisFrame { get; set; }
    public bool GuardingThisFrame { get; set; }
    public bool TransmitThisFrame { get; set; }
    public bool ReceiveThisFrame { get; set; }
    public bool RangedAttackingThisFrame { get; set; }
    public Vector3 RawMovement { get; }
    public bool Grounded { get; }
    public int ComboStep { get; }
    public bool AttackOver { get; }
    public bool Dead{ get; set; }
}

public struct RayRange 
{
    public RayRange(float x1, float y1, float x2, float y2, Vector2 dir) 
    {
        Start = new Vector2(x1, y1);
        End = new Vector2(x2, y2);
        Dir = dir;
    }

    public readonly Vector2 Start, End, Dir;
}
