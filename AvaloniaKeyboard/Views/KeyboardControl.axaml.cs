using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AvaloniaKeyboard.Views;

public partial class KeyboardControl : UserControl
{
    public static readonly StyledProperty<int> KeySizeProperty =
            AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySize), 50);

    public static readonly StyledProperty<int> KeySizeTwoProperty =
           AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySizeTwo), 75);
    public static readonly StyledProperty<int> KeySizeThreeProperty =
           AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySizeThree), 100);
    public static readonly StyledProperty<int> KeySizeFourProperty =
           AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySizeFour), 125);
    public static readonly StyledProperty<int> KeySizeSpaceProperty =
           AvaloniaProperty.Register<KeyboardControl, int>(nameof(KeySizeSpace), 350);

    public int KeySize
    {
        get { return GetValue(KeySizeProperty); }
        set
        {
            SetValue(KeySizeProperty, value);
            SetValue(KeySizeTwoProperty, (int)(value * 1.5));
            SetValue(KeySizeThreeProperty, (value * 2));
            SetValue(KeySizeFourProperty, (int)(value * 2.5));
            SetValue(KeySizeSpaceProperty, (value * 7));
        }
    }
    public int KeySizeTwo
    {
        get { return GetValue(KeySizeTwoProperty); }
    }
    public int KeySizeThree
    {
        get { return GetValue(KeySizeThreeProperty); }
    }
    public int KeySizeFour
    {
        get { return GetValue(KeySizeFourProperty); }
    }
    public int KeySizeSpace
    {
        get { return GetValue(KeySizeSpaceProperty); }
    }

    private static readonly string[] LanList = ["жа", "гЂ"];

    private bool _isENInput = true;
    private bool _isCaps, _isLeftShift, _isRightShift;
    private bool _isLeftCtrl, _isRightCtrl, _isLeftAlt, _isRightAlt;
    private bool _isRun;
    private TextBox? _textBox;
    private Key _keyDownSave;
    private int _count;
    private ITransform _matrix = new MatrixTransform(Matrix.CreateScale(0.95, 0.95));

    public KeyboardControl()
    {
        InitializeComponent();
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

        KeyDown += KeyboardControl_KeyDown;
    }

    private void KeyboardControl_KeyDown(object? sender, KeyEventArgs e)
    {
        
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _isRun = true;

        new Thread(() =>
        {
            while (_isRun)
            {
                Thread.Sleep(100);
                if (_textBox == null || _keyDownSave == Key.None)
                {
                    continue;
                }
                _count++;
                if (_count > 30)
                {
                    if (_keyDownSave == Key.Tab)
                    {
                        _textBox.Text += "  ";
                    }
                    else if (_keyDownSave == Key.Back)
                    {
                        var temp = _textBox.Text;
                        if (!string.IsNullOrEmpty(temp))
                        {
                            _textBox.Text = temp[..^1];
                        }
                    }
                    else
                    {
                        _textBox.Text += KeyToChar(_keyDownSave);
                    }
                }
                else
                {
                    _count++;
                }
            }
        }).Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _isRun = false;
    }

    private void LanSwitch_Click(object? sender, RoutedEventArgs e)
    {
        _isENInput = !_isENInput;
        LanSwitch.Content = LanList[_isENInput ? 1 : 0];
    }

    public void SetTextBox(TextBox text)
    {
        _textBox = text;
    }

    private void KeyInput(object? obj, Key key)
    {
        if (obj is Border border)
        {
            border.RenderTransform = _matrix;
        }

        if (_textBox == null)
        {
            return;
        }

        switch (key)
        {
            case Key.Escape:
                _textBox = null;
                IsVisible = false;
                return;
            case Key.CapsLock:
                _isCaps = !_isCaps;
                if (_isCaps)
                {
                    CapsLock.Classes.Add("k2");
                }
                else
                {
                    CapsLock.Classes.Remove("k2");
                }
                return;
            case Key.LeftCtrl:
                _isLeftCtrl = true;
                return;
            case Key.RightCtrl:
                _isRightCtrl = true;
                return;
            case Key.LeftAlt:
                _isLeftAlt = true;
                return;
            case Key.RightAlt:
                _isRightAlt = true;
                return;
            case Key.LeftShift:
                _isLeftShift = true;
                return;
            case Key.RightShift:
                _isRightShift = true;
                return;
            case Key.Back:
                var temp = _textBox.Text;
                if (!string.IsNullOrEmpty(temp))
                {
                    _textBox.Text = temp[..^1];
                }
                _count = 0;
                _keyDownSave = Key.Back;
                return;
            case Key.Tab:
                _textBox.Text += "  ";
                _count = 0;
                _keyDownSave = Key.Tab;
                break;
            default:
                _count = 0;
                _keyDownSave = key;
                _textBox.Text += KeyToChar(key);
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
            //case Key.CapsLock:
            //    _isCaps = false;
            //    return;
            case Key.LeftCtrl:
                _isLeftCtrl = false;
                return;
            case Key.RightCtrl:
                _isRightCtrl = false;
                return;
            case Key.LeftAlt:
                _isLeftAlt = false;
                return;
            case Key.RightAlt:
                _isRightAlt = false;
                return;
            case Key.LeftShift:
                _isLeftShift = false;
                return;
            case Key.RightShift:
                _isLeftShift = false;
                return;
            default:
                _count = 0;
                _keyDownSave = Key.None;
                return;
        }
    }

    private char GetKey(char a, char b)
    {
        var b1 = _isLeftShift || _isRightShift;
        if (b1 && _isCaps)
        {
            return a;
        }
        else if (b1 && !_isCaps || !b1 && _isCaps)
        {
            return b;
        }

        return a;
    }

    private char GetShiftKey(char a, char b)
    {
        return (_isLeftShift || _isRightShift) ? a : b;
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
            Key.Q => GetKey('q', 'Q'),
            Key.W => GetKey('w', 'W'),
            Key.E => GetKey('e', 'E'),
            Key.R => GetKey('r', 'R'),
            Key.T => GetKey('t', 'T'),
            Key.Y => GetKey('y', 'Y'),
            Key.U => GetKey('u', 'U'),
            Key.I => GetKey('i', 'I'),
            Key.O => GetKey('o', 'O'),
            Key.P => GetKey('p', 'P'),
            Key.Oem4 => GetShiftKey('{', '['),
            Key.Oem6 => GetShiftKey('}', ']'),
            Key.A => GetKey('a', 'A'),
            Key.S => GetKey('s', 'S'),
            Key.D => GetKey('d', 'D'),
            Key.F => GetKey('f', 'F'),
            Key.G => GetKey('g', 'G'),
            Key.H => GetKey('h', 'H'),
            Key.J => GetKey('j', 'J'),
            Key.K => GetKey('k', 'K'),
            Key.L => GetKey('l', 'L'),
            Key.Oem5 => GetShiftKey('|', '\\'),
            Key.Oem1 => GetShiftKey(':', ';'),
            Key.Oem7 => GetShiftKey('\'', '"'),
            Key.Z => GetKey('z', 'Z'),
            Key.X => GetKey('x', 'X'),
            Key.C => GetKey('c', 'C'),
            Key.V => GetKey('v', 'V'),
            Key.B => GetKey('b', 'B'),
            Key.N => GetKey('n', 'N'),
            Key.M => GetKey('m', 'M'),
            Key.OemComma => GetShiftKey('<', ','),
            Key.OemPeriod => GetShiftKey('>', '.'),
            Key.Oem2 => GetShiftKey('?', '/'),
            Key.Enter => '\n',
            Key.Space => ' ',
            _ => ' ',
        };
    }
}
