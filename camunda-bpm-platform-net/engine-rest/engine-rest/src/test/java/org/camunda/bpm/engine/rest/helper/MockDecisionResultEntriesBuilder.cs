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
namespace org.camunda.bpm.engine.rest.helper
{

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnDecisionResultEntries = org.camunda.bpm.dmn.engine.DmnDecisionResultEntries;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class MockDecisionResultEntriesBuilder
	{

	  protected internal readonly MockDecisionResultBuilder mockDecisionResultBuilder;

	  protected internal IDictionary<string, TypedValue> entries = new Dictionary<string, TypedValue>();

	  public MockDecisionResultEntriesBuilder(MockDecisionResultBuilder mockDecisionResultBuilder)
	  {
		this.mockDecisionResultBuilder = mockDecisionResultBuilder;
	  }

	  public virtual MockDecisionResultEntriesBuilder entry(string key, TypedValue value)
	  {
		entries[key] = value;
		return this;
	  }

	  public virtual MockDecisionResultBuilder endResultEntries()
	  {
		SimpleDecisionResultEntries resultEntires = new SimpleDecisionResultEntries(entries);

		mockDecisionResultBuilder.addResultEntries(resultEntires);

		return mockDecisionResultBuilder;
	  }

	  public virtual MockDecisionResultEntriesBuilder resultEntries()
	  {
		return endResultEntries().resultEntries();
	  }

	  public virtual DmnDecisionResult build()
	  {
		return endResultEntries().build();
	  }

	  protected internal class SimpleDecisionResultEntries : Dictionary<string, object>, DmnDecisionResultEntries
	  {

		internal const long serialVersionUID = 1L;

		protected internal readonly IDictionary<string, TypedValue> typedEntries;

		public SimpleDecisionResultEntries(IDictionary<string, TypedValue> entries) : base(asEntryMap(entries))
		{

		  this.typedEntries = entries;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private static java.util.Map<? extends String, ?> asEntryMap(java.util.Map<String, org.camunda.bpm.engine.variable.value.TypedValue> typedValueMap)
		internal static IDictionary<string, ?> asEntryMap(IDictionary<string, TypedValue> typedValueMap)
		{
		  IDictionary<string, object> entryMap = new Dictionary<string, object>();

		  foreach (KeyValuePair<string, TypedValue> entry in typedValueMap.SetOfKeyValuePairs())
		  {
			entryMap[entry.Key] = entry.Value.Value;
		  }

		  return entryMap;
		}

		public override T getFirstEntry<T>()
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override T getSingleEntry<T>()
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T> T getEntry(String name)
		public override T getEntry<T>(string name)
		{
		  return (T) typedEntries[name].Value;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public <T extends org.camunda.bpm.engine.variable.value.TypedValue> T getEntryTyped(String name)
		public override T getEntryTyped<T>(string name) where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
		  return (T) typedEntries[name];
		}

		public override T getFirstEntryTyped<T>() where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override T getSingleEntryTyped<T>() where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override IDictionary<string, object> EntryMap
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override IDictionary<string, TypedValue> EntryMapTyped
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

	  }

	}

}