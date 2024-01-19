using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class FindFirstPlayer : DealerStateBase
{
    private DealerState _state = DealerState.FindFirstPlayer;
    private Dealer _dealer;
    private List<PlayerScene> _players;

    private SignalBus _signalBus;

    public override async void Enter(Dictionary<string, object> parameters = null)
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");

        _signalBus.EmitRequestCardBoxDisabledSignal();

        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        if (parameters.ContainsKey("Players"))
        {
            try
            {
                _players = (List<PlayerScene>)parameters["Players"];

                await DetermineFirstPlayer();
            }
            catch (Exception)
            {
                GD.Print("*** FindFirstPlayer.Enter() -> Could not locate Players in 'parameters' dictionary ***");
                throw;
            }
        }
        else
        {
            GD.Print("*** FindFirstPlayer.Enter() -> Could not locate Players in 'parameters' dictionary ***");
        }
    }

    public override void Exit()
    {
        _signalBus.EmitRequestCardBoxEnabledSignal();
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // This really goes against convention. Find a better way.
        _dealer = GetNode<Dealer>("../../../Dealer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private async Task DetermineFirstPlayer()
    {
        bool isTieBreakerRound = false;
        PlayerScene firstPlayer = null;

        if (_players != null || _players.Count() == 1)
        {
            // Don't have enough players to deal
            // Maybe emit state change back to AntePlayerState?
        }

        while (firstPlayer == null)
        {
            if (isTieBreakerRound)
            {            
                _signalBus.EmitRequestCardBoxDisabledSignal();
            }
            
            foreach (var player in _players)
            {
                DealToPlayer(player);

                GD.Print("Timer started.");
                await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
                GD.Print("Timer ended.");
            }

            // Are the values the same?
            var playerOrderedGroups = _players
                .GroupBy(p => p.GetCardsInHand().Last().ModeganCardValue)
                .OrderBy(g => g.Key)
                .ToList();

            if (playerOrderedGroups.First().Count() == 1)
            {
                firstPlayer = playerOrderedGroups.First().ToList().First();

                // TODO: Not triggering in CardTable.cs
                //EmitSignal(SignalName.FirstPlayerFound, firstPlayer);
                _signalBus.EmitPlayerFocusChangedSignal(firstPlayer);
                await DealPlayerCardsToBoxes(_players);

                EmitSignal(SignalName.DealerStateTransitionRequested, DealerState.DealPlayerTurn.ToString());
            }
            else
            {
                isTieBreakerRound = true;
                await DealPlayerCardsToBoxes(_players);
                _players = playerOrderedGroups.First().ToList();
            }
        }
    }

    private void DealToPlayer(Node2D playerNode)
    {
        var card = _dealer.GenerateSpecificCard(0);//_cardGenerator.GetCard(0);
        
        this.AddChild(card);

        var direction = _dealer.GlobalPosition.DirectionTo(playerNode.GlobalPosition).Normalized();
        card.SetToDealt(direction, _dealer.DealSpeed);
    }

    private async Task DealPlayerCardsToBoxes(List<PlayerScene> players)
    {
        _signalBus.EmitRequestCardBoxEnabledSignal();

        foreach (var player in players)
        {
            player.CallDeferred("DisableCollisionBox");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        foreach (var player in players)
        {
            var playerCards = player.GetCardsInHand();

            foreach (var card in playerCards)
            {
                if (card.ModeganCardValue != 10)
                {
                    await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

                    // Sets TableBoxToDealPosition from CardTable (Main)
                    _dealer.RequestDeal();

                    var direction = card.GlobalPosition.DirectionTo(_dealer.TableBoxToDealPosition).Normalized();
                    card.SetToDealt(direction, _dealer.DealSpeed);
                }
            }
        }

        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        foreach (var player in players)
        {
            player.CallDeferred("EnableCollisionBox");
        }
    }
}
