﻿using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    public class HookReader : IHookReader
    {
        private IHooker mHooker;

        public bool IsRecord { get; private set; } = false;

        public IList<IInput> ReadSequence { get; }

        private HookEventHandler hookEventHandler;

        public HookReader(IHooker hooker)
        {
            mHooker = hooker;
            hookEventHandler = RecordSequence;
            ReadSequence = new List<IInput>();
        }

        public HookReader(IHooker hooker, IList<IInput> sequence) : this(hooker)
        {
            ReadSequence = sequence;
        }

        protected virtual bool RecordSequence(IInput e)
        {
            ReadSequence.Add(e);
            return true;
        }

        public void StartNewRecord()
        {
            Clear();
            StartRecord();
        }

        public void StartRecord()
        {
            IsRecord = true;
            mHooker.Hooked += hookEventHandler;
        }

        public void StopRecord()
        {
            IsRecord = false;
            mHooker.Hooked -= hookEventHandler;
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }
    }
}
