using Godot;

public class HUD : CanvasLayer
{
    private Label _monsterKillLabel;
    private Label _maxHealthLabel;
    private Label _damageLabel;
    private Label _speedLabel;

    public int MonsterKill { set => _monsterKillLabel.Text = $"{value}"; }
    public int MaxHealth { set => _maxHealthLabel.Text = $"{value}"; }
    public int Damage { set => _damageLabel.Text = $"{value}"; }
    public int Speed { set => _speedLabel.Text = $"{value}"; }

    public override void _Ready()
    {
        _monsterKillLabel = GetNode<Label>("MonsterKill");
        _maxHealthLabel = GetNode<Label>("MaxHealth");
        _damageLabel = GetNode<Label>("Damage");
        _speedLabel = GetNode<Label>("Speed");
    }

}
