using Blackstone.Code.Buses;
using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class FindFirstPlayerState : DealerStateBase
{
    private Dealer _dealer;
    private List<PlayerScene> _players;

    private SignalBus _signalBus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.State = DealerState.FindFirstPlayer;
        // This really goes against convention. Find a better way.
        _dealer = GetNode<Dealer>("../../../Dealer");
    }

    public override async void Enter(Dictionary<string, object> parameters = null)
    {
        _players?.Clear();
        _signalBus = GetNode<SignalBus>("/root/SignalBus");

        //_signalBus.EmitRequestCardBoxDisabledSignal();
        //_signalBus.EmitPlayerCollisionChangeRequestSignal(isCollisionEnabled: true);

        // TODO: Having a List of PlayerScenes and ordering them might need to be centralized?
        var players = 
                base.ExtractCollectionFromParameters<PlayerScene>(parameters, "Players")
                .OrderBy(p => p.SeatPositon)
                .ToList();

        // Ensure no player is marked as Active before determination is made.
        foreach (var player in players) 
        {
            player.SetToPassive();
        }

        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        if (players.Any())
        {
            try
            {
                _players = players;

                await DetermineFirstPlayer();
            }
            catch (Exception)
            {
                GD.Print("*** FindFirstPlayer.Enter() -> Could not locate Players in 'parameters' dictionary ***");
                this.Exit();
                EmitSignal(SignalName.DealerStateTransitionRequested, DealerState.PlayerAnte.ToString());
                return;
            }
        }
        else
        {
            GD.Print("*** FindFirstPlayer.Enter() -> Could not locate Players in 'parameters' dictionary ***");
        }
    }

    public override void Exit()
    {
        //_players?.Clear();
        //_signalBus.EmitRequestCardBoxEnabledSignal();
        //_signalBus.EmitPlayerCollisionChangeRequestSignal(isCollisionEnabled: false);
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

        var players = new List<PlayerScene>();
        foreach (var p in _players) 
        {
            players.Add(p);
        }

        while (firstPlayer == null)
        {
            if (isTieBreakerRound)
            {            
                //_signalBus.EmitRequestCardBoxDisabledSignal();
            }
            
            foreach (var player in players)
            {
                DealToPlayer(player);

                await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
            }

            // Are the values the same?
            var playerOrderedGroups = players
                .GroupBy(p => p.GetCardsInHand().Last().ModeganCardValue)
                .OrderBy(g => g.Key)
                .ToList();

            if (playerOrderedGroups.First().Count() == 1)
            {
                firstPlayer = playerOrderedGroups.First().ToList().First();

                // TODO: Not triggering in CardTable.cs
                //EmitSignal(SignalName.FirstPlayerFound, firstPlayer);
                _signalBus.EmitPlayerFocusChangedSignal(firstPlayer);
                await DealPlayerCardsToBoxes(players);

                //EmitSignal(SignalName.DealerStateTransitionRequested, DealerState.DealPlayerTurn.ToString());
                var parameters = new Dictionary<string, object>
                {
                    { "Players", _players }
                };

                _signalBus.EmitPlayerStateChangeRequestedSignal(DealerState.DealPlayerTurn, parameters);
            }
            else
            {
                isTieBreakerRound = true;
                await DealPlayerCardsToBoxes(players);
                players = playerOrderedGroups.First().ToList();
            }
        }
    }

    private void DealToPlayer(Node2D playerNode)
    {
        var card = _dealer.GenerateSpecificCard(0);//_cardGenerator.GetCard(0);

        card.GlobalPosition = _dealer.GlobalPosition;

        this.AddChild(card);

        var direction = _dealer.GlobalPosition.DirectionTo(playerNode.GlobalPosition).Normalized();
        card.SetToDealt(direction, _dealer.DealSpeed, DealTarget.Player);
    }

    private async Task DealPlayerCardsToBoxes(List<PlayerScene> players)
    {
        //_signalBus.EmitRequestCardBoxEnabledSignal();

        //foreach (var player in players)
        //{
        //    player.CallDeferred("DisableCollisionBox");
        //}

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
                    //_dealer.RequestDeal();

                    //var direction = card.GlobalPosition.DirectionTo(_dealer.TableBoxToDealPosition).Normalized();
                    //card.SetToDealt(direction, _dealer.DealSpeed);
                    _dealer.DealToCardBox(card);
                }
            }
        }

        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        foreach (var player in players)
        {
            //player.CallDeferred("EnableCollisionBox");
        }
    }
}
