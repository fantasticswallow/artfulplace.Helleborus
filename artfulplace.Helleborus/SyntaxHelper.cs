using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace artfulplace.Helleborus
{
    public class SyntaxHelper
    {
        public static CSharpSyntaxTree ParseFileText(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(sr.ReadToEnd(), CSharpParseOptions.Default);
            }
        }

        public static CSharpSyntaxTree ParseText(string document)
        {
            return (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(document, CSharpParseOptions.Default);
        }

        public static string GetSyntaxName(SyntaxNodeOrToken target)
        {
            if (target.IsNode)
            {
                var node = (SyntaxNode)target;
                return node.GetType().Name;                
            }
            else if (target.IsToken)
            {
                var token = (SyntaxToken)target;
                return token.Kind().ToString();
            }

            return "Detection Error!";
        }

        public static string GetSyntaxName(object target, ElementType type)
        {
            switch (type)
            {
                case ElementType.SyntaxNode:
                    return ((SyntaxNode)target).GetType().Name;
                case ElementType.SytaxToken:
                    return ((SyntaxToken)target).Kind().ToString();
                case ElementType.SyntaxTrivia:
                    return ((SyntaxTrivia)target).Kind().ToString();
                default:
                    return "Detection Error!";
            }
        }

        public static IEnumerable<(object element, ElementType type, int nestedLevel)> GetElements(CSharpSyntaxTree target)
        {
            var root = target.GetRoot();
            yield return (root, ElementType.SyntaxNode, 1);
            foreach (var element in GetChildElements(root.ChildNodesAndTokens(), 2))
            {
                yield return element;
            }
        }

        static IEnumerable<(object element, ElementType type, int nestedLevel)> GetChildElements(IEnumerable<SyntaxNodeOrToken> targets, int nestedLevel)
        {
            foreach (var child in targets)
            {
                if (child.IsNode)
                {
                    yield return ((SyntaxNode)child, GetElementType(child.IsNode), nestedLevel);
                }
                else
                {
                    yield return ((SyntaxToken)child, GetElementType(child.IsNode), nestedLevel);
                }
                
                foreach (var obj in GetChildElements(child.ChildNodesAndTokens(), nestedLevel + 1))
                {
                    yield return obj;
                }

                foreach (var obj in GetTrivia(child, nestedLevel + 1))
                {
                    yield return obj;
                }
            }
        }

        static IEnumerable<(object element, ElementType type, int nestedLevel)> GetTrivia(SyntaxNodeOrToken target, int nestedLevel)
        {
            if (target.HasLeadingTrivia)
            {
                foreach (var trivia in target.GetLeadingTrivia())
                {
                    yield return (trivia, ElementType.SyntaxTrivia, nestedLevel);
                }
            }

            if (target.HasTrailingTrivia)
            {
                foreach (var trivia in target.GetTrailingTrivia())
                {
                    yield return (trivia, ElementType.SyntaxTrivia, nestedLevel);
                }
            }
        }

        static ElementType GetElementType(bool isNode) => isNode ? ElementType.SyntaxNode : ElementType.SytaxToken;
        
        public enum ElementType
        {
            SyntaxNode,
            SytaxToken,
            SyntaxTrivia,
        }

        public static IEnumerable<(string key, string value)> GetElementProperties(object element)
        {
            var type = element.GetType();
            foreach (var prop in type.GetProperties())
            {
                yield return (prop.Name, prop.GetValue(element).ToString());
            }
        }
    }
}
