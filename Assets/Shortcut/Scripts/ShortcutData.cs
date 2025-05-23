using UnityEngine;

namespace WC.Shortcuts
{
    /// <summary>
    /// Common data structure for handling shortcut info (both Android and iOS)
    /// </summary>
    public struct ShortcutData
    {
        /// <summary>OS determines if a shortcut has been added based on ID, and if shortcut is triggered, ID can be used to identify it</summary>
        public string id { get; private set; }

        /// <summary><i>[Android only]</i> If a shortcut is made into it's own icon, then it uses the short label</summary>
        public string shortLabel { get; private set; }

        /// <summary>The label used for both Android and iOS shortcuts (visible in long press context menu)</summary>
        public string longLabel { get; private set; }

        /// <summary>Optionally, icons can be added with shortcuts (recommended to be square dimensions and around 64x64 due to screen size)
        /// <para>Must be <b>'readable'</b> and <b>'uncompressed'</b> else no icon will be shown (shortcut will still be created)</para></summary>
        public Sprite icon { get; private set; }
        /// <summary>A set of predefined icons, these are 40x40 and closely resemble iOS system icons</summary>
        public ShortcutSystemIcons systemIcon { get; private set; } // Uses Google SF
        public ShortcutData(string id, string shortLabel, string longLabel, Sprite icon = null, ShortcutSystemIcons systemIcon = ShortcutSystemIcons.NONE)
        {
            this.id = id;
            this.shortLabel = shortLabel;
            this.longLabel = longLabel;
            this.icon = icon;
            this.systemIcon = systemIcon;
        }
    }

    public enum ShortcutSystemIcons
    {
        NONE,
        COMPOSE,
        PLAY,
        PAUSE,
        ADD,
        LOCATION,
        SEARCH,
        SHARE,
        PROHIBIT,
        CONTACT,
        HOME,
        MARK_LOCATION,
        FAVORITE,
        LOVE,
        CLOUD,
        INVITATION,
        CONFIRMATION,
        MAIL,
        MESSAGE,
        DATE,
        TIME,
        CAPTURE_PHOTO,
        CAPTURE_VIDEO,
        TASK,
        TASK_COMPLETED,
        ALARM,
        BOOKMARK,
        SHUFFLE,
        AUDIO,
        UPDATE
    }
}