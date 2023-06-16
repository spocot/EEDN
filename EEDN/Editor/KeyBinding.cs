using OpenTK.Graphics.ES20;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN.Editor
{

    public delegate TextLocation MotionAction(TextBuffer tb);
    public delegate void CommandAction(TextBuffer tb, TextSelection targetText);
    public delegate void StandaloneAction(TextBuffer tb);

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
            int startLineIdx = selection.StartLoc.LineIdx;
            int endLineIdx = selection.EndLoc.LineIdx;

            int startX = selection.StartLoc.ColIdx;
            int endX = selection.EndLoc.ColIdx;

            // Set cursor to where selection started
            tb.LineIdx = selection.StartLoc.LineIdx;
            tb.ColIdx = selection.StartLoc.ColIdx;

            // Delete the selection in the only selected line and return.
            if (startLineIdx == endLineIdx)
            {
                int count = endX - startX;
                if (endX < tb.Lines[startLineIdx].Length)
                    count += 1;

                tb.Lines[startLineIdx] = tb.Lines[startLineIdx].Remove(startX, count);
                return;
            }

            // If more than one line in the selection set start line to be
            // startLine until selection start + endLine after selection end.
            tb.Lines[startLineIdx] = tb.Lines[startLineIdx].Remove(startX);
            if (endX < tb.Lines[endLineIdx].Length)
            {
                if (endX < tb.Lines[endLineIdx].Length - 1)
                {
                    tb.Lines[startLineIdx] += tb.Lines[endLineIdx].Substring(endX + 1);
                } else if (endLineIdx < tb.Lines.Count)
                {
                    tb.Lines[startLineIdx] += tb.Lines[endLineIdx + 1];
                    tb.Lines.RemoveAt(endLineIdx + 1);
                }
            }

            // Remove every selected line after our start line we've modified.
            tb.Lines.RemoveRange(startLineIdx + 1, endLineIdx - startLineIdx);
        }
    }
}
