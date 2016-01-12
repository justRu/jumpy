using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Jumpy
{
    [Export(typeof(IKeyProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Name("JumpyKeyProcessor")]
    internal sealed class JumpyKeyProcessorProvider : IKeyProcessorProvider
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("Jumpy")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        public AdornmentLayerDefinition EditorAdornmentLayer;

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new JumpyKeyProcessor(wpfTextView);
        }
    }
}
