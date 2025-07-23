﻿using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace TableCloth3.Help.Controls;

public class InlineCodeText : AvaloniaObject
{
    public static readonly AttachedProperty<string?> TextProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, string?>(
            "Text", typeof(InlineCodeText));

    public static void SetText(AvaloniaObject element, string? value) =>
        element.SetValue(TextProperty, value);

    public static string? GetText(AvaloniaObject element) =>
        element.GetValue(TextProperty);

    public static readonly AttachedProperty<IBrush> CodeForegroundProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, IBrush>(
            "CodeForeground", typeof(InlineCodeText), Brushes.White);

    public static void SetCodeForeground(AvaloniaObject element, IBrush value) =>
        element.SetValue(CodeForegroundProperty, value);

    public static IBrush GetCodeForeground(AvaloniaObject element) =>
        element.GetValue(CodeForegroundProperty);

    public static readonly AttachedProperty<IBrush> CodeBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, IBrush>(
            "CodeBackground", typeof(InlineCodeText), Brushes.Black);

    public static void SetCodeBackground(AvaloniaObject element, IBrush value) =>
        element.SetValue(CodeBackgroundProperty, value);

    public static IBrush GetCodeBackground(AvaloniaObject element) =>
        element.GetValue(CodeBackgroundProperty);

    public static readonly AttachedProperty<CornerRadius> CodeCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, CornerRadius>(
            "CodeCornerRadius", typeof(InlineCodeText), new CornerRadius(4));

    public static void SetCodeCornerRadius(AvaloniaObject element, CornerRadius value) =>
        element.SetValue(CodeCornerRadiusProperty, value);

    public static CornerRadius GetCodeCornerRadius(AvaloniaObject element) =>
        element.GetValue(CodeCornerRadiusProperty);

    public static readonly AttachedProperty<FontFamily> CodeFontFamilyProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, FontFamily>(
            "CodeFontFamily", typeof(InlineCodeText), new FontFamily("Consolas"));

    public static void SetCodeFontFamily(AvaloniaObject element, FontFamily value) =>
        element.SetValue(CodeFontFamilyProperty, value);

    public static FontFamily GetCodeFontFamily(AvaloniaObject element) =>
        element.GetValue(CodeFontFamilyProperty);

    static InlineCodeText()
    {
        TextProperty.Changed.AddClassHandler<TextBlock>(OnTextChanged);
    }

    private static void OnTextChanged(TextBlock obj, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string newText)
        {
            UpdateInlines(obj, newText);
        }
        else
        {
            obj.Inlines?.Clear();
        }
    }

    private static void UpdateInlines(TextBlock textBlock, string text)
    {
        textBlock.Inlines?.Clear();

        if (string.IsNullOrWhiteSpace(text))
            return;

        text = text.Trim();

        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
            {
                textBlock.Inlines!.Add(new LineBreak());
                continue;
            }

            ProcessLineWithInlineCode(textBlock, line);
            textBlock.Inlines!.Add(new LineBreak());
        }
    }

    private static void ProcessLineWithInlineCode(TextBlock textBlock, string line)
    {
        var regex = new Regex(@"`([^`]+)`");
        int lastIndex = 0;

        var codeFg = GetCodeForeground(textBlock);
        var codeBg = GetCodeBackground(textBlock);
        var codeRadius = GetCodeCornerRadius(textBlock);
        var codeFont = GetCodeFontFamily(textBlock);

        foreach (Match match in regex.Matches(line))
        {
            if (match.Index > lastIndex)
            {
                string before = line.Substring(lastIndex, match.Index - lastIndex);
                textBlock.Inlines!.Add(new Run { Text = before });
            }

            string code = match.Groups[1].Value.Trim();

            var container = new InlineUIContainer
            {
                BaselineAlignment = BaselineAlignment.Center,
                Child = new Border
                {
                    Background = codeBg,
                    CornerRadius = codeRadius,
                    Padding = new Thickness(2, 0, 2, 0),
                    Margin = new Thickness(1, 4, 1, 0),
                    Child = new TextBlock
                    {
                        Text = code,
                        FontFamily = codeFont,
                        Foreground = codeFg
                    }
                }
            };

            textBlock.Inlines!.Add(container);

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < line.Length)
        {
            string after = line.Substring(lastIndex);
            textBlock.Inlines!.Add(new Run { Text = after });
        }
    }
}
