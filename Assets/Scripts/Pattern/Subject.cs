public interface Subject
{
    /// <summary>
    /// Adds the observer.
    /// </summary>
    /// <param name="o">O.</param>
    void AddObserver(Observer o);
    /// <summary>
    /// Removes the observer from ni
    /// </summary>
    /// <param name="o">O.</param>
    void RemoveObserver(Observer o);
    /// <summary>
    /// Notifies all added observer.
    /// </summary>
    void Notify();
}
