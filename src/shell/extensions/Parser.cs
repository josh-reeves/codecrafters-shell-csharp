using Interfaces;

namespace Shell.Extensions;

public class Parser : IParser
{
    public Parser() {}

    /// <summary>
    /// Parses the provided string into a list. By default, the string will be
    /// partitioned based on the contents for the separators parameter.
    /// Optional parameters can be provided to modify behavior.
    /// </summary>
    /// <param name="input">The string to be parsed.</param>
    /// <param name="separators">
    /// The list of characters with which the input
    /// string will be partitioned.
    /// </param>
    /// <param name="operators">
    /// An optional list of operator characters to parse from the input string.
    /// Operators are placed into their own groups along with preceding or
    ///  subsequent occurences of the same character.
    /// </param>
    /// <param name="groupDelimiters"></param>
    /// <param name="delimiterParsers"></param>
    /// <returns></returns>
    public IList<string> Parse(string input, IList<char> separators, IList<char>? operators = null, Dictionary<char, char>? groupDelimiters = null, Dictionary<char, Action>? delimiterParsers = null)
    {
        if (input.Length <= 0)
        {
            return [];

        }

        // Holds parsed input during parsing:
        string token = string.Empty;

        List<string> parsedTokens = new();

        for(int i = 0; i < input.Length; i++)
        {
            /* Don't add the character if it's specified as a delimiter, 
             * separator, or operator. Otherwise add it to the current input 
             * string:*/
            if (!separators.Contains(input[i]) && !(groupDelimiters?.Keys.Contains(input[i]) ?? false))
            {
                token += input[i];
            
            }

            /* This section handles group delimiters ('', "", etc. By default it 
             * will handle add the entire group to a single index as a string
             * literal, but it can be overidden via the delimiterParsers
             * argument:*/
            if (groupDelimiters?.Keys.Contains(input[i]) ?? false)
            {
                char delimKey = input[i];

                i++;

                if (delimiterParsers?.Keys.Contains(delimKey) ?? false)
                {
                    delimiterParsers[delimKey]?.Invoke();

                }
                else
                {
                    while(i < input.Length && input[i] != groupDelimiters[delimKey])
                    {
                        token += input[i];

                        i++;

                    }

                }

            }
            
            /* Finally, check to see if the token needs to be added to the list
             *  of parsed tokens and reset. This occurs in any of the following
             *  situations:
             *      1. If the parsing method has passed the end of the input
             *          string.
             *      2. If the current character is a separator.
             *      3. If the next character is a control character and the
             *          current character is not.
             *      4. If the current character is a control character and the 
             *          next character is not.
             * 
             * The last two conditions ensure that operator characters are 
             *  always placed in their own group alongside any preceding or
             *  subsequent occurences of the same character.
             *
             * This is admittedly a little clumsy, but it's functional, it's 
             *  for an optional feature and the complexity created by splitting
             *  the logic out into methods or any sort of pattern would probably
             *  be worse: */
            if (i >= input.Length - 1 || separators.Contains(input[i]) || ((operators?.Contains(input[i + 1]) ?? false) && input[i + 1] != input[i]) || ((operators?.Contains(input[i]) ?? false) && input[i + 1] != input[i]))
            {
                parsedTokens.Add(token);
                token = string.Empty;     

            }

        }

        return parsedTokens;
        
    }    
    
}