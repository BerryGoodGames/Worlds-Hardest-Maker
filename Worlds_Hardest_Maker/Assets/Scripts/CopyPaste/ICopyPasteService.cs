using System.Collections;
using WorldsHardestMaker.Selection;

namespace WorldsHardestMaker.CopyPaste
{
    public interface ICopyPasteService
    {
        public bool IsPasting { get; }

        public void Copy(SelectionArea area);
        public IEnumerator PasteCoroutine();
    }
}