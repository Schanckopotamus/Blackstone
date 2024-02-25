using Blackstone.Code.Buses;
using Blackstone.Code.DTOs;
using Blackstone.Code.Enums;
using Blackstone.Code.States.Dealer;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class DealRoundState : DealerStateBase
{
    private SignalBus _signalBus;
    private Dealer _dealer;

    private bool _didEndGameTrigger = false;

    private PlayerScene _activePlayer;
    private List<PlayerScene> _players;

    public override void _Ready()
    {
        _signalBus = GetNode<SignalBus>("/root/SignalBus");
        _signalBus.PlayerFoldRequest += HandlePlayerFoldSignal;

        // TODO: Find a better way to get information necessary from Dealer instead of using a reference because it is a Parent
        _dealer = GetNode<Dealer>("../../../Dealer");

        this.State = DealerState.DealPlayerTurn;
    }

    public override async void Deal(int numCardsToDeal)
    {        
        await DealToDealer(numCardsToDeal);

        await DealRevealedDealerCardsToCardBoxes();

        if (_didEndGameTrigger)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "Players", _players }
                };

            _signalBus.EmitDealerStateChangeRequestedSignal(DealerState.EndRound, parameters);
        }
        else
        { 
            SetNextActivePlayer();
                       
            var popupDto = new PlayerPopupDTO(_activePlayer.Name.ToString(), _players.Count());
            _signalBus.EmitPlayerPopUpRequestedSignal(popupDto);
        }    
        
        _dealer.ResetDrawPosition(); 
    }

    private void SetNextActivePlayer()
    {
        var index = GetIndexOfNextActivePlayer();
        _activePlayer = _players[index];

        // ActivePlayer UI indicator needs to be reset.
        _signalBus.EmitPlayerFocusChangedSignal(_activePlayer);
    }

    private int GetIndexOfNextActivePlayer()
    {
        var activePlayerIndex = _players.IndexOf(_activePlayer);

        if (activePlayerIndex == -1)
        {
            // TODO: trigger endgame or some other condition for when we don't know the next player
            return activePlayerIndex;
        }
        else if (activePlayerIndex >= _players.Count() - 1) // Active player is in the last seat so get the first player in list
        {
            return 0;
        }
        else
        {
            return ++activePlayerIndex;
        }
    }

    private async Task DealRevealedDealerCardsToCardBoxes()
    {
        //_signalBus.EmitRequestCardBoxEnabledSignal();

        var cards = _dealer.GetCardsInHand();
        cards.Reverse(); // We want to deal out in reverse order in which they were revealed

        foreach ( var card in cards ) 
        {
            if (card.IsWhitestone)
            {
                //_signalBus.EmitRequestCardBoxEnabledSignal();
                _dealer.DealToCardBox(card);
            }
            else
            {
                //_signalBus.EmitRequestCardBoxDisabledSignal();
                await DealCardToActivePlayer(card);
            }

            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
        }

        //_signalBus.EmitRequestCardBoxDisabledSignal();
    }

    private async Task DealCardToActivePlayer(Card card)
    {
        var direction = card.GlobalPosition.DirectionTo(_activePlayer.GlobalPosition).Normalized();

        card.SetToDealt(direction, _dealer.DealSpeed, DealTarget.Player);
    }

    private async Task DealToDealer(int numCardsToDeal)
    {
        //await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        //var didEndGameTrigger = false;

        for (int i = 0; i < numCardsToDeal; i++)
        {
            var card = _dealer.DrawCard();
            _dealer.CardToDealer(card);

            await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

            // Check to see if the card pulled is a 10 (blackstone)
            // If so, check to see if the player is holding a blackstone already.
            // If so, End Game state (once someone gets 2 blackstone the round is over)
            // If 2 blackstones are revealed that is also an endgame state as all blackstones revealed by dealer are owned by active player.
            if (card.IsBlackstone
                && (_activePlayer.GetCardsInHand().Any(c => c.IsBlackstone)
                || _dealer.GetCardsInHand().Count(c => c.IsBlackstone).Equals(2)))
            {
                // Do not continue drawing cards, break loop.
                _didEndGameTrigger = true;
                break;
            }
        }

        if (_didEndGameTrigger)
        {
            // End Game Signal or transition to an EndGameState
        }
    }

    public override async void Enter(Dictionary<string, object> parameters = null)
    {
        // TODO: Having a List of PlayerScenes and ordering them might need to be centralized?
        _players = 
            base.ExtractCollectionFromParameters<PlayerScene>(parameters, "Players")
            .OrderBy(p => p.SeatPositon)
            .ToList();

        if (!_players.Any() && _players.Count() < 2 && _players.Count() > 8)
        {
            // Log error
            // Transition to previous state
            _signalBus.EmitDealerStateChangeRequestedSignal(DealerState.FindFirstPlayer, parameters);
            return;
        }

        _activePlayer = _players.Where(p => p.IsActive).FirstOrDefault();

        if (_activePlayer != null) 
        {
            // Escape the loop
            var popupDto = new PlayerPopupDTO(_activePlayer.Name.ToString(), _players.Count());

            // Want the popup to be available for maybe 30 sec, afterward, default to fold without action.
            //await ToSignal(_signalBus.EmitPlayerPopUpRequestedSignal(popupDto)
                
            _signalBus.EmitPlayerPopUpRequestedSignal(popupDto);
        }
    }

    public override void Exit()
    {
        _dealer.ResetDrawPosition();
        _didEndGameTrigger = false;
    }

    private void HandlePlayerFoldSignal()
    { 
        if (_activePlayer != null) 
        {
            var playerFolded = _activePlayer;

            if (_players.Count == 2) // One player left, other player is winner
            {
                
            }
            else
            { 
                _signalBus.EmitPlayerFoldedEventSignal(playerFolded);
                SetNextActivePlayer();

                _players.Remove(playerFolded);

                var popupDto = new PlayerPopupDTO(_activePlayer.Name.ToString(), _players.Count());
                _signalBus.EmitPlayerPopUpRequestedSignal(popupDto);
            }
            
        }
    }
}
