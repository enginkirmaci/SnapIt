using SharpHook;
using SnapIt.Common.Contracts;

namespace SnapIt.Services.Contracts;

public interface IGlobalHookService : IInitialize
{
    SimpleGlobalHook? Hook { get; set; }
}