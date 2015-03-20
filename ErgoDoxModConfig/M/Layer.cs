using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ErgoDoxModConfig.M
{
    [DataContract]
    public class Layer
    {
        public Layer(LayoutInfo li)
        {
            Init(li);
        }

        private AssignInfo create_assign_info(Dictionary<string, KeyInfo> assign_map, LayoutInfo.LayoutElement elm)
        {
            return new AssignInfo(elm.ElementKey, elm,
                        assign_map.ContainsKey(elm.ElementKey) ?
                        assign_map[elm.ElementKey] :
                        null,
                        this);
        }

        private void Init(LayoutInfo li)
        {
            m_li = li;
            m_assign_map = new BehaviorSubject<Dictionary<string, KeyInfo>>(new Dictionary<string, KeyInfo>());

            AssignInfos =
                li.Elements
                .CombineLatest(m_assign_map, (elms, assign_map) => new { elms, assign_map })
                .Select(elm_and_map =>
                    elm_and_map.elms
                    .Select(elm => create_assign_info(elm_and_map.assign_map, elm))
                    .ToList()
                );
        }

        public void Assign(string element_key, KeyInfo ki){
            var current_assign_map = m_assign_map.Value;
            var new_assign_map = new Dictionary<string, KeyInfo>(current_assign_map);
            new_assign_map[element_key] = ki;
            m_assign_map.OnNext(new_assign_map);
        }

        public class AssignInfo
        {
            public string ElementKey { get; private set; }
            public LayoutInfo.LayoutElement LayoutElement { get; private set; }
            public KeyInfo KeyInfo { get; private set; }
#region safe keyinfo accessor
            private string get_safe(Func<KeyInfo, string> f, string def) { return KeyInfo != null ? f(KeyInfo) : def; }
            public string SafeScanCode { get { return get_safe(ki => ki.ScanCode, "0"); } }
            public string SafeFaceText { get { return get_safe(ki => ki.FaceText, ""); } }
            public string SafeFuncPress { get { return get_safe(ki => ki.FuncPress, "NULL"); } }
            public string SafeFuncRelease { get { return get_safe(ki => ki.FuncRelease, "NULL"); } }
#endregion
            public Layer Layer
            {
                get
                {
                    Layer ret;
                    if (m_weak_layer.TryGetTarget(out ret))
                        return ret;
                    throw new InvalidOperationException(string.Format("AssignInfo{0} has no parent layout", ElementKey));
                }
            }

            public AssignInfo(string element_key, LayoutInfo.LayoutElement layout_element, KeyInfo key_info, Layer layer)
            {
                ElementKey = element_key;
                LayoutElement = layout_element;
                KeyInfo = key_info;
                m_weak_layer = new WeakReference<M.Layer>(layer);
            }

            WeakReference<Layer> m_weak_layer;
        }
        public IEnumerable<AssignInfo> CurrentAssignInfo { get {
            var am = m_assign_map.Value;
            return m_li.GetAllElements()
                .Select(elm => create_assign_info(am, elm));
        } }

        BehaviorSubject<Dictionary<string, KeyInfo>> m_assign_map;
        public IObservable<List<AssignInfo>> AssignInfos { get; private set; }
        LayoutInfo m_li;

        #region save&load settings
        [DataContract]
        class AssignInfoPersistent{
            [DataMember]
            public string ElementKey{get; set;}
            [DataMember]
            public string KeyInfoKey{get; set;}
        }
        [DataMember]
        List<AssignInfoPersistent> AssignInfoPersistents { get; set; }
        [OnSerializing]
        private void OnSerializeing(StreamingContext c)
        {
            AssignInfoPersistents = m_assign_map.Value
                .Select(kv => new AssignInfoPersistent { ElementKey = kv.Key, KeyInfoKey = kv.Value.KeyInfoKey })
                .ToList();
        }
        public void PostDeserialized(LayoutInfo li, Func<string, KeyInfo> find_key)
        {
            Init(li);
            foreach (var aip in AssignInfoPersistents)
            {
                Assign(aip.ElementKey, find_key(aip.KeyInfoKey));
            }
            // discard persistents
            AssignInfoPersistents = null;
        }
        #endregion
    }
}
