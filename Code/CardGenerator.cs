using Blackstone.Code;
using Godot;
using System;
using System.Linq;

public partial class CardGenerator : Node2D
{
    private PackedScene CardScene { get; set; }
    private string cardTexturePath = "res://Assets/Cards/md{x}.png";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Card.tscn");
        //CreateReferenceCardsInTree();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // 0 makes back of card result.
    public Card GetCard(int cardValue)
    {
        if (cardValue <= 0 || cardValue > 10) return CreateDefaultCardScene();

        var card = this.GetChildren()
            .Where(c => c.IsInGroup("card"))
            .Select(x => (Card)x)
            .FirstOrDefault(y => y.ModeganCardValue == cardValue);

        if (card == null)
        {
            card = CreateCard(cardValue);
        }

        // Disassociate from parent so it can be added to a different tree.
        card.GetParent()?.RemoveChild(card);

        return card;
    }

    /// <summary>
	/// Build an instance of all types of face cards including back card
	/// </summary>
	private void CreateReferenceCardsInTree()
    {
        for (int i = 0; i <= 10; i++)
        {
            CreateCard(i);            
        }
    }

    private Card CreateCard(int cardValue)
    {
        var card = CreateDefaultCardScene();
        //card.ModeganCardValue = cardValue;
        //card.GlobalPosition = Vector2.Zero;
        card.SetDeferred("ModeganCardValue", cardValue);
        card.SetDeferred("GlobalPosition", Vector2.Zero);
        //card.Visible = false;

        // Create child scene in tree for instatiation so Texture can be reset
        this.AddChild(card);

        if (cardValue <= 0 || cardValue > 10)
        {
            return card;
        }        

        var path = cardTexturePath.Replace("{x}", cardValue.ToString());

        if (!FileAccess.FileExists(path))
        {
            GD.Print($"CardAssets.CreateCard; Card Asset Path Does NOT Exist\n=> Path: {path}");
            return card;
        }

        var texture = GD.Load<Texture2D>(path);
        card.ChangeTexture(texture);

        return card;
    }   

    /// <summary>
    /// Create a Card scene from the Packed scene.
    /// Default texture is card back.
    /// </summary>
    /// <returns>Unparented Card scene</returns>
    private Card CreateDefaultCardScene()
    {
        var card = CardScene.Instantiate<Card>();

        return card;
    }
}
