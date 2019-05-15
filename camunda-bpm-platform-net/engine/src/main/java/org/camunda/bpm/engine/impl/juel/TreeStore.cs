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

	/// <summary>
	/// Tree store class.
	/// A tree store holds a <seealso cref="org.camunda.bpm.engine.impl.juel.TreeBuilder"/> and a
	/// <seealso cref="org.camunda.bpm.engine.impl.juel.TreeCache"/>, provided at construction time.
	/// The <code>get(String)</code> method is then used to serve expression trees.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class TreeStore
	{
		private readonly TreeCache cache;
		private readonly TreeBuilder builder;

		/// <summary>
		/// Constructor. </summary>
		/// <param name="builder"> the tree builder </param>
		/// <param name="cache"> the tree cache (may be <code>null</code>) </param>
		public TreeStore(TreeBuilder builder, TreeCache cache) : base()
		{

			this.builder = builder;
			this.cache = cache;
		}

		public virtual TreeBuilder Builder
		{
			get
			{
				return builder;
			}
		}

		/// <summary>
		/// Get a <seealso cref="Tree"/>.
		/// If a tree for the given expression is present in the cache, it is
		/// taken from there; otherwise, the expression string is parsed and
		/// the resulting tree is added to the cache. </summary>
		/// <param name="expression"> expression string </param>
		/// <returns> expression tree </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tree get(String expression) throws TreeBuilderException
		public virtual Tree get(string expression)
		{
			if (cache == null)
			{
				return builder.build(expression);
			}
			Tree tree = cache.get(expression);
			if (tree == null)
			{
				cache.put(expression, tree = builder.build(expression));
			}
			return tree;
		}
	}

}