namespace EL.Common
{
    public interface IDraggableTarget
    {
        bool IsAllowDrop(IDraggable item);

        void OnDrop(IDraggable item);
    }
}