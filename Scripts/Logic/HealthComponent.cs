using Godot;

namespace ResearchVertical.Scripts.Logic;

public partial class HealthComponent : Node2D
{
    [Signal]
    public delegate void HealthChangedEventHandler(HealthUpdate healthUpdate);

    [Signal]
    public delegate void DiedEventHandler();

    private float Health { get; set; }
    
    [Export]
    private float MaxHealth { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Harm(float damage)
    {
        this.Health -= damage;
        SendHealthUpdate(damage);
        if (this.Health <= 0)
        {
            Die();
        }
    }

    private void SendHealthUpdate(float health)
    {
        EmitSignal(nameof(HealthChangedEventHandler), new HealthUpdate
        {
            PreviousHealth = this.Health - health,
            CurrentHealth = this.Health,
            MaxHealth = this.MaxHealth
        });
    }

    public void Heal(float health)
    {
        this.Health += health;
        SendHealthUpdate(health);
    }

    private void Die()
    {
        EmitSignal(nameof(DiedEventHandler));
    }

    public partial class HealthUpdate : RefCounted
    {
        public float PreviousHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
    }
}