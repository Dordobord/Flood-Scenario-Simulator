using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    public event Action<bool> Finished;

    public event Action<string> FeedbackShown;

    public event Action<string> InstructionsChanged;

    public virtual bool UsesConfirm
    {
        get { return false; }
    }

    public virtual bool CanReopen
    {
        get { return false; }
    }

    public int Mistakes { get; protected set; }

    protected MinigameSettings settings;

    private bool finished;

    protected bool IsFinished
    {
        get { return finished; }
    }

    public void Begin(MinigameSettings newSettings)
    {
        settings = newSettings;
        finished = false;
        Mistakes = 0;
        gameObject.SetActive(true);
        Setup();
    }

    public void Reopen(MinigameSettings newSettings)
    {
        settings = newSettings;
        finished = true;
        gameObject.SetActive(true);
        ShowFinished();
    }

    protected virtual void ShowFinished()
    {
    }

    public virtual void End()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Setup();

    public abstract void Tick();

    public virtual void Confirm()
    {
    }

    protected void Finish(bool success)
    {
        if (finished) return;
        finished = true;

        if (Finished != null) Finished(success);
    }

    protected void ShowFeedback(string message)
    {
        if (FeedbackShown != null) FeedbackShown(message);
    }

    protected void ShowInstructions(string message)
    {
        if (InstructionsChanged != null) InstructionsChanged(message);
    }

    protected RectTransform GetCanvasLayer()
    {
        Canvas canvas = GetComponentInParent<Canvas>(true);
        if (canvas == null) return (RectTransform)transform;
        return (RectTransform)canvas.rootCanvas.transform;
    }

    protected static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    protected static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
        }
    }
}
