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
        public TextBuffer Buffer { get; set; } = new TextBuffer();
        public Dictionary<KeyActionState, MotionAction> MotionBindings { get; set; } = new Dictionary<KeyActionState, MotionAction>();
        public Dictionary<KeyActionState, CommandAction> CommandBindings { get; set; } = new Dictionary<KeyActionState, CommandAction>();
        public Dictionary<KeyActionState, StandaloneAction> StandaloneBindings { get; set; } = new Dictionary<KeyActionState, StandaloneAction>();
        public EditorMode CurrentMode { get; set; } = EditorMode.Insert;
        public Theme CurrentTheme { get; set; }

        private DrawBuffer _DrawBuffer;
        private Color4 _LineCursorColor;
        private Color4 _BlockCursorColor;

        public CommandAction? PendingCommand { get; set; } = null;

        public EednEngine(DrawBuffer drawBuffer, Theme theme)
        {
            _DrawBuffer = drawBuffer;
            CurrentTheme = theme;

            _LineCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 1.0f);
            _BlockCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 0.5f);

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
            AddVanillaStandaloneBinding(Keys.A, EditorMode.Command, (tb) => { CurrentMode = EditorMode.Insert; tb.ColIdx += 1; });
            AddShiftStandaloneBinding(Keys.I, EditorMode.Command, (tb) => { CurrentMode = EditorMode.Insert; tb.ColIdx = 0; });
            AddShiftStandaloneBinding(Keys.A, EditorMode.Command, (tb) => { CurrentMode = EditorMode.Insert; tb.ColIdx = tb.Lines[tb.LineIdx].Length; });

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
                standaloneAction(Buffer);
                return;
            }

            // Check for command actions
            bool keyHasCommand = CommandBindings.TryGetValue(keyActionState, out CommandAction? commandAction);
            if (keyHasCommand && commandAction is not null)
            {
                // If no pending or pending is different from incoming command - set incoming as pending.
                if (PendingCommand is null || PendingCommand != commandAction)
                {
                    PendingCommand = commandAction;
                    return;
                }

                // If pending is the same as incoming - process command on current line.
                commandAction(Buffer, new TextSelection(
                    (Buffer.LineIdx, 0), (Buffer.LineIdx, Buffer.Lines[Buffer.LineIdx].Length - 1))
                {
                    IsFullBlock = true
                });

                PendingCommand = null;
                return;
            }

            bool keyHasMotion = MotionBindings.TryGetValue(keyActionState, out MotionAction? motionAction);
            if (keyHasMotion && motionAction is not null)
            {
                TextLocation endLoc = motionAction(Buffer);
                if (PendingCommand is not null)
                {
                    TextSelection selection = new TextSelection(
                        new TextLocation(Buffer.LineIdx, Buffer.ColIdx),
                        endLoc);
                    PendingCommand(Buffer, selection);
                    PendingCommand = null;
                }
                else
                {
                    Buffer.LineIdx = endLoc.LineIdx;
                    Buffer.ColIdx = endLoc.ColIdx;
                }
                return;
            }
        }

        public void Draw()
        {
            var x = 0;
            var y = 0;

            for (var i = 0; i < Buffer.Lines.Count; i++)
            {
                // Draw buffer text
                var line = Buffer.Lines[i];
                _DrawBuffer.DrawString(CurrentTheme.FgColor, line, Buffer.FontSize, new Point(x, y));
                y += Buffer.FontSize + 8;

                // Draw cursor
                if (i == Buffer.LineIdx && DateTime.Now.Millisecond >= 500)
                {
                    var size = _DrawBuffer.MeasureString(Buffer.ColIdx != line.Length ? line.Remove(Buffer.ColIdx) : line, Buffer.FontSize);
                    _DrawBuffer.DrawRect(
                        CurrentMode switch
                        {
                            EditorMode.Insert => _LineCursorColor,
                            _ => _BlockCursorColor
                        },
                        new Rect(
                            new Point(x + size.Width, y - Buffer.FontSize - 2),
                            new Size((CurrentMode switch
                            {
                                EditorMode.Insert => 2,
                                _ => Buffer.FontSize - 4
                            }), Buffer.FontSize)));
                }
            }
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
