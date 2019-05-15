using System;
using System.Collections.Generic;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.juel
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Builder.Feature.METHOD_INVOCATIONS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Builder.Feature.NULL_PROPERTIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.COLON;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.COMMA;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.EMPTY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.END_EVAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.EOF;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.FALSE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.FLOAT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.IDENTIFIER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.INTEGER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.LPAREN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.MINUS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.NOT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.NULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.QUESTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.RBRACK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.RPAREN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.START_EVAL_DEFERRED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.START_EVAL_DYNAMIC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.STRING;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.TEXT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.juel.Scanner.Symbol.TRUE;


	using Feature = org.camunda.bpm.engine.impl.juel.Builder.Feature;
	using ScanException = org.camunda.bpm.engine.impl.juel.Scanner.ScanException;
	using Symbol = org.camunda.bpm.engine.impl.juel.Scanner.Symbol;
	using Token = org.camunda.bpm.engine.impl.juel.Scanner.Token;


	/// <summary>
	/// Handcrafted top-down parser.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class Parser
	{
		/// <summary>
		/// Parse exception type
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public static class ParseException extends Exception
		public class ParseException : Exception
		{
			internal readonly int position;
			internal readonly string encountered;
			internal readonly string expected;
			public ParseException(int position, string encountered, string expected) : base(LocalMessages.get("error.parse", position, encountered, expected))
			{
				this.position = position;
				this.encountered = encountered;
				this.expected = expected;
			}
		}

		/// <summary>
		/// Token type (used to store lookahead)
		/// </summary>
		private sealed class LookaheadToken
		{
			internal readonly Token token;
			internal readonly int position;

			internal LookaheadToken(Token token, int position)
			{
				this.token = token;
				this.position = position;
			}
		}

		public enum ExtensionPoint
		{
			OR,
			AND,
			EQ,
			CMP,
			ADD,
			MUL,
			UNARY,
			LITERAL
		}

		/// <summary>
		/// Provide limited support for syntax extensions.
		/// </summary>
		public abstract class ExtensionHandler
		{
			internal readonly ExtensionPoint point;

			public ExtensionHandler(ExtensionPoint point)
			{
				this.point = point;
			}

			/// <returns> the extension point specifying where this syntax extension is active </returns>
			public virtual ExtensionPoint ExtensionPoint
			{
				get
				{
					return point;
				}
			}

			/// <summary>
			/// Called by the parser if it handles a extended token associated with this handler
			/// at the appropriate extension point. </summary>
			/// <param name="children"> </param>
			/// <returns> abstract syntax tree node </returns>
			public abstract AstNode createAstNode(params AstNode[] children);
		}

		private static readonly string EXPR_FIRST = IDENTIFIER + "|" + STRING + "|" + FLOAT + "|" + INTEGER + "|" + TRUE + "|" + FALSE + "|" + NULL + "|" + MINUS + "|" + NOT + "|" + EMPTY + "|" + LPAREN;

		protected internal readonly Builder context;
		protected internal readonly Scanner scanner;

		private IList<IdentifierNode> identifiers = Collections.emptyList();
		private IList<FunctionNode> functions = Collections.emptyList();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private IList<LookaheadToken> lookahead_Renamed = Collections.emptyList();

		private Token token; // current token
		private int position; // current token's position

		protected internal IDictionary<Scanner.ExtensionToken, ExtensionHandler> extensions = Collections.emptyMap();

		public Parser(Builder context, string input)
		{
			this.context = context;
			this.scanner = createScanner(input);
		}

		protected internal virtual Scanner createScanner(string expression)
		{
			return new Scanner(expression);
		}

		public virtual void putExtensionHandler(Scanner.ExtensionToken token, ExtensionHandler extension)
		{
			if (extensions.Count == 0)
			{
				extensions = new Dictionary<Scanner.ExtensionToken, ExtensionHandler>(16);
			}
			extensions[token] = extension;
		}

		protected internal virtual ExtensionHandler getExtensionHandler(Token token)
		{
			return extensions[token];
		}

		/// <summary>
		/// Parse an integer literal. </summary>
		/// <param name="string"> string to parse </param>
		/// <returns> <code>Long.valueOf(string)</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Number parseInteger(String string) throws ParseException
		protected internal virtual Number parseInteger(string @string)
		{
			try
			{
				return Convert.ToInt64(@string);
			}
			catch (System.FormatException)
			{
				fail(INTEGER);
				return null;
			}
		}

		/// <summary>
		/// Parse a floating point literal. </summary>
		/// <param name="string"> string to parse </param>
		/// <returns> <code>Double.valueOf(string)</code> </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Number parseFloat(String string) throws ParseException
		protected internal virtual Number parseFloat(string @string)
		{
			try
			{
				return Convert.ToDouble(@string);
			}
			catch (System.FormatException)
			{
				fail(FLOAT);
				return null;
			}
		}

		protected internal virtual AstBinary createAstBinary(AstNode left, AstNode right, AstBinary.Operator @operator)
		{
			return new AstBinary(left, right, @operator);
		}

		protected internal virtual AstBracket createAstBracket(AstNode @base, AstNode property, bool lvalue, bool strict)
		{
			return new AstBracket(@base, property, lvalue, strict);
		}

		protected internal virtual AstChoice createAstChoice(AstNode question, AstNode yes, AstNode no)
		{
			return new AstChoice(question, yes, no);
		}

		protected internal virtual AstComposite createAstComposite(IList<AstNode> nodes)
		{
			return new AstComposite(nodes);
		}

		protected internal virtual AstDot createAstDot(AstNode @base, string property, bool lvalue)
		{
			return new AstDot(@base, property, lvalue);
		}

		protected internal virtual AstFunction createAstFunction(string name, int index, AstParameters @params)
		{
			return new AstFunction(name, index, @params, context.isEnabled(Feature.VARARGS));
		}

		protected internal virtual AstIdentifier createAstIdentifier(string name, int index)
		{
			return new AstIdentifier(name, index);
		}

		protected internal virtual AstMethod createAstMethod(AstProperty property, AstParameters @params)
		{
			return new AstMethod(property, @params);
		}

		protected internal virtual AstUnary createAstUnary(AstNode child, AstUnary.Operator @operator)
		{
			return new AstUnary(child, @operator);
		}

		protected internal IList<FunctionNode> Functions
		{
			get
			{
				return functions;
			}
		}

		protected internal IList<IdentifierNode> Identifiers
		{
			get
			{
				return identifiers;
			}
		}

		protected internal Token Token
		{
			get
			{
				return token;
			}
		}

		/// <summary>
		/// throw exception
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void fail(String expected) throws ParseException
		protected internal virtual void fail(string expected)
		{
			throw new ParseException(position, "'" + token.Image + "'", expected);
		}

		/// <summary>
		/// throw exception
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void fail(org.camunda.bpm.engine.impl.juel.Scanner.Symbol expected) throws ParseException
		protected internal virtual void fail(Symbol expected)
		{
			fail(expected.ToString());
		}

		/// <summary>
		/// get lookahead symbol.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final org.camunda.bpm.engine.impl.juel.Scanner.Token lookahead(int index) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal Token lookahead(int index)
		{
			if (lookahead_Renamed.Count == 0)
			{
				lookahead_Renamed = new LinkedList<LookaheadToken>();
			}
			while (index >= lookahead_Renamed.Count)
			{
				lookahead_Renamed.Add(new LookaheadToken(scanner.next(), scanner.Position));
			}
			return lookahead_Renamed[index].token;
		}

		/// <summary>
		/// consume current token (get next token). </summary>
		/// <returns> the consumed token (which was the current token when calling this method) </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final org.camunda.bpm.engine.impl.juel.Scanner.Token consumeToken() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal Token consumeToken()
		{
			Token result = token;
			if (lookahead_Renamed.Count == 0)
			{
				token = scanner.next();
				position = scanner.Position;
			}
			else
			{
				LookaheadToken next = lookahead_Renamed.RemoveAt(0);
				token = next.token;
				position = next.position;
			}
			return result;
		}

		/// <summary>
		/// consume current token (get next token); throw exception if the current token doesn't
		/// match the expected symbol.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final org.camunda.bpm.engine.impl.juel.Scanner.Token consumeToken(org.camunda.bpm.engine.impl.juel.Scanner.Symbol expected) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal Token consumeToken(Symbol expected)
		{
			if (token.Symbol != expected)
			{
				fail(expected);
			}
			return consumeToken();
		}

		/// <summary>
		/// tree := text? ((dynamic text?)+ | (deferred text?)+)? 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tree tree() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		public virtual Tree tree()
		{
			consumeToken();
			AstNode t = text();
			if (token.Symbol == EOF)
			{
				if (t == null)
				{
					t = new AstText("");
				}
				return new Tree(t, functions, identifiers, false);
			}
			AstEval e = eval();
			if (token.Symbol == EOF && t == null)
			{
				return new Tree(e, functions, identifiers, e.Deferred);
			}
			List<AstNode> list = new List<AstNode>();
			if (t != null)
			{
				list.Add(t);
			}
			list.Add(e);
			t = text();
			if (t != null)
			{
				list.Add(t);
			}
			while (token.Symbol != EOF)
			{
				if (e.Deferred)
				{
					list.Add(eval(true, true));
				}
				else
				{
					list.Add(eval(true, false));
				}
				t = text();
				if (t != null)
				{
					list.Add(t);
				}
			}
			return new Tree(createAstComposite(list), functions, identifiers, e.Deferred);
		}

		/// <summary>
		/// text := &lt;TEXT&gt;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode text() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode text()
		{
			AstNode v = null;
			if (token.Symbol == TEXT)
			{
				v = new AstText(token.Image);
				consumeToken();
			}
			return v;
		}

		/// <summary>
		/// eval := dynamic | deferred
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstEval eval() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstEval eval()
		{
			AstEval e = eval(false, false);
			if (e == null)
			{
				e = eval(false, true);
				if (e == null)
				{
					fail(START_EVAL_DEFERRED + "|" + START_EVAL_DYNAMIC);
				}
			}
			return e;
		}

		/// <summary>
		/// dynmamic := &lt;START_EVAL_DYNAMIC&gt; expr &lt;END_EVAL&gt;
		/// deferred := &lt;START_EVAL_DEFERRED&gt; expr &lt;END_EVAL&gt;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstEval eval(boolean required, boolean deferred) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstEval eval(bool required, bool deferred)
		{
			AstEval v = null;
			Symbol start_eval = deferred ? START_EVAL_DEFERRED : START_EVAL_DYNAMIC;
			if (token.Symbol == start_eval)
			{
				consumeToken();
				v = new AstEval(expr(true), deferred);
				consumeToken(END_EVAL);
			}
			else if (required)
			{
				fail(start_eval);
			}
			return v;
		}

		/// <summary>
		/// expr := or (&lt;QUESTION&gt; expr &lt;COLON&gt; expr)?
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode expr(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode expr(bool required)
		{
			AstNode v = or(required);
			if (v == null)
			{
				return null;
			}
			if (token.Symbol == QUESTION)
			{
				consumeToken();
				AstNode a = expr(true);
				consumeToken(COLON);
				AstNode b = expr(true);
				v = createAstChoice(v, a, b);
			}
			return v;
		}

		/// <summary>
		/// or := and (&lt;OR&gt; and)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode or(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode or(bool required)
		{
			AstNode v = and(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case OR:
						consumeToken();
						v = createAstBinary(v, and(true), AstBinary.OR);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.OR)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, and(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// and := eq (&lt;AND&gt; eq)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode and(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode and(bool required)
		{
			AstNode v = eq(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case AND:
						consumeToken();
						v = createAstBinary(v, eq(true), AstBinary.AND);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.AND)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, eq(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// eq := cmp (&lt;EQ&gt; cmp | &lt;NE&gt; cmp)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode eq(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode eq(bool required)
		{
			AstNode v = cmp(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case EQ:
						consumeToken();
						v = createAstBinary(v, cmp(true), AstBinary.EQ);
						break;
					case NE:
						consumeToken();
						v = createAstBinary(v, cmp(true), AstBinary.NE);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.EQ)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, cmp(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// cmp := add (&lt;LT&gt; add | &lt;LE&gt; add | &lt;GE&gt; add | &lt;GT&gt; add)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode cmp(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode cmp(bool required)
		{
			AstNode v = add(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case LT:
						consumeToken();
						v = createAstBinary(v, add(true), AstBinary.LT);
						break;
					case LE:
						consumeToken();
						v = createAstBinary(v, add(true), AstBinary.LE);
						break;
					case GE:
						consumeToken();
						v = createAstBinary(v, add(true), AstBinary.GE);
						break;
					case GT:
						consumeToken();
						v = createAstBinary(v, add(true), AstBinary.GT);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.CMP)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, add(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// add := add (&lt;PLUS&gt; mul | &lt;MINUS&gt; mul)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode add(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode add(bool required)
		{
			AstNode v = mul(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case PLUS:
						consumeToken();
						v = createAstBinary(v, mul(true), AstBinary.ADD);
						break;
					case MINUS:
						consumeToken();
						v = createAstBinary(v, mul(true), AstBinary.SUB);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.ADD)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, mul(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// mul := unary (&lt;MUL&gt; unary | &lt;DIV&gt; unary | &lt;MOD&gt; unary)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode mul(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode mul(bool required)
		{
			AstNode v = unary(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case MUL:
						consumeToken();
						v = createAstBinary(v, unary(true), AstBinary.MUL);
						break;
					case DIV:
						consumeToken();
						v = createAstBinary(v, unary(true), AstBinary.DIV);
						break;
					case MOD:
						consumeToken();
						v = createAstBinary(v, unary(true), AstBinary.MOD);
						break;
					case EXTENSION:
						if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.MUL)
						{
							v = getExtensionHandler(consumeToken()).createAstNode(v, unary(true));
							break;
						}
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// unary := &lt;NOT&gt; unary | &lt;MINUS&gt; unary | &lt;EMPTY&gt; unary | value
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode unary(boolean required) throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode unary(bool required)
		{
			AstNode v = null;
			switch (token.Symbol)
			{
				case NOT:
					consumeToken();
					v = createAstUnary(unary(true), AstUnary.NOT);
					break;
				case MINUS:
					consumeToken();
					v = createAstUnary(unary(true), AstUnary.NEG);
					break;
				case EMPTY:
					consumeToken();
					v = createAstUnary(unary(true), AstUnary.EMPTY);
					break;
				case EXTENSION:
					if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.UNARY)
					{
						v = getExtensionHandler(consumeToken()).createAstNode(unary(true));
						break;
					}
				default:
					v = value();
				break;
			}
			if (v == null && required)
			{
				fail(EXPR_FIRST);
			}
			return v;
		}

		/// <summary>
		/// value := (nonliteral | literal) (&lt;DOT&gt; &lt;IDENTIFIER&gt; | &lt;LBRACK&gt; expr &lt;RBRACK&gt;)*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode value() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode value()
		{
			bool lvalue = true;
			AstNode v = nonliteral();
			if (v == null)
			{
				v = literal();
				if (v == null)
				{
					return null;
				}
				lvalue = false;
			}
			while (true)
			{
				switch (token.Symbol)
				{
					case DOT:
						consumeToken();
						string name = consumeToken(IDENTIFIER).Image;
						AstDot dot = createAstDot(v, name, lvalue);
						if (token.Symbol == LPAREN && context.isEnabled(METHOD_INVOCATIONS))
						{
							v = createAstMethod(dot, @params());
						}
						else
						{
							v = dot;
						}
						break;
					case LBRACK:
						consumeToken();
						AstNode property = expr(true);
						bool strict = !context.isEnabled(NULL_PROPERTIES);
						consumeToken(RBRACK);
						AstBracket bracket = createAstBracket(v, property, lvalue, strict);
						if (token.Symbol == LPAREN && context.isEnabled(METHOD_INVOCATIONS))
						{
							v = createAstMethod(bracket, @params());
						}
						else
						{
							v = bracket;
						}
						break;
					default:
						return v;
				}
			}
		}

		/// <summary>
		/// nonliteral := &lt;IDENTIFIER&gt; | function | &lt;LPAREN&gt; expr &lt;RPAREN&gt;
		/// function   := (&lt;IDENTIFIER&gt; &lt;COLON&gt;)? &lt;IDENTIFIER&gt; &lt;LPAREN&gt; list? &lt;RPAREN&gt;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode nonliteral() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode nonliteral()
		{
			AstNode v = null;
			switch (token.Symbol)
			{
				case IDENTIFIER:
					string name = consumeToken().Image;
					if (token.Symbol == COLON && lookahead(0).Symbol == IDENTIFIER && lookahead(1).Symbol == LPAREN)
					{ // ns:f(...)
						consumeToken();
						name += ":" + token.Image;
						consumeToken();
					}
					if (token.Symbol == LPAREN)
					{ // function
						v = function(name, @params());
					}
					else
					{ // identifier
						v = identifier(name);
					}
					break;
				case LPAREN:
					consumeToken();
					v = expr(true);
					consumeToken(RPAREN);
					v = new AstNested(v);
					break;
			}
			return v;
		}

		/// <summary>
		/// params := &lt;LPAREN&gt; (expr (&lt;COMMA&gt; expr)*)? &lt;RPAREN&gt;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstParameters params() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstParameters @params()
		{
			consumeToken(LPAREN);
			IList<AstNode> l = Collections.emptyList();
			AstNode v = expr(false);
			if (v != null)
			{
				l = new List<AstNode>();
				l.Add(v);
				while (token.Symbol == COMMA)
				{
					consumeToken();
					l.Add(expr(true));
				}
			}
			consumeToken(RPAREN);
			return new AstParameters(l);
		}

		/// <summary>
		/// literal := &lt;TRUE&gt; | &lt;FALSE&gt; | &lt;STRING&gt; | &lt;INTEGER&gt; | &lt;FLOAT&gt; | &lt;NULL&gt;
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AstNode literal() throws org.camunda.bpm.engine.impl.juel.Scanner.ScanException, ParseException
		protected internal virtual AstNode literal()
		{
			AstNode v = null;
			switch (token.Symbol)
			{
				case TRUE:
					v = new AstBoolean(true);
					consumeToken();
					break;
				case FALSE:
					v = new AstBoolean(false);
					consumeToken();
					break;
				case STRING:
					v = new AstString(token.Image);
					consumeToken();
					break;
				case INTEGER:
					v = new AstNumber(parseInteger(token.Image));
					consumeToken();
					break;
				case FLOAT:
					v = new AstNumber(parseFloat(token.Image));
					consumeToken();
					break;
				case NULL:
					v = new AstNull();
					consumeToken();
					break;
				case EXTENSION:
					if (getExtensionHandler(token).ExtensionPoint == ExtensionPoint.LITERAL)
					{
						v = getExtensionHandler(consumeToken()).createAstNode();
						break;
					}
			}
			return v;
		}

		protected internal AstFunction function(string name, AstParameters @params)
		{
			if (functions.Count == 0)
			{
				functions = new List<FunctionNode>(4);
			}
			AstFunction function = createAstFunction(name, functions.Count, @params);
			functions.Add(function);
			return function;
		}

		protected internal AstIdentifier identifier(string name)
		{
			if (identifiers.Count == 0)
			{
				identifiers = new List<IdentifierNode>(4);
			}
			AstIdentifier identifier = createAstIdentifier(name, identifiers.Count);
			identifiers.Add(identifier);
			return identifier;
		}
	}
}