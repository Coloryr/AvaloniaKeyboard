using System;
using RimeSharp;

namespace AvaloniaKeyboard;

public static class RimeUtils
{
    public static bool IsEnable { get; private set; }
    public static Exception? Init()
    {
        try
        {
            Rime.Init(AppContext.BaseDirectory, Handel);
            IsEnable = true;

            return null;
        }
        catch (Exception e)
        {
            IsEnable = false;
            return e;
        }
    }

    public static void Handel(IntPtr context_object, IntPtr session_id, string message_type, string message_value)
    {
        Console.WriteLine($"message: [{session_id}] [{message_type}] {message_value}");
    }
}
