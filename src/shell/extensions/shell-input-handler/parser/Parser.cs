using Interfaces;
using Shell.Extensions.ShellInputHandler.Lexer.Tokens;
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

            ast.Root = ParseCommand(tokens);    

            return ast;
           
        }
        catch
        {
            throw new Exception("A parsing error occurred.");
            
        }

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

                case RedirectStdOutToken or RedirectStdErrToken:
                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Create, node));

                    break;

                case AppendStdOutToken or AppendStdErrToken:
                    node.Children.Add(new RedirectorNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Append, node));

                    break;
            
            }

        }

        return node;

    }

    #endregion

}
