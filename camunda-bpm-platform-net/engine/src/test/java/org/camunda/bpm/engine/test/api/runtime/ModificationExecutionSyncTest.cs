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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class ModificationExecutionSyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public ModificationExecutionSyncTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(rule);
			helper = new BatchModificationHelper(rule);
			ruleChain = RuleChain.outerRule(rule).around(testRule);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchModificationHelper helper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal BpmnModelInstance instance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createBpmnModelInstance()
	  public virtual void createBpmnModelInstance()
	  {
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").userTask("user1").sequenceFlowId("seq").userTask("user2").userTask("user3").endEvent("end").done();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeInstanceIds()
	  public virtual void removeInstanceIds()
	  {
		helper.currentProcessInstances = new List<string>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createSimpleModificationPlan()
	  public virtual void createSimpleModificationPlan()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		IList<string> instances = helper.startInstances("process1", 2);
		runtimeService.createModification(processDefinition.Id).startBeforeActivity("user2").cancelAllForActivity("user1").processInstanceIds(instances).execute();

		foreach (string instanceId in instances)
		{

		  IList<string> activeActivityIds = runtimeService.getActiveActivityIds(instanceId);
		  assertEquals(1, activeActivityIds.Count);
		  assertEquals(activeActivityIds.GetEnumerator().next(), "user2");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessInstanceIdsList()
	  public virtual void createModificationWithNullProcessInstanceIdsList()
	  {

		try
		{
		 runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds((IList<string>) null).execute();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationUsingProcessInstanceIdsListWithNullValue()
	  public virtual void createModificationUsingProcessInstanceIdsListWithNullValue()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds(Arrays.asList("foo", null, "bar")).execute();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithEmptyProcessInstanceIdsList()
	  public virtual void createModificationWithEmptyProcessInstanceIdsList()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds(System.Linq.Enumerable.Empty<string> ()).execute();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessDefinitionId()
	  public virtual void createModificationWithNullProcessDefinitionId()
	  {
		try
		{
		  runtimeService.createModification(null).cancelAllForActivity("activityId").processInstanceIds(Arrays.asList("20", "1--0")).execute();
		  fail("Should not succed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("processDefinitionId is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNullProcessInstanceIdsArray()
	  public virtual void createModificationWithNullProcessInstanceIdsArray()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceIds((string[]) null).execute();
		  fail("Should not be able to modify");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationUsingProcessInstanceIdsArrayWithNullValue()
	  public virtual void createModificationUsingProcessInstanceIdsArrayWithNullValue()
	  {

		try
		{
		  runtimeService.createModification("processDefinitionId").cancelAllForActivity("user1").processInstanceIds("foo", null, "bar").execute();
		  fail("Should not be able to modify");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceQuery()
	  public virtual void testNullProcessInstanceQuery()
	  {
		try
		{
		  runtimeService.createModification("processDefinitionId").startAfterActivity("user1").processInstanceQuery(null).execute();
		  fail("Should not succeed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createModificationWithNotMatchingProcessDefinitionId()
	  public virtual void createModificationWithNotMatchingProcessDefinitionId()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);
		try
		{
		  runtimeService.createModification("foo").cancelAllForActivity("activityId").processInstanceIds(processInstanceIds).execute();
		  fail("Should not succed");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("processDefinition is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartBefore()
	  public virtual void testStartBefore()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		runtimeService.createModification(definition.Id).startBeforeActivity("user2").processInstanceIds(processInstanceIds).execute();

		foreach (string processInstanceId in processInstanceIds)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user1").activity("user2").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartAfter()
	  public virtual void testStartAfter()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		runtimeService.createModification(definition.Id).startAfterActivity("user2").processInstanceIds(processInstanceIds).execute();

		foreach (string processInstanceId in processInstanceIds)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user1").activity("user3").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartTransition()
	  public virtual void testStartTransition()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		runtimeService.createModification(definition.Id).startTransition("seq").processInstanceIds(processInstanceIds).execute();

		foreach (string processInstanceId in processInstanceIds)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user1").activity("user2").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelAll()
	  public virtual void testCancelAll()
	  {
		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);
		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		runtimeService.createModification(processDefinition.Id).cancelAllForActivity("user1").processInstanceIds(processInstanceIds).execute();

		foreach (string processInstanceId in processInstanceIds)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNull(updatedTree);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartBeforeAndCancelAll()
	  public virtual void testStartBeforeAndCancelAll()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 2);

		runtimeService.createModification(definition.Id).cancelAllForActivity("user1").startBeforeActivity("user2").processInstanceIds(processInstanceIds).execute();

		foreach (string processInstanceId in processInstanceIds)
		{
		  ActivityInstance updatedTree = runtimeService.getActivityInstance(processInstanceId);
		  assertNotNull(updatedTree);
		  assertEquals(processInstanceId, updatedTree.ProcessInstanceId);

		  assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user2").done());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDifferentStates()
	  public virtual void testDifferentStates()
	  {
		DeploymentWithDefinitions deployment = testRule.deploy(instance);
		ProcessDefinition definition = deployment.DeployedProcessDefinitions[0];

		IList<string> processInstanceIds = helper.startInstances("process1", 1);
		Task task = rule.TaskService.createTaskQuery().singleResult();
		rule.TaskService.complete(task.Id);

		IList<string> anotherProcessInstanceIds = helper.startInstances("process1", 1);
		((IList<string>)processInstanceIds).AddRange(anotherProcessInstanceIds);

		runtimeService.createModification(definition.Id).startBeforeActivity("user3").processInstanceIds(processInstanceIds).execute();

		ActivityInstance updatedTree = null;
		string processInstanceId = processInstanceIds[0];
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user2").activity("user3").done());

		processInstanceId = processInstanceIds[1];
		updatedTree = runtimeService.getActivityInstance(processInstanceId);
		assertNotNull(updatedTree);
		assertEquals(processInstanceId, updatedTree.ProcessInstanceId);
		assertThat(updatedTree).hasStructure(describeActivityInstanceTree(definition.Id).activity("user1").activity("user3").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithoutFlag()
	  public virtual void testCancelWithoutFlag()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user").processInstanceIds(processInstanceIds).execute();

		// then
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithoutFlag2()
	  public virtual void testCancelWithoutFlag2()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", false).processInstanceIds(processInstanceIds).execute();

		// then
		assertEquals(0, runtimeService.createExecutionQuery().list().size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithFlag()
	  public virtual void testCancelWithFlag()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 1);

		// when
		runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", true).processInstanceIds(processInstanceIds).execute();

		// then
		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().singleResult();
		assertNotNull(execution);
		assertEquals("user", execution.ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCancelWithFlagForManyInstances()
	  public virtual void testCancelWithFlagForManyInstances()
	  {
		// given
		this.instance = Bpmn.createExecutableProcess("process1").startEvent("start").serviceTask("ser").camundaExpression("${true}").userTask("user").endEvent("end").done();

		ProcessDefinition processDefinition = testRule.deployAndGetDefinition(instance);

		IList<string> processInstanceIds = helper.startInstances("process1", 10);

		// when
		runtimeService.createModification(processDefinition.Id).startBeforeActivity("ser").cancelAllForActivity("user", true).processInstanceIds(processInstanceIds).execute();

		// then
		foreach (string processInstanceId in processInstanceIds)
		{
		  Execution execution = runtimeService.createExecutionQuery().processInstanceId(processInstanceId).singleResult();
		  assertNotNull(execution);
		  assertEquals("user", ((ExecutionEntity) execution).ActivityId);
		}
	  }

	}

}