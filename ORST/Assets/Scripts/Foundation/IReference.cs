namespace ORST.Foundation {
    public interface IReference<out T> {
        T Value { get; }
    }
}