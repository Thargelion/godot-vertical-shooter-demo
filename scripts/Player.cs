using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
    private Node2D _muzzle;
    private int _fireRate = 250;
    public const string Group = "player";
    private const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;
    private PackedScene _laserScene = ResourceLoader.Load(Laser.Path) as PackedScene;
    private bool _isCannonHot;

    [Export(PropertyHint.Range, "0,10,1,or_greater")]
    public int Health { get; set; } = 40;

    [Signal]
    public delegate void LaserShotEventHandler(PackedScene laserScene, Vector2 location);

    [Signal]
    public delegate void KilledEventHandler();

    // Get the Gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("quit"))
        {
            GetTree().Quit();
        }
        else if (Input.IsActionJustPressed("reset"))
        {
            GetTree().ReloadCurrentScene();
        }
        else if (Input.IsActionPressed("shoot"))
        {
            PullTrigger();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        this._muzzle = GetNode("Muzzle") as Node2D;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        this.Velocity = direction * Speed;
        MoveAndSlide();
        GlobalPosition = GlobalPosition.Clamp(Vector2.Zero, GetViewportRect().Size);
    }

    private void PullTrigger()
    {
        if (_isCannonHot) return;
        EmitSignal(SignalName.LaserShot, _laserScene, this._muzzle.GlobalPosition);
        this._isCannonHot = true;
        Task.Run(() =>
        {
            Thread.Sleep(this._fireRate);
            this._isCannonHot = false;
        });
    }

    public void Harm(double damage)
    {
        this.Health -= (int)damage;
        if (this.Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EmitSignal(SignalName.Killed);
        QueueFree();
    }
}