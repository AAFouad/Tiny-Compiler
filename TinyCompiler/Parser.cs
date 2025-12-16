using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }

        // Program -> Function_Statement_List Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Function_Statement_List());
            program.Children.Add(Main_Function());
            MessageBox.Show("Success");
            return program;
        }

        // Function_Statement_List -> Function_Statement FSL’ | ε
        Node Function_Statement_List()
        {
            Node node = new Node("Function_Statement_List");

            if (IsDataType())
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.T_Main)
                { 
                    return null;
                }

                node.Children.Add(Function_Statement());
                node.Children.Add(FSL_Prime());
                return node;
            }
            return null;
        }

        // FSL’ → Function_Statement FSL’ | ε
        Node FSL_Prime()
        {
            Node node = new Node("FSL'");
            if (IsDataType())
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.T_Main)
                {
                    return null;
                }

                node.Children.Add(Function_Statement());
                node.Children.Add(FSL_Prime());
                return node;
            }
            return null;
        }

        // Main_Function -> Data_Type main ( ) Function_Body
        Node Main_Function()
        {
            Node node = new Node("Main_Function");
            node.Children.Add(Data_Type());
            node.Children.Add(match(Token_Class.T_Main));
            node.Children.Add(match(Token_Class.T_LParanthesis));
            node.Children.Add(match(Token_Class.T_RParanthesis));
            node.Children.Add(Function_Body());
            return node;
        }

        // Function_Statement -> Function_Declaration Function_Body
        Node Function_Statement()
        {
            Node node = new Node("Function_Statement");
            node.Children.Add(Function_Declaration());
            node.Children.Add(Function_Body());
            return node;
        }

        // Function_Declaration -> Data_Type Function_Name ( Parameter_List )
        Node Function_Declaration()
        {
            Node node = new Node("Function_Declaration");
            node.Children.Add(Data_Type());
            node.Children.Add(Function_Name());
            node.Children.Add(match(Token_Class.T_LParanthesis));
            node.Children.Add(Parameter_List());
            node.Children.Add(match(Token_Class.T_RParanthesis));
            return node;
        }

        // Function_Body -> { Statement_List Return_Statement }
        Node Function_Body()
        {
            Node node = new Node("Function_Body");
            node.Children.Add(match(Token_Class.T_LCurly));
            node.Children.Add(Statement_List());
            node.Children.Add(Return_Statement());
            node.Children.Add(match(Token_Class.T_RCurly));
            return node;
        }

        // Data_Type -> int | float | string
        Node Data_Type()
        {
            Node node = new Node("Data_Type");
            if (CheckToken(Token_Class.T_Int) || CheckToken(Token_Class.T_Float) || CheckToken(Token_Class.T_String))
            {
                node.Children.Add(match(TokenStream[InputPointer].token_type));
                return node;
            }
            return null;
        }

        // Parameter_List → Parameter PL’ | ε
        Node Parameter_List()
        {
            Node node = new Node("Parameter_List");
            if (IsDataType())
            {
                node.Children.Add(Parameter());
                node.Children.Add(PL_Prime());
                return node;
            }
            return null;
        }

        // PL’ → Parameter PL’ | ε
        Node PL_Prime()
        {
            Node node = new Node("PL'");
            if (CheckToken(Token_Class.T_Comma))
            {
                node.Children.Add(match(Token_Class.T_Comma));
                node.Children.Add(Parameter());
                node.Children.Add(PL_Prime());
                return node;
            }
            return null;
        }

        // Parameter -> Data_Type identifier
        Node Parameter()
        {
            Node node = new Node("Parameter");
            node.Children.Add(Data_Type());
            node.Children.Add(match(Token_Class.T_Identifier));
            return node;
        }

        // Statement_List → Statement SL’ | ε
        Node Statement_List()
        {
            Node node = new Node("Statement_List");
            if (IsStatementStart())
            {
                node.Children.Add(Statement());
                node.Children.Add(SL_Prime());
                return node;
            }
            return null;
        }

        // SL’ → Statement SL’ | ε
        Node SL_Prime()
        {
            Node node = new Node("SL'");
            if (IsStatementStart())
            {
                node.Children.Add(Statement());
                node.Children.Add(SL_Prime());
                return node;
            }
            return null;
        }

        // Statement → Comment_Statement | Function_Call | Assignment_Statement |
        // Declaration_Statement | Write_Statement | Read_Statement |
        // If_Statement | Repeat_Statement
        Node Statement()
        {
            Node node = new Node("Statement");
            if (InputPointer >= TokenStream.Count) return null;

            Token_Class currentType = TokenStream[InputPointer].token_type;

            if (currentType == Token_Class.T_Comment)
            {
                node.Children.Add(match(Token_Class.T_Comment));
            }
            else if (IsDataType())
            {
                node.Children.Add(Declaration_Statement());
            }
            else if (currentType == Token_Class.T_Write)
            {
                node.Children.Add(Write_Statement());
            }
            else if (currentType == Token_Class.T_Read)
            {
                node.Children.Add(Read_Statement());
            }
            else if (currentType == Token_Class.T_If)
            {
                node.Children.Add(If_Statement());
            }
            else if (currentType == Token_Class.T_Repeat)
            {
                node.Children.Add(Repeat_Statement());
            }
            else if (currentType == Token_Class.T_Identifier)
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.T_LParanthesis)
                {
                    node.Children.Add(Function_Call());
                }
                else
                {
                    node.Children.Add(Assignment_Statement());
                }
            }
            return node;
        }

        // Assignment_Statement -> identifier := Expression
        Node Assignment_Statement()
        {
            Node node = new Node("Assignment_Statement");
            node.Children.Add(match(Token_Class.T_Identifier));
            node.Children.Add(match(Token_Class.T_EqualOp));
            node.Children.Add(Expression());
            node.Children.Add(match(Token_Class.T_Semicolon));
            return node;
        }

        // Declaration_Statement → Data_Type Identifier_List D’
        Node Declaration_Statement()
        {
            Node node = new Node("Declaration_Statement");
            node.Children.Add(Data_Type());
            node.Children.Add(Identifier_List());
            node.Children.Add(D_Prime());
            return node;
        }

        // D’ → ; | : = Expression
        Node D_Prime()
        {
            Node node = new Node("D'");
            if (CheckToken(Token_Class.T_Semicolon))
            {
                node.Children.Add(match(Token_Class.T_Semicolon));
            }
            else if (CheckToken(Token_Class.T_EqualOp))
            {
                node.Children.Add(match(Token_Class.T_EqualOp));
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.T_Semicolon));
            }
            return node;
        }

        // Write_Statement → write W’
        Node Write_Statement()
        {
            Node node = new Node("Write_Statement");
            node.Children.Add(match(Token_Class.T_Write));
            node.Children.Add(W_Prime());
            return node;
        }

        // W’ → Expression ; | endl ;
        Node W_Prime()
        {
            Node node = new Node("W'");
            if (CheckToken(Token_Class.T_Endl))
            {
                node.Children.Add(match(Token_Class.T_Endl));
                node.Children.Add(match(Token_Class.T_Semicolon));
            }
            else
            {
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.T_Semicolon));
            }
            return node;
        }

        // Read_Statement → read identifier ;
        Node Read_Statement()
        {
            Node node = new Node("Read_Statement");
            node.Children.Add(match(Token_Class.T_Read));
            node.Children.Add(match(Token_Class.T_Identifier));
            node.Children.Add(match(Token_Class.T_Semicolon));
            return node;
        }

        // If_Statement → if Condition_Statement then Statement_List Else_Clause
        Node If_Statement()
        {
            Node node = new Node("If_Statement");
            node.Children.Add(match(Token_Class.T_If));
            node.Children.Add(Condition_Statement());
            node.Children.Add(match(Token_Class.T_Then));
            node.Children.Add(Statement_List());
            node.Children.Add(Else_Clause());
            return node;
        }

        // Else_Clause → Else_If_Statement | Else_Statement | end
        Node Else_Clause()
        {
            Node node = new Node("Else_Clause");
            if (CheckToken(Token_Class.T_ElseIf))
            {
                node.Children.Add(match(Token_Class.T_ElseIf));
                node.Children.Add(Condition_Statement());
                node.Children.Add(match(Token_Class.T_Then));
                node.Children.Add(Statement_List());
                node.Children.Add(Else_Clause());
            }
            else if (CheckToken(Token_Class.T_Else))
            {
                node.Children.Add(match(Token_Class.T_Else));
                node.Children.Add(Statement_List());
                node.Children.Add(match(Token_Class.T_End));
            }
            else if (CheckToken(Token_Class.T_End))
            {
                node.Children.Add(match(Token_Class.T_End));
            }
            return node;
        }

        // Repeat_Statement → repeat Statement_List until Condition_Statement
        Node Repeat_Statement()
        {
            Node node = new Node("Repeat_Statement");
            node.Children.Add(match(Token_Class.T_Repeat));
            node.Children.Add(Statement_List());
            node.Children.Add(match(Token_Class.T_Until));
            node.Children.Add(Condition_Statement());
            return node;
        }

        // Condition_Statement → Condition CondState’
        Node Condition_Statement()
        {
            Node node = new Node("Condition_Statement");
            node.Children.Add(Condition());
            node.Children.Add(CondState_Prime());
            return node;
        }

        // CondState’ → Boolean_Operator Condition CondState’ | ε
        Node CondState_Prime()
        {
            Node node = new Node("CondState'");
            if (CheckToken(Token_Class.T_And) || CheckToken(Token_Class.T_Or))
            {
                node.Children.Add(Boolean_Operator());
                node.Children.Add(Condition());
                node.Children.Add(CondState_Prime());
                return node;
            }
            return null;
        }

        // Condition → identifier Condition_Operator Term
        Node Condition()
        {
            Node node = new Node("Condition");
            node.Children.Add(match(Token_Class.T_Identifier));
            node.Children.Add(Condition_Operator());
            node.Children.Add(Term());
            return node;
        }

        // Expression → string | Term | Equation
        Node Expression()
        {
            Node node = new Node("Expression");
            if (CheckToken(Token_Class.T_String))
            {
                node.Children.Add(match(Token_Class.T_String));
            }
            else
            {
                node.Children.Add(Equation());
            }
            return node;
        }

        // Equation → ( Equation ) EQ’ | Term EQ’
        Node Equation()
        {
            Node node = new Node("Equation");
            if (CheckToken(Token_Class.T_LParanthesis))
            {
                node.Children.Add(match(Token_Class.T_LParanthesis));
                node.Children.Add(Equation());
                node.Children.Add(match(Token_Class.T_RParanthesis));
                node.Children.Add(EQ_Prime());
            }
            else
            {
                node.Children.Add(Term());
                node.Children.Add(EQ_Prime());
            }
            return node;
        }

        // EQ’ → Arithmetc_Operator Equation EQ’ | ε
        Node EQ_Prime()
        {
            Node node = new Node("EQ'");
            if (CheckToken(Token_Class.T_PlusOp) || CheckToken(Token_Class.T_MinusOp) ||
                CheckToken(Token_Class.T_MultiplyOp) || CheckToken(Token_Class.T_DivideOp))
            {
                node.Children.Add(Arithmetic_Operator());
                node.Children.Add(Equation());
                node.Children.Add(EQ_Prime());
                return node;
            }
            return null;
        }

        // Term → number | identifier | Function_Call
        Node Term()
        {
            Node node = new Node("Term");
            if (CheckToken(Token_Class.T_Constant))
            {
                node.Children.Add(match(Token_Class.T_Constant));
            }
            else if (CheckToken(Token_Class.T_Identifier))
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.T_LParanthesis)
                {
                    node.Children.Add(Function_Call());
                }
                else
                {
                    node.Children.Add(match(Token_Class.T_Identifier));
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected Term (Constant or Identifier) but found " +
                     (InputPointer < TokenStream.Count ? TokenStream[InputPointer].token_type.ToString() : "End of File") + "\r\n");
            }
            return node;
        }

        // Function_Call → Function_Name ( Argument_List )
        Node Function_Call()
        {
            Node node = new Node("Function_Call");
            node.Children.Add(Function_Name());
            node.Children.Add(match(Token_Class.T_LParanthesis));
            node.Children.Add(Argument_List());
            node.Children.Add(match(Token_Class.T_RParanthesis));
            return node;
        }

        // Argument_List → Expression Argument_List’ | ε
        Node Argument_List()
        {
            Node node = new Node("Argument_List");
            if (CheckToken(Token_Class.T_Identifier) || CheckToken(Token_Class.T_Constant) ||
                CheckToken(Token_Class.T_String) || CheckToken(Token_Class.T_LParanthesis))
            {
                node.Children.Add(Expression());
                node.Children.Add(Arg_List_Prime());
                return node;
            }
            return null;
        }

        // Argument_List’  → , Expression Argument_List’ | ε
        Node Arg_List_Prime()
        {
            Node node = new Node("Arg_List_Prime");
            if (CheckToken(Token_Class.T_Comma))
            {
                node.Children.Add(match(Token_Class.T_Comma));
                node.Children.Add(Expression());
                node.Children.Add(Arg_List_Prime());
                return node;
            }
            return null;
        }

        // Identifier_List → identifier IL’ | ε
        Node Identifier_List()
        {
            Node node = new Node("Identifier_List");
            if (CheckToken(Token_Class.T_Identifier))
            {
                node.Children.Add(match(Token_Class.T_Identifier));
                node.Children.Add(IL_Prime());
                return node;
            }
            return null;
        }

        // IL’ → , identifier IL’ | ε
        Node IL_Prime()
        {
            Node node = new Node("IL'");
            if (CheckToken(Token_Class.T_Comma))
            {
                node.Children.Add(match(Token_Class.T_Comma));
                node.Children.Add(match(Token_Class.T_Identifier));
                node.Children.Add(IL_Prime());
                return node;
            }
            return null;
        }

        // Return_Statement → return Expression
        Node Return_Statement()
        {
            Node node = new Node("Return_Statement");
            node.Children.Add(match(Token_Class.T_Return));
            node.Children.Add(Expression());
            node.Children.Add(match(Token_Class.T_Semicolon));
            return node;
        }

        // Function_Name → identifier
        Node Function_Name() { return match(Token_Class.T_Identifier); }

        // VALIDATE OPERATORS
        Node Boolean_Operator()
        {
            Node node = new Node("Boolean_Operator");
            if (CheckToken(Token_Class.T_And)) node.Children.Add(match(Token_Class.T_And));
            else if (CheckToken(Token_Class.T_Or)) node.Children.Add(match(Token_Class.T_Or));
            return node;
        }

        Node Condition_Operator()
        {
            Node node = new Node("Condition_Operator");
            if (CheckToken(Token_Class.T_LessThanOp)) node.Children.Add(match(Token_Class.T_LessThanOp));
            else if (CheckToken(Token_Class.T_GreaterThanOp)) node.Children.Add(match(Token_Class.T_GreaterThanOp));
            else if (CheckToken(Token_Class.T_CondtionEqual)) node.Children.Add(match(Token_Class.T_CondtionEqual));
            else if (CheckToken(Token_Class.T_NotEqualOp)) node.Children.Add(match(Token_Class.T_NotEqualOp));
            return node;
        }

        Node Arithmetic_Operator()
        {
            Node node = new Node("Arithmetic_Operator");
            if (CheckToken(Token_Class.T_PlusOp)) node.Children.Add(match(Token_Class.T_PlusOp));
            else if (CheckToken(Token_Class.T_MinusOp)) node.Children.Add(match(Token_Class.T_MinusOp));
            else if (CheckToken(Token_Class.T_MultiplyOp)) node.Children.Add(match(Token_Class.T_MultiplyOp));
            else if (CheckToken(Token_Class.T_DivideOp)) node.Children.Add(match(Token_Class.T_DivideOp));
            return node;
        }

        public bool IsDataType()
        {
            if (InputPointer >= TokenStream.Count) return false;
            Token_Class type = TokenStream[InputPointer].token_type;
            return type == Token_Class.T_Int || type == Token_Class.T_Float || type == Token_Class.T_String;
        }

        public bool IsStatementStart()
        {
            if (InputPointer >= TokenStream.Count) return false;
            Token_Class type = TokenStream[InputPointer].token_type;
            return type == Token_Class.T_Comment || type == Token_Class.T_Write || type == Token_Class.T_Read ||
                   type == Token_Class.T_If || type == Token_Class.T_Repeat || type == Token_Class.T_Identifier ||
                   IsDataType();
        }

        public bool CheckToken(Token_Class type)
        {
            if (InputPointer >= TokenStream.Count) return false;
            return TokenStream[InputPointer].token_type == type;
        }

        public Node match(Token_Class ExpectedToken)
        {
            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    newNode.Children.Add(new Node(TokenStream[InputPointer - 1].lex));
                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found at " + TokenStream[InputPointer].lex + "\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }

        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}