using Interfaces;
using Shell.Core.Input.ShellInputHandler.Parser;
using Shell.Core.Input.ShellInputHandler.Parser.Nodes;

namespace Shell;

/* The resulting syntax tree sould look like this:
 *  Command: ls -lS | grep "^-" | head -n 3
 *  Tree:
 *                                     ls
 *                                    /  \
 *                                  -lS   |
 *                                       / 
 *                                     grep
 *                                     /  \
 *                                    ^_.  |
 *                                        /
 *                                      head
 *                                      /
 *                                     -n
 *                                    /
 *                                   3
 *  
 * Commands, arguments and file redirectors go to the left. Pipes go to 
 *  the right. This is somewhat unconventional, but the resulting tree is a good
 *  representation of the original command that can be traversed in a fairly
 *  straight-forward fashion.*/

static class ParsingMethods
{
    const string errMsg = "A parsing error occurred.";

    public static IDebugger? Debugger { get; set; }

    public static ITree Parse(Queue<IToken> tokens)
    {
        CommandTree ast = new CommandTree();

        try
        {
            if (tokens.Count <= 0)
            {
                return ast;

            }

            Queue<IShellToken> shellTokens = new(tokens.Cast<IShellToken>());

            ast.Root = ParsePipes(shellTokens);    

            return ast;
            
        }
        catch (Exception ex)
        {
            Debugger?.WriteLine(ex.Message);

            return ast;
            
        }

    }

    private static IShellNode ParsePipes (Queue<IShellToken> tokens)
    {
        IShellNode root,
                   node = root = ParseOperators(tokens);

        while (tokens.Count > 0 && tokens.Peek().Type is TokenType.Pipe)
        {
            node.RightChild = new ShellNode(NodeType.OutputRedirection, tokens.Dequeue(), node);
            node = (ShellNode)node.RightChild;
#if DEBUG
            Debugger?.WriteLine($"PARSING: Pipe ({node.Data.ExpandedValue}) parsed.");
#endif
            node.LeftChild = ParseOperators(tokens);
            node = (ShellNode)node.LeftChild;
#if DEBUG
            Debugger?.WriteLine($"PARSING: Node ({node.Data.ExpandedValue}) set as left child.");
#endif            
        }

        return root;

    }

    private static IShellNode ParseOperators(Queue<IShellToken> tokens)
    {
        IShellNode command = ParseWords(tokens);

        TokenType[] operators = [
            TokenType.RedirectStdErr,
            TokenType.RedirectStdOut,
            TokenType.AppendStdErr, 
            TokenType.AppendStdOut];

        while (tokens.Count > 0 && operators.Contains(tokens.Peek().Type))
        {
            ITreeNode node = command.GetLastChild();

            if (tokens.Peek().Type is TokenType.RedirectStdOut or TokenType.RedirectStdErr)
            {
                node.LeftChild  = new OutputToFileNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Create, node);

            }
            else if (tokens.Peek().Type is TokenType.AppendStdOut or TokenType.AppendStdErr)
            {
                node.LeftChild = new OutputToFileNode(tokens.Dequeue(), tokens.Dequeue(), FileMode.Append, node);

            }

        }

        return command;

    }

    private static IShellNode ParseWords(Queue<IShellToken> tokens)
    {
        if (tokens.Count <= 0 || tokens.Peek().Type is not TokenType.Word)
        {
            throw new Exception();
            
        }

        IShellNode command = new ShellNode(NodeType.Command, tokens.Dequeue());
#if DEBUG
        Debugger?.WriteLine($"PARSING: Command parsed: {command.Data.ExpandedValue}. Beginning argument parsing.");
#endif
        while (tokens.Count > 0 && tokens.Peek().Type is TokenType.Word)
        {
            ITreeNode node = command.GetLastChild();

            node.LeftChild = new ShellNode(NodeType.Argument, tokens.Dequeue(), node);
#if DEBUG
            Debugger?.WriteLine($"PARSING: Argument parsed: {((ShellNode)node.LeftChild).Data.ExpandedValue}");
#endif           
        }
#if DEBUG
        Debugger?.WriteLine($"PARSING: Argument Parsing Complete");
#endif
        return command;
        
    }

}
