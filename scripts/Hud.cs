using Godot;
using System;

public partial class Hud : Control
{
    private Label Score { get; set; }
    private Label HighScore { get; set; }

    public Hud()
    {
        GD.Print("holis");
    }

    public void SetScore(uint value)
    {
        this.Score.Text = $"Score: {value}";
    }

    public void SetHighScore(uint value)
    {
        this.HighScore.Text = $"Hi-Score: {value}";
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Score = GetNode("Score") as Label;
        this.HighScore = GetNode("HighScore") as Label;
        this.SetScore(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}