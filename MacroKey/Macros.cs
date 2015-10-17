using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey
{
    public class Macros : IEnumerable
    {
        private IEnumerable<KeyData> m_macro { get; }
        private IEnumerable<KeyData> m_sequence { get; }

        public Macros(IEnumerable<KeyData> sequence, IEnumerable<KeyData> macros)
        {
            m_sequence = sequence;
            m_macro = macros;
        }

        public IEnumerator GetEnumerator()
        {
            return m_macro.GetEnumerator();
        }

        public override string ToString()
        {
            Func<KeyData, string> getStr = keyData => keyData.KeyMessage == KeyData.KeyboardMessage.WM_KEYDOWM ? keyData.KeyValue.ToLower() : keyData.KeyValue.ToUpper();
            return string.Join("", m_sequence.Select(getStr)) + " -> " + string.Join("", m_macro.Select(getStr));
        }
    }
}
