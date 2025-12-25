namespace Interfaces;

public interface IParser
{
    public IList<string> Parse(string input, IList<char> separators, IList<char>? operators = null, Dictionary<char, char>? groupDelimiters = null, Dictionary<char, Action>? delimiterParsers = null);
   
}