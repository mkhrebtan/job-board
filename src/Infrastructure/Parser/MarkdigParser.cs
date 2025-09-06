using System.Text;
using Domain.Abstraction.Interfaces;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Infrastructure.Parser;

internal class MarkdigParser : IMarkdownParser
{
    public string ToPlainText(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var document = Markdown.Parse(markdown, pipeline);
        var builder = new StringBuilder();

        foreach (var block in document)
        {
            ExtractTextFromBlock(block, builder);
        }

        return NormalizeWhitespace(builder.ToString());
    }

    private static void ExtractTextFromBlock(Block block, StringBuilder builder)
    {
        switch (block)
        {
            case HeadingBlock heading:
                if (heading.Inline is not null)
                {
                    ExtractTextFromInline(heading.Inline, builder);
                    builder.AppendLine();
                }

                break;

            case ParagraphBlock paragraph:
                if (paragraph.Inline is not null)
                {
                    ExtractTextFromInline(paragraph.Inline, builder);
                    builder.AppendLine();
                }

                break;

            case ListBlock list:
                foreach (var item in list)
                {
                    if (item is ListItemBlock listItem)
                    {
                        foreach (var subBlock in listItem)
                        {
                            ExtractTextFromBlock(subBlock, builder);
                        }
                    }
                }

                break;

            case QuoteBlock quote:
                foreach (var subBlock in quote)
                {
                    ExtractTextFromBlock(subBlock, builder);
                }

                break;
        }
    }

    private static void ExtractTextFromInline(ContainerInline container, StringBuilder builder)
    {
        foreach (var inline in container)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    builder.Append(literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length));
                    break;

                case EmphasisInline emphasis:
                    ExtractTextFromInline(emphasis, builder);
                    break;

                case LineBreakInline:
                    builder.AppendLine();
                    break;
            }
        }
    }

    private static string NormalizeWhitespace(string input)
    {
        return string.Join(
            Environment.NewLine,
            input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(line => line.Trim())
                 .Where(line => !string.IsNullOrWhiteSpace(line)));
    }
}
