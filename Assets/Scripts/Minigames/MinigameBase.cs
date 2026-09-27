using System;
using System.Collections.Generic;
using UnityEngine;

namespace StormWaits
{

    public abstract class MinigameBase : MonoBehaviour
    {
        public event Action<bool> Finished;

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

        public void End()
        {
            gameObject.SetActive(false);
        }

        
        protected abstract void Setup();

        
        public abstract void Tick();

        protected void Finish(bool success)
        {
            if (finished) return;
            finished = true;

            if (Finished != null) Finished(success);
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
}
