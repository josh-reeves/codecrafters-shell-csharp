using Interfaces;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

namespace Shell.Extensions.ShellInputHandler.Parser;

public class Parser : IParser
{
    #region Constructor(s)
    public Parser()
    {

    }

    #endregion

    #region Methods
    public ITree Parse(Queue<IToken> tokens)
    {   
        try
        {
            CommandTree ast = new();

            if (tokens.Count <= 0)
            {
                return ast;

            }

            Queue<IShellToken> shellTokens = new(tokens.Cast<IShellToken>());

            ast.Root = ParseCommand(shellTokens);    

            return ast;
           
        }
        catch
        {
            throw new Exception("A parsing error occurred.");
            
        }

    }

    private ITreeNode ParseCommand(Queue<IShellToken> tokens)
    {
        CommandNode node = new(tokens.Dequeue());

        while (tokens.Count > 0)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.Word:
                    node.Children.Add(new ArgumentNode(tokens.Dequeue(), node));

                    break;

                case TokenType.RedirectStdOut or TokenType.RedirectStdErr:
                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Create, node));

                    break;

                case TokenType.AppendStdOut or TokenType.AppendStdErr:
                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Append, node));

                    break;

                default:
                    tokens.Dequeue();

                    break;
            
            }

        }

        return node;

    }

    #endregion

}
