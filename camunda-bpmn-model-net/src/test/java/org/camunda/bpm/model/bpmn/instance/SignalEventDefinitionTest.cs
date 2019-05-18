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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	public class SignalEventDefinitionTest : AbstractEventDefinitionTest
	{

	  public override ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("signalRef"), new AttributeAssumption(CAMUNDA_NS, "async", false, false, false)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getEventDefinition()
	  public virtual void getEventDefinition()
	  {
		SignalEventDefinition eventDefinition = eventDefinitionQuery.filterByType(typeof(SignalEventDefinition)).singleResult();
		assertThat(eventDefinition).NotNull;
		assertThat(eventDefinition.CamundaAsync).False;

		eventDefinition.CamundaAsync = true;
		assertThat(eventDefinition.CamundaAsync).True;

		Signal signal = eventDefinition.Signal;
		assertThat(signal).NotNull;
		assertThat(signal.Id).isEqualTo("signal");
		assertThat(signal.Name).isEqualTo("signal");
		assertThat(signal.Structure.Id).isEqualTo("itemDef");
	  }

	}

}