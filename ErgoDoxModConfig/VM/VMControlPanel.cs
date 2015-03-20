using Microsoft.TeamFoundation.MVVM;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ErgoDoxModConfig.VM
{
    public class VMControlPanel : VMPanelBase
    {
        public ReactiveProperty<int> SelectedLayerIndex { get; private set; }
        public ReactiveProperty<bool> LayoutMode { get; private set; }
        public List<string> Layers { get; private set; }

        public ICommand MoveButtons
        {
            get
            {
                return new RelayCommand(() =>
                {
                    m_cfg.LayoutInfo.MoveButtons();
                });
            }
        }

        public ICommand RotateButtons
        {
            get
            {
                return new RelayCommand(() =>
                {
                    m_cfg.LayoutInfo.RotateButtons();
                });
            }
        }

        public ICommand OutputCommand
        {
            get
            {
                return new RelayCommand(() => m_cfg.Output());
            }
        }

        public VMControlPanel(ErgoDoxModConfig.M.Config cfg)
        {
            m_cfg = cfg;
            Layers = cfg.Layers.Select((l, i) => string.Format("Layer : {0}", i)).ToList();
            SelectedLayerIndex = new ReactiveProperty<int>(0);
            LayoutMode = new ReactiveProperty<bool>(false);
        }

        ErgoDoxModConfig.M.Config m_cfg;
    }
}
