using Godot;
using System;
using System.Linq;

public partial class RulesMenu : Control
{
    public TabContainer TabContainer { get; set; }
    public ScrollContainer BlackstoneContainer { get; set; }
    public Button BackButton { get; set; }

    public int TabCount { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		TabContainer = GetNode<TabContainer>("TabContainer");
        TabCount = TabContainer.GetTabBar().TabCount;
        BackButton = GetNode<Button>("BackButton");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("BumperNext"))
		{
			if (TabContainer.CurrentTab == TabCount - 1)
			{
				TabContainer.CurrentTab = 0;
			}
			else
			{
				TabContainer.CurrentTab++;
			}
		}
		else if (Input.IsActionJustReleased("BumperPrevious"))
		{
            if (TabContainer.CurrentTab == 0)
            {
                TabContainer.CurrentTab = TabCount-1;
            }
            else
            {
                TabContainer.CurrentTab--;
            }
        }

		if (Input.IsActionJustPressed("Back"))
		{
			OnBackButtonPressed();
		}
	}

	public void OnBackButtonPressed()
	{
        this.GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}
