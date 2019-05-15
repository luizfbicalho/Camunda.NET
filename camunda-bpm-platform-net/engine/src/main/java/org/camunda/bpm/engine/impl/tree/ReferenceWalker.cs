using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
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
namespace org.camunda.bpm.engine.impl.tree
{

	/// <summary>
	/// <para>A walker for walking through an object reference structure (e.g. an execution tree).
	/// Any visited element can have any number of following elements. The elements are visited
	/// with a breadth-first approach: The walker maintains a list of next elements to which it adds
	/// a new elements at the end whenever it has visited an element. The walker stops when it encounters
	/// an element that fulfills the given <seealso cref="WalkCondition"/>.
	/// 
	/// </para>
	/// <para>Subclasses define the type of objects and provide the walking behavior.
	/// 
	/// @author Thorben Lindhauer
	/// </para>
	/// </summary>
	public abstract class ReferenceWalker<T>
	{

	  protected internal IList<T> currentElements;

	  protected internal IList<TreeVisitor<T>> preVisitor = new List<TreeVisitor<T>>();

	  protected internal IList<TreeVisitor<T>> postVisitor = new List<TreeVisitor<T>>();

	  protected internal abstract ICollection<T> nextElements();

	  public ReferenceWalker(T initialElement)
	  {
		currentElements = new LinkedList<T>();
		currentElements.Add(initialElement);
	  }

	  public ReferenceWalker(IList<T> initialElements)
	  {
		currentElements = new LinkedList<T>(initialElements);
	  }

	  public virtual ReferenceWalker<T> addPreVisitor(TreeVisitor<T> collector)
	  {
		this.preVisitor.Add(collector);
		return this;
	  }

	  public virtual ReferenceWalker<T> addPostVisitor(TreeVisitor<T> collector)
	  {
		this.postVisitor.Add(collector);
		return this;
	  }

	  public virtual T walkWhile()
	  {
		return walkWhile(new ReferenceWalker.NullCondition<T>());
	  }

	  public virtual T walkUntil()
	  {
		return walkUntil(new ReferenceWalker.NullCondition<T>());
	  }

	  public virtual T walkWhile(ReferenceWalker.WalkCondition<T> condition)
	  {
		while (!condition.isFulfilled(CurrentElement))
		{
		  foreach (TreeVisitor<T> collector in preVisitor)
		  {
			collector.visit(CurrentElement);
		  }

		  ((IList<T>)currentElements).AddRange(nextElements());
		  currentElements.RemoveAt(0);

		  foreach (TreeVisitor<T> collector in postVisitor)
		  {
			collector.visit(CurrentElement);
		  }
		}
		return CurrentElement;
	  }

	  public virtual T walkUntil(ReferenceWalker.WalkCondition<T> condition)
	  {
		do
		{
		  foreach (TreeVisitor<T> collector in preVisitor)
		  {
			collector.visit(CurrentElement);
		  }

		  ((IList<T>)currentElements).AddRange(nextElements());
		  currentElements.RemoveAt(0);

		  foreach (TreeVisitor<T> collector in postVisitor)
		  {
			collector.visit(CurrentElement);
		  }
		} while (!condition.isFulfilled(CurrentElement));
		return CurrentElement;
	  }

	  public virtual T CurrentElement
	  {
		  get
		  {
			return currentElements.Count == 0 ? default(T) : currentElements[0];
		  }
	  }

	  public interface WalkCondition<S>
	  {
		bool isFulfilled(S element);
	  }

	  public class NullCondition<S> : ReferenceWalker.WalkCondition<S>
	  {

		public virtual bool isFulfilled(S element)
		{
		  return element == default(S);
		}

		public static ReferenceWalker.WalkCondition<S> notNull<S>()
		{
		  return new NullCondition<S>();
		}

	  }


	}

}