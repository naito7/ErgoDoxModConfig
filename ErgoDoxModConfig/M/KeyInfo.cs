using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgoDoxModConfig.M
{
    public class KeyInfo
    {
        public enum EKeyKind { Common, Alphabet, NumberRow, Spacing, Punctuation, FunctionKey, Navigation, NumberPad, ControlKey, ToggleLayers, PushLayers, PopLayers, Misc, };
        public string ScanCode { get; private set; }
        public string FaceText { get; private set; }
        public string FuncPress { get; private set; }
        public string FuncRelease { get; private set; }
        public bool IsModifier { get{ return KeyKind == EKeyKind.ControlKey;}}
        public EKeyKind KeyKind { get; private set; }
        public string KeyInfoKey { get { return string.Format("{0}:{1}", KeyKind==EKeyKind.Common ? FaceText : ScanCode, KeyKind); } }

        public KeyInfo(string scan_code, string face_text, EKeyKind kk, string func_press, string func_release)
        {
            KeyKind = kk;
            ScanCode = scan_code;
            FaceText = face_text;
            FuncPress = (func_press == "NULL" ? "" : "&") + func_press;
            FuncRelease = (func_release == "NULL" ? "" : "&") + func_release;
        }
    }
}
