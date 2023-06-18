using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN.Editor
{
    public delegate TextLocation MotionAction(TextBuffer tb);
    public delegate void CommandAction(TextBuffer tb, TextSelection targetText);
    public delegate void StandaloneAction(TextBuffer tb);

    public class KeyActions
    {
        private EednEngine _Engine;

        public Dictionary<KeyActionState, MotionAction> MotionBindings { get; set; } = new Dictionary<KeyActionState, MotionAction>();
        public Dictionary<KeyActionState, CommandAction> CommandBindings { get; set; } = new Dictionary<KeyActionState, CommandAction>();
        public Dictionary<KeyActionState, StandaloneAction> StandaloneBindings { get; set; } = new Dictionary<KeyActionState, StandaloneAction>();

        public Frame CurrentFrame
        {
            get => _Engine.Frames[_Engine.CurrentFrameIdx];
        }

        public TextBuffer CurrentBuffer
        {
            get => CurrentFrame.Buffer;
        }

        public EditorMode CurrentMode
        {
            get => CurrentFrame.CurrentMode;
            set => CurrentFrame.CurrentMode = value;
        }

        public CommandAction? PendingCommand { get; set; } = null;

        public TextSelection? CurrentSelection
        {
            get => CurrentFrame.CurrentSelection;
            set => CurrentFrame.CurrentSelection = value;
        }

        public KeyActions(EednEngine engine)
        {
            _Engine = engine;
            SetupInsertMode();
            SetupCommandMode();
            SetupVisualMode();
        }

        public void SetupInsertMode()
        {
            AddVanillaStandaloneBinding(Keys.Escape, EditorMode.Insert, (_) => { CurrentMode = EditorMode.Command; });

            // Alpha
            for (int kInt = 65; kInt <= 90; ++kInt)
            {
                int cInt = kInt;

                // Lowercase
                AddVanillaStandaloneBinding((Keys)kInt,
                    EditorMode.Insert,
                    (tb) => tb.Insert((char)(cInt + 32)));

                // Uppercase
                AddShiftStandaloneBinding((Keys)kInt, EditorMode.Insert,
                    (tb) => tb.Insert((char)cInt));
            }

            // Numeric
            AddVanillaStandaloneBinding(Keys.D1, EditorMode.Insert, (tb) => tb.Insert('1'));
            AddVanillaStandaloneBinding(Keys.D2, EditorMode.Insert, (tb) => tb.Insert('2'));
            AddVanillaStandaloneBinding(Keys.D3, EditorMode.Insert, (tb) => tb.Insert('3'));
            AddVanillaStandaloneBinding(Keys.D4, EditorMode.Insert, (tb) => tb.Insert('4'));
            AddVanillaStandaloneBinding(Keys.D5, EditorMode.Insert, (tb) => tb.Insert('5'));
            AddVanillaStandaloneBinding(Keys.D6, EditorMode.Insert, (tb) => tb.Insert('6'));
            AddVanillaStandaloneBinding(Keys.D7, EditorMode.Insert, (tb) => tb.Insert('7'));
            AddVanillaStandaloneBinding(Keys.D8, EditorMode.Insert, (tb) => tb.Insert('8'));
            AddVanillaStandaloneBinding(Keys.D9, EditorMode.Insert, (tb) => tb.Insert('9'));
            AddVanillaStandaloneBinding(Keys.D0, EditorMode.Insert, (tb) => tb.Insert('0'));

            // Keypad Numerics
            AddVanillaStandaloneBinding(Keys.KeyPad1, EditorMode.Insert, (tb) => tb.Insert('1'));
            AddVanillaStandaloneBinding(Keys.KeyPad2, EditorMode.Insert, (tb) => tb.Insert('2'));
            AddVanillaStandaloneBinding(Keys.KeyPad3, EditorMode.Insert, (tb) => tb.Insert('3'));
            AddVanillaStandaloneBinding(Keys.KeyPad4, EditorMode.Insert, (tb) => tb.Insert('4'));
            AddVanillaStandaloneBinding(Keys.KeyPad5, EditorMode.Insert, (tb) => tb.Insert('5'));
            AddVanillaStandaloneBinding(Keys.KeyPad6, EditorMode.Insert, (tb) => tb.Insert('6'));
            AddVanillaStandaloneBinding(Keys.KeyPad7, EditorMode.Insert, (tb) => tb.Insert('7'));
            AddVanillaStandaloneBinding(Keys.KeyPad8, EditorMode.Insert, (tb) => tb.Insert('8'));
            AddVanillaStandaloneBinding(Keys.KeyPad9, EditorMode.Insert, (tb) => tb.Insert('9'));
            AddVanillaStandaloneBinding(Keys.KeyPad0, EditorMode.Insert, (tb) => tb.Insert('0'));

            // Numerical symbols
            AddShiftStandaloneBinding(Keys.D1, EditorMode.Insert, (tb) => tb.Insert('!'));
            AddShiftStandaloneBinding(Keys.D2, EditorMode.Insert, (tb) => tb.Insert('@'));
            AddShiftStandaloneBinding(Keys.D3, EditorMode.Insert, (tb) => tb.Insert('#'));
            AddShiftStandaloneBinding(Keys.D4, EditorMode.Insert, (tb) => tb.Insert('$'));
            AddShiftStandaloneBinding(Keys.D5, EditorMode.Insert, (tb) => tb.Insert('%'));
            AddShiftStandaloneBinding(Keys.D6, EditorMode.Insert, (tb) => tb.Insert('^'));
            AddShiftStandaloneBinding(Keys.D7, EditorMode.Insert, (tb) => tb.Insert('&'));
            AddShiftStandaloneBinding(Keys.D8, EditorMode.Insert, (tb) => tb.Insert('*'));
            AddVanillaStandaloneBinding(Keys.KeyPadMultiply, EditorMode.Insert, (tb) => tb.Insert('*'));
            AddShiftStandaloneBinding(Keys.D9, EditorMode.Insert, (tb) => tb.Insert('('));
            AddShiftStandaloneBinding(Keys.D0, EditorMode.Insert, (tb) => tb.Insert(')'));

            AddVanillaStandaloneBinding(Keys.Space, EditorMode.Insert, (tb) => tb.Insert(' '));
            AddShiftStandaloneBinding(Keys.Space, EditorMode.Insert, (tb) => tb.Insert(' '));

            AddVanillaStandaloneBinding(Keys.GraveAccent, EditorMode.Insert, (tb) => tb.Insert('`'));
            AddShiftStandaloneBinding(Keys.GraveAccent, EditorMode.Insert, (tb) => tb.Insert('~'));

            AddVanillaStandaloneBinding(Keys.Minus, EditorMode.Insert, (tb) => tb.Insert('-'));
            AddVanillaStandaloneBinding(Keys.KeyPadSubtract, EditorMode.Insert, (tb) => tb.Insert('-'));
            AddShiftStandaloneBinding(Keys.Minus, EditorMode.Insert, (tb) => tb.Insert('_'));

            AddVanillaStandaloneBinding(Keys.Equal, EditorMode.Insert, (tb) => tb.Insert('='));
            AddVanillaStandaloneBinding(Keys.KeyPadEqual, EditorMode.Insert, (tb) => tb.Insert('='));
            AddShiftStandaloneBinding(Keys.Equal, EditorMode.Insert, (tb) => tb.Insert('+'));
            AddVanillaStandaloneBinding(Keys.KeyPadAdd, EditorMode.Insert, (tb) => tb.Insert('+'));

            AddVanillaStandaloneBinding(Keys.Backslash, EditorMode.Insert, (tb) => tb.Insert('\\'));
            AddShiftStandaloneBinding(Keys.Backslash, EditorMode.Insert, (tb) => tb.Insert('|'));

            AddVanillaStandaloneBinding(Keys.LeftBracket, EditorMode.Insert, (tb) => tb.Insert('['));
            AddShiftStandaloneBinding(Keys.LeftBracket, EditorMode.Insert, (tb) => tb.Insert('{'));
            AddVanillaStandaloneBinding(Keys.RightBracket, EditorMode.Insert, (tb) => tb.Insert(']'));
            AddShiftStandaloneBinding(Keys.RightBracket, EditorMode.Insert, (tb) => tb.Insert('}'));

            AddVanillaStandaloneBinding(Keys.Semicolon, EditorMode.Insert, (tb) => tb.Insert(';'));
            AddShiftStandaloneBinding(Keys.Semicolon, EditorMode.Insert, (tb) => tb.Insert(':'));

            AddVanillaStandaloneBinding(Keys.Apostrophe, EditorMode.Insert, (tb) => tb.Insert('\''));
            AddShiftStandaloneBinding(Keys.Apostrophe, EditorMode.Insert, (tb) => tb.Insert('"'));

            AddVanillaStandaloneBinding(Keys.Comma, EditorMode.Insert, (tb) => tb.Insert(','));
            AddShiftStandaloneBinding(Keys.Comma, EditorMode.Insert, (tb) => tb.Insert('<'));

            AddVanillaStandaloneBinding(Keys.Period, EditorMode.Insert, (tb) => tb.Insert('.'));
            AddVanillaStandaloneBinding(Keys.KeyPadDecimal, EditorMode.Insert, (tb) => tb.Insert('.'));
            AddShiftStandaloneBinding(Keys.Period, EditorMode.Insert, (tb) => tb.Insert('>'));

            AddVanillaStandaloneBinding(Keys.Slash, EditorMode.Insert, (tb) => tb.Insert('/'));
            AddVanillaStandaloneBinding(Keys.KeyPadDivide, EditorMode.Insert, (tb) => tb.Insert('/'));
            AddShiftStandaloneBinding(Keys.Slash, EditorMode.Insert, (tb) => tb.Insert('?'));

            // Movement keys
            AddVanillaMotionBinding(Keys.Left, EditorMode.Insert, MoveLeft);
            AddVanillaMotionBinding(Keys.Right, EditorMode.Insert, MoveRight);
            AddVanillaMotionBinding(Keys.Up, EditorMode.Insert, MoveUp);
            AddVanillaMotionBinding(Keys.Down, EditorMode.Insert, MoveDown);

            AddVanillaStandaloneBinding(Keys.Backspace, EditorMode.Insert, Backspace);
            AddVanillaStandaloneBinding(Keys.Delete, EditorMode.Insert, Delete);

            AddVanillaStandaloneBinding(Keys.Enter, EditorMode.Insert, AddLine);
            AddVanillaStandaloneBinding(Keys.KeyPadEnter, EditorMode.Insert, AddLine);

            AddVanillaMotionBinding(Keys.Home, EditorMode.Insert, GoToHome);
            AddVanillaMotionBinding(Keys.End, EditorMode.Insert, GoToEnd);

            // Font size +/-
            StandaloneBindings[new KeyActionState(Keys.Equal, EditorMode.Insert)
            {
                IsShiftActive = true,
                IsControlActive = true
            }] = IncreaseFontSize;
            AddControlStandaloneBinding(Keys.Minus, EditorMode.Insert, DecreaseFontSize);
        }

        public void SetupCommandMode()
        {
            AddVanillaStandaloneBinding(Keys.I, EditorMode.Command, (_) => { CurrentMode = EditorMode.Insert; });
            AddVanillaStandaloneBinding(Keys.A, EditorMode.Command, (tb) =>
            {
                CurrentMode = EditorMode.Insert;
                if (tb.ColIdx < tb.Lines[tb.LineIdx].Length)
                    tb.ColIdx += 1;
            });
            AddShiftStandaloneBinding(Keys.I, EditorMode.Command, (tb) => { CurrentMode = EditorMode.Insert; tb.ColIdx = 0; });
            AddShiftStandaloneBinding(Keys.A, EditorMode.Command, (tb) => { CurrentMode = EditorMode.Insert; tb.ColIdx = tb.Lines[tb.LineIdx].Length; });

            AddVanillaStandaloneBinding(Keys.V, EditorMode.Command, (_) =>
            {
                CurrentMode = EditorMode.Visual;
                CurrentSelection = new TextSelection(
                    (CurrentBuffer.LineIdx, CurrentBuffer.ColIdx),
                    (CurrentBuffer.LineIdx, CurrentBuffer.ColIdx));
            });

            // Movement keys
            AddVanillaMotionBinding(Keys.H, EditorMode.Command, MoveLeft);
            AddVanillaMotionBinding(Keys.L, EditorMode.Command, MoveRight);
            AddVanillaMotionBinding(Keys.K, EditorMode.Command, MoveUp);
            AddVanillaMotionBinding(Keys.J, EditorMode.Command, MoveDown);

            // Font size +/-
            StandaloneBindings[new KeyActionState(Keys.Equal, EditorMode.Command)
            {
                IsShiftActive = true,
                IsControlActive = true
            }] = IncreaseFontSize;
            AddControlStandaloneBinding(Keys.Minus, EditorMode.Command, DecreaseFontSize);

            // Delete char / selection
            AddVanillaCommandBinding(Keys.D, EditorMode.Command, DeleteSelection);
            AddVanillaStandaloneBinding(Keys.X, EditorMode.Command, Delete);

            // New empty lines
            AddVanillaStandaloneBinding(Keys.O, EditorMode.Command, tb =>
            {
                CurrentMode = EditorMode.Insert;
                AddEmptyLine(tb);
            });
            AddShiftStandaloneBinding(Keys.O, EditorMode.Command, tb =>
            {
                CurrentMode = EditorMode.Insert;
                AddEmptyLineAbove(tb);
            });
        }

        public void SetupVisualMode()
        {
            AddVanillaStandaloneBinding(Keys.Escape, EditorMode.Visual, (_) =>
            {
                CurrentMode = EditorMode.Command;
                CurrentSelection = null;
            });

            // Movement keys
            AddVanillaMotionBinding(Keys.H, EditorMode.Visual, (tb) => MoveAndSelect(MoveLeft(tb)));
            AddVanillaMotionBinding(Keys.L, EditorMode.Visual, (tb) => MoveAndSelect(MoveRight(tb)));
            AddVanillaMotionBinding(Keys.K, EditorMode.Visual, (tb) => MoveAndSelect(MoveUp(tb)));
            AddVanillaMotionBinding(Keys.J, EditorMode.Visual, (tb) => MoveAndSelect(MoveDown(tb)));

            AddVanillaCommandBinding(Keys.D, EditorMode.Visual, DeleteSelection);
            AddVanillaCommandBinding(Keys.X, EditorMode.Visual, DeleteSelection);
        }

        public TextLocation MoveAndSelect(TextLocation loc)
        {
            if (CurrentSelection is not null)
                CurrentSelection = new TextSelection(CurrentSelection.StartLoc, loc);

            return loc;
        }

        public void AddVanillaMotionBinding(Keys key, EditorMode mode, MotionAction action)
            => MotionBindings[new KeyActionState(key, mode)] = action;

        public void AddShiftMotionBinding(Keys key, EditorMode mode, MotionAction action)
            => MotionBindings[new KeyActionState(key, mode)
            {
                IsShiftActive = true
            }] = action;

        public void AddControlMotionBinding(Keys key, EditorMode mode, MotionAction action)
            => MotionBindings[new KeyActionState(key, mode)
            {
                IsControlActive = true
            }] = action;

        public void AddVanillaCommandBinding(Keys key, EditorMode mode, CommandAction action)
            => CommandBindings[new KeyActionState(key, mode)] = action;

        public void AddShiftCommandBinding(Keys key, EditorMode mode, CommandAction action)
            => CommandBindings[new KeyActionState(key, mode)
            {
                IsShiftActive = true
            }] = action;

        public void AddControlCommandBinding(Keys key, EditorMode mode, CommandAction action)
            => CommandBindings[new KeyActionState(key, mode)
            {
                IsControlActive = true
            }] = action;

        public void AddVanillaStandaloneBinding(Keys key, EditorMode mode, StandaloneAction action)
            => StandaloneBindings[new KeyActionState(key, mode)] = action;

        public void AddShiftStandaloneBinding(Keys key, EditorMode mode, StandaloneAction action)
            => StandaloneBindings[new KeyActionState(key, mode)
            {
                IsShiftActive = true
            }] = action;

        public void AddControlStandaloneBinding(Keys key, EditorMode mode, StandaloneAction action)
            => StandaloneBindings[new KeyActionState(key, mode)
            {
                IsControlActive = true
            }] = action;

        public TextLocation MoveLeft(TextBuffer tb)
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

        public TextLocation MoveRight(TextBuffer tb)
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

        public TextLocation MoveUp(TextBuffer tb)
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

        public TextLocation MoveDown(TextBuffer tb)
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

        public void AddEmptyLine(TextBuffer tb)
        {
            tb.Lines.Insert(tb.LineIdx + 1, "");

            tb.LineIdx += 1;
            tb.ColIdx = 0;
        }

        public void AddEmptyLineAbove(TextBuffer tb)
        {
            tb.Lines.Insert(tb.LineIdx, "");

            tb.ColIdx = 0;
        }

        public void AddLine(TextBuffer tb)
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

        public void Backspace(TextBuffer tb)
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

        public void Delete(TextBuffer tb)
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

        public void GoToLocation(TextBuffer tb, TextLocation location)
        {
            tb.LineIdx = location.LineIdx;
            tb.ColIdx = location.ColIdx;
        }

        public TextLocation GoToEnd(TextBuffer tb)
            => new TextLocation(tb.LineIdx, tb.Lines[tb.LineIdx].Length);

        public TextLocation GoToHome(TextBuffer tb)
            => new TextLocation(tb.LineIdx, 0);

        public void IncreaseFontSize(TextBuffer tb)
            => tb.FontSize += 1;

        public void DecreaseFontSize(TextBuffer tb)
            => tb.FontSize -= 1;

        public void DeleteSelection(TextBuffer tb, TextSelection selection)
        {
            int startLineIdx = selection.StartLoc.LineIdx;
            int endLineIdx = selection.EndLoc.LineIdx;

            int startX = selection.StartLoc.ColIdx;
            int endX = selection.EndLoc.ColIdx;

            // TODO: Completely remove this debug print once we know this function works :)
            //Console.WriteLine($"Line: ({startLineIdx}, {endLineIdx}) Col: ({startX}, {endX})");

            if (endLineIdx < startLineIdx)
            {
                int temp = startLineIdx;
                startLineIdx = endLineIdx;
                endLineIdx = temp;

                temp = startX;
                startX = endX;
                endX = temp;
            }
            else if (startLineIdx == endLineIdx && endX < startX)
            {
                int temp = startX;
                startX = endX;
                endX = temp;
            }

            // Set cursor to where selection started
            tb.LineIdx = startLineIdx;
            tb.ColIdx = startX;

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
                }
                else if (endLineIdx < tb.Lines.Count)
                {
                    tb.Lines[startLineIdx] += tb.Lines[endLineIdx + 1];
                    tb.Lines.RemoveAt(endLineIdx + 1);
                }
            }

            // Remove every selected line after our start line we've modified.
            tb.Lines.RemoveRange(startLineIdx + 1, endLineIdx - startLineIdx);
        }

        public void ProcessKey(KeyboardKeyEventArgs e)
        {
            KeyActionState keyActionState = new KeyActionState(e.Key, CurrentMode)
            {
                IsShiftActive = e.Shift,
                IsAltActive = e.Alt,
                IsCommandActive = e.Command,
                IsControlActive = e.Control
            };

            // Check for standalone actions
            bool keyHasStandalone = StandaloneBindings.TryGetValue(keyActionState, out StandaloneAction? standaloneAction);
            if (keyHasStandalone && standaloneAction is not null)
            {
                PendingCommand = null;
                standaloneAction(CurrentBuffer);
                return;
            }

            // Check for command actions
            bool keyHasCommand = CommandBindings.TryGetValue(keyActionState, out CommandAction? commandAction);
            if (keyHasCommand && commandAction is not null)
            {
                if (CurrentMode == EditorMode.Visual)
                {
                    if (CurrentSelection is null)
                        throw new Exception("Should be unreachable...");

                    commandAction(CurrentBuffer, CurrentSelection);
                    CurrentMode = EditorMode.Command;
                    CurrentSelection = null;
                    return;
                }

                // If no pending or pending is different from incoming command - set incoming as pending.
                if (PendingCommand is null || PendingCommand != commandAction)
                {
                    PendingCommand = commandAction;
                    return;
                }

                // If pending is the same as incoming - process command on current line.
                commandAction(CurrentBuffer, new TextSelection(
                    (CurrentBuffer.LineIdx, 0),
                    (CurrentBuffer.LineIdx, CurrentBuffer.Lines[CurrentBuffer.LineIdx].Length - 1)));

                PendingCommand = null;
                return;
            }

            bool keyHasMotion = MotionBindings.TryGetValue(keyActionState, out MotionAction? motionAction);
            if (keyHasMotion && motionAction is not null)
            {
                TextLocation endLoc = motionAction(CurrentBuffer);
                if (PendingCommand is not null)
                {
                    TextSelection selection = new TextSelection(
                        new TextLocation(CurrentBuffer.LineIdx, CurrentBuffer.ColIdx),
                        endLoc);
                    PendingCommand(CurrentBuffer, selection);
                    PendingCommand = null;
                }
                else
                {
                    CurrentBuffer.LineIdx = endLoc.LineIdx;
                    CurrentBuffer.ColIdx = endLoc.ColIdx;
                }
                return;
            }
        }
    }
}
