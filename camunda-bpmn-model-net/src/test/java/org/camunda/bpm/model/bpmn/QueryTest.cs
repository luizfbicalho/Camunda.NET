using System;

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
namespace org.camunda.bpm.model.bpmn
{
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using Gateway = org.camunda.bpm.model.bpmn.instance.Gateway;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using AfterClass = org.junit.AfterClass;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.fail;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class QueryTest
	{

	  private static BpmnModelInstance modelInstance;
	  private static Query<FlowNode> startSucceeding;
	  private static Query<FlowNode> gateway1Succeeding;
	  private static Query<FlowNode> gateway2Succeeding;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void createModelInstance()
	  public static void createModelInstance()
	  {
		modelInstance = Bpmn.createProcess().startEvent().id("start").userTask().id("user").parallelGateway().id("gateway1").serviceTask().endEvent().moveToLastGateway().parallelGateway().id("gateway2").userTask().endEvent().moveToLastGateway().serviceTask().endEvent().moveToLastGateway().scriptTask().endEvent().done();

		startSucceeding = ((FlowNode) modelInstance.getModelElementById("start")).SucceedingNodes;
		gateway1Succeeding = ((FlowNode) modelInstance.getModelElementById("gateway1")).SucceedingNodes;
		gateway2Succeeding = ((FlowNode) modelInstance.getModelElementById("gateway2")).SucceedingNodes;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void validateModelInstance()
	  public static void validateModelInstance()
	  {
		Bpmn.validateModel(modelInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testList()
	  public virtual void testList()
	  {
		assertThat(startSucceeding.list()).hasSize(1);
		assertThat(gateway1Succeeding.list()).hasSize(2);
		assertThat(gateway2Succeeding.list()).hasSize(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCount()
	  public virtual void testCount()
	  {
		assertThat(startSucceeding.count()).isEqualTo(1);
		assertThat(gateway1Succeeding.count()).isEqualTo(2);
		assertThat(gateway2Succeeding.count()).isEqualTo(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterByType()
	  public virtual void testFilterByType()
	  {
		ModelElementType taskType = modelInstance.Model.getType(typeof(Task));
		ModelElementType gatewayType = modelInstance.Model.getType(typeof(Gateway));

		assertThat(startSucceeding.filterByType(taskType).list()).hasSize(1);
		assertThat(startSucceeding.filterByType(gatewayType).list()).hasSize(0);

		assertThat(gateway1Succeeding.filterByType(taskType).list()).hasSize(1);
		assertThat(gateway1Succeeding.filterByType(gatewayType).list()).hasSize(1);

		assertThat(gateway2Succeeding.filterByType(taskType).list()).hasSize(3);
		assertThat(gateway2Succeeding.filterByType(gatewayType).list()).hasSize(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSingleResult()
	  public virtual void testSingleResult()
	  {
		assertThat(startSucceeding.singleResult().Id).isEqualTo("user");
		try
		{
		  gateway1Succeeding.singleResult();
		  fail("gateway1 has more than one succeeding flow node");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(BpmnModelException)).hasMessageEndingWith("<2>");
		}
		try
		{
		  gateway2Succeeding.singleResult();
		  fail("gateway2 has more than one succeeding flow node");
		}
		catch (Exception e)
		{
		  assertThat(e).isInstanceOf(typeof(BpmnModelException)).hasMessageEndingWith("<3>");
		}
	  }
	}

}