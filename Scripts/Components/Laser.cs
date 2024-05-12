using Godot;

namespace ResearchVertical.Scripts.Components;

public partial class Laser : Area2D
{
    public const string Path = "res://scenes/laser.tscn";

    [Export(PropertyHint.Range, "0,600,1,or_greater")]
    public float Speed { get; set; } = 600.0f;

    [Export(PropertyHint.Range, "0,10,1,or_greater")]
    public int Damage { get; set; } = 10;


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
        newY -= Speed * (float)delta;
        var newPos = new Vector2(GlobalPosition.X, newY);
        GlobalPosition = newPos;
    }

    public void OnVisibleOnScreenEnabler()
    {
        QueueFree();
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is not ResearchVertical.Scripts.Enemy enemy) return;
        enemy.Harm(this.Damage);
        QueueFree();
    }
}