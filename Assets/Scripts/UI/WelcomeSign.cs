using UnityEngine;
using static Managers.GameManager;

namespace UI
{
    public class WelcomeSign : UiUpdater
    {
        private TextMesh _textMesh;
        private const string SignContent = "Welcome Adventurers!\nPopulation: ";

        private void Start()
        {
            _textMesh = GetComponent<TextMesh>();
        }

        protected override void UpdateUi()
        {
            _textMesh.text = SignContent + (Manager.Adventurers.Count > 0 ? Manager.Adventurers.Count.ToString() : "None");
        }
    }
}
