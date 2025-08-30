using ProcGenSys.Common.LevelBundle;

namespace ProcGenSys.Common.LevelBundle
{
    public interface ICapabilityProvider
    {
        bool TryGet<T>(out T capability) where T : ICapability;
    }

}
