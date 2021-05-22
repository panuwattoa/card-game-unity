public interface Observer
{
    /// <summary>
    /// Notify method.
    /// </summary>
    /// <param name="data">A data for an observer to use. </param>
    void Notify(Subject o);
}
