using Blackstone.Code.Extensions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code
{
    public static class CardFactory
    {
        static CardFactory()
        {
            
        }

        // 0 makes back of card result.
        //public static Card GetCard(int cardValue)
        //{
        //    var card = CreateDefaultCardScene();
        //    //InstantiateCardToTree(card);

        //    if (cardValue <= 0 || cardValue > 10) 
        //    {
        //        return card;
        //    }

        //    var path = cardTexturePath.Replace("{x}", cardValue.ToString());

        //    if (!FileAccess.FileExists(path))
        //    {
        //        GD.Print($"CardFactory.GetCard; Card Asset Path Does NOT Exist\n=> Path: {path}");
        //        return card;
        //    }

        //    var texture = GD.Load<Texture2D>(path);
        //    card.ChangeTexture(texture);

        //    return card;
        //}

        public static CardDeck CreateDeck(bool shouldRemoveBlackstones = false)
        {
            return new CardDeck(true, shouldRemoveBlackstones);
        }

        ///// <summary>
        ///// Create a Card scene from the Packed scene.
        ///// Default texture is card back.
        ///// </summary>
        ///// <returns>Unparented Card scene</returns>
        //private static Card CreateDefaultCardScene()
        //{ 
        //    var card = CardScene.Instantiate<Card>();

        //    return card;
        //}

        private static void InstantiateCardToTree(Card card)
        {
            var tree = new Tree();
            tree.AddChild(card);
            card.GetParent().RemoveChild(card);
        }
    }

    public class CardDeck
    {
        private int _deckSize = 55;

        private Stack<ModeganCard> _cards;

        public CardDeck(bool shouldBeShuffled = true, bool shouldRemoveBlackstones = false)
        {
            _cards = new Stack<ModeganCard>();

            CreateFreshDeck();

            if (shouldBeShuffled) 
            {
                var cardList = _cards.ToList();
                cardList.Shuffle(numberOfShuffles: 3);
                
                _cards = new Stack<ModeganCard>(cardList);
            }

            // THIS IS FOR TESTING PURPOSES
            if (shouldRemoveBlackstones) 
            {
                var cleanStack = new Stack<ModeganCard>();
                var removedCards = _cards.ToList().Where(c => c.Value != 10);

                foreach (var card in removedCards) 
                {
                    cleanStack.Push(card);
                }

                _cards = cleanStack;
            }
        }

        public ModeganCard DrawCard()
        {
            if (_cards != null && _cards.Any())
            { 
                return _cards.Pop();
            }

            return new ModeganCard();
        }

        private void CreateFreshDeck()
        {
            for (int i = 1; i <= 10; i++)
            {
                for(int j = 0; j < i; j++) // Create number of cards equal to the value of the card
                {
                    _cards.Push(new ModeganCard(i));
                }
            }
        }
    }

    public class ModeganCard
    {
        public Guid Id { get; }
        public int Value { get; }
        public ModeganCardType Stone { get; }

        public ModeganCard()
        {
            Id = Guid.NewGuid();
            Value = 0;
            Stone = ModeganCardType.Unknown;
        }

        public ModeganCard(int value)
        {
            if (value <= 0 || value > 10)
            {
                return;
            }

            Id = Guid.NewGuid();
            this.Value = value;
            this.Stone = value == 10 ? ModeganCardType.Black : ModeganCardType.White;
        }
    }

    public enum ModeganCardType
    { 
        Unknown,
        White,
        Black
    }
}
