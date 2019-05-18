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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.test.AbstractModelElementInstanceTest.modelInstance;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ServiceTaskTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(Task), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("implementation", false, false, "##WebService"), new AttributeAssumption("operationRef"), new AttributeAssumption(CAMUNDA_NS, "class"), new AttributeAssumption(CAMUNDA_NS, "delegateExpression"), new AttributeAssumption(CAMUNDA_NS, "expression"), new AttributeAssumption(CAMUNDA_NS, "resultVariable"), new AttributeAssumption(CAMUNDA_NS, "topic"), new AttributeAssumption(CAMUNDA_NS, "type"), new AttributeAssumption(CAMUNDA_NS, "taskPriority")
		   );
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskPriority()
	  public virtual void testCamundaTaskPriority()
	  {
		//given
		ServiceTask service = modelInstance.newInstance(typeof(ServiceTask));
		assertThat(service.CamundaTaskPriority).Null;
		//when
		service.CamundaTaskPriority = BpmnTestConstants.TEST_PROCESS_TASK_PRIORITY;
		//then
		assertThat(service.CamundaTaskPriority).isEqualTo(BpmnTestConstants.TEST_PROCESS_TASK_PRIORITY);
	  }
	}

}