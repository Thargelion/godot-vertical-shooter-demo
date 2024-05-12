using Godot;

namespace ResearchVertical.Scripts;

public partial class GameOverScreen : Control
{
    private Label _score;
    private Label _highScore;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this._score = GetNode("Panel/Score") as Label;
        this._highScore = GetNode("Panel/HighScore") as Label;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SetScore(uint value)
    {
        this._score.Text = $"Score: {value}";
    }

    public void SetHighScore(uint value)
    {
        this._highScore.Text = $"Hi-Score: {value}";
    }

    /**
 * Restarts the game.
 */
    public void OnRestartButtonPressed()
    {
        GetTree().ReloadCurrentScene();
    }
}