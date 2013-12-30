namespace JSLintNet.VisualStudio.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal class JSLintTagger : ITagger<IErrorTag>
    {
        private static readonly Regex WordBoundaryPattern = new Regex(@"[^\$\w]", RegexOptions.Compiled);

        private ITextBuffer buffer;

        private IJSLintErrorListProvider errorListProvider;

        private string fileName;

        private IList<JSLintTag> tags;

        public JSLintTagger(ITextBuffer buffer, IJSLintErrorListProvider errorListProvider, string fileName)
        {
            this.buffer = buffer;
            this.errorListProvider = errorListProvider;
            this.fileName = fileName;

            this.tags = new List<JSLintTag>();
            this.PopulateTags();

            this.errorListProvider.ErrorListChange += this.OnErrorListChange;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection snapshotSpans)
        {
            var list = new List<TagSpan<IErrorTag>>();

            if (this.tags.Count > 0)
            {
                foreach (var snapshotSpan in snapshotSpans)
                {
                    foreach (var tag in this.tags)
                    {
                        var snapshot = snapshotSpan.Snapshot;
                        var span = tag.TrackingSpan.GetSpan(snapshot);

                        if (span.IntersectsWith(snapshotSpan))
                        {
                            var tagSpan = new TagSpan<IErrorTag>(new SnapshotSpan(snapshot, span), tag);

                            list.Add(tagSpan);
                        }
                    }
                }
            }

            return list;
        }

        private static Span GetErrorSpan(ITextSnapshot snapshot, JSLintErrorTask error)
        {
            var line = snapshot.GetLineFromLineNumber(error.Line);
            var text = line.GetText();

            if (text.Length < error.Column)
            {
                return new Span(line.End.Position, 1);
            }

            var start = line.Start.Position + error.Column;
            var length = line.End.Position - start;
            var match = WordBoundaryPattern.Match(text, error.Column);

            if (match.Success)
            {
                length = match.Index - error.Column;
            }

            return new Span(start, length);
        }

        private void OnErrorListChange(object sender, ErrorListChangeEventArgs e)
        {
            if (this.IsRelevant(e))
            {
                this.PopulateTags();

                var handler = this.TagsChanged;

                if (handler != null)
                {
                    var snapshot = this.buffer.CurrentSnapshot;
                    var span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));

                    handler(this, new SnapshotSpanEventArgs(span));
                }
            }
        }

        private bool IsRelevant(ErrorListChangeEventArgs e)
        {
            switch (e.Action)
            {
                case ErrorListAction.ClearFile:
                case ErrorListAction.AddFile:
                    return this.fileName == e.FileName;
                case ErrorListAction.ClearAll:
                    return true;
            }

            return false;
        }

        private void PopulateTags()
        {
            this.tags.Clear();

            var errors = this.errorListProvider.GetErrors(this.fileName);
            var snapshot = this.buffer.CurrentSnapshot;

            foreach (var error in errors)
            {
                var errorSpan = GetErrorSpan(snapshot, error);
                var trackingSpan = snapshot.CreateTrackingSpan(errorSpan, SpanTrackingMode.EdgeInclusive);

                this.tags.Add(new JSLintTag(trackingSpan, error.Text));
            }
        }
    }
}
