using Blackstone.Code;
using Blackstone.Code.Buses;
using Godot;
using System;

public partial class PlayerDrawPopup : Control
{
    private SignalBus _signalBus;
    private GameMetadata _gameMetadata;

    private PopupPanel _popupPanel;

	private Label _playerNameLabel;
	private HSlider _drawSlider;
	private Label _defaultDrawCount;

	private Button _foldButton;
	private Button _dealButton;

	private Label _dealMoreCount;

	private Label _totalCardsToDeal;

    private int _viewportWidth;
    private int _viewportHeight;

	public int DealMoreCount
	{
		get
		{
			return int.TryParse(_dealMoreCount.Text, out int dealMoreCount)
					? dealMoreCount
					: 1;
		}
		set
		{
			_dealMoreCount.Text = value.ToString();
		}
	}

	public int NumberOfCardsToDeal
	{
		get
		{
			return _gameMetadata.DrawAmount + DealMoreCount;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
        _gameMetadata = GetNode<GameMetadata>("/root/GameMetadata");
        _signalBus.OnEndGame += HandleEndGame;

        _viewportWidth = ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
        _viewportHeight = ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();

        _popupPanel = GetNode<PopupPanel>("PopupPanel");
		var halfPanalY = _popupPanel.Size.Y/2;
		var halfPanalX = _popupPanel.Size.X/2;
		_popupPanel.Position = new Vector2I(_viewportWidth/2-halfPanalX, _viewportHeight/2-halfPanalY);
        _popupPanel.Transient = true;
        _popupPanel.Exclusive = true;
		_popupPanel.PopupWindow = false;		

		_playerNameLabel = GetNode<Label>("PopupPanel/VBoxContainer/PlayerNameLabel");
		_playerNameLabel.Text = "Player 1";

		_foldButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/FoldButton");
        _dealButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/DealButton");

		_defaultDrawCount = GetNode<Label>("PopupPanel/VBoxContainer/CardsToDrawContainer/CardDrawCount");

		_dealMoreCount = GetNode<Label>("PopupPanel/VBoxContainer/DealMoreContainer/DealMoreCount");
		_drawSlider = GetNode<HSlider>("PopupPanel/VBoxContainer/DrawSlider");

		_totalCardsToDeal = GetNode<Label>("PopupPanel/VBoxContainer/TotalCardsToDealContainer/CardsToDealCount");
    }

	public void Reset(string name, int numPlayers, int drawCount)
	{
		_playerNameLabel.Text = name;
		_drawSlider.MaxValue = numPlayers;
		_drawSlider.TickCount = numPlayers+1;
		_drawSlider.Value = 0;
		_defaultDrawCount.Text = _gameMetadata.DrawAmount.ToString();
		_totalCardsToDeal.Text = _defaultDrawCount.Text;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HandleFoldButtonPressed()
	{
		_popupPanel.QueueFree();
		_signalBus.EmitPlayerFoldRequestSignal();
	}

	private void HandleDealButtonPressed() 
	{
		_gameMetadata.IncreaseDrawAmount(DealMoreCount);
		_signalBus.EmitDealRequestSignal(_gameMetadata.DrawAmount);

		_popupPanel.QueueFree();
	}

	private void HandleSliderValueChanged(float newValue) 
	{
		_dealMoreCount.Text = newValue.ToString();
		_totalCardsToDeal.Text = NumberOfCardsToDeal.ToString();
    }

	private void HandleEndGame()
	{
		_gameMetadata.ResetDrawAmount();
		DealMoreCount = 0;
        _totalCardsToDeal.Text = _gameMetadata.DrawAmount.ToString();
    }
}
