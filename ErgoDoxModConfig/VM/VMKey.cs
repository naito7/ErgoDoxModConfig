using Microsoft.TeamFoundation.MVVM;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ErgoDoxModConfig.VM
{
    public class VMKey : VMPanelBase
    {
        public Size KeySize { get; private set; }
        public Point CenterPoint { get; private set; }
        public string Text { get; private set; }
        public Transform Transform { get; private set; }
        public bool IsModifier { get; private set; }
        public double Left { get { return CenterPoint.X - KeySize.Width / 2; } }
        public double Top { get { return CenterPoint.Y - KeySize.Height / 2; } }
        public double Right { get { return CenterPoint.X + KeySize.Width / 2; } }
        public double Bottom { get { return CenterPoint.Y + KeySize.Height / 2; } }
        public double Width { get { return KeySize.Width; } }
        public double Height { get { return KeySize.Height; } }
        public ICommand CmdSingleSelect { get; private set; }
        public ICommand CmdMultiSelect { get; private set; }
        public ReactiveProperty<bool> IsSelected { get; private set; }

        public VMKey(M.LayoutInfo.LayoutElement elm, M.KeyInfo ki, M.Config cfg, M.Layer layer, IObservable<bool> layout_mode_seq, BehaviorSubject<List<string>> selected_element_key)
        {
            KeySize = elm.Size;
            CenterPoint = elm.CenterPoint;
            Text = elm.TitleText;
            Transform = elm.Transform;
            if (ki != null)
                IsModifier = ki.IsModifier;

            var element_key = elm.ElementKey;
            CmdSingleSelect = new RelayCommand(() => selected_element_key.OnNext(new List<string>(new []{element_key})));
            CmdMultiSelect = new RelayCommand(() => {
                var newlist = new List<string>(selected_element_key.Value);
                newlist.Add(element_key);
                selected_element_key.OnNext(newlist.Distinct().ToList());
            });
            IsSelected = selected_element_key
                .Select(s => s.Contains(element_key))
                .ToReactiveProperty();

            MenuItems = layout_mode_seq
                .Select(layout_mode =>
                {
                    var menu_items = new List<MenuItemInfo>();
                    menu_items.Add(new MenuItemInfo(elm.TitleText, null, MenuItemInfo.State.Normal));
                    menu_items.Add(MenuItemInfo.CreateSeparator());
                    if (layout_mode)
                    {
                        // レイアウト変更
                        double[,] size_vec = new double[,] { { 1, 1 }, { 1.5, 1 }, { 2, 1 }, { 1, 1.5 }, { 1, 2 } };
                        menu_items.AddRange(
                            Enumerable.Range(0, size_vec.GetLength(0))
                            .Select(idx => new { x = size_vec[idx,0], y = size_vec[idx,1]})
                            .Select(sr => new MenuItemInfo(string.Format("{0}x{1}", sr.x, sr.y), new RelayCommand(() =>{
                                cfg.LayoutInfo.ChangeSize(elm.ElementKey, sr.x, sr.y);
                            }), MenuItemInfo.State.Normal)));
                        menu_items.Add(MenuItemInfo.CreateSeparator());
                        menu_items.Add(new MenuItemInfo("Add New Key", new RelayCommand(() =>
                        {
                            cfg.LayoutInfo.AddNew();
                        }), MenuItemInfo.State.Normal));
                    }
                    else
                    {
                        // キー情報変更
                        menu_items.AddRange(cfg.AllKeyInfo.GroupBy(keyinfo => keyinfo.KeyKind).Select(
                            grp =>
                            {
                                var group_root = new MenuItemInfo(grp.Key.ToString(), null, MenuItemInfo.State.Normal);
                                group_root.AddRange(grp.Select(child_keyinfo =>
                                {
                                    bool ischecked = ki != null ? child_keyinfo.KeyInfoKey == ki.KeyInfoKey : false;
                                    ICommand cmd = null;
                                    if (!ischecked)
                                    {
                                        cmd = new RelayCommand(() => layer.Assign(element_key, child_keyinfo));
                                    }
                                    return new MenuItemInfo(child_keyinfo.FaceText, cmd, ischecked ? MenuItemInfo.State.Checked : MenuItemInfo.State.Normal);
                                }));
                                return group_root;
                            }));
                    }
                    return menu_items;
                })
                .ToReactiveProperty();



            if (ki!=null)
            {
                Text += "\n" + ki.FaceText;
            }

        }

        #region ContextMenu
        public class MenuItemInfo
        {
            public string MIHeader { get; private set; }
            public ICommand MICommand { get; private set; }
            public enum State { Normal, Unselectable, Checked, Separator }
            public bool IsChecked { get { return m_state == State.Checked ? true : false; } }
            public bool IsSelectable { get { return m_state == State.Unselectable ? false : true; } }
            public bool IsSeparator { get { return m_state == State.Separator ? true : false; } }
            public IReadOnlyCollection<MenuItemInfo> Children { get { return m_children; } }

            public MenuItemInfo(string header, ICommand cmd, State state)
            {
                MIHeader = header;
                MICommand = cmd;
                m_state = state;
            }
            public void AddChild(MenuItemInfo mii)
            {
                m_children.Add(mii);
            }
            public void AddRange(IEnumerable<MenuItemInfo> miivec)
            {
                m_children.AddRange(miivec);
            }
            public static MenuItemInfo CreateSeparator()
            {
                return new MenuItemInfo("", null, State.Separator);
            }

            #region private members
            private State m_state;
            private List<MenuItemInfo> m_children = new List<MenuItemInfo>();
            #endregion
        }
        public ReactiveProperty<List<MenuItemInfo>> MenuItems { get; private set; }
        #endregion
    }
}
