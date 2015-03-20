using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ErgoDoxModConfig
{
    class KeyViewSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var e = container as FrameworkElement;
            var vmk = item as VM.VMKey;
            if (e != null && vmk != null)
            {
                if (vmk.IsModifier)
                    return e.FindResource("VMKeyModifier") as DataTemplate;
                else
                    return e.FindResource("VMKeyNormal") as DataTemplate;

            }
            var vmc = item as VM.VMControlPanel;
            if (e != null && vmc != null)
            {
                return e.FindResource("VMKeyControlPanel") as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
