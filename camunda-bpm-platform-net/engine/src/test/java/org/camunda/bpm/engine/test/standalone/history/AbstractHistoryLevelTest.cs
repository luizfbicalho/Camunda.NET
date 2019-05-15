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
namespace org.camunda.bpm.engine.test.standalone.history
{
	using AbstractHistoryLevel = org.camunda.bpm.engine.impl.history.AbstractHistoryLevel;
	using HistoryEventType = org.camunda.bpm.engine.impl.history.@event.HistoryEventType;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	public class AbstractHistoryLevelTest
	{


	  public class MyHistoryLevel : AbstractHistoryLevel
	  {

		public override int Id
		{
			get
			{
			  return 4711;
			}
		}

		public override string Name
		{
			get
			{
			  return "myName";
			}
		}

		public override bool isHistoryEventProduced(HistoryEventType eventType, object entity)
		{
		  return false;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensureCorrectToString()
	  public virtual void ensureCorrectToString()
	  {
		Assert.assertThat((new MyHistoryLevel()).ToString(), CoreMatchers.@is("MyHistoryLevel(name=myName, id=4711)"));
	  }
	}
}