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
namespace org.camunda.bpm.model.bpmn.builder.di
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.END_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.SEQUENCE_FLOW_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.START_EVENT_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.USER_TASK_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using BpmnEdge = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnEdge;
	using After = org.junit.After;
	using Test = org.junit.Test;

	public class DiGeneratorForSequenceFlowsTest
	{

	  private BpmnModelInstance instance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void validateModel() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void validateModel()
	  {
		if (instance != null)
		{
		  Bpmn.validateModel(instance);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateEdgeForSequenceFlow()
	  public virtual void shouldGenerateEdgeForSequenceFlow()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId(SEQUENCE_FLOW_ID).endEvent(END_EVENT_ID).done();

		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		assertEquals(1, allEdges.Count);

		assertBpmnEdgeExists(SEQUENCE_FLOW_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateEdgesForSequenceFlowsUsingGateway()
	  public virtual void shouldGenerateEdgesForSequenceFlowsUsingGateway()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId("s1").parallelGateway("gateway").sequenceFlowId("s2").endEvent("e1").moveToLastGateway().sequenceFlowId("s3").endEvent("e2").done();

		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		assertEquals(3, allEdges.Count);

		assertBpmnEdgeExists("s1");
		assertBpmnEdgeExists("s2");
		assertBpmnEdgeExists("s3");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateEdgesWhenUsingMoveToActivity()
	  public virtual void shouldGenerateEdgesWhenUsingMoveToActivity()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId("s1").exclusiveGateway().sequenceFlowId("s2").userTask(USER_TASK_ID).sequenceFlowId("s3").endEvent("e1").moveToActivity(USER_TASK_ID).sequenceFlowId("s4").endEvent("e2").done();

		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		assertEquals(4, allEdges.Count);

		assertBpmnEdgeExists("s1");
		assertBpmnEdgeExists("s2");
		assertBpmnEdgeExists("s3");
		assertBpmnEdgeExists("s4");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateEdgesWhenUsingMoveToNode()
	  public virtual void shouldGenerateEdgesWhenUsingMoveToNode()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId("s1").exclusiveGateway().sequenceFlowId("s2").userTask(USER_TASK_ID).sequenceFlowId("s3").endEvent("e1").moveToNode(USER_TASK_ID).sequenceFlowId("s4").endEvent("e2").done();

		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		assertEquals(4, allEdges.Count);

		assertBpmnEdgeExists("s1");
		assertBpmnEdgeExists("s2");
		assertBpmnEdgeExists("s3");
		assertBpmnEdgeExists("s4");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGenerateEdgesWhenUsingConnectTo()
	  public virtual void shouldGenerateEdgesWhenUsingConnectTo()
	  {

		ProcessBuilder builder = Bpmn.createExecutableProcess();

		instance = builder.startEvent(START_EVENT_ID).sequenceFlowId("s1").exclusiveGateway("gateway").sequenceFlowId("s2").userTask(USER_TASK_ID).sequenceFlowId("s3").endEvent(END_EVENT_ID).moveToNode(USER_TASK_ID).sequenceFlowId("s4").connectTo("gateway").done();

		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		assertEquals(4, allEdges.Count);

		assertBpmnEdgeExists("s1");
		assertBpmnEdgeExists("s2");
		assertBpmnEdgeExists("s3");
		assertBpmnEdgeExists("s4");
	  }

	  protected internal virtual BpmnEdge findBpmnEdge(string sequenceFlowId)
	  {
		ICollection<BpmnEdge> allEdges = instance.getModelElementsByType(typeof(BpmnEdge));
		IEnumerator<BpmnEdge> iterator = allEdges.GetEnumerator();

		while (iterator.MoveNext())
		{
		  BpmnEdge edge = iterator.Current;
		  if (edge.BpmnElement.Id.Equals(sequenceFlowId))
		  {
			return edge;
		  }
		}
		return null;
	  }

	  protected internal virtual void assertBpmnEdgeExists(string id)
	  {
		BpmnEdge edge = findBpmnEdge(id);
		assertNotNull(edge);
	  }
	}

}