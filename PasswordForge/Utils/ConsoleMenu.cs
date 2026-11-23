namespace PasswordForge.Utils;

public static class ConsoleMenu
{
    private const string SelectedMarker = " ▸ ";
    private const string Unselected = "   ";

    /// <summary>Total width a menu takes on screen, including the selection marker and padding.</summary>
    public static int GetWidth(IEnumerable<string> items) =>
        items.Max(item => item.Length) + SelectedMarker.Length + 3;

    /// <summary>
    /// Shows a centered menu navigated with the arrow keys.
    /// Returns the selected index, or -1 if the user pressed Escape.
    /// </summary>
    /// <param name="drawHeader">Clears the screen and draws everything above the menu.</param>
    public static int Select(Action drawHeader, IReadOnlyList<string> items, string hint, int selected = 0)
    {
        if (items.Count == 0)
            throw new ArgumentException("A menu needs at least one item.", nameof(items));

        selected = Math.Clamp(selected, 0, items.Count - 1);

        if (!ConsoleHelper.IsInteractive)
            return SelectByNumber(drawHeader, items);

        Console.CursorVisible = false;

        try
        {
            int width = GetWidth(items);
            int windowWidth = 0, windowHeight = 0, top = 0, left = 0, visible = 0, offset = 0;

            while (true)
            {
                if (windowWidth != ConsoleHelper.WindowWidth || windowHeight != ConsoleHelper.WindowHeight)
                {
                    windowWidth = ConsoleHelper.WindowWidth;
                    windowHeight = ConsoleHelper.WindowHeight;

                    drawHeader();

                    visible = Math.Clamp(windowHeight - Console.CursorTop - 3, 1, items.Count);
                    int reserved = visible + 2;

                    // Reserve the rows first so the console scrolls now instead of while redrawing.
                    for (int i = 0; i < reserved; i++)
                        Console.WriteLine();

                    top = Console.CursorTop - reserved;
                    left = ConsoleHelper.CenterLeft(width);
                }

                offset = Math.Clamp(
                    offset,
                    Math.Max(0, selected - visible + 1),
                    Math.Min(selected, items.Count - visible));

                for (int row = 0; row < visible; row++)
                    DrawItem(left, top + row, items[offset + row], width, offset + row == selected);

                string position = items.Count > visible
                    ? $"   ({(selected + 1).ToString().PadLeft(items.Count.ToString().Length)}/{items.Count})"
                    : "";
                Console.SetCursorPosition(0, top + visible + 1);
                ConsoleHelper.WriteCentered(hint + position, Theme.Muted);

                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + items.Count) % items.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % items.Count;
                        break;
                    case ConsoleKey.Home:
                        selected = 0;
                        break;
                    case ConsoleKey.End:
                        selected = items.Count - 1;
                        break;
                    case ConsoleKey.Enter:
                        return selected;
                    case ConsoleKey.Escape:
                        return -1;
                }
            }
        }
        finally
        {
            Console.ResetColor();
            Console.ForegroundColor = Theme.Text;
            Console.CursorVisible = true;
        }
    }

    private static void DrawItem(int left, int row, string item, int width, bool isSelected)
    {
        string label = (isSelected ? SelectedMarker : Unselected) + item;
        label = label.PadRight(width);

        int maxWidth = Math.Max(1, ConsoleHelper.WindowWidth - left - 1);
        if (label.Length > maxWidth)
            label = label[..maxWidth];

        Console.SetCursorPosition(left, row);

        if (isSelected)
        {
            Console.BackgroundColor = Theme.Primary;
            Console.ForegroundColor = Theme.SelectedText;
        }
        else
        {
            Console.ForegroundColor = Theme.Text;
        }

        Console.Write(label);
        Console.ResetColor();
        Console.ForegroundColor = Theme.Text;
    }

    // Arrow keys need a real console; when input or output is redirected, fall back to numbers.
    private static int SelectByNumber(Action drawHeader, IReadOnlyList<string> items)
    {
        drawHeader();

        int left = ConsoleHelper.CenterLeft(GetWidth(items));
        for (int i = 0; i < items.Count; i++)
            ConsoleHelper.WritePartsAt(left, ($"{i + 1}. ", Theme.Muted), (items[i], Theme.Text));

        Console.WriteLine();

        while (true)
        {
            string input = ConsoleHelper.ReadLineCentered("Choose an option: ").Trim();

            if (input.Length == 0)
                return -1;

            if (int.TryParse(input, out int number) && number >= 1 && number <= items.Count)
                return number - 1;

            ConsoleHelper.WriteError($"Enter a number between 1 and {items.Count}.");
        }
    }
}
