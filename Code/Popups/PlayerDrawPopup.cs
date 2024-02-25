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
	private Button _dealButton;

	private Label _dealMoreCount;

	private Label _totalCardsToDeal;

    private int _viewportWidth;
    private int _viewportHeight;

    public int BaseDealCount 
	{
		get
		{
			return int.TryParse(_defaultDrawCount.Text, out int baseDealCount)
					? baseDealCount
					: 1;
		}
		set
		{
            _defaultDrawCount.Text = value.ToString();
        } 
	}
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
			return this.BaseDealCount + DealMoreCount;
        }
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
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
		_popupPanel.Hide();		

		_playerNameLabel = GetNode<Label>("PopupPanel/VBoxContainer/PlayerNameLabel");
		_playerNameLabel.Text = "Player 1";

		_foldButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/FoldButton");
        _dealButton = GetNode<Button>("PopupPanel/VBoxContainer/HBoxContainer/DealButton");

		_defaultDrawCount = GetNode<Label>("PopupPanel/VBoxContainer/CardsToDrawContainer/CardDrawCount");
		BaseDealCount = 1;

		_dealMoreCount = GetNode<Label>("PopupPanel/VBoxContainer/DealMoreContainer/DealMoreCount");
		_drawSlider = GetNode<HSlider>("PopupPanel/VBoxContainer/DrawSlider");

		_totalCardsToDeal = GetNode<Label>("PopupPanel/VBoxContainer/TotalCardsToDealContainer/CardsToDealCount");

		//_cardDrawCount = GetNode<Label>("PopupPanel/VBoxContainer/CardsToDrawContainer/CardDrawCount");
    }

	public void Reset(string name, int numPlayers, int drawCount)
	{
		_playerNameLabel.Text = name;
		_drawSlider.MaxValue = numPlayers;
		_drawSlider.TickCount = numPlayers+1;
		_drawSlider.Value = 0;
		//_defaultDrawCount.Text = drawCount.ToString();
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

	private void HandleDealButtonPressed() 
	{
		_signalBus.EmitDealRequestSignal(NumberOfCardsToDeal);

		_popupPanel.Hide();

		BaseDealCount += DealMoreCount;
	}

	private void HandleSliderValueChanged(float newValue) 
	{
        //_defaultDrawCount.Text = newValue.ToString();
		_dealMoreCount.Text = newValue.ToString();
		_totalCardsToDeal.Text = NumberOfCardsToDeal.ToString();
    }

	private void HandleEndGame()
	{
		//NumberOfCardsToDeal = 1;
		BaseDealCount = 1;
		DealMoreCount = 0;
	}
}
