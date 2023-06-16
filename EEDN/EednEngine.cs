using EEDN.Editor;
using EEDN.Render;
using EEDN.Themes;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN
{
    public class EednEngine
    {
        public List<string> Affixes { get; set; } = new List<string>();
        public int CurrentFrameIdx { get; set; } = -1;
        public List<Frame> Frames { get; set; } = new List<Frame>();
        public Frame CurrentFrame
        {
            get => Frames[CurrentFrameIdx];
        }
        public TextBuffer CurrentBuffer
        {
            get => CurrentFrame.Buffer;
        }
        public Dictionary<KeyActionState, MotionAction> MotionBindings { get; set; } = new Dictionary<KeyActionState, MotionAction>();
        public Dictionary<KeyActionState, CommandAction> CommandBindings { get; set; } = new Dictionary<KeyActionState, CommandAction>();
        public Dictionary<KeyActionState, StandaloneAction> StandaloneBindings { get; set; } = new Dictionary<KeyActionState, StandaloneAction>();
        public EditorMode CurrentMode
        {
            get => CurrentFrame.CurrentMode;
            set => CurrentFrame.CurrentMode = value;
        }
        public Theme CurrentTheme { get; set; }

        private DrawBuffer _DrawBuffer;
        private Color4 _LineCursorColor;
        private Color4 _BlockCursorColor;
        private Color4 _SelectionColor;

        public CommandAction? PendingCommand { get; set; } = null;
        public TextSelection? CurrentSelection
        {
            get => CurrentFrame.CurrentSelection;
            set => CurrentFrame.CurrentSelection = value;
        }

        public EednEngine(DrawBuffer drawBuffer, Theme theme)
        {
            _DrawBuffer = drawBuffer;
            Frames.Add(new Frame(0, 0, _DrawBuffer.ScreenSize, new TextBuffer()));
            CurrentFrameIdx = 0;
            CurrentTheme = theme;

            _LineCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 1.0f);
            _BlockCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 0.5f);
            _SelectionColor = new Color4(CurrentTheme.SelectionColor.R, CurrentTheme.SelectionColor.G, CurrentTheme.SelectionColor.B, 0.5f);

            // ====================== Insert Mode ====================== 
            AddVanillaStandaloneBinding(Keys.Escape, EditorMode.Insert, (_) => { CurrentMode = EditorMode.Command; });

            // Alpha
            for (int kInt = 65; kInt <= 90; ++kInt)
            {
                int cInt = kInt;

                // Lowercase
                AddVanillaStandaloneBinding((Keys)kInt,
                    EditorMode.Insert,
                    (tb) => tb.Insert(cInt + 32));

                // Uppercase
                AddShiftStandaloneBinding((Keys)kInt, EditorMode.Insert,
                    (tb) => tb.Insert(cInt));
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
            AddVanillaMotionBinding(Keys.Left, EditorMode.Insert, KeyActions.MoveLeft);
            AddVanillaMotionBinding(Keys.Right, EditorMode.Insert, KeyActions.MoveRight);
            AddVanillaMotionBinding(Keys.Up, EditorMode.Insert, KeyActions.MoveUp);
            AddVanillaMotionBinding(Keys.Down, EditorMode.Insert, KeyActions.MoveDown);

            AddVanillaStandaloneBinding(Keys.Backspace, EditorMode.Insert, KeyActions.Backspace);
            AddVanillaStandaloneBinding(Keys.Delete, EditorMode.Insert, KeyActions.Delete);

            AddVanillaStandaloneBinding(Keys.Enter, EditorMode.Insert, KeyActions.AddLine);
            AddVanillaStandaloneBinding(Keys.KeyPadEnter, EditorMode.Insert, KeyActions.AddLine);

            AddVanillaMotionBinding(Keys.Home, EditorMode.Insert, KeyActions.GoToHome);
            AddVanillaMotionBinding(Keys.End, EditorMode.Insert, KeyActions.GoToEnd);

            // Font size +/-
            StandaloneBindings[new KeyActionState(Keys.Equal, EditorMode.Insert)
            {
                IsShiftActive = true,
                IsControlActive = true
            }] = KeyActions.IncreaseFontSize;
            AddControlStandaloneBinding(Keys.Minus, EditorMode.Insert, KeyActions.DecreaseFontSize);

            // ====================== Command Mode ====================== 
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
            AddVanillaMotionBinding(Keys.H, EditorMode.Command, KeyActions.MoveLeft);
            AddVanillaMotionBinding(Keys.L, EditorMode.Command, KeyActions.MoveRight);
            AddVanillaMotionBinding(Keys.K, EditorMode.Command, KeyActions.MoveUp);
            AddVanillaMotionBinding(Keys.J, EditorMode.Command, KeyActions.MoveDown);

            AddVanillaStandaloneBinding(Keys.X, EditorMode.Command, KeyActions.Delete);

            // Font size +/-
            StandaloneBindings[new KeyActionState(Keys.Equal, EditorMode.Command)
            {
                IsShiftActive = true,
                IsControlActive = true
            }] = KeyActions.IncreaseFontSize;
            AddControlStandaloneBinding(Keys.Minus, EditorMode.Command, KeyActions.DecreaseFontSize);

            AddVanillaCommandBinding(Keys.D, EditorMode.Command, KeyActions.DeleteSelection);

            // ====================== Visual Mode ====================== 
            AddVanillaStandaloneBinding(Keys.Escape, EditorMode.Visual, (_) =>
            {
                CurrentMode = EditorMode.Command;
                CurrentSelection = null;
            });

            // Movement keys
            AddVanillaMotionBinding(Keys.H, EditorMode.Visual, (tb) => MoveAndSelect(KeyActions.MoveLeft(tb)));
            AddVanillaMotionBinding(Keys.L, EditorMode.Visual, (tb) => MoveAndSelect(KeyActions.MoveRight(tb)));
            AddVanillaMotionBinding(Keys.K, EditorMode.Visual, (tb) => MoveAndSelect(KeyActions.MoveUp(tb)));
            AddVanillaMotionBinding(Keys.J, EditorMode.Visual, (tb) => MoveAndSelect(KeyActions.MoveDown(tb)));

            AddVanillaCommandBinding(Keys.D, EditorMode.Visual, KeyActions.DeleteSelection);
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

        public void Draw()
        {
            foreach (Frame frame in Frames)
            {
                float x = frame.X;
                float y = frame.Y;

                TextBuffer tBuffer = frame.Buffer;
                for (var i = 0; i < tBuffer.Lines.Count; i++)
                {
                    // Draw buffer text
                    var line = tBuffer.Lines[i];
                    _DrawBuffer.DrawString(CurrentTheme.FgColor, line, tBuffer.FontSize, (x, y));
                    y += tBuffer.FontSize + 8;

                    // Draw cursor
                    if (i == tBuffer.LineIdx && DateTime.Now.Millisecond >= 500)
                    {
                        var size = _DrawBuffer.MeasureString(tBuffer.ColIdx != line.Length ? line.Remove(tBuffer.ColIdx) : line, tBuffer.FontSize);
                        _DrawBuffer.DrawRect(
                            CurrentMode switch
                            {
                                EditorMode.Insert => _LineCursorColor,
                                _ => _BlockCursorColor
                            },
                            new Rect(
                                new Point(x + size.Width, y - tBuffer.FontSize - 2),
                                new Size((CurrentMode switch
                                {
                                    EditorMode.Insert => 2,
                                    _ => tBuffer.FontSize - 4
                                }), tBuffer.FontSize)));
                    }
                }

                // Highlight current selection
                if (CurrentSelection is not null)
                {
                    for (int i = CurrentSelection.StartLoc.LineIdx; i <= CurrentSelection.EndLoc.LineIdx; ++i)
                    {
                        float startX, endX;

                        startX = i != CurrentSelection.StartLoc.LineIdx
                            ? 0
                            : _DrawBuffer.MeasureString(tBuffer.Lines[i].Remove(CurrentSelection.StartLoc.ColIdx), tBuffer.FontSize).Width;
                        endX = i != CurrentSelection.EndLoc.LineIdx
                            ? _DrawBuffer.MeasureString(tBuffer.Lines[i], tBuffer.FontSize).Width
                            : _DrawBuffer.MeasureString(tBuffer.Lines[i].Remove(CurrentSelection.EndLoc.ColIdx), tBuffer.FontSize).Width;

                        float dX = x + startX;
                        float dY = (i * (tBuffer.FontSize + 8)) + 6;

                        _DrawBuffer.DrawRect(
                            _SelectionColor,
                            new Rect(
                                new Point(dX, dY),
                                new Size(endX + tBuffer.FontSize - startX, tBuffer.FontSize)));
                    }
                }

                // Draw current mode indicator
                _DrawBuffer.DrawString(CurrentTheme.CursorColor, CurrentMode.ToString(), 16, new Point(0, _DrawBuffer.ScreenSize.Height - 20));
            }
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
    }
}
