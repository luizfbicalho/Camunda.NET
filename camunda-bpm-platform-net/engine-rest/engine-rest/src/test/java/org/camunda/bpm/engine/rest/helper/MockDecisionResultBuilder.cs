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
	public class MockDecisionResultBuilder
	{

	  protected internal IList<DmnDecisionResultEntries> entries = new List<DmnDecisionResultEntries>();

	  public virtual MockDecisionResultEntriesBuilder resultEntries()
	  {
		return new MockDecisionResultEntriesBuilder(this);
	  }

	  public virtual void addResultEntries(DmnDecisionResultEntries resultEntries)
	  {
		entries.Add(resultEntries);
	  }

	  public virtual DmnDecisionResult build()
	  {
		SimpleDecisionResult decisionTableResult = new SimpleDecisionResult(this);
		decisionTableResult.AddRange(entries);
		return decisionTableResult;
	  }

	  protected internal class SimpleDecisionResult : List<DmnDecisionResultEntries>, DmnDecisionResult
	  {
		  private readonly MockDecisionResultBuilder outerInstance;

		  public SimpleDecisionResult(MockDecisionResultBuilder outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		internal const long serialVersionUID = 1L;

		public override DmnDecisionResultEntries FirstResult
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override DmnDecisionResultEntries SingleResult
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

		public override IList<T> collectEntries<T>(string outputName)
		{
		  throw new System.NotSupportedException();
		}

		public override IList<IDictionary<string, object>> ResultList
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

		public override T getSingleEntryTyped<T>() where T : org.camunda.bpm.engine.variable.value.TypedValue
		{
			get
			{
			  throw new System.NotSupportedException();
			}
		}

	  }
	}

}