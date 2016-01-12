using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Jumpy
{
    internal sealed class JumpyKeyProcessor : KeyProcessor
    {
        private readonly IWpfTextView _textView;
        private readonly JumpyLayer _jumpLayer;

        public JumpyKeyProcessor(IWpfTextView textView)
        {
            _textView = textView;
            _jumpLayer = new JumpyLayer(textView);
        }

        public State State { get; set; }

        public override void PreviewKeyDown(KeyEventArgs args)
        {
            // This method describes state machine with transitions between states: Idle, WaitFirstKey, WaitSecondKey
            var prevState = State;
            if (State != State.Idle && Escape(args))
            {
                Reset();
            }
            else if (State == State.Idle)
            {
                if (IsHotKey())
                {
                    _jumpLayer.Draw();
                    State = State.WaitFirstKey;
                }
            }
            else if (State == State.WaitFirstKey)
            {
                char? firstLetter = GetPressedCharacter(args);
                if (!firstLetter.HasValue || !_jumpLayer.RemoveByFirstLetter(firstLetter.Value))
                {
                    Reset();
                }
                else
                {
                    State = State.WaitSecondKey;
                }
            }
            else if (State == State.WaitSecondKey)
            {
                char? secondLetter = GetPressedCharacter(args);
                int? pos;
                if (secondLetter.HasValue && (pos = _jumpLayer.GetFinalPosition(secondLetter.Value)).HasValue)
                {
                    var snapshotPoint = new SnapshotPoint(_textView.TextSnapshot, pos.Value);
                    _textView.Caret.MoveTo(snapshotPoint);
                }
                Reset();
            }
            args.Handled = State != prevState;
        }

        private static char? GetPressedCharacter(KeyEventArgs args)
        {
            var keyStr = KeyUtility.GetKey(args.Key);
            if (keyStr.Length != 1)
            {
                return null;
            }
            var ch = keyStr[0];
            return char.ToLowerInvariant(ch);
        }

        private void Reset()
        {
            _jumpLayer.Clear();
            State = State.Idle;
        }

        private bool IsHotKey()
        {
            // TODO: get from plugin options
            return Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.RightShift);
        }

        private static bool Escape(KeyEventArgs args)
        {
            Key[] keys = { Key.Escape, Key.Left, Key.Right, Key.Up, Key.Down, Key.Enter };
            return keys.Contains(args.Key) || keys.Contains(args.SystemKey);
        }
    }
}