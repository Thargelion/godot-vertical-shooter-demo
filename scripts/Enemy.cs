using Godot;

public partial class Enemy : Area2D
{
    public const string Group = "enemy";

    [Export(PropertyHint.Range, "0,600,1,or_greater")]
    public float Speed { get; set; } = 100.0f;

    [Export(PropertyHint.Range, "0,600,1,or_greater")]
    public uint Value { get; set; } = 100;

    [Export(PropertyHint.Range, "0,10,1,or_greater")]
    public int Health { get; set; } = 20;

    [Export(PropertyHint.Range, "0,10,1,or_greater")]
    public uint Damage { get; set; } = 20;

    [Signal]
    public delegate void KilledEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var newY = GlobalPosition.Y;
        newY += Speed * (float)delta;
        var newPos = new Vector2(GlobalPosition.X, newY);
        GlobalPosition = newPos;
    }

    public void Harm(double damage)
    {
        this.Health -= (int)damage;
        if (this.Health > 0) return;
        EmitSignal(SignalName.Killed);
        QueueFree();
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is not Player player) return;
        player.Harm(this.Damage);
        QueueFree();
    }

    public void OnVisibleOnScreenExited()
    {
        QueueFree();
        GD.Print("Enemy is no more visible on screen.");
    }
}