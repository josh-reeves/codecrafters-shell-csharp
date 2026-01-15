using Interfaces;
using Shell.Extensions.ShellInputHandler.Lexer.Tokens;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

namespace Shell.Extensions.ShellInputHandler.Parser;

public class Parser : IParser
{
    #region Constructor(s)

    #endregion

    #region Methods
    public ITree Parse(Queue<IToken> tokens)
    {   
        CommandTree ast = new();

        ast.Root = ParseCommand(tokens);    

        return ast;

    }

    private ITreeNode ParseCommand(Queue<IToken> tokens)
    {
        CommandNode node = new CommandNode(tokens.Dequeue());

        while (tokens.Count > 0)
        {
            switch (tokens.Peek())
            {
                case WordToken:
                    node.Children.Add(new ArgumentNode(tokens.Dequeue(), node));

                    break;

                case RedirectStdOutToken:
                    tokens.Dequeue();

                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), FileMode.Create, node));

                    break;

                case RedirectStdErrToken:
                    tokens.Dequeue();

                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), FileMode.Create, node));

                    break;

                case AppendStdOutToken:
                    tokens.Dequeue();

                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), FileMode.Append, node));

                    break;

                case AppendStdErrToken:
                    tokens.Dequeue();

                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), FileMode.Append, node));

                    break;
            
            }

        }

        return node;

    }

    #endregion

}
