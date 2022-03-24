using System;

namespace EL.Card
{
    public struct DragAndDropState
    {
        public Action<Card> OnStart;
        public Action<Card> OnCancel;
        public Action<Card> OnDrop;
    }
}