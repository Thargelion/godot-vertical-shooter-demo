using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;
using Timer = Godot.Timer;

namespace ResearchVertical.Scripts;

public partial class Game : Node2D
{
    private Node2D _player;
    private Node2D _playerSpawnPosition;
    private Node2D _laserContainer;
    private Node2D _enemyContainer;
    private Timer _enemySpawnTimer;
    private Hud _hud;
    private GameOverScreen _gameOverScreen;
    private ParallaxBackground _parallaxBackground;
    private uint _score;
    private uint _highScore;

    private const int TotalEnemyTypes = 3;
    private int _scrollSpeed = 90;
    private const string SaveGameFile = "user://savegame.data";

    private AudioStreamPlayer _laserSound;
    private AudioStreamPlayer _hitSound;
    private AudioStreamPlayer _explodeSound;

    [Export] public Array<PackedScene> EnemyScenes { get; set; } = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _loadGame();
        _getNodes();
        this._player.GlobalPosition = this._playerSpawnPosition.GlobalPosition;
        this._player.Connect("LaserShot",
            Callable.From(
                (PackedScene laserScene, Vector2 location) => { this._onPlayerLaserShot(laserScene, location); }
            )
        );
        this._player.Connect("Killed", Callable.From(() => { this._onPlayerKilled(); }));
    }

    private void _saveGame()
    {
        using var file = FileAccess.Open(SaveGameFile, FileAccess.ModeFlags.Write);
        file.Store32(this._highScore);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
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

        if (this._enemySpawnTimer.WaitTime < 0.5) return;
        this._enemySpawnTimer.WaitTime -= delta * 0.005;
        AdvanceBackground((float)delta);
    }

    private void AdvanceBackground(float delta)
    {
        var newOffset = 0f;
        if (this._parallaxBackground.ScrollOffset.Y <= 960)
        {
            newOffset = this._parallaxBackground.ScrollOffset.Y + delta * this._scrollSpeed;
        }

        this._parallaxBackground.ScrollOffset =
            new Vector2(this._parallaxBackground.ScrollOffset.X, newOffset);
    }

    public void _onEnemyKilled(ResearchVertical.Scripts.Enemy enemy)
    {
        this._score += enemy.Value;
        if (this._score > this._highScore)
        {
            GD.Print("Score: " + this._score + " is higher than high score: " + this._highScore + ". Saving...");
            this._highScore = this._score;
            this._hud.SetHighScore(this._highScore);
        }
        this._explodeSound.Play();

        this._hud.SetScore(this._score);
    }

    public void OnEnemySpawnTimerTimeout()
    {
        var rand = new RandomNumberGenerator();
        if (EnemyScenes[rand.RandiRange(0, TotalEnemyTypes - 1)].Instantiate() is not ResearchVertical.Scripts.Enemy enemy) return;
        enemy.GlobalPosition = new Vector2(rand.RandfRange(0, 540), -10);
        enemy.Connect("Killed", Callable.From(
            () => { this._onEnemyKilled(enemy); })
        );
        this._enemyContainer.AddChild(enemy);
    }

    private async void _loadGame()
    {
        const double timeout = 1.5;
        using var file = FileAccess.Open(SaveGameFile, FileAccess.ModeFlags.Read);
        if (file != null)
        {
            this._highScore = file.Get32();
        }
        else
        {
            this._highScore = 0;
            this._saveGame();
        }

        await ToSignal(GetTree().CreateTimer(timeout), SceneTreeTimer.SignalName.Timeout);

        this._hud.SetHighScore(this._highScore);
    }

    private void _getNodes()
    {
        this._enemySpawnTimer = GetNode("EnemySpawnTimer") as Timer;
        this._playerSpawnPosition = GetNode("PlayerSpawnPos") as Node2D;
        this._laserContainer = GetNode("LaserContainer") as Node2D;
        this._enemyContainer = GetNode("EnemyContainer") as Node2D;
        this._hud = GetNode("UILayer/HUD") as Hud;
        this._gameOverScreen = GetNode("UILayer/GameOverScreen") as GameOverScreen;
        this._player = GetNode("Player") as Node2D;
        this._parallaxBackground = GetNode("ParallaxBackground") as ParallaxBackground;
        this._laserSound = GetNode("SFX/LaserSound") as AudioStreamPlayer;
        this._hitSound = GetNode("SFX/HitSound") as AudioStreamPlayer;
        this._explodeSound = GetNode("SFX/ExplodeSound") as AudioStreamPlayer;
    }

    private void _onPlayerLaserShot(PackedScene laserScene, Vector2 location)
    {
        var laser = laserScene.Instantiate() as Components.Laser;
        laser.GlobalPosition = location;
        this._laserContainer.AddChild(laser);
        this._laserSound.Play();
    }

    private async void _onPlayerKilled()
    {
        const double timeout = 1.5;
        this._explodeSound.Play();
        this._gameOverScreen.SetScore(this._score);
        this._gameOverScreen.SetHighScore(this._highScore);
        this._saveGame();
        await ToSignal(GetTree().CreateTimer(timeout), SceneTreeTimer.SignalName.Timeout);
        this._gameOverScreen.Visible = true;
    }
}