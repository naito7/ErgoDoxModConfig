using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Reactive.Bindings;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.TeamFoundation.MVVM;
using System.Reactive.Subjects;

namespace ErgoDoxModConfig.VM
{
    public class VMPanelBase
    {

    }

    public class VMMain
    {
        public ReactiveProperty<List<VMPanelBase>> Panels { get; private set; }
        public IObservable<List<VMKey>> Keys { get; private set; }
        private BehaviorSubject<List<string>> m_selected_element_key = new BehaviorSubject<List<string>>(new List<string>());

        ErgoDoxModConfig.M.Config m_cfg;
        VMControlPanel m_ctrl;
        public VMMain():this(new ErgoDoxModConfig.M.Config())
        {
        }
        public VMMain(ErgoDoxModConfig.M.Config cfg)
        {
            m_cfg = cfg;
            m_ctrl = new VMControlPanel(cfg);

            Keys = Observable.Merge(
                cfg
                .Layers
                .Select((l, i) => l.AssignInfos.CombineLatest(m_ctrl.SelectedLayerIndex,
                    (ai, selected_idx) => new { ai, selected_idx, layer_idx = i }))
                )
                .Do(x =>
                {
                    Debug.WriteLine(x);
                })
                .Where(layer_with_idx => layer_with_idx.layer_idx == layer_with_idx.selected_idx)
                .Select(layer_with_idx => layer_with_idx.ai)
                .Select(assign_infos =>
                    {
                        return assign_infos
                            .Select(ai => new VMKey(ai.LayoutElement, ai.KeyInfo, cfg, ai.Layer, m_ctrl.LayoutMode, m_selected_element_key))
                            .ToList();
                    })
                .ToReactiveProperty();
            Panels = Keys.Select(keys =>
            {
                var ret = new List<VMPanelBase>();
                ret.AddRange(keys);
                ret.Add(m_ctrl);
                return ret;
            })
            .ToReactiveProperty();
        }

        internal void SetKeyInputObservable(IObservable<Key> observable)
        {
            observable.Subscribe(
                k => m_cfg.LayoutInfo.Move(m_selected_element_key.Value, k));
        }
    }
}
