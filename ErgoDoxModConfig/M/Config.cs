using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErgoDoxModConfig.M
{
    [DataContract]
    public class LayersHolder
    {
        [DataMember]
        public List<Layer> Layers = new List<Layer>();

        #region save&load settings
        private string config_file_path { get { return "layeres.xml"; } }
        internal void SaveConfig()
        {
            var ser = new DataContractSerializer(typeof(LayersHolder));
            using (var file = new FileStream(config_file_path, FileMode.OpenOrCreate))
            {
                ser.WriteObject(file, this);
            }
        }

        internal void LoadConfig(LayoutInfo li, Func<string, KeyInfo> find_key)
        {
            try
            {
                using (var file = new FileStream(config_file_path, FileMode.Open))
                {
                    var ser = new DataContractSerializer(typeof(LayersHolder));
                    var lic = (LayersHolder)ser.ReadObject(file);
                    this.Layers = lic.Layers;
                    foreach (var l in Layers)
                    {
                        l.PostDeserialized(li, find_key);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing
            }
        }
        #endregion

    }
    public class Config
    {
        public LayersHolder LayersHolder = new LayersHolder();
        public List<Layer> Layers { get { return LayersHolder.Layers; } }
        public LayoutInfo LayoutInfo = new LayoutInfo();
        private List<KeyInfo> KeyInfo = new List<KeyInfo>();

        static List<KeyInfo> create_normal_keys(string[,] src, KeyInfo.EKeyKind kind)
        {
            return Enumerable.Range(0, src.GetLength(0))
                .Select(i => new {scan_code=src[i,0], face=src[i,1]})
                .Select(sp => new KeyInfo(sp.scan_code, sp.face, kind, "kbfun_press_release", "kbfun_press_release"))
                .ToList();
        }
        static IEnumerable<KeyInfo> create_layer_keys(IEnumerable<int> e, KeyInfo.EKeyKind kind)
        {
            return e.Select(i =>
            {
                string func_press = "NULL", func_release = "NULL";
                if (kind == ErgoDoxModConfig.M.KeyInfo.EKeyKind.ToggleLayers || kind == ErgoDoxModConfig.M.KeyInfo.EKeyKind.PushLayers)
                    func_press = string.Format("kbfun_layer_push_{0}", i);
                if(kind == ErgoDoxModConfig.M.KeyInfo.EKeyKind.PopLayers)
                    func_press = string.Format("kbfun_layer_pop_{0}", i);
                if(kind == ErgoDoxModConfig.M.KeyInfo.EKeyKind.ToggleLayers)
                    func_release = string.Format("kbfun_layer_pop_{0}", i);
                return new KeyInfo(i.ToString(), kind.ToString() + i, kind, func_press, func_release);
            });
        }
        static List<KeyInfo> create_keyinfo()
        {
            var ret = new List<KeyInfo>();

            ret.Add(new KeyInfo("0", "trans", ErgoDoxModConfig.M.KeyInfo.EKeyKind.Common, "kbfun_transparent", "kbfun_transparent"));
            ret.Add(new KeyInfo("0", "[Teensy]", ErgoDoxModConfig.M.KeyInfo.EKeyKind.Common, "kbfun_jump_to_bootloader", "NULL"));
            ret.Add(new KeyInfo("0", "[L0]", ErgoDoxModConfig.M.KeyInfo.EKeyKind.Common, "kbfun_layer_pop_all", "NULL"));
            var alphabets = new string[,]{
                {"KEY_a_A", "A a"},
                {"KEY_b_B", "B b"},
                {"KEY_c_C", "C c"},
                {"KEY_d_D", "D d"},
                {"KEY_e_E", "E e"},
                {"KEY_f_F", "F f"},
                {"KEY_g_G", "G g"},
                {"KEY_h_H", "H h"},
                {"KEY_i_I", "I i"},
                {"KEY_j_J", "J j"},
                {"KEY_k_K", "K k"},
                {"KEY_l_L", "L l"},
                {"KEY_m_M", "M m"},
                {"KEY_n_N", "N n"},
                {"KEY_o_O", "O o"},
                {"KEY_p_P", "P p"},
                {"KEY_q_Q", "Q q"},
                {"KEY_r_R", "R r"},
                {"KEY_s_S", "S s"},
                {"KEY_t_T", "T t"},
                {"KEY_u_U", "U u"},
                {"KEY_v_V", "V v"},
                {"KEY_w_W", "W w"},
                {"KEY_x_X", "X x"},
                {"KEY_y_Y", "Y y"},
                {"KEY_z_Z", "Z z"},
            };
            ret.AddRange(create_normal_keys(alphabets, ErgoDoxModConfig.M.KeyInfo.EKeyKind.Alphabet));
            var numrows = new string[,]{
                {"KEY_1_Exclamation", "! 1"},
                {"KEY_2_At", "@ 2"},
                {"KEY_3_Pound", "# 3"},
                {"KEY_4_Dollar", "$ 4"},
                {"KEY_5_Percent", "% 5"},
                {"KEY_6_Caret", "^ 6"},
                {"KEY_7_Ampersand", "& 7"},
                {"KEY_8_Asterisk", "* 8"},
                {"KEY_9_LeftParenthesis", "( 9"},
                {"KEY_0_RightParenthesis", ") 0"},
            };
            ret.AddRange(create_normal_keys(numrows, ErgoDoxModConfig.M.KeyInfo.EKeyKind.NumberRow));
            var spacings = new string[,]{
                {"KEY_ReturnEnter", "[Ent]"},
                {"KEY_Escape", "[Esc]"},
                {"KEY_DeleteBackspace", "[Bkspc]"},
                {"KEY_Tab", "[Tab]"},
                {"KEY_Spacebar", "[Spc]"},
            };
            ret.AddRange(create_normal_keys(spacings, ErgoDoxModConfig.M.KeyInfo.EKeyKind.Spacing));
            var punctuations = new string[,]{
                {"KEY_Dash_Underscore", "_ -"},
                {"KEY_Equal_Plus", "+ ="},
                {"KEY_LeftBracket_LeftBrace", "{ ["},
                {"KEY_RightBracket_RightBrace", "} ]"},
                {"KEY_Backslash_Pipe", "| \\"},
                {"KEY_Semicolon_Colon", ": ;"},
                {"KEY_SingleQuote_DoubleQuote", "\" '"},
                {"KEY_GraveAccent_Tilde", "~ `"},
                {"KEY_Comma_LessThan", "< ,"},
                {"KEY_Period_GreaterThan", "> ."},
                {"KEY_Slash_Question", "? /"},
            };
            ret.AddRange(create_normal_keys(punctuations, ErgoDoxModConfig.M.KeyInfo.EKeyKind.Punctuation));
            var functionkeys = new string[,]{
                {"KEY_F1", "F1"},
                {"KEY_F2", "F2"},
                {"KEY_F3", "F3"},
                {"KEY_F4", "F4"},
                {"KEY_F5", "F5"},
                {"KEY_F6", "F6"},
                {"KEY_F7", "F7"},
                {"KEY_F8", "F8"},
                {"KEY_F9", "F9"},
                {"KEY_F10", "F10"},
                {"KEY_F11", "F11"},
                {"KEY_F12", "F12"},
            };
            ret.AddRange(create_normal_keys(functionkeys, ErgoDoxModConfig.M.KeyInfo.EKeyKind.FunctionKey));
            var navigations = new string[,]{
                {"KEY_Insert", "[Ins]"},
                {"KEY_Home", "[Home]"},
                {"KEY_PageUp", "[PgUp]"},
                {"KEY_DeleteForward", "[Del]"},
                {"KEY_End", "[End]"},
                {"KEY_PageDown", "[PgDn]"},
                {"KEY_RightArrow", "→"},
                {"KEY_LeftArrow", "←"},
                {"KEY_DownArrow", "↓"},
                {"KEY_UpArrow", "↑"},
            };
            ret.AddRange(create_normal_keys(navigations, ErgoDoxModConfig.M.KeyInfo.EKeyKind.Navigation));
            var controlkeys = new string[,]{
                {"KEY_LeftControl", "[LCtrl]"},
                {"KEY_LeftShift", "[LShift]"},
                {"KEY_LeftAlt", "[LAlt]"},
                {"KEY_LeftGUI", "[LGui]"},
                {"KEY_RightControl", "[RCtrl]"},
                {"KEY_RightShift", "[RShift]"},
                {"KEY_RightAlt", "[RAlt]"},
                {"KEY_RightGUI", "[RGui]"},
            };
            ret.AddRange(create_normal_keys(controlkeys, ErgoDoxModConfig.M.KeyInfo.EKeyKind.ControlKey));
            var numberpads = new string[,]{
                {"KEYPAD_NumLock_Clear", "[NumLk]"},
                {"KEYPAD_Slash", "/"},
                {"KEYPAD_Asterisk", "*"},
                {"KEYPAD_Minus", "-"},
                {"KEYPAD_Plus", "+"},
                {"KEYPAD_ENTER", "[NEnt]"},
                {"KEYPAD_1_End", "[End] 1"},
                {"KEYPAD_2_DownArrow", "↓ 2"},
                {"KEYPAD_3_PageDown", "[PgDn] 3"},
                {"KEYPAD_4_LeftArrow", "← 4"},
                {"KEYPAD_5", "5"},
                {"KEYPAD_6_RightArrow", "→ 6"},
                {"KEYPAD_7_Home", "[Home] 7"},
                {"KEYPAD_8_UpArrow", "↑ 8"},
                {"KEYPAD_9_PageUp", "[PgUp] 9"},
                {"KEYPAD_0_Insert", "[NIns]"},
                {"KEYPAD_Period_Delete", "[Del] ."},
            };
            ret.AddRange(create_normal_keys(numrows, ErgoDoxModConfig.M.KeyInfo.EKeyKind.NumberRow));
            var miscs = new string[,]{
                {"KEY_PrintScreen", "[PrtSc]"},
                {"KEY_CapsLock", "[CapsLk]"},
                {"KEY_ScrollLock", "[ScrLk]"},
                {"KEY_Pause", "[Pause]"},
                {"KEY_NonUS_Backslash_Pipe", "| \\"},
                {"KEY_NonUS_Pound_Tilde", "~ #"},
            };
            ret.AddRange(create_normal_keys(miscs, ErgoDoxModConfig.M.KeyInfo.EKeyKind.Misc));
            ret.AddRange(create_layer_keys(Enumerable.Range(1,10), ErgoDoxModConfig.M.KeyInfo.EKeyKind.PushLayers));
            ret.AddRange(create_layer_keys(Enumerable.Range(1,10), ErgoDoxModConfig.M.KeyInfo.EKeyKind.PopLayers));
            ret.AddRange(create_layer_keys(Enumerable.Range(1,10), ErgoDoxModConfig.M.KeyInfo.EKeyKind.ToggleLayers));
            return ret;
        }

        public Config()
        {
            // Initialize KeyInfo
            //KeyInfo.Add(new KeyInfo("A", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("B", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("C", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("D", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("E", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("F", M.KeyInfo.EKeyKind.Alphabet, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("RETURN", M.KeyInfo.EKeyKind.Spacing, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("SPC", M.KeyInfo.EKeyKind.Spacing, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("ESC", M.KeyInfo.EKeyKind.Spacing, "PRESS_RELEASE", "PRESS_RELEASE"));
            //KeyInfo.Add(new KeyInfo("LSHIFT", M.KeyInfo.EKeyKind.ControlKey, "ADD_MODIFYER", "REMOVE_MODIFYER"));
            //KeyInfo.Add(new KeyInfo("RSHIFT", M.KeyInfo.EKeyKind.ControlKey, "ADD_MODIFYER", "REMOVE_MODIFYER"));
            KeyInfo = create_keyinfo();

            // Initialize LayoutInfo
            //LayoutInfo.Add("SW", 0, 0);
            //LayoutInfo.Add("SW", 0, 1);
            //LayoutInfo.Add("SW", 0, 2);
            //LayoutInfo.Add("SW", 0, 3);
            //LayoutInfo.Add("SW", 0, 4);
            //LayoutInfo.Add("SW", 0, 5);
            //LayoutInfo.Add("SW", 1, 0);
            //LayoutInfo.Add("SW", 1, 1);
            //LayoutInfo.Add("SW", 1, 2);
            //LayoutInfo.Add("SW", 1, 3);
            //LayoutInfo.Add("SW", 1, 4);
            //LayoutInfo.Add("SW", 1, 5);
            LayoutInfo.AddAllKey("SW");

            // Initalize Layers
            foreach (var i in Enumerable.Range(0, 3))
            {
                var layer = new Layer(LayoutInfo);
                layer.Assign("SW0:0", KeyInfo[i]);
                Layers.Add(layer);
            }

        }

        public IReadOnlyCollection<KeyInfo> AllKeyInfo { get { return KeyInfo; } }

        #region save&load settings
        internal void SaveConfig()
        {
            LayoutInfo.SaveConfig();
            LayersHolder.SaveConfig();
        }

        internal void LoadConfig()
        {
            LayoutInfo.LoadConfig();
            LayersHolder.LoadConfig(LayoutInfo, (string key) =>
            {
                var k = KeyInfo.Find(ki => ki.KeyInfoKey == key);
                if (k == null)
                    throw new InvalidOperationException(string.Format("KeyInfo {0} not found...", key));
                return k;
            });
        }
        #endregion

        internal bool Output()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".c";
            dlg.Filter = "C|*.c";
            dlg.FileName = "default--layout.c";
            var res = dlg.ShowDialog();
            if (res == true)
            {
                using (var fst = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
                using (var fs = new StreamWriter(fst, Encoding.ASCII))
                {
                    var sb = new StringBuilder(1024 * 128);
                    // write header
                    sb.Append(@"
/* ----------------------------------------------------------------------------
* ergoDOX layout : auto generated layout (modified from the Kinesis layout)
* -----------------------------------------------------------------------------
* Copyright (c) 2012 Ben Blazak <benblazak.dev@gmail.com>
* Released under The MIT License (MIT) (see ""license.md"")
* Project located at <https://github.com/benblazak/ergodox-firmware>
* -------------------------------------------------------------------------- */
#include <stdint.h>
#include <stddef.h>
#include <avr/pgmspace.h>
#include ""../../../lib/data-types/misc.h""
#include ""../../../lib/usb/usage-page/keyboard--short-names.h""
#include ""../../../lib/key-functions/public.h""
#include ""../matrix.h""
#include ""../layout.h""
// FUNCTIONS ------------------------------------------------------------------
void kbfun_layer_pop_all(void) {
  kbfun_layer_pop_1();
  kbfun_layer_pop_2();
  kbfun_layer_pop_3();
  kbfun_layer_pop_4();
  kbfun_layer_pop_5();
  kbfun_layer_pop_6();
  kbfun_layer_pop_7();
  kbfun_layer_pop_8();
  kbfun_layer_pop_9();
  kbfun_layer_pop_10();
}
");

                    foreach (var i in Enumerable.Range(0, 3))
                    {
                        switch (i)
                        {
                            case 0:
                                sb.Append(@"// LAYOUT ---------------------------------------------------------------------
const uint8_t PROGMEM _kb_layout[KB_LAYERS][KB_ROWS][KB_COLUMNS] = {
");
                                break;
                            case 1:
                                sb.Append(@"// PRESS ----------------------------------------------------------------------
const void_funptr_t PROGMEM _kb_layout_press[KB_LAYERS][KB_ROWS][KB_COLUMNS] = {
");
                                break;
                            case 2:
                                sb.Append(@"// RELEASE --------------------------------------------------------------------
const void_funptr_t PROGMEM _kb_layout_release[KB_LAYERS][KB_ROWS][KB_COLUMNS] = {
");
                                break;
                        }

                        foreach (var l in Layers.Select((l,idx)=>new{l,idx}))
                        {
                            sb.AppendFormat("// LAYER {0}\n{{\n", l.idx);
                            foreach (var ai_grp in l.l.CurrentAssignInfo.GroupBy(ai => ai.LayoutElement.RowIdx))
                            {
                                sb.AppendFormat("{{ // row {0}\n", ai_grp.Key);
                                foreach (var ai in ai_grp)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            sb.AppendFormat("    {0}", ai.SafeScanCode);
                                            break;
                                        case 1:
                                            sb.AppendFormat("    {0}", ai.SafeFuncPress);
                                            break;
                                        case 2:
                                            sb.AppendFormat("    {0}", ai.SafeFuncRelease);
                                            break;
                                    }
                                    sb.AppendFormat(", // {0} {1} {2} {3} {4}\n", ai.LayoutElement.TitleText, ai.SafeFaceText, ai.SafeScanCode, ai.SafeFuncPress, ai.SafeFuncRelease);
                                }
                                sb.Append("},\n");
                            }
                            sb.Append("},\n");
                        }

                        sb.Append("};\n");
                    }
                    fs.Write(sb.Replace("\r\n", "\n").Replace("\n", "\r\n"));
                }
            }
            return true;
        }
    }
}
