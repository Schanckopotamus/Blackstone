using Blackstone.Code;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class CardGenerator : Node2D
{
    private PackedScene CardScene { get; set; }
    private string cardTexturePath = "res://Assets/Cards/md{x}.png";

    // Dictionary that uses the Card value as the Key, 0 = back card
    private Dictionary<int, Card> _preGenCardDict;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Card.tscn");
        LoadPreGenCardsIntoMemory();
    }

    private void LoadPreGenCardsIntoMemory()
    {
        _preGenCardDict = this.GetChildren()
            .Select(c => (Card)c)
            .ToDictionary(c =>
            {
                if (int.TryParse(c.Name.ToString(), out var cardId))
                {
                    return cardId;
                }

                // Some random number much higher than any single card value could be to see an issue at a glance
                return GD.RandRange(100, 1000);
            });

        // Remove any Dictionary entry that clearly failed parsing when loading
        foreach (var item in _preGenCardDict)
        {
            if (item.Key >= 100)
            {
                _preGenCardDict.Remove(item.Key);
                continue;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // 0 makes back of card result.
    public Card GetCard(int cardValue)
    {
        if (cardValue <= 0
            || cardValue > 10
            || !_preGenCardDict.ContainsKey(cardValue))
        {
            return CreateDefaultCardScene();
        }

        return (Card)_preGenCardDict[cardValue].Duplicate((int)(DuplicateFlags.Groups | DuplicateFlags.Scripts));
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
