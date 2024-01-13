public interface ISubscriber<T>
{
    void OnEventHappen(T e);
}
