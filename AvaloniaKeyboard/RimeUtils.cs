using RimeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        catch(Exception e)
        {
            IsEnable = false;
            return e;
        }
    }

    internal static void PrintStatus(RimeStatus status)
    {
        Console.WriteLine($"schema: {status.schema_id} / {status.schema_name}");
        Console.Write("status: ");
        if (status.is_disabled)
            Console.Write("disabled ");
        if (status.is_composing)
            Console.Write("composing ");
        if (status.is_ascii_mode)
            Console.Write("ascii ");
        if (status.is_full_shape)
            Console.Write("full_shape ");
        if (status.is_simplified)
            Console.Write("simplified ");
        Console.WriteLine();
    }

    internal static void PrintComposition(RimeComposition composition)
    {
        string preedit = composition.preedit;
        if (preedit == null)
            return;
        int len = preedit.Length;
        int start = composition.sel_start;
        int end = composition.sel_end;
        int cursor = composition.cursor_pos;
        for (int i = 0; i <= len; ++i)
        {
            if (start < end)
            {
                if (i == start)
                {
                    Console.Write('[');
                }
                else if (i == end)
                {
                    Console.Write(']');
                }
            }
            if (i == cursor)
                Console.Write('|');
            if (i < len)
                Console.Write(preedit[i]);
        }
        Console.WriteLine();
    }

    internal static void PrintMenu(RimeMenu menu)
    {
        if (menu.num_candidates == 0)
            return;
        Console.WriteLine($"page: {menu.page_no + 1}{(menu.is_last_page ? '$' : ' ')} (of size {menu.page_size})");
        for (int i = 0; i < menu.num_candidates; ++i)
        {
            bool highlighted = i == menu.highlighted_candidate_index;
            Console.WriteLine($"{i + 1}. {(highlighted ? '[' : ' ')}{menu.candidates[i].text}{(highlighted ? ']' : ' ')}{(menu.candidates[i].comment != null ? menu.candidates[i].comment : "")}");
        }
    }

    internal static void PrintContext(RimeContext context)
    {
        if (context.composition.length > 0 || context.menu.num_candidates > 0)
        {
            PrintComposition(context.composition);
        }
        else
        {
            Console.WriteLine("(not composing)");
        }
        PrintMenu(context.menu);
    }

    public static void Handel(IntPtr context_object, IntPtr session_id, string message_type, string message_value)
    {
        Console.WriteLine($"message: [{session_id}] [{message_type}] {message_value}");
    }
}
