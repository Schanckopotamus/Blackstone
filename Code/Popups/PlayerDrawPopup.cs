using Blackstone.Code.Buses;
using Godot;
using System;

public partial class PlayerDrawPopup : Control
{
    private SignalBus _signalBus;

    private PopupPanel _popupPanel;

	private Label _playerNameLabel;
	private HSlider _drawSlider;
	private Label _defaultDrawCount;

	private Button _foldButton;
	private Button _drawButton;

	private Label _cardDrawCount;

    private int _viewportWidth;
    private int _viewportHeight;

    public int NumberOfCardsToDeal 
	{
		get
		{
			return int.TryParse(_cardDrawCount.Text, out var cardCount)
				? cardCount
				: 1;
        }
		private set
		{
			_cardDrawCount.Text = value.ToString();
		}
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");

        _viewportWidth = ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
        _viewportHeight = ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();

        _popupPanel = GetNode<PopupPanel>("PopupPanel");
		var halfPanalY = _popupPanel.Size.Y/2;
		var halfPanalX = _popupPanel.Size.X/2;
		_popupPanel.Position = new Vector2I(_viewportWidth/2-halfPanalX, _viewportHeight/2-halfPanalY);
        _popupPanel.Transient = true;
        _popupPanel.Exclusive = true;
		_popupPanel.PopupWindow = false;
		_popupPanel.Hide();		

		_playerNameLabel = GetNode<Label>("PopupPanel/VBoxContainer/PlayerNameLabel");
		_playerNameLabel.Text = "Player 1";
		_drawSlider = GetNode<HSlider>("PopupPanel/VBoxContainer/DrawSlider");
		_defaultDrawCount = GetNode<Label>("PopupPanel/VBoxContainer/ValueLabel");
		_cardDrawCount = GetNode<Label>("PopupPanel/VBoxContainer/CardsToDrawContainer/CardDrawCount");

		//_drawSlider.MaxValue = 5;
		//_drawSlider.TickCount = 5;
		//_drawCountLabel.Text = _drawSlider.MinValue.ToString();

		_foldButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/FoldButton");
        _drawButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/DrawButton");
    }

	public void Reset(string name, int numPlayers, int drawCount)
	{
		_playerNameLabel.Text = name;
		_drawSlider.MaxValue = numPlayers;
		_drawSlider.TickCount = numPlayers+1;
		_drawSlider.Value = 0;
		_defaultDrawCount.Text = drawCount.ToString();
	}

	public void Popup()
	{
		_popupPanel.Show();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void HandleFoldButtonPressed()
	{
		_popupPanel.Hide();
		_signalBus.EmitPlayerFoldRequestSignal();
	}

	private void HandleDrawButtonPressed() 
	{
		// We need to be able to tell he Dealer to draw. Which innevitably should call into 
		// the deal method.
			// Maybe we have a property that stores the value and Deal() just uses that value
			// to do the dealing.

		var drawSliderValue = (int)_drawSlider.Value;

		NumberOfCardsToDeal += drawSliderValue;

		_signalBus.EmitDealRequestSignal(NumberOfCardsToDeal);

		_popupPanel.Hide();
	}

	private void HandleSliderValueChanged(float newValue) 
	{
        _defaultDrawCount.Text = newValue.ToString();
    }
}
