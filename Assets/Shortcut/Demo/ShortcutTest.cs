using System.Collections.Generic;
using UnityEngine;

namespace   WC.Shortcuts.Demo
{
    public class ShortcutTest : MonoBehaviour
    {
        [SerializeField] private Sprite _giftSprite;

        private string _shortcutIDPrefix = "com.example.gamename";

        private void Awake()
        {
            ShortcutManager.OnShortcutTriggered += OnTestShortcutTriggered;
        }

        private void Start()
        {
            List<string> words = new() { "love", "great", "wow!" };
            ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.love", "Reason to love", "Reason: " + words[Random.Range(0, words.Count)], null, ShortcutSystemIcons.LOVE));
            ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.daily_gift", "Get your gift!", "Hello, have gift!", _giftSprite));
            ShortcutManager.CreateShortcut(new ShortcutData($"{_shortcutIDPrefix}.support", "Contact support", "Issues? Let us know!", null, ShortcutSystemIcons.MAIL));
        }

        private void OnTestShortcutTriggered(string ID, ShortcutTriggerType triggerType)
        {
            Debug.Log($"Shortcut was triggered: {ID}, type: {triggerType}");
        }
    }

}