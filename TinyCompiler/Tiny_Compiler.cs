using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_Compiler
{ 
    public static class Tiny_Compiler
    {
        public static Scanner Tiny_Scanner = new Scanner();
        public static Parser Tiny_Parser = new Parser();
        public static List<Token> TokenStream = new List<Token>();
        public static Node treeroot;

        public static void Start_Compiling(string SourceCode) //character by character
        {
            // 1. Reset everything to ensure no old data remains
            Errors.Error_List.Clear();
            Tiny_Scanner.Tokens.Clear();
            TokenStream.Clear();

            // 2. Run Scanner
            Tiny_Scanner.StartScanning(SourceCode);

            // 3. LINK THE DATA (The Missing Step)
            TokenStream = Tiny_Scanner.Tokens;

            // 4. Run Parser
            Tiny_Parser.StartParsing(TokenStream);
            treeroot = Tiny_Parser.root;
        }
    }
}
