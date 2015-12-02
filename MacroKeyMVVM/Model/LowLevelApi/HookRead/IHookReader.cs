using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi.HookReader
{
    public interface IHookReader
    {
        IList<IInput> ReadSequence { get; }

        bool IsRecord { get; }

        void Clear();

        void StartNewRecord();

        void StartRecord();

        void StopRecord();
    }
}
