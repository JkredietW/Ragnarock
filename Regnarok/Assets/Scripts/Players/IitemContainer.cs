public interface IitemContainer
{
    bool ContainsItem(Item item);
    bool RemoveItem(Item item);
    bool AddItem(Item item);
    bool IsFull(Item item);
}
