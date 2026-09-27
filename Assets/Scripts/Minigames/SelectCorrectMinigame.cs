using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StormWaits
{
    
    
    
    public class SelectCorrectMinigame : MinigameBase
    {
        [SerializeField] private Transform choicesParent;
        [SerializeField] private ChoiceButton choiceButtonPrefab;
        [SerializeField] private Button confirmButton;

        private List<ChoiceButton> choiceButtons = new List<ChoiceButton>();

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        protected override void Setup()
        {
            
            ClearChildren(choicesParent);
            choiceButtons.Clear();

            
            List<SelectChoice> choices = new List<SelectChoice>();
            if (settings.choices != null) choices.AddRange(settings.choices);
            Shuffle(choices);

            foreach (SelectChoice choice in choices)
            {
                ChoiceButton newButton = Instantiate(choiceButtonPrefab, choicesParent);
                newButton.Setup(choice);
                choiceButtons.Add(newButton);
            }
        }

        
        public override void Tick()
        {
        }

        private void OnConfirmClicked()
        {
            bool allCorrect = true;

            foreach (ChoiceButton choiceButton in choiceButtons)
            {
                if (choiceButton.IsSelected != choiceButton.Choice.isCorrect)
                {
                    allCorrect = false;
                    break;
                }
            }

            Finish(allCorrect);
        }
    }
}
