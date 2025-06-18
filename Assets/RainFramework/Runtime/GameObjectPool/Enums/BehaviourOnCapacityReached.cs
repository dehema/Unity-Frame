namespace Rain.Core
{
    public enum BehaviourOnCapacityReached : byte
    {
        ReturnNullableClone,
        Instantiate,
        InstantiateWithCallbacks,
        Recycle,
        ThrowException
    }
}