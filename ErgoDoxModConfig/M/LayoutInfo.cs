using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ErgoDoxModConfig.M
{
    public class LayoutInfo
    {
        private readonly int NUM_COLS = 14;
        private readonly int NUM_ROWS = 6;
        private readonly int SZ = 50;

        [DataContract(Name="LayoutElement")]
        public class LayoutElement
        {
            public string TitleText { get { return string.Format("{0}{2}:{1}", RectPrefix, ColIdx, RowIdx); } }
            public string ElementKey { get { return string.Format("{0}:{1}", ColIdx, RowIdx); } }
            [DataMember]
            public int RowIdx { get; private set; }
            [DataMember]
            public int ColIdx { get; private set; }
            [DataMember]
            public string RectPrefix { get; private set; }
            [DataMember]
            public Point CenterPoint { get; private set; }
            [DataMember]
            public Transform Transform { get; private set; }
            [DataMember]
            public Size Size { get; private set; }

            public LayoutElement(string rect_prefix, int row_idx, int col_idx, Point center_point, Size size):this(rect_prefix, row_idx, col_idx, center_point, size, new TranslateTransform())
            {
            }
            public LayoutElement(string rect_prefix, int row_idx, int col_idx, Point center_point, Size size, Transform transform)
            {
                RowIdx = row_idx;
                ColIdx = col_idx;
                RectPrefix = rect_prefix;
                CenterPoint = center_point;
                Size = size;
                Transform = transform;
            }
            public LayoutElement(LayoutElement src)
            {
                RowIdx = src.RowIdx;
                ColIdx = src.ColIdx;
                RectPrefix = src.RectPrefix;
                CenterPoint = src.CenterPoint;
                Transform = src.Transform.Clone();
                Size = src.Size;
            }
        }

        private BehaviorSubject<List<LayoutElement>> m_elements = new BehaviorSubject<List<LayoutElement>>(new List<LayoutElement>());
        public IObservable<List<LayoutElement>> Elements { get { return m_elements; } }

        public bool Add(string rect_prefix, int row_idx, int col_idx){
            var current_list = m_elements.Value;
            if (current_list.Find(e => e.RectPrefix == rect_prefix && e.RowIdx == row_idx && e.ColIdx == col_idx) != null)
                throw new InvalidOperationException(string.Format("{0}({1},{2}) already exists", rect_prefix, col_idx, row_idx));
            var new_list = new List<LayoutElement>(current_list);
            var size = create_size(1, 1);
            var center_point = new Point(SZ * (col_idx + 0.5), SZ * (row_idx + 0.5));
            new_list.Add(new LayoutElement(rect_prefix, row_idx, col_idx, center_point, size));
            m_elements.OnNext(new_list);
            return true;
        }
        private Size create_size(double size_x, double size_y)
        {
            return new Size(size_x * SZ, size_y * SZ);
        }

        internal void AddNew(string rect_prefix=null)
        {
            var rnc = from r in Enumerable.Range(0, NUM_ROWS)
                      from c in Enumerable.Range(0, NUM_COLS)
                      select Tuple.Create(r,c);

            var exists = m_elements.Value
                .Select(le => Tuple.Create(le.RowIdx, le.ColIdx));

            var remains = rnc.Except(exists)
                .OrderBy(rc => rc.Item1 * 100 + rc.Item2);

            if (rect_prefix == null) {
                var exists_first = m_elements.Value.FirstOrDefault();
                if (exists_first != null)
                    rect_prefix = exists_first.RectPrefix;
            }
            if (rect_prefix == null)
                throw new InvalidOperationException("rect_prefix is null");
            var new_element = remains.FirstOrDefault();
            if (new_element != null)
            {
                Add(rect_prefix, new_element.Item1, new_element.Item2);
            }
        }
        internal void AddAllKey(string rect_prefix)
        {
            var rnc = from r in Enumerable.Range(0, NUM_ROWS)
                      from c in Enumerable.Range(0, NUM_COLS)
                      select Tuple.Create(r,c);
            foreach (var x in rnc)
                AddNew(rect_prefix);
        }

        public List<string> GetAllElementKeys()
        {
            return m_elements.Value.Select(le => le.ElementKey).ToList();
        }
        public IReadOnlyCollection<LayoutElement> GetAllElements()
        {
            return m_elements.Value;
        }


        #region for Testing
        internal void MoveButtons()
        {
            change_layout_info(le =>
            {
                int x_off = 0, y_off = 0;
                if (le.TitleText == "SW0:3" || le.TitleText == "SW0:4" || le.TitleText == "SW0:5" || le.TitleText == "SW1:3" || le.TitleText == "SW1:4" || le.TitleText == "SW1:5")
                {
                    x_off = 10;
                }
                if (le.TitleText == "SW0:2" || le.TitleText == "SW1:2" || le.TitleText == "SW0:3" || le.TitleText == "SW1:3")
                {
                    y_off = 10;
                }
                return new LayoutElement(le.RectPrefix, le.RowIdx, le.ColIdx, new Point(le.CenterPoint.X + x_off, le.CenterPoint.Y + y_off), le.Size, le.Transform);
            });
        }

        internal void RotateButtons()
        {
            change_layout_info(le =>
            {
                if (le.TitleText == "SW0:2" || le.TitleText == "SW1:2")
                {
                    return new LayoutElement(le.TitleText, le.RowIdx, le.ColIdx, le.CenterPoint, le.Size, new RotateTransform(30, le.CenterPoint.X, le.CenterPoint.Y));
                }
                else if (le.TitleText == "SW0:3" || le.TitleText == "SW1:3")
                {
                    return new LayoutElement(le.RectPrefix, le.RowIdx, le.ColIdx, le.CenterPoint, le.Size, new RotateTransform(-30, le.CenterPoint.X, le.CenterPoint.Y));
                }
                else
                {
                    return new LayoutElement(le);
                }
            });
        }
#endregion

        #region save&load settings
        [DataContract(Name="LayoutInfo")]
        [KnownType(typeof(TranslateTransform))]
        [KnownType(typeof(RotateTransform))]
        class LayoutInfoConfig
        {
            [DataMember]
            public List<LayoutElement> Elements { get; set; }

        }
        private string config_file_path { get { return "layout_info.xml"; } }
        internal void SaveConfig()
        {
            var ser = new DataContractSerializer(typeof(LayoutInfoConfig));
            using (var file = new FileStream(config_file_path, FileMode.OpenOrCreate))
            {
                var lic = new LayoutInfoConfig { Elements = m_elements.Value };
                ser.WriteObject(file, lic);
            }
        }

        internal void LoadConfig()
        {
            try
            {
                using (var file = new FileStream(config_file_path, FileMode.Open))
                {
                    var ser = new DataContractSerializer(typeof(LayoutInfoConfig));
                    var lic = (LayoutInfoConfig)ser.ReadObject(file);
                    m_elements.OnNext(lic.Elements);
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing
            }
        }
        #endregion

        #region changing layout info
        private void change_layout_info(Func<LayoutElement, LayoutElement> f)
        {
            var current_list = m_elements.Value;
            var new_list = new List<LayoutElement>();
            new_list.AddRange(current_list.Select(le => f(le)));
            m_elements.OnNext(new_list);
        }
        internal void Move(List<string> element_key, System.Windows.Input.Key k)
        {
            double x_off = 0, y_off = 0;
            double diff_sz = SZ;
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                diff_sz /= 2;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
                diff_sz /= 4;
            if (k == System.Windows.Input.Key.Left)
                x_off -= diff_sz;
            if (k == System.Windows.Input.Key.Right)
                x_off += diff_sz;
            if (k == System.Windows.Input.Key.Up)
                y_off -= diff_sz;
            if (k == System.Windows.Input.Key.Down)
                y_off += diff_sz;

            change_layout_info(le => element_key.Contains(le.ElementKey) ?
                new LayoutElement(le.RectPrefix, le.RowIdx, le.ColIdx, new Point(le.CenterPoint.X + x_off, le.CenterPoint.Y + y_off), le.Size, le.Transform) :
                le);
        }
        internal void ChangeSize(string element_key, double size_x, double size_y)
        {
            change_layout_info(le => le.ElementKey == element_key ?
                new LayoutElement(le.RectPrefix, le.RowIdx, le.ColIdx, le.CenterPoint, create_size(size_x, size_y), le.Transform) :
                le);
        }
        #endregion
    }
}
