
public interface ICapabilityProvider
{
    bool TryGet<T>(out T capability) where T : ICapability;
}
