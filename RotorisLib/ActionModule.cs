namespace RotorisLib
{
    /// <summary>
    /// Represents an action module, containing a script and a flag for sequential execution.
    /// </summary>
    public struct ActionModule
    {
        /// <summary>Indicates if the next action module in a sequence should be executed.</summary>
        public bool CallNext { set; get; }
        /// <summary>The script content of the module.</summary>
        public string Script { set; get; }

        /// <summary>
        /// Creates an <see cref="ActionModule"/> struct by parsing a script string.
        /// </summary>
        /// <param name="str">The full script string.</param>
        /// <returns>A new <see cref="ActionModule"/> instance.</returns>
        public ActionModule(string script)
        {
            bool shouldCallNext = script.TrimStart().StartsWith("--!call-next", System.StringComparison.Ordinal);
            CallNext = shouldCallNext;
            Script = script;
        }
    }
}
