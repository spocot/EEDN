using OpenTK.Graphics.ES20;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN.Editor
{
    public enum EditorMode
    {
        Command,
        Insert,
        Visual
    }

    public record TextSelection
    {
        public TextLocation StartLoc { get; init; }
        public TextLocation EndLoc { get; init; }
        public bool IsFullBlock { get; init; } = false;

        public TextSelection(TextLocation startLoc, TextLocation endLoc)
            => (StartLoc, EndLoc) = (startLoc, endLoc);

        public static implicit operator TextSelection((TextLocation, TextLocation) locTuple)
            => new TextSelection(locTuple.Item1, locTuple.Item2);
    }

    public struct TextLocation
    {
        public int LineIdx { get; set; }
        public int ColIdx { get; set; }

        public TextLocation(int lineIdx, int colIdx)
            => (LineIdx, ColIdx) = (lineIdx, colIdx);

        public static implicit operator TextLocation((int, int) idxTuple)
            => new TextLocation(idxTuple.Item1, idxTuple.Item2);
    }

    public delegate TextLocation MotionAction(TextBuffer tb);
    public delegate void CommandAction(TextBuffer tb, TextSelection targetText);
    public delegate void StandaloneAction(TextBuffer tb);

    public record KeyActionState
    {
        public bool IsShiftActive { get; init; } = false;
        public bool IsAltActive { get; init; } = false;
        public bool IsCommandActive { get; init; } = false;
        public bool IsControlActive { get; init; } = false;
        public EditorMode Mode { get; init; }
        public Keys Key { get; init; }

        public KeyActionState(Keys key, EditorMode mode) => (Key, Mode) = (key, mode);
    }

    public static class KeyActions
    {
        public static TextLocation MoveLeft(TextBuffer tb)
        {
            TextLocation loc = new TextLocation(tb.LineIdx, tb.ColIdx);
            if (loc.ColIdx == 0)
            {
                if (loc.LineIdx != 0)
                {
                    loc.ColIdx = tb.Lines[loc.LineIdx - 1].Length;
                    loc.LineIdx--;
                }
            }
            else
            {
                loc.ColIdx--;
            }
            return loc;
        }

        public static TextLocation MoveRight(TextBuffer tb)
        {
            TextLocation loc = new TextLocation(tb.LineIdx, tb.ColIdx);
            if (loc.ColIdx == tb.Lines[loc.LineIdx].Length)
            {
                if (loc.LineIdx != tb.Lines.Count - 1)
                {
                    loc.LineIdx++;
                    loc.ColIdx = 0;
                }
            }
            else
            {
                loc.ColIdx++;
            }
            return loc;
        }

        public static TextLocation MoveUp(TextBuffer tb)
        {
            TextLocation loc = new TextLocation(tb.LineIdx, tb.ColIdx);
            if (loc.LineIdx != 0)
            {
                if (loc.ColIdx >= tb.Lines[loc.LineIdx - 1].Length)
                {
                    loc.ColIdx = tb.Lines[loc.LineIdx - 1].Length;
                }

                loc.LineIdx -= 1;
            }
            return loc;
        }

        public static TextLocation MoveDown(TextBuffer tb)
        {
            TextLocation loc = new TextLocation(tb.LineIdx, tb.ColIdx);
            if (loc.LineIdx >= tb.Lines.Count - 1)
            {
                if (tb.Lines.Count != 0) loc.ColIdx = tb.Lines[loc.LineIdx].Length;
            }
            else
            {
                if (loc.ColIdx >= tb.Lines[loc.LineIdx + 1].Length)
                {
                    loc.ColIdx = tb.Lines[loc.LineIdx + 1].Length;
                }

                loc.LineIdx += 1;
            }
            return loc;
        }

        public static void AddLine(TextBuffer tb)
        {
            if (tb.ColIdx < tb.Lines[tb.LineIdx].Length)
            {
                tb.Lines.Insert(tb.LineIdx + 1, tb.Lines[tb.LineIdx].Substring(tb.ColIdx));
                tb.Lines[tb.LineIdx] = tb.Lines[tb.LineIdx].Remove(tb.ColIdx);
            }
            else
            {
                tb.Lines.Insert(tb.LineIdx + 1, "");
            }

            tb.LineIdx += 1;
            tb.ColIdx = 0;
        }

        public static void Backspace(TextBuffer tb)
        {
            if (tb.ColIdx > 0)
            {
                tb.Lines[tb.LineIdx] = tb.Lines[tb.LineIdx].Remove(tb.ColIdx - 1, 1);
                tb.ColIdx--;
                return;
            }

            if (tb.LineIdx > 0)
            {
                tb.ColIdx = tb.Lines[tb.LineIdx - 1].Length;
                if (tb.Lines[tb.LineIdx].Length > 0)
                {
                    tb.Lines[tb.LineIdx - 1] += tb.Lines[tb.LineIdx];
                }
                tb.Lines.RemoveAt(tb.LineIdx--);
            }
        }

        public static void Delete(TextBuffer tb)
        {
            if (tb.ColIdx < tb.Lines[tb.LineIdx].Length)
            {
                tb.Lines[tb.LineIdx] = tb.Lines[tb.LineIdx].Remove(tb.ColIdx, 1);
                return;
            }

            if (tb.LineIdx < tb.Lines.Count - 1)
            {
                if (tb.Lines[tb.LineIdx + 1].Length > 0)
                {
                    tb.Lines[tb.LineIdx] += tb.Lines[tb.LineIdx + 1];
                }
                tb.Lines.RemoveAt(tb.LineIdx + 1);
            }
        }

        public static void GoToLocation(TextBuffer tb, TextLocation location)
        {
            tb.LineIdx = location.LineIdx;
            tb.ColIdx = location.ColIdx;
        }

        public static TextLocation GoToEnd(TextBuffer tb)
            => new TextLocation(tb.LineIdx, tb.Lines[tb.LineIdx].Length);

        public static TextLocation GoToHome(TextBuffer tb)
            => new TextLocation(tb.LineIdx, 0);

        public static void IncreaseFontSize(TextBuffer tb)
            => tb.FontSize += 1;

        public static void DecreaseFontSize(TextBuffer tb)
            => tb.FontSize -= 1;

        public static void DeleteSelection(TextBuffer tb, TextSelection selection)
        {
            if (selection.IsFullBlock)
            {
                // If only one line in buffer, then full block delete should empty it, but not remove it.
                if (tb.Lines.Count <= 1)
                {
                    tb.Lines[0] = "";
                    tb.ColIdx = 0;
                    return;
                }

                tb.Lines.RemoveRange(selection.StartLoc.LineIdx, selection.EndLoc.LineIdx - selection.StartLoc.LineIdx + 1);
                if (tb.LineIdx >= tb.Lines.Count)
                    tb.LineIdx = tb.Lines.Count - 1;
                if (tb.ColIdx > tb.Lines[tb.LineIdx].Length)
                    GoToLocation(tb, KeyActions.GoToEnd(tb));
                return;
            }

            if (selection.StartLoc.LineIdx == selection.EndLoc.LineIdx)
            {
                tb.Lines[selection.StartLoc.LineIdx] = tb.Lines[selection.StartLoc.LineIdx]
                    .Remove(selection.StartLoc.ColIdx, selection.EndLoc.ColIdx - selection.StartLoc.ColIdx);
            }
        }
    }
}
