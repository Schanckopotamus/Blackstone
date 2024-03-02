using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code
{
    /// <summary>
    /// Represents a pair of card boxes for the table. This is an 
    /// encapsulation class as we want to manage dealing in pairs 
    /// and managing things like visibility and if the boxes are 
    /// active/dealable
    /// </summary>
    public class CardTableBoxPair
    {
        public Guid PairId { get; set; }
        public CardTableBox Box1 { get; set; }
        public CardTableBox Box2 { get; set; }

        public bool IsActivePair { 
            get { return GetBoxesActiveFlag(); } 
            set { SetBoxesActiveFlag(value); } 
        }

        public bool Visibility { 
            get { return GetBoxVisibility(); } 
            set { SetBoxVisibility(value); } 
        }

        public CardTableBoxPair(CardTableBox box1, CardTableBox box2)
        {
            PairId = Guid.NewGuid();
            Box1 = box1;
            Box2 = box2;
            IsActivePair = false;
        }

        public void SetBoxVisibility(bool visibility)
        { 
            this.Box1.Visible = visibility;
            this.Box2.Visible = visibility;
        }

        public void SetPairAsActive()
        {
            this.SetBoxVisibility(true);
            this.Box1.IsBoxActive = true;
            this.Box2.IsBoxActive = true; 
        }

        public bool GetBoxVisibility()
        {
            return this.Box1.Visible || this.Box2.Visible;
        }

        public List<CardTableBox> GetActiveBoxes()
        { 
            var boxes = new List<CardTableBox>();

            if (Box1.IsBoxActive)
            {
                boxes.Add(Box1);
            }

            if (Box2.IsBoxActive) 
            {
                boxes.Add(Box2);
            }

            return boxes;
        }

        public CardTableBox GetBoxWithFewerCards() 
        {
            var boxes = this.GetActiveBoxes();

            if (!boxes.Any())
            {
                return null;
            }

            if (boxes.Count() == 1)
            {
                return boxes[0];
            }

            return boxes.OrderBy(x => x.GetCardCount()).First();            
        }

        public CardTableBox GetBoxById(ulong id)
        {
            if (Box1.GetInstanceId().Equals(id))
            {
                return Box1;
            }

            if (Box2.GetInstanceId().Equals(id))
            {
                return Box2;
            }

            return null;
        }

        public void Reset()
        {
            Box1.Reset();
            Box2.Reset();
        }

        private void SetBoxesActiveFlag(bool value)
        { 
            Box1.IsBoxActive = value;
            Box2.IsBoxActive = value;
        }

        private bool GetBoxesActiveFlag() 
        {
            return Box1.IsBoxActive || Box2.IsBoxActive;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var pair = obj as CardTableBoxPair;

            return this.PairId == pair.PairId;
        }
    }
}
