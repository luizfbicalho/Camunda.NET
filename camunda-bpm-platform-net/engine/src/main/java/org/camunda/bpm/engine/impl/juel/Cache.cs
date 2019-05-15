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



	/// <summary>
	/// Simple (thread-safe) LRU cache.
	/// After the cache size reached a certain limit, the least recently used entry is removed,
	/// when adding a new entry.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public sealed class Cache : TreeCache
	{
	  private readonly IDictionary<string, Tree> primary;
	  private readonly IDictionary<string, Tree> secondary;

	  /// <summary>
	  /// Constructor.
	  /// Use a <seealso cref="WeakHashMap"/> as secondary map. </summary>
	  /// <param name="size"> maximum primary cache size </param>
		public Cache(int size) : this(size, new WeakHashMap<string, Tree>())
		{
		}

		/// <summary>
		/// Constructor.
		/// If the least recently used entry is removed from the primary cache, it is added to
		/// the secondary map. </summary>
		/// <param name="size"> maximum primary cache size </param>
		/// <param name="secondary"> the secondary map (may be <code>null</code>) </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("serial") public Cache(final int size, java.util.Map<String,Tree> secondary)
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public Cache(int size, IDictionary<string, Tree> secondary)
		{
			this.primary = Collections.synchronizedMap(new LinkedHashMapAnonymousInnerClass(this, secondary));
			this.secondary = secondary == null ? null : Collections.synchronizedMap(secondary);
		}

		private class LinkedHashMapAnonymousInnerClass : LinkedHashMap<string, Tree>
		{
			private readonly Cache outerInstance;

			private IDictionary<string, Tree> secondary;

			public LinkedHashMapAnonymousInnerClass(Cache outerInstance, IDictionary<string, Tree> secondary) : base(16, 0.75f, true)
			{
				this.outerInstance = outerInstance;
				this.secondary = secondary;
			}

			protected internal override bool removeEldestEntry(KeyValuePair<string, Tree> eldest)
			{
				if (size() > size)
				{
					if (outerInstance.secondary != null)
					{ // move to secondary cache
						outerInstance.secondary.put(eldest.Key, eldest.Value);
					}
					return true;
				}
				return false;
			}
		}

		public Tree get(string expression)
		{
			if (secondary == null)
			{
				return primary[expression];
			}
			else
			{
				Tree tree = primary[expression];
				if (tree == null)
				{
					tree = secondary[expression];
				}
				return tree;
			}
		}

		public void put(string expression, Tree tree)
		{
			primary[expression] = tree;
		}
	}

}