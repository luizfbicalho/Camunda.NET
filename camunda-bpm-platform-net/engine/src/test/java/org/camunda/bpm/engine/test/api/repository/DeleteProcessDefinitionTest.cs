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
namespace org.camunda.bpm.engine.test.api.repository
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using IncrementCounterListener = org.camunda.bpm.engine.test.api.runtime.util.IncrementCounterListener;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Cache = org.camunda.commons.utils.cache.Cache;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.repository.RedeploymentTest.DEPLOYMENT_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DeleteProcessDefinitionTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeleteProcessDefinitionTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.util.ProcessEngineTestRule testHelper = new org.camunda.bpm.engine.test.util.ProcessEngineTestRule(engineRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal HistoryService historyService;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal Deployment deployment;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		historyService = engineRule.HistoryService;
		repositoryService = engineRule.RepositoryService;
		runtimeService = engineRule.RuntimeService;
		processEngineConfiguration = (ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		if (deployment != null)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		  deployment = null;
		}
	  }

	  protected internal const string IO_MAPPING_PROCESS_KEY = "ioMappingProcess";
	  protected internal static readonly BpmnModelInstance IO_MAPPING_PROCESS = Bpmn.createExecutableProcess(IO_MAPPING_PROCESS_KEY).startEvent().userTask().camundaOutputParameter("inputParameter", "${notExistentVariable}").endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionNullId()
	  public virtual void testDeleteProcessDefinitionNullId()
	  {
		// declare expected exception
		thrown.expect(typeof(NullValueException));
		thrown.expectMessage("processDefinitionId is null");

		repositoryService.deleteProcessDefinition(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingProcessDefinition()
	  public virtual void testDeleteNonExistingProcessDefinition()
	  {
		// declare expected exception
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("No process definition found with id 'notexist': processDefinition is null");

		repositoryService.deleteProcessDefinition("notexist");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinition()
	  public virtual void testDeleteProcessDefinition()
	  {
		// given deployment with two process definitions in one xml model file
		deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml").deploy();
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		//when a process definition is been deleted
		repositoryService.deleteProcessDefinition(processDefinitions[0].Id);

		//then only one process definition should remain
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionWithProcessInstance()
	  public virtual void testDeleteProcessDefinitionWithProcessInstance()
	  {
		// given process definition and a process instance
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", bpmnModel).deploy();
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();
		runtimeService.createProcessInstanceByKey("process").executeWithVariablesInReturn();

		//when the corresponding process definition is deleted from the deployment
		try
		{
		  repositoryService.deleteProcessDefinition(processDefinition.Id);
		  fail("Should fail, since there exists a process instance");
		}
		catch (ProcessEngineException pee)
		{
		  // then Exception is expected, the deletion should fail since there exist a process instance
		  // and the cascade flag is per default false
		  assertTrue(pee.Message.contains("Deletion of process definition without cascading failed."));
		}
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionCascade()
	  public virtual void testDeleteProcessDefinitionCascade()
	  {
		// given process definition and a process instance
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", bpmnModel).deploy();
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult();
		runtimeService.createProcessInstanceByKey("process").executeWithVariablesInReturn();

		//when the corresponding process definition is cascading deleted from the deployment
		repositoryService.deleteProcessDefinition(processDefinition.Id, true);

		//then exist no process instance and no definition
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());
		assertEquals(0, repositoryService.createProcessDefinitionQuery().count());
		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id)
		{
		  assertEquals(0, engineRule.HistoryService.createHistoricActivityInstanceQuery().count());
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionClearsCache()
	  public virtual void testDeleteProcessDefinitionClearsCache()
	  {
		// given process definition and a process instance
		BpmnModelInstance bpmnModel = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();
		deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", bpmnModel).deploy();
		string processDefinitionId = repositoryService.createProcessDefinitionQuery().processDefinitionKey("process").singleResult().Id;

		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// ensure definitions and models are part of the cache
		assertNotNull(deploymentCache.ProcessDefinitionCache.get(processDefinitionId));
		assertNotNull(deploymentCache.BpmnModelInstanceCache.get(processDefinitionId));

		repositoryService.deleteProcessDefinition(processDefinitionId, true);

		// then the definitions and models are removed from the cache
		assertNull(deploymentCache.ProcessDefinitionCache.get(processDefinitionId));
		assertNull(deploymentCache.BpmnModelInstanceCache.get(processDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionAndRefillDeploymentCache()
	  public virtual void testDeleteProcessDefinitionAndRefillDeploymentCache()
	  {
		// given a deployment with two process definitions in one xml model file
		deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml").deploy();
		ProcessDefinition processDefinitionOne = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").singleResult();
		ProcessDefinition processDefinitionTwo = repositoryService.createProcessDefinitionQuery().processDefinitionKey("two").singleResult();

		string idOne = processDefinitionOne.Id;
		//one is deleted from the deployment
		repositoryService.deleteProcessDefinition(idOne);

		//when clearing the deployment cache
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();

		//then creating process instance from the existing process definition
		ProcessInstanceWithVariables procInst = runtimeService.createProcessInstanceByKey("two").executeWithVariablesInReturn();
		assertNotNull(procInst);
		assertTrue(procInst.ProcessDefinitionId.Contains("two"));

		//should refill the cache
		Cache cache = processEngineConfiguration.DeploymentCache.ProcessDefinitionCache;
		assertNotNull(cache.get(processDefinitionTwo.Id));
		//The deleted process definition should not be recreated after the cache is refilled
		assertNull(cache.get(processDefinitionOne.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionAndRedeploy()
	  public virtual void testDeleteProcessDefinitionAndRedeploy()
	  {
		// given a deployment with two process definitions in one xml model file
		deployment = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/repository/twoProcesses.bpmn20.xml").deploy();

		ProcessDefinition processDefinitionOne = repositoryService.createProcessDefinitionQuery().processDefinitionKey("one").singleResult();

		//one is deleted from the deployment
		repositoryService.deleteProcessDefinition(processDefinitionOne.Id);

		//when the process definition is redeployed
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment.Id).deploy();

		//then there should exist three process definitions
		//two of the redeployment and the remaining one
		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());

		//clean up
		repositoryService.deleteDeployment(deployment2.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByNotExistingKey()
	  public virtual void testDeleteProcessDefinitionsByNotExistingKey()
	  {
		// then
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("No process definition found");

		// when
		repositoryService.deleteProcessDefinitions().byKey("no existing key").withoutTenantId().delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyIsNull()
	  public virtual void testDeleteProcessDefinitionsByKeyIsNull()
	  {
		// then
		thrown.expect(typeof(NullValueException));
		thrown.expectMessage("cannot be null");

		// when
		repositoryService.deleteProcessDefinitions().byKey(null).withoutTenantId().delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKey()
	  public virtual void testDeleteProcessDefinitionsByKey()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey("processOne").withoutTenantId().delete();

		// then
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyWithRunningProcesses()
	  public virtual void testDeleteProcessDefinitionsByKeyWithRunningProcesses()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}
		runtimeService.startProcessInstanceByKey("processOne");

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Deletion of process definition");

		// when
		repositoryService.deleteProcessDefinitions().byKey("processOne").withoutTenantId().delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyCascading()
	  public virtual void testDeleteProcessDefinitionsByKeyCascading()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}

		IDictionary<string, object> variables = new Dictionary<string, object>();

		for (int i = 0; i < 3; i++)
		{
		  variables["varName" + i] = "varValue";
		}

		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("processOne", variables);
		  runtimeService.startProcessInstanceByKey("processTwo", variables);
		}

		// when
		repositoryService.deleteProcessDefinitions().byKey("processOne").withoutTenantId().cascade().delete();

		repositoryService.deleteProcessDefinitions().byKey("processTwo").withoutTenantId().cascade().delete();

		// then
		assertThat(historyService.createHistoricVariableInstanceQuery().count(), @is(0L));
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyWithCustomListenersSkipped()
	  public virtual void testDeleteProcessDefinitionsByKeyWithCustomListenersSkipped()
	  {
		// given
		IncrementCounterListener.counter = 0;
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}

		runtimeService.startProcessInstanceByKey("processOne");

		// when
		repositoryService.deleteProcessDefinitions().byKey("processOne").withoutTenantId().cascade().skipCustomListeners().delete();

		// then
		assertThat(IncrementCounterListener.counter, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByKeyWithIoMappingsSkipped()
	  public virtual void testDeleteProcessDefinitionsByKeyWithIoMappingsSkipped()
	  {
		// given
		testHelper.deploy(IO_MAPPING_PROCESS);
		runtimeService.startProcessInstanceByKey(IO_MAPPING_PROCESS_KEY);

		testHelper.deploy(IO_MAPPING_PROCESS);
		runtimeService.startProcessInstanceByKey(IO_MAPPING_PROCESS_KEY);

		// when
		repositoryService.deleteProcessDefinitions().byKey(IO_MAPPING_PROCESS_KEY).withoutTenantId().cascade().skipIoMappings().delete();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		// then
		assertThat(processDefinitions.Count, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByNotExistingIds()
	  public virtual void testDeleteProcessDefinitionsByNotExistingIds()
	  {
		// then
		thrown.expect(typeof(NotFoundException));
		thrown.expectMessage("No process definition found");

		// when
		repositoryService.deleteProcessDefinitions().byIds("not existing", "also not existing").delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdIsNull()
	  public virtual void testDeleteProcessDefinitionsByIdIsNull()
	  {
		// then
		thrown.expect(typeof(NullValueException));
		thrown.expectMessage("cannot be null");

		// when
		repositoryService.deleteProcessDefinitions().byIds(null).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIds()
	  public virtual void testDeleteProcessDefinitionsByIds()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}

		string[] processDefinitionIds = findProcessDefinitionIdsByKey("processOne");

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();

		// then
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsWithRunningProcesses()
	  public virtual void testDeleteProcessDefinitionsByIdsWithRunningProcesses()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}
		string[] processDefinitionIds = findProcessDefinitionIdsByKey("processOne");
		runtimeService.startProcessInstanceByKey("processOne");

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Deletion of process definition");

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).delete();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsCascading()
	  public virtual void testDeleteProcessDefinitionsByIdsCascading()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}
		string[] processDefinitionIdsOne = findProcessDefinitionIdsByKey("processOne");
		string[] processDefinitionIdsTwo = findProcessDefinitionIdsByKey("processTwo");
		IDictionary<string, object> variables = new Dictionary<string, object>();

		for (int i = 0; i < 3; i++)
		{
		  variables["varName" + i] = "varValue";
		}

		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("processOne", variables);
		  runtimeService.startProcessInstanceByKey("processTwo", variables);
		}

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIdsOne).cascade().delete();

		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIdsTwo).cascade().delete();

		// then
		assertThat(historyService.createHistoricVariableInstanceQuery().count(), @is(0L));
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
		assertThat(repositoryService.createProcessDefinitionQuery().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsWithCustomListenersSkipped()
	  public virtual void testDeleteProcessDefinitionsByIdsWithCustomListenersSkipped()
	  {
		// given
		IncrementCounterListener.counter = 0;
		for (int i = 0; i < 3; i++)
		{
		  deployTwoProcessDefinitions();
		}
		string[] processDefinitionIds = findProcessDefinitionIdsByKey("processOne");
		runtimeService.startProcessInstanceByKey("processOne");

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).cascade().skipCustomListeners().delete();

		// then
		assertThat(IncrementCounterListener.counter, @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProcessDefinitionsByIdsWithIoMappingsSkipped()
	  public virtual void testDeleteProcessDefinitionsByIdsWithIoMappingsSkipped()
	  {
		// given
		testHelper.deploy(IO_MAPPING_PROCESS);
		runtimeService.startProcessInstanceByKey(IO_MAPPING_PROCESS_KEY);

		testHelper.deploy(IO_MAPPING_PROCESS);
		runtimeService.startProcessInstanceByKey(IO_MAPPING_PROCESS_KEY);

		string[] processDefinitionIds = findProcessDefinitionIdsByKey(IO_MAPPING_PROCESS_KEY);

		// when
		repositoryService.deleteProcessDefinitions().byIds(processDefinitionIds).cascade().skipIoMappings().delete();

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		// then
		assertThat(processDefinitions.Count, @is(0));
	  }

	  private void deployTwoProcessDefinitions()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		testHelper.deploy(Bpmn.createExecutableProcess("processOne").startEvent().userTask().camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(IncrementCounterListener).FullName).endEvent().done(), Bpmn.createExecutableProcess("processTwo").startEvent().userTask().endEvent().done());
	  }

	  private string[] findProcessDefinitionIdsByKey(string processDefinitionKey)
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).list();
		IList<string> processDefinitionIds = new List<string>();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionIds.Add(processDefinition.Id);
		}

		return processDefinitionIds.ToArray();
	  }
	}

}