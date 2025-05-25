using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WC.Shortcuts.Demo
{
    public class ShortcutTest : MonoBehaviour
    {
        [Space, Header("UI")]
        [SerializeField] private TMP_Text _shortcutCountText;
        [SerializeField] private TMP_Text _currentShortcutsText;
        [SerializeField] private TMP_Text _shortcutTriggerText;

        [Space, Header("Shortcut")]
        [SerializeField] private Sprite _giftSprite;

        private string _shortcutIDPrefix = "com.example.gamename";
        private Coroutine _displayCoroutine;

        private void Awake()
        {
            ShortcutManager.OnShortcutTriggered += OnTestShortcutTriggered;
        }

        private void Start()
        {
            List<string> words = new() { "love", "great", "wow!" };
            ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.love", "Reason to love", "Reason: " + words[Random.Range(0, words.Count)], null, ShortcutSystemIcons.LOVE));
            ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.daily_gift", "Get your gift!", "Hello, have gift!", _giftSprite));

            UpdateShortcutCountText();
            UpdateCurrentShortcutsText();
            _shortcutTriggerText.text = "";
        }

        private void OnTestShortcutTriggered(string ID, ShortcutTriggerType triggerType)
        {
            UpdateShortcutTriggerText(ID, triggerType);
        }

        #region UI

        public void OnContactBtnClicked()
        {
            if (ShortcutManager.HasShortcut($"{_shortcutIDPrefix}.support"))
            {
                ShortcutManager.RemoveShortcut($"{_shortcutIDPrefix}.support");
            }
            else
            {
                ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.support", "Contact support", "Issues? Let us know!", null, ShortcutSystemIcons.MAIL));
            }

            UpdateShortcutCountText();
            UpdateCurrentShortcutsText();
        }

        private void UpdateShortcutCountText()
        {
            int count = ShortcutManager.ShortcutCount;
            _shortcutCountText.text = $"Shortcut Count: {count}";
        }

        private void UpdateCurrentShortcutsText()
        {
            var ids = ShortcutManager.ShortcutIDs;
            if(ids == null)
            {
                _currentShortcutsText.text = "Current Shortcuts: None";
            }
            else
            {
                System.Text.StringBuilder stringBuilder = new("Current Shortcuts:\n");
                for(int i = 0; i < ids.Length; i++)
                    stringBuilder.AppendLine($"{i+1}. {ids[i]}");
                _currentShortcutsText.text = stringBuilder.ToString();
            }
        }

        private void UpdateShortcutTriggerText(string ID, ShortcutTriggerType triggerType)
        {
            if(_displayCoroutine != null)
                StopCoroutine(_displayCoroutine);
            _displayCoroutine = StartCoroutine(DisplayShortcutTriggerTextCoroutine(ID, triggerType, 3f));
        }

        private IEnumerator DisplayShortcutTriggerTextCoroutine(string ID, ShortcutTriggerType triggerType, float delay)
        {
            yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("ShortcutDemo", System.StringComparison.OrdinalIgnoreCase));
            _shortcutTriggerText.text = $"Triggered: {ID}, {triggerType.ToString()}";
            yield return new WaitForSeconds(delay);
            _shortcutTriggerText.text = "";
            _displayCoroutine = null;
        }

        #endregion
    }

}