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
namespace org.camunda.bpm.model.bpmn.instance
{
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	public class TimerEventDefinitionTest : AbstractEventDefinitionTest
	{

	  public override ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(TimeDate), 0, 1), new ChildElementAssumption(typeof(TimeDuration), 0, 1), new ChildElementAssumption(typeof(TimeCycle), 0, 1)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getElementDefinition()
	  public virtual void getElementDefinition()
	  {
		IList<TimerEventDefinition> eventDefinitions = eventDefinitionQuery.filterByType(typeof(TimerEventDefinition)).list();
		assertThat(eventDefinitions).hasSize(3);
		foreach (TimerEventDefinition eventDefinition in eventDefinitions)
		{
		  string id = eventDefinition.Id;
		  string textContent = null;
		  if (id.Equals("date"))
		  {
			textContent = eventDefinition.TimeDate.TextContent;
		  }
		  else if (id.Equals("duration"))
		  {
			textContent = eventDefinition.TimeDuration.TextContent;
		  }
		  else if (id.Equals("cycle"))
		  {
			textContent = eventDefinition.TimeCycle.TextContent;
		  }

		  assertThat(textContent).isEqualTo("${test}");
		}
	  }

	}

}