using System;

public static class GameEvents
{
    public static event Action<string> MessageRaised;

    public static event Action PreparationEnded;

    public static event Action EvacuationCompleted;

    public static void RaiseMessage(string message)
    {
        if (MessageRaised != null) MessageRaised(message);
    }

    public static void RaisePreparationEnded()
    {
        if (PreparationEnded != null) PreparationEnded();
    }

    public static void RaiseEvacuationCompleted()
    {
        if (EvacuationCompleted != null) EvacuationCompleted();
    }
}
