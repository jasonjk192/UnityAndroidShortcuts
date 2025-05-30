namespace WC.Shortcuts
{
    internal static class ShortcutManagerEditorData
    {
        /// <summary>[Editor only] Supply a list of IDs to be simulated. Keep 'none' as the 1st element for clarify in the inspector.
        /// <para>Since we are not actually managing shortcuts in Editor, we can edit this array with the IDs we want to simulate.</para>
        /// </summary>
        internal static readonly string[] simulationShortcutIDs = new string[]
        {
            "none", // Don't edit this out

            // From ShortcutTest.cs; replace your own IDs for simulation
            "com.example.gamename.love",
            "com.example.gamename.daily_gift",
        };
    }
}

