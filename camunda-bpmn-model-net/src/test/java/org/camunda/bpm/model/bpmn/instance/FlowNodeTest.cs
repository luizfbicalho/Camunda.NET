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
	using Incoming = org.camunda.bpm.model.bpmn.impl.instance.Incoming;
	using Outgoing = org.camunda.bpm.model.bpmn.impl.instance.Outgoing;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FlowNodeTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(FlowElement), true);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Incoming)), new ChildElementAssumption(typeof(Outgoing))
		   );
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption(CAMUNDA_NS, "asyncAfter", false, false, false), new AttributeAssumption(CAMUNDA_NS, "asyncBefore", false, false, false), new AttributeAssumption(CAMUNDA_NS, "exclusive", false, false, true), new AttributeAssumption(CAMUNDA_NS, "jobPriority")
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateIncomingOutgoingChildElements()
	  public virtual void testUpdateIncomingOutgoingChildElements()
	  {
		BpmnModelInstance modelInstance = Bpmn.createProcess().startEvent().userTask("test").endEvent().done();

		// save current incoming and outgoing sequence flows
		UserTask userTask = modelInstance.getModelElementById("test");
		ICollection<SequenceFlow> incoming = userTask.Incoming;
		ICollection<SequenceFlow> outgoing = userTask.Outgoing;

		// create a new service task
		ServiceTask serviceTask = modelInstance.newInstance(typeof(ServiceTask));
		serviceTask.Id = "new";

		// replace the user task with the new service task
		userTask.replaceWithElement(serviceTask);

		// assert that the new service task has the same incoming and outgoing sequence flows
		assertThat(serviceTask.Incoming).containsExactlyElementsOf(incoming);
		assertThat(serviceTask.Outgoing).containsExactlyElementsOf(outgoing);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaAsyncBefore()
	  public virtual void testCamundaAsyncBefore()
	  {
		Task task = modelInstance.newInstance(typeof(Task));
		assertThat(task.CamundaAsyncBefore).False;

		task.CamundaAsyncBefore = true;
		assertThat(task.CamundaAsyncBefore).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaAsyncAfter()
	  public virtual void testCamundaAsyncAfter()
	  {
		Task task = modelInstance.newInstance(typeof(Task));
		assertThat(task.CamundaAsyncAfter).False;

		task.CamundaAsyncAfter = true;
		assertThat(task.CamundaAsyncAfter).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaAsyncAfterAndBefore()
	  public virtual void testCamundaAsyncAfterAndBefore()
	  {
		Task task = modelInstance.newInstance(typeof(Task));

		assertThat(task.CamundaAsyncAfter).False;
		assertThat(task.CamundaAsyncBefore).False;

		task.CamundaAsyncBefore = true;

		assertThat(task.CamundaAsyncAfter).False;
		assertThat(task.CamundaAsyncBefore).True;

		task.CamundaAsyncAfter = true;

		assertThat(task.CamundaAsyncAfter).True;
		assertThat(task.CamundaAsyncBefore).True;

		task.CamundaAsyncBefore = false;

		assertThat(task.CamundaAsyncAfter).True;
		assertThat(task.CamundaAsyncBefore).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaExclusive()
	  public virtual void testCamundaExclusive()
	  {
		Task task = modelInstance.newInstance(typeof(Task));

		assertThat(task.CamundaExclusive).True;

		task.CamundaExclusive = false;

		assertThat(task.CamundaExclusive).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaJobPriority()
	  public virtual void testCamundaJobPriority()
	  {
		Task task = modelInstance.newInstance(typeof(Task));
		assertThat(task.CamundaJobPriority).Null;

		task.CamundaJobPriority = "15";

		assertThat(task.CamundaJobPriority).isEqualTo("15");
	  }
	}

}