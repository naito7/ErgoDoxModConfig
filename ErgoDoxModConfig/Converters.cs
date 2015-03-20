using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ErgoDoxModConfig
{
    public class BoolToBrushForSelected : IValueConverter
    {
        // 概要:
        //     値を変換します。
        //
        // パラメーター:
        //   value:
        //     バインディング ソースによって生成された値。
        //
        //   targetType:
        //     バインディング ターゲット プロパティの型。
        //
        //   parameter:
        //     使用するコンバーター パラメーター。
        //
        //   culture:
        //     コンバーターで使用するカルチャ。
        //
        // 戻り値:
        //     変換された値。 メソッドが null を返す場合は、有効な null 値が使用されています。
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return v ? Brushes.Red : Brushes.Transparent;
        }
        //
        // 概要:
        //     値を変換します。
        //
        // パラメーター:
        //   value:
        //     バインディング ターゲットによって生成される値。
        //
        //   targetType:
        //     変換後の型。
        //
        //   parameter:
        //     使用するコンバーター パラメーター。
        //
        //   culture:
        //     コンバーターで使用するカルチャ。
        //
        // 戻り値:
        //     変換された値。 メソッドが null を返す場合は、有効な null 値が使用されています。
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
