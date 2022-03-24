using EL.Common;

namespace EL.Desk
{
    public class TableContainer : DeckContainer, IDraggableTarget
    {
        public bool IsAllowDrop(IDraggable item)
        {
            return item is Card.Card;
        }

        public void OnDrop(IDraggable item)
        {
            if (!(item is Card.Card card)) return;
            card.View.IsSelected = false;
            AddCard(card);
        }
    }
}