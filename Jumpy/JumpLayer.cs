using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Jumpy
{
    internal sealed class JumpyLayer
    {
        private readonly IAdornmentLayer _jumpyLayer;
        private readonly IWpfTextView _textView;

        public JumpyLayer(IWpfTextView textView)
        {
            _textView = textView;
            _jumpyLayer = textView.GetAdornmentLayer("Jumpy");
        }

        public void Draw()
        {
            var lines = _textView.TextViewLines;
            var snapshot = _textView.TextSnapshot;
            var text = snapshot.GetText();

            // TODO: move to options
            var regex = new Regex(@"[A-Z]+([0-9a-z]*)|[a-z0-9]{2,}", RegexOptions.Compiled);
            var matches = regex.Matches(text).OfType<Match>().ToArray();

            var pairs = KeyboardLayoutHelper.EnumeratePairs().GetEnumerator();
            foreach (var match in matches)
            {
                int position = match.Index;
                var span = new SnapshotSpan(snapshot, position, 1);
                Geometry charGeometry = lines.GetMarkerGeometry(span);
                if (charGeometry != null)
                {
                    if (!pairs.MoveNext())
                        return;
                    var pair = pairs.Current;
                    var model = new JumpKeysViewModel
                    {
                        Position = position,
                        Active = true,
                        FirstKey = pair.Item1,
                        SecondKey = pair.Item2,
                    };
                    var control = new JumpKeysControl(model);
                    // TODO: set line height and offset based on _textView.LineHeight
                    int verticalOffset = 8;
                    Canvas.SetLeft(control, charGeometry.Bounds.Left);
                    Canvas.SetTop(control, charGeometry.Bounds.Top - verticalOffset);
                    _jumpyLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, control, null);
                }
            }
        }

        public bool RemoveByFirstLetter(char firstChar)
        {
            _jumpyLayer.RemoveMatchingAdornments(
                adornment => adornment.Adornment is JumpKeysControl
                    && ((JumpKeysControl)adornment.Adornment).Model.FirstKey != firstChar);
            return !_jumpyLayer.IsEmpty;
        }

        public int? GetFinalPosition(char secondChar)
        {
            var model = _jumpyLayer.Elements.Select(e => e.Adornment)
                .Cast<JumpKeysControl>()
                .Select(control => control.Model)
                .FirstOrDefault(m => m.SecondKey == secondChar);
            return model != null ? model.Position : (int?) null;
        }

        public void Clear()
        {
            _jumpyLayer.RemoveAllAdornments();
        }
    }
}