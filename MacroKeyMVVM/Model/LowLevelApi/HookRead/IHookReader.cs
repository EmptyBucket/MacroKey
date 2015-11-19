using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi.HookReader
{
    interface IHookReader
    {
        IList<Input> ReadSequence { get; }

        bool IsRecord { get; }

        void Clear();

        void StartNewRecord();

        void StartRecord();

        void StopRecord();
    }
}
