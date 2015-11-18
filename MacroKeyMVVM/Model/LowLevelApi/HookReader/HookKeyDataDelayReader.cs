﻿using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    public class HookKeyDataDelayReader : HookReader<KeyDataDelay>
    {
        private KeyDataDelay mPrewKeyData = new KeyDataDelay(0, 0, 0, 0);

        public HookKeyDataDelayReader(IHooker hooker) : base(hooker) { }

        public HookKeyDataDelayReader(IHooker hooker, IList<KeyDataDelay> sequence) : base(hooker, sequence) { }

        protected override bool RecordSequence(HookEventArgs e)
        {
            var arg = (KeyHookEventArgs)e;
            KeyDataDelay keyData = new KeyDataDelay(arg.VirtualKeyCode, arg.KeyMassage, arg.Time, 0);
            if (keyData.Message != mPrewKeyData.Message || keyData.VirtualKeyCode != mPrewKeyData.VirtualKeyCode)
                ReadSequence.Add(keyData);
            mPrewKeyData = keyData;
            return true;
        }
    }
}