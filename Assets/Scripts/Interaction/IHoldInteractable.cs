public interface IHoldInteractable
{
    bool CanHold { get; }
    void HoldTick();
    void HoldCancel();
}
