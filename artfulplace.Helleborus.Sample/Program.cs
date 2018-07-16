using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace artfulplace.Helleborus.Sample
{
    class Program
    {
        const string path = @"sample.txt";

        static void Main(string[] args)
        {
            var tree = SyntaxHelper.ParseFileText(path);

            foreach (var (element, type, nestedLevel) in SyntaxHelper.GetElements(tree))
            {
                ConsoleWrite(SyntaxHelper.GetSyntaxName(element, type), nestedLevel);
            }

            // var root = tree.GetRoot();
            // ConsoleWrite(root, 1, true);
            // SeekChildren(root.ChildNodesAndTokens(), 2);
            // Console.ResetColor();
        }

        static void SeekChildren(IEnumerable<SyntaxNodeOrToken> children, int nestedLevel)
        {
            foreach (var child in children)
            {
                ConsoleWrite(child, nestedLevel, child.IsNode);
                SeekChildren(child.ChildNodesAndTokens(), nestedLevel + 1);
                SeekTrivia(child, nestedLevel + 1);
            }
        }

        static void SeekTrivia(SyntaxNodeOrToken target, int nestedLevel)
        {
            if (target.HasLeadingTrivia)
            {
                foreach (var trivia in target.GetLeadingTrivia())
                {
                    ConsoleWrite(trivia, nestedLevel);
                }
            }

            if (target.HasTrailingTrivia)
            {
                foreach (var trivia in target.GetTrailingTrivia())
                {
                    ConsoleWrite(trivia, nestedLevel);
                }
            }
        }

        static void ConsoleWrite(SyntaxNodeOrToken node, int nestedLevel, bool isNode = false)
        {
            var tabs = string.Concat(Enumerable.Range(0, nestedLevel).Skip(1).Select(x => "  ").ToArray());

            if (isNode)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"{tabs}{SyntaxHelper.GetSyntaxName(node)}, {node.Span.ToString()}");
        }

        static void ConsoleWrite(SyntaxTrivia trivia, int nestedLevel)
        {
            var tabs = string.Concat(Enumerable.Range(0, nestedLevel).Skip(1).Select(x => "  ").ToArray());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{tabs}{trivia.Kind().ToString()} {trivia.Span.ToString()}");
        }

        static void ConsoleWrite(string name, int nestedLevel)
        {
            var tabs = string.Concat(Enumerable.Range(0, nestedLevel).Skip(1).Select(x => "  ").ToArray());
            Console.WriteLine($"{tabs}{name} ");
        }
    }
}
