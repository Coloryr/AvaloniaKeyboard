using System;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using RimeSharp;

namespace AvaloniaKeyboard.Views;

public partial class KeyboardControl : TemplatedControl
{
    public static readonly StyledProperty<int> KeySizeProperty =
            AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySize), 50);
    public static readonly StyledProperty<int> LoopCountProperty =
            AvaloniaProperty.Register<KeyboardControl, int>(nameof(LoopCount), 25);

    public static readonly StyledProperty<bool> IsENInputProperty =
           AvaloniaProperty.Register<KeyboardControl, bool>(nameof(IsENInput), true);
    public static readonly StyledProperty<bool> IsCapsProperty =
           AvaloniaProperty.Register<KeyboardControl, bool>(nameof(IsCaps), false);
    public static readonly StyledProperty<bool> IsShiftProperty =
           AvaloniaProperty.Register<KeyboardControl, bool>(nameof(IsShift), false);
    public static readonly StyledProperty<bool> IsCtrlProperty =
           AvaloniaProperty.Register<KeyboardControl, bool>(nameof(IsShift), false);
    public static readonly StyledProperty<bool> IsAltProperty =
           AvaloniaProperty.Register<KeyboardControl, bool>(nameof(IsShift), false);

    public static readonly StyledProperty<TextBox?> TextBoxProperty =
           AvaloniaProperty.Register<KeyboardControl, TextBox?>(nameof(TextBox));

    public int KeySize
    {
        get { return GetValue(KeySizeProperty); }
        set { SetValue(KeySizeProperty, value); }
    }
    public int LoopCount
    {
        get { return GetValue(LoopCountProperty); }
        set { SetValue(LoopCountProperty, value); }
    }
    public bool IsENInput
    {
        get { return GetValue(IsENInputProperty); }
        set { SetValue(IsENInputProperty, value); }
    }
    public bool IsCaps
    {
        get { return GetValue(IsCapsProperty); }
        set { SetValue(IsCapsProperty, value); }
    }
    public bool IsShift
    {
        get { return GetValue(IsShiftProperty); }
    }
    public bool IsCtrl
    {
        get { return GetValue(IsCtrlProperty); }
    }
    public bool IsAlt
    {
        get { return GetValue(IsAltProperty); }
    }

    public TextBox? TextBox
    {
        get { return GetValue(TextBoxProperty); }
        set { SetValue(TextBoxProperty, value); }
    }

    private static readonly string[] LanList = ["中", "en"];

    private bool _isLeftShift, _isRightShift;
    private bool _isLeftCtrl, _isRightCtrl, _isLeftAlt, _isRightAlt;
    private bool _isRun;

    private Key _keyDownSave;
    private int _count;
    private readonly ITransform _matrix = new MatrixTransform(Matrix.CreateScale(0.95, 0.95));
    private IntPtr _session;
    private string _input = "";
    private int _select = 0;
    private int _page = 0;
    private int _maxPage = 0;

    public class InputedEventArgs(TextBox? text) : EventArgs
    {
        public TextBox? TextBox { get; init; } = text;
    }

    public event EventHandler<InputedEventArgs>? Inputed;

#pragma warning disable CS8618
    internal StackPanel CHSelect;
    internal Button LanSwitch;
    internal TextBlock Input;
    internal TextBlock InputPage;
    internal WrapPanel InputSelect;
    internal Border Oem3;
    internal Border NumPad1;
    internal Border NumPad2;
    internal Border NumPad3;
    internal Border NumPad4;
    internal Border NumPad5;
    internal Border NumPad6;
    internal Border NumPad7;
    internal Border NumPad8;
    internal Border NumPad9;
    internal Border NumPad0;
    internal Border OemMinus;
    internal Border OemPlus;
    internal Border Back;
    internal Border Tab;
    internal Border Q;
    internal Border W;
    internal Border E;
    internal Border R;
    internal Border T;
    internal Border Y;
    internal Border U;
    internal Border I;
    internal Border O;
    internal Border P;
    internal Border Oem4;
    internal Border Oem6;
    internal Border Oem5;
    internal Border CapsLock;
    internal Border A;
    internal Border S;
    internal Border D;
    internal Border F;
    internal Border G;
    internal Border H;
    internal Border J;
    internal Border K;
    internal Border L;
    internal Border Oem1;
    internal Border Oem7;
    internal Border Enter;
    internal Border LeftShift;
    internal Border Z;
    internal Border X;
    internal Border C;
    internal Border V;
    internal Border B;
    internal Border N;
    internal Border M;
    internal Border OemComma;
    internal Border OemPeriod;
    internal Border Oem2;
    internal Border RightShift;
    internal Border LeftCtrl;
    internal Border LeftAlt;
    internal Border Space;
    internal Border RightAlt;
    internal Border RightCtrl;
    internal Border Escape;
#pragma warning restore CS8618
#pragma warning disable CS8601
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CHSelect = e.NameScope.Find<StackPanel>("CHSelect");
        LanSwitch = e.NameScope.Find<Button>("LanSwitch");
        Input = e.NameScope.Find<TextBlock>("Input");
        InputPage = e.NameScope.Find<TextBlock>("InputPage");
        InputSelect = e.NameScope.Find<WrapPanel>("InputSelect");
        Oem3 = e.NameScope.Find<Border>("Oem3");
        NumPad1 = e.NameScope.Find<Border>("NumPad1");
        NumPad2 = e.NameScope.Find<Border>("NumPad2");
        NumPad3 = e.NameScope.Find<Border>("NumPad3");
        NumPad4 = e.NameScope.Find<Border>("NumPad4");
        NumPad5 = e.NameScope.Find<Border>("NumPad5");
        NumPad6 = e.NameScope.Find<Border>("NumPad6");
        NumPad7 = e.NameScope.Find<Border>("NumPad7");
        NumPad8 = e.NameScope.Find<Border>("NumPad8");
        NumPad9 = e.NameScope.Find<Border>("NumPad9");
        NumPad0 = e.NameScope.Find<Border>("NumPad0");
        OemMinus = e.NameScope.Find<Border>("OemMinus");
        OemPlus = e.NameScope.Find<Border>("OemPlus");
        Back = e.NameScope.Find<Border>("Back");
        Tab = e.NameScope.Find<Border>("Tab");
        Q = e.NameScope.Find<Border>("Q");
        W = e.NameScope.Find<Border>("W");
        E = e.NameScope.Find<Border>("E");
        R = e.NameScope.Find<Border>("R");
        T = e.NameScope.Find<Border>("T");
        Y = e.NameScope.Find<Border>("Y");
        U = e.NameScope.Find<Border>("U");
        I = e.NameScope.Find<Border>("I");
        O = e.NameScope.Find<Border>("O");
        P = e.NameScope.Find<Border>("P");
        Oem4 = e.NameScope.Find<Border>("Oem4");
        Oem6 = e.NameScope.Find<Border>("Oem6");
        Oem5 = e.NameScope.Find<Border>("Oem5");
        CapsLock = e.NameScope.Find<Border>("CapsLock");
        A = e.NameScope.Find<Border>("A");
        S = e.NameScope.Find<Border>("S");
        D = e.NameScope.Find<Border>("D");
        F = e.NameScope.Find<Border>("F");
        G = e.NameScope.Find<Border>("G");
        H = e.NameScope.Find<Border>("H");
        J = e.NameScope.Find<Border>("J");
        K = e.NameScope.Find<Border>("K");
        L = e.NameScope.Find<Border>("L");
        Oem1 = e.NameScope.Find<Border>("Oem1");
        Oem7 = e.NameScope.Find<Border>("Oem7");
        Enter = e.NameScope.Find<Border>("Enter");
        LeftShift = e.NameScope.Find<Border>("LeftShift");
        Z = e.NameScope.Find<Border>("Z");
        X = e.NameScope.Find<Border>("X");
        C = e.NameScope.Find<Border>("C");
        V = e.NameScope.Find<Border>("V");
        B = e.NameScope.Find<Border>("B");
        N = e.NameScope.Find<Border>("N");
        M = e.NameScope.Find<Border>("M");
        OemComma = e.NameScope.Find<Border>("OemComma");
        OemPeriod = e.NameScope.Find<Border>("OemPeriod");
        Oem2 = e.NameScope.Find<Border>("Oem2");
        RightShift = e.NameScope.Find<Border>("RightShift");
        LeftCtrl = e.NameScope.Find<Border>("LeftCtrl");
        LeftAlt = e.NameScope.Find<Border>("LeftAlt");
        Space = e.NameScope.Find<Border>("Space");
        RightAlt = e.NameScope.Find<Border>("RightAlt");
        RightCtrl = e.NameScope.Find<Border>("RightCtrl");
        Escape = e.NameScope.Find<Border>("Escape");

        AddEvent();
        InitDisplay();
    }
#pragma warning restore CS8601

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsCapsProperty)
        {
            if (IsCaps)
            {
                CapsLock.Classes.Add("k2");
            }
            else
            {
                CapsLock.Classes.Remove("k2");
            }
        }
        else if (change.Property == IsENInputProperty)
        {
            LanSwitch.Content = LanList[IsENInput ? 1 : 0];
            _count = 0;
            _keyDownSave = Key.None;
            RimeClear();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Dispatcher.UIThread.ShutdownStarted += UIThread_ShutdownStarted;
        _isRun = true;

        DispatcherTimer.Run(() =>
        {
            Tick();
            return _isRun;
        }, TimeSpan.FromMilliseconds(20));
    }

    private void UIThread_ShutdownStarted(object? sender, EventArgs e)
    {
        _isRun = false;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Dispatcher.UIThread.ShutdownStarted -= UIThread_ShutdownStarted;
        _isRun = false;
    }

    public void AddEvent()
    {
        LanSwitch.Click += LanSwitch_Click;
        Oem3.PointerPressed += (a, b) => KeyInput(a, Key.Oem3);
        Oem3.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem3);
        NumPad1.PointerPressed += (a, b) => KeyInput(a, Key.NumPad1);
        NumPad1.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad1);
        NumPad2.PointerPressed += (a, b) => KeyInput(a, Key.NumPad2);
        NumPad2.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad2);
        NumPad3.PointerPressed += (a, b) => KeyInput(a, Key.NumPad3);
        NumPad3.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad3);
        NumPad4.PointerPressed += (a, b) => KeyInput(a, Key.NumPad4);
        NumPad4.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad4);
        NumPad5.PointerPressed += (a, b) => KeyInput(a, Key.NumPad5);
        NumPad5.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad5);
        NumPad6.PointerPressed += (a, b) => KeyInput(a, Key.NumPad6);
        NumPad6.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad6);
        NumPad7.PointerPressed += (a, b) => KeyInput(a, Key.NumPad7);
        NumPad7.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad7);
        NumPad8.PointerPressed += (a, b) => KeyInput(a, Key.NumPad8);
        NumPad8.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad8);
        NumPad9.PointerPressed += (a, b) => KeyInput(a, Key.NumPad9);
        NumPad9.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad9);
        NumPad0.PointerPressed += (a, b) => KeyInput(a, Key.NumPad0);
        NumPad0.PointerReleased += (a, b) => OnKeyUp(a, Key.NumPad0);
        OemMinus.PointerPressed += (a, b) => KeyInput(a, Key.OemMinus);
        OemMinus.PointerReleased += (a, b) => OnKeyUp(a, Key.OemMinus);
        OemPlus.PointerPressed += (a, b) => KeyInput(a, Key.OemPlus);
        OemPlus.PointerReleased += (a, b) => OnKeyUp(a, Key.OemPlus);
        Back.PointerPressed += (a, b) => KeyInput(a, Key.Back);
        Back.PointerReleased += (a, b) => OnKeyUp(a, Key.Back);
        Tab.PointerPressed += (a, b) => KeyInput(a, Key.Tab);
        Tab.PointerReleased += (a, b) => OnKeyUp(a, Key.Tab);
        Q.PointerPressed += (a, b) => KeyInput(a, Key.Q);
        Q.PointerReleased += (a, b) => OnKeyUp(a, Key.Q);
        W.PointerPressed += (a, b) => KeyInput(a, Key.W);
        W.PointerReleased += (a, b) => OnKeyUp(a, Key.W);
        E.PointerPressed += (a, b) => KeyInput(a, Key.E);
        E.PointerReleased += (a, b) => OnKeyUp(a, Key.E);
        R.PointerPressed += (a, b) => KeyInput(a, Key.R);
        R.PointerReleased += (a, b) => OnKeyUp(a, Key.R);
        T.PointerPressed += (a, b) => KeyInput(a, Key.T);
        T.PointerReleased += (a, b) => OnKeyUp(a, Key.T);
        Y.PointerPressed += (a, b) => KeyInput(a, Key.Y);
        Y.PointerReleased += (a, b) => OnKeyUp(a, Key.Y);
        U.PointerPressed += (a, b) => KeyInput(a, Key.U);
        U.PointerReleased += (a, b) => OnKeyUp(a, Key.U);
        I.PointerPressed += (a, b) => KeyInput(a, Key.I);
        I.PointerReleased += (a, b) => OnKeyUp(a, Key.I);
        O.PointerPressed += (a, b) => KeyInput(a, Key.O);
        O.PointerReleased += (a, b) => OnKeyUp(a, Key.O);
        P.PointerPressed += (a, b) => KeyInput(a, Key.P);
        P.PointerReleased += (a, b) => OnKeyUp(a, Key.P);
        Oem4.PointerPressed += (a, b) => KeyInput(a, Key.Oem4);
        Oem4.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem4);
        Oem6.PointerPressed += (a, b) => KeyInput(a, Key.Oem6);
        Oem6.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem6);
        Oem5.PointerPressed += (a, b) => KeyInput(a, Key.Oem5);
        Oem5.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem5);
        CapsLock.PointerPressed += (a, b) => KeyInput(a, Key.CapsLock);
        CapsLock.PointerReleased += (a, b) => OnKeyUp(a, Key.CapsLock);
        A.PointerPressed += (a, b) => KeyInput(a, Key.A);
        A.PointerReleased += (a, b) => OnKeyUp(a, Key.A);
        S.PointerPressed += (a, b) => KeyInput(a, Key.S);
        S.PointerReleased += (a, b) => OnKeyUp(a, Key.S);
        D.PointerPressed += (a, b) => KeyInput(a, Key.D);
        D.PointerReleased += (a, b) => OnKeyUp(a, Key.D);
        F.PointerPressed += (a, b) => KeyInput(a, Key.F);
        F.PointerReleased += (a, b) => OnKeyUp(a, Key.F);
        G.PointerPressed += (a, b) => KeyInput(a, Key.G);
        G.PointerReleased += (a, b) => OnKeyUp(a, Key.G);
        H.PointerPressed += (a, b) => KeyInput(a, Key.H);
        H.PointerReleased += (a, b) => OnKeyUp(a, Key.H);
        J.PointerPressed += (a, b) => KeyInput(a, Key.J);
        J.PointerReleased += (a, b) => OnKeyUp(a, Key.J);
        K.PointerPressed += (a, b) => KeyInput(a, Key.K);
        K.PointerReleased += (a, b) => OnKeyUp(a, Key.K);
        L.PointerPressed += (a, b) => KeyInput(a, Key.L);
        L.PointerReleased += (a, b) => OnKeyUp(a, Key.L);
        Oem1.PointerPressed += (a, b) => KeyInput(a, Key.Oem1);
        Oem1.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem1);
        Oem7.PointerPressed += (a, b) => KeyInput(a, Key.Oem7);
        Oem7.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem7);
        Enter.PointerPressed += (a, b) => KeyInput(a, Key.Enter);
        Enter.PointerReleased += (a, b) => OnKeyUp(a, Key.Enter);
        LeftShift.PointerPressed += (a, b) => KeyInput(a, Key.LeftShift);
        LeftShift.PointerReleased += (a, b) => OnKeyUp(a, Key.LeftShift);
        Z.PointerPressed += (a, b) => KeyInput(a, Key.Z);
        Z.PointerReleased += (a, b) => OnKeyUp(a, Key.Z);
        X.PointerPressed += (a, b) => KeyInput(a, Key.X);
        X.PointerReleased += (a, b) => OnKeyUp(a, Key.X);
        C.PointerPressed += (a, b) => KeyInput(a, Key.C);
        C.PointerReleased += (a, b) => OnKeyUp(a, Key.C);
        V.PointerPressed += (a, b) => KeyInput(a, Key.V);
        V.PointerReleased += (a, b) => OnKeyUp(a, Key.V);
        B.PointerPressed += (a, b) => KeyInput(a, Key.B);
        B.PointerReleased += (a, b) => OnKeyUp(a, Key.B);
        N.PointerPressed += (a, b) => KeyInput(a, Key.N);
        N.PointerReleased += (a, b) => OnKeyUp(a, Key.N);
        M.PointerPressed += (a, b) => KeyInput(a, Key.M);
        M.PointerReleased += (a, b) => OnKeyUp(a, Key.M);
        OemComma.PointerPressed += (a, b) => KeyInput(a, Key.OemComma);
        OemComma.PointerReleased += (a, b) => OnKeyUp(a, Key.OemComma);
        OemPeriod.PointerPressed += (a, b) => KeyInput(a, Key.OemPeriod);
        OemPeriod.PointerReleased += (a, b) => OnKeyUp(a, Key.OemPeriod);
        Oem2.PointerPressed += (a, b) => KeyInput(a, Key.Oem2);
        Oem2.PointerReleased += (a, b) => OnKeyUp(a, Key.Oem2);
        RightShift.PointerPressed += (a, b) => KeyInput(a, Key.RightShift);
        RightShift.PointerReleased += (a, b) => OnKeyUp(a, Key.RightShift);
        LeftCtrl.PointerPressed += (a, b) => KeyInput(a, Key.LeftCtrl);
        LeftCtrl.PointerReleased += (a, b) => OnKeyUp(a, Key.LeftCtrl);
        LeftAlt.PointerPressed += (a, b) => KeyInput(a, Key.LeftAlt);
        LeftAlt.PointerReleased += (a, b) => OnKeyUp(a, Key.LeftAlt);
        Space.PointerPressed += (a, b) => KeyInput(a, Key.Space);
        Space.PointerReleased += (a, b) => OnKeyUp(a, Key.Space);
        RightAlt.PointerPressed += (a, b) => KeyInput(a, Key.RightAlt);
        RightAlt.PointerReleased += (a, b) => OnKeyUp(a, Key.RightAlt);
        RightCtrl.PointerPressed += (a, b) => KeyInput(a, Key.RightCtrl);
        RightCtrl.PointerReleased += (a, b) => OnKeyUp(a, Key.RightCtrl);
        Escape.PointerPressed += (a, b) => KeyInput(a, Key.Escape);
        Escape.PointerReleased += (a, b) => OnKeyUp(a, Key.Escape);
    }

    private void InitDisplay()
    {
        LanSwitch.Content = LanList[IsENInput ? 1 : 0];

        if (RimeUtils.IsEnable)
        {
            _session = Rime.CreateSession();
            CHSelect.IsVisible = true;
        }
        else
        {
            CHSelect.IsVisible = false;
        }
    }

    private static string PrintComposition(RimeComposition composition, out string input)
    {
        string preedit = composition.preedit;
        if (preedit == null)
        {
            input = "";
            return "";
        }
        int len = composition.length;
        int start = composition.sel_start;
        int end = composition.sel_end;
        int cursor = composition.cursor_pos;
        var temp = Encoding.UTF8.GetBytes(preedit);
        using var list = new MemoryStream();
        using var list1 = new MemoryStream();
        for (int i = 0; i <= len; ++i)
        {
            if (start < end)
            {
                if (i == start)
                {
                    list.WriteByte((byte)'[');
                }
                else if (i == end)
                {
                    list.WriteByte((byte)']');
                }
            }
            if (i == cursor)
            {
                list.WriteByte((byte)'|');
            }
            if (i < len)
            {
                if (i >= start && temp[i] != (byte)' ')
                {
                    list1.WriteByte(temp[i]);
                }
                list.WriteByte(temp[i]);
            }
        }

        input = Encoding.UTF8.GetString(list1.ToArray());
        return Encoding.UTF8.GetString(list.ToArray());
    }

    private string PrintMenu(RimeMenu menu)
    {
        InputSelect.Children.Clear();
        if (menu.num_candidates == 0)
        {
            return "";
        }
        var page = $"{menu.page_no + 1} / {menu.page_size}  ";
        _select = menu.highlighted_candidate_index;
        _page = menu.page_no;
        _maxPage = menu.page_size;

        var builder = new StringBuilder();
        for (int i = 0; i < menu.num_candidates; ++i)
        {
            bool highlighted = i == menu.highlighted_candidate_index;
            var label = new Label()
            {
                Content = $"{i + 1}. {(highlighted ? '[' : ' ')}{menu.candidates[i].text}" +
                $"{(highlighted ? ']' : ' ')}{menu.candidates[i].comment ?? ""}",
                Background = Brush.Parse("#FAFAFA"),
                BorderBrush = Brush.Parse("#EFEFEF"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(2, 0, 2, 0)
            };
            label.PointerPressed += (a, b) =>
            {
                string temp = ((a as Label)!.Content as string)!;
                RimeInput($"{temp[..1]}");
            };
            InputSelect.Children.Add(label);
        }

        return page;
    }

    private void RimeClear()
    {
        _input = "";
        if (Rime.SetInput(_session, ""))
        {
            RimeDisplay();
        }
    }

    private void RimeInput(string key)
    {
        var res = Rime.SimulateKeySequence(_session, key);
        if (res)
        {
            RimeDisplay(key);
        }
    }

    private void RimeDelete()
    {
        if (_input.Length > 0)
        {
            if (Rime.SimulateKeySequence(_session, "{BackSpace}"))
            {
                RimeDisplay();
            }
        }
        else
        {
            var box = TextBox;
            if (box != null)
            {
                DeleteText(box);
            }
        }
    }

    private void RimeTab()
    {
        _select++;
        if (_select >= 9)
        {
            _select = 0;
        }
        if (Rime.HighlightCandidateOnCurrentPage(_session, (ulong)_select))
        {
            RimeDisplay();
        }
    }

    private void RimeLast()
    {
        if (_page == 0)
        {
            return;
        }

        if (Rime.ChangePage(_session, true))
        {
            RimeDisplay();
        }
    }

    private void RimeNext()
    {
        if (_page >= _maxPage - 1)
        {
            return;
        }

        if (Rime.ChangePage(_session, false))
        {
            RimeDisplay();
        }
    }

    private void RimeDisplay(string key = "")
    {
        bool comm = false;
        if (Rime.GetCommit(_session, out var res) && res is { } commit)
        {
            var box = TextBox;
            if (box != null)
            {
                comm = true;
                InsertText(box, commit.text);
                _input = "";
            }
        }
        if (Rime.RimeGetContext(_session, out var res1) && res1 is { } context)
        {
            if (context.composition.length > 0 || context.menu.num_candidates > 0)
            {
                Input.Text = PrintComposition(context.composition, out _input);
            }
            else
            {
                _input = "";
                Input.Text = "";
                if (!comm && TextBox is { } box)
                {
                    InsertText(box, key);
                }
            }

            InputPage.Text = PrintMenu(context.menu);
        }
        else
        {
            _input = "";
            Input.Text = "";
            InputSelect.Children.Clear();
        }
    }

    private void Tick()
    {
        if (_isRun == false)
        {
            return;
        }
        var box = TextBox;
        if (box == null || _keyDownSave == Key.None)
        {
            _count = 0;
            return;
        }
        if (_count > LoopCount)
        {
            if (_keyDownSave == Key.Tab)
            {
                PressTab();
            }
            else if (_keyDownSave == Key.Back)
            {
                PressBack();
            }
            else if (_keyDownSave == Key.OemComma)
            {
                PressOemComma();
            }
            else if (_keyDownSave == Key.OemPeriod)
            {
                PressOemPeriod();
            }
            else
            {
                PressKey(_keyDownSave);
            }
        }
        else
        {
            _count++;
        }
    }

    private static void InsertText(TextBox textBox, string text)
    {
        // 获取当前光标位置
        int insertionIndex = textBox.SelectionStart;

        if (textBox.Text == null)
        {
            textBox.Text = text;
        }
        else
        {
            int selectionLength = textBox.SelectionEnd - textBox.SelectionStart;
            if (selectionLength > 0)
            {
                textBox.Text = textBox.Text.Remove(insertionIndex, selectionLength);
            }

            textBox.Text = textBox.Text.Insert(insertionIndex, text);
        }

        textBox.SelectionStart = textBox.SelectionEnd = insertionIndex + text.Length;
    }

    /// <summary>
    /// 删除光标前的字符
    /// </summary>
    /// <param name="textBox"></param>
    private static void DeleteTextBeforeCursor(TextBox textBox)
    {
        int cursorPosition = textBox.SelectionStart;
        if (cursorPosition > 0 && textBox.Text != null)
        {
            textBox.Text = textBox.Text.Remove(cursorPosition - 1, 1);
            textBox.SelectionEnd = textBox.SelectionStart = cursorPosition - 1;
        }
    }

    /// <summary>
    /// 删除光标后的字符
    /// </summary>
    /// <param name="textBox"></param>
    //private static void DeleteTextAfterCursor(TextBox textBox)
    //{
    //    int cursorPosition = textBox.SelectionStart;
    //    if (cursorPosition < textBox.Text.Length)
    //    {
    //        textBox.Text = textBox.Text.Remove(cursorPosition, 1);
    //        // 光标位置不需要改变
    //    }
    //}

    /// <summary>
    /// 删除选中的文本
    /// </summary>
    /// <param name="textBox"></param>
    private static void DeleteSelectedText(TextBox textBox)
    {
        if (textBox.Text != null)
        {
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionEnd - textBox.SelectionStart;

            if (selectionLength > 0)
            {
                textBox.Text = textBox.Text.Remove(selectionStart, selectionLength);
                textBox.SelectionEnd = textBox.SelectionStart = selectionStart;
            }
        }
    }

    private static void DeleteText(TextBox textBox)
    {
        if (textBox.SelectionStart > 0 && textBox.SelectionEnd > 0
            && textBox.SelectionStart != textBox.SelectionEnd)
        {
            DeleteSelectedText(textBox);
        }
        else
        {
            DeleteTextBeforeCursor(textBox);
        }
    }

    private void LanSwitch_Click(object? sender, RoutedEventArgs e)
    {
        IsENInput = !IsENInput;
    }

    private void PressTab()
    {
        if (IsENInput)
        {
            if (TextBox is { } box)
            {
                InsertText(box, "  ");
            }
        }
        else
        {
            RimeTab();
        }
    }

    private void PressBack()
    {
        if (IsENInput)
        {
            if (TextBox is { } box)
            {
                DeleteText(box);
            }
        }
        else
        {
            RimeDelete();
        }
    }

    private void PressOemComma()
    {
        if (IsENInput)
        {
            if (TextBox is { } box)
            {
                InsertText(box, KeyToChar(Key.OemComma).ToString());
            }
        }
        else
        {
            RimeLast();
        }
    }

    private void PressOemPeriod()
    {
        if (IsENInput)
        {
            if (TextBox is { } box)
            {
                InsertText(box, KeyToChar(Key.OemPeriod).ToString());
            }
        }
        else
        {
            RimeNext();
        }
    }

    private void PressKey(Key key)
    {
        if (IsENInput)
        {
            if (TextBox is { } box)
            {
                InsertText(box, KeyToChar(key).ToString());
            }
        }
        else
        {
            RimeInput(KeyToChar(key).ToString());
        }
    }

    private void KeyInput(object? obj, Key key)
    {
        if (obj is Border border)
        {
            border.RenderTransform = _matrix;
        }

        if (TextBox == null)
        {
            return;
        }

        switch (key)
        {
            case Key.Escape:
                Inputed?.Invoke(this, new(TextBox));
                return;
            case Key.CapsLock:
                IsCaps = !IsCaps;
                return;
            case Key.LeftCtrl:
                _isLeftCtrl = true;
                SetValue(IsCtrlProperty, _isLeftCtrl || _isRightCtrl);
                return;
            case Key.RightCtrl:
                _isRightCtrl = true;
                SetValue(IsCtrlProperty, _isLeftCtrl || _isRightCtrl);
                return;
            case Key.LeftAlt:
                _isLeftAlt = true;
                SetValue(IsAltProperty, _isLeftAlt || _isRightAlt);
                return;
            case Key.RightAlt:
                _isRightAlt = true;
                SetValue(IsAltProperty, _isLeftAlt || _isRightAlt);
                return;
            case Key.LeftShift:
                _isLeftShift = true;
                SetValue(IsShiftProperty, _isLeftShift || _isRightShift);
                return;
            case Key.RightShift:
                _isRightShift = true;
                SetValue(IsShiftProperty, _isLeftShift || _isRightShift);
                return;
            case Key.Back:
                _count = 0;
                _keyDownSave = Key.Back;
                PressBack();
                return;
            case Key.Tab:
                _count = 0;
                _keyDownSave = Key.Tab;
                PressTab();
                break;
            case Key.OemComma:
                _count = 0;
                _keyDownSave = key;
                PressOemComma();
                break;
            case Key.OemPeriod:
                _count = 0;
                _keyDownSave = key;
                PressOemPeriod();
                break;
            default:
                _count = 0;
                _keyDownSave = key;
                PressKey(key);
                return;
        }
    }

    private void OnKeyUp(object? obj, Key key)
    {
        if (obj is Border border)
        {
            border.RenderTransform = null;
        }

        switch (key)
        {
            case Key.LeftCtrl:
                _isLeftCtrl = false;
                SetValue(IsCtrlProperty, _isLeftCtrl || _isRightCtrl);
                return;
            case Key.RightCtrl:
                _isRightCtrl = false;
                SetValue(IsCtrlProperty, _isLeftCtrl || _isRightCtrl);
                return;
            case Key.LeftAlt:
                _isLeftAlt = false;
                SetValue(IsAltProperty, _isLeftAlt || _isRightAlt);
                return;
            case Key.RightAlt:
                _isRightAlt = false;
                SetValue(IsAltProperty, _isLeftAlt || _isRightAlt);
                return;
            case Key.LeftShift:
                _isLeftShift = false;
                SetValue(IsShiftProperty, _isLeftShift || _isRightShift);
                return;
            case Key.RightShift:
                _isLeftShift = false;
                SetValue(IsShiftProperty, _isLeftShift || _isRightShift);
                return;
            default:
                _count = 0;
                _keyDownSave = Key.None;
                return;
        }
    }

    private char GeCapsKey(char a, char b)
    {
        var b2 = IsCaps;
        var b1 = IsShift;
        if (b1 && b2)
        {
            return a;
        }
        else if (b1 && !b2 || !b1 && b2)
        {
            return b;
        }

        return a;
    }

    private char GetShiftKey(char a, char b)
    {
        return IsShift ? a : b;
    }

    private char KeyToChar(Key key)
    {
        return key switch
        {
            Key.Oem3 => GetShiftKey('~', '`'),
            Key.NumPad1 => GetShiftKey('!', '1'),
            Key.NumPad2 => GetShiftKey('@', '2'),
            Key.NumPad3 => GetShiftKey('#', '3'),
            Key.NumPad4 => GetShiftKey('$', '4'),
            Key.NumPad5 => GetShiftKey('%', '5'),
            Key.NumPad6 => GetShiftKey('^', '6'),
            Key.NumPad7 => GetShiftKey('&', '7'),
            Key.NumPad8 => GetShiftKey('*', '8'),
            Key.NumPad9 => GetShiftKey('(', '9'),
            Key.NumPad0 => GetShiftKey(')', '0'),
            Key.OemMinus => GetShiftKey('_', '-'),
            Key.OemPlus => GetShiftKey('+', '='),
            Key.Q => GeCapsKey('q', 'Q'),
            Key.W => GeCapsKey('w', 'W'),
            Key.E => GeCapsKey('e', 'E'),
            Key.R => GeCapsKey('r', 'R'),
            Key.T => GeCapsKey('t', 'T'),
            Key.Y => GeCapsKey('y', 'Y'),
            Key.U => GeCapsKey('u', 'U'),
            Key.I => GeCapsKey('i', 'I'),
            Key.O => GeCapsKey('o', 'O'),
            Key.P => GeCapsKey('p', 'P'),
            Key.Oem4 => GetShiftKey('{', '['),
            Key.Oem6 => GetShiftKey('}', ']'),
            Key.A => GeCapsKey('a', 'A'),
            Key.S => GeCapsKey('s', 'S'),
            Key.D => GeCapsKey('d', 'D'),
            Key.F => GeCapsKey('f', 'F'),
            Key.G => GeCapsKey('g', 'G'),
            Key.H => GeCapsKey('h', 'H'),
            Key.J => GeCapsKey('j', 'J'),
            Key.K => GeCapsKey('k', 'K'),
            Key.L => GeCapsKey('l', 'L'),
            Key.Oem5 => GetShiftKey('|', '\\'),
            Key.Oem1 => GetShiftKey(':', ';'),
            Key.Oem7 => GetShiftKey('\'', '"'),
            Key.Z => GeCapsKey('z', 'Z'),
            Key.X => GeCapsKey('x', 'X'),
            Key.C => GeCapsKey('c', 'C'),
            Key.V => GeCapsKey('v', 'V'),
            Key.B => GeCapsKey('b', 'B'),
            Key.N => GeCapsKey('n', 'N'),
            Key.M => GeCapsKey('m', 'M'),
            Key.OemComma => GetShiftKey('<', ','),
            Key.OemPeriod => GetShiftKey('>', '.'),
            Key.Oem2 => GetShiftKey('?', '/'),
            Key.Enter => '\n',
            Key.Space => ' ',
            _ => ' ',
        };
    }

}
