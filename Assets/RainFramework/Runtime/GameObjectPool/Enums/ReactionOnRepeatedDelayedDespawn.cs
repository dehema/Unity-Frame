namespace Rain.Core
{
    internal enum ReactionOnRepeatedDelayedDespawn
    {
        Ignore,
        ResetDelay,
        ResetDelayIfNewTimeIsLess,
        ResetDelayIfNewTimeIsGreater,
        ThrowException
    }
}