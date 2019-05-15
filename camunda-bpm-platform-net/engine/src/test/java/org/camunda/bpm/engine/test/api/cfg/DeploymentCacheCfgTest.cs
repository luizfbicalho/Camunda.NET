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
namespace org.camunda.bpm.engine.test.api.cfg
{
	using org.camunda.bpm.engine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using CallActivityModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CallActivityModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Cache = org.camunda.commons.utils.cache.Cache;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Johannes Heinemann
	/// </summary>
	public class DeploymentCacheCfgTest
	{
		private bool InstanceFieldsInitialized = false;

		public DeploymentCacheCfgTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(cacheFactoryEngineRule);
			ruleChain = RuleChain.outerRule(cacheFactoryBootstrapRule).around(cacheFactoryEngineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule cacheFactoryBootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			// apply configuration options here
			configuration.CacheCapacity = 2;
			configuration.CacheFactory = new MyCacheFactory();
			configuration.EnableFetchProcessDefinitionDescription = false;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule cacheFactoryEngineRule = new ProvidedProcessEngineRule(cacheFactoryBootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(cacheFactoryBootstrapRule).around(cacheFactoryEngineRule).around(testRule);
	  public RuleChain ruleChain;
	  internal RepositoryService repositoryService;
	  internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  internal RuntimeService runtimeService;
	  internal TaskService taskService;
	  internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initialize()
	  public virtual void initialize()
	  {
		repositoryService = cacheFactoryEngineRule.RepositoryService;
		processEngineConfiguration = cacheFactoryEngineRule.ProcessEngineConfiguration;
		runtimeService = cacheFactoryEngineRule.RuntimeService;
		taskService = cacheFactoryEngineRule.TaskService;
		managementService = cacheFactoryEngineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPlugInOwnCacheImplementation()
	  public virtual void testPlugInOwnCacheImplementation()
	  {

		// given
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// when
		Cache<string, ProcessDefinitionEntity> cache = deploymentCache.ProcessDefinitionCache;

		// then
		assertThat(cache, instanceOf(typeof(MyCacheImplementation)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultCacheRemovesElementWhenMaxSizeIsExceeded()
	  public virtual void testDefaultCacheRemovesElementWhenMaxSizeIsExceeded()
	  {
		// The engine rule sets the maximum number of elements of the to 2.
		// Accordingly, one process should not be contained in the cache anymore at the end.

		// given
		IList<BpmnModelInstance> modelInstances = createProcesses(3);
		deploy(modelInstances);
		string processDefinitionIdZero = repositoryService.createProcessDefinitionQuery().processDefinitionKey("Process0").singleResult().Id;
		string processDefinitionIdOne = repositoryService.createProcessDefinitionQuery().processDefinitionKey("Process1").singleResult().Id;
		string processDefinitionIdTwo = repositoryService.createProcessDefinitionQuery().processDefinitionKey("Process2").singleResult().Id;

		// when
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

		// then
		int numberOfProcessesInCache = 0;
		numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.get(processDefinitionIdZero) == null ? 0 : 1;
		numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.get(processDefinitionIdOne) == null ? 0 : 1;
		numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.get(processDefinitionIdTwo) == null ? 0 : 1;

		assertEquals(2, numberOfProcessesInCache);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
	  public virtual void testDisableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
	  {

		// given
		deploy(ProcessModels.ONE_TASK_PROCESS_WITH_DOCUMENTATION);
		ProcessInstance pi = runtimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		// when
		repositoryService.createProcessDefinitionQuery().processDefinitionKey(ProcessModels.PROCESS_KEY).singleResult().Id;

		// then
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		BpmnModelInstance modelInstance = deploymentCache.BpmnModelInstanceCache.get(pi.ProcessDefinitionId);
		assertNull(modelInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEnableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
	  public virtual void testEnableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
	  {

		// given
		deploy(ProcessModels.ONE_TASK_PROCESS_WITH_DOCUMENTATION);
		processEngineConfiguration.EnableFetchProcessDefinitionDescription = true;
		ProcessInstance pi = runtimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		// when
		repositoryService.createProcessDefinitionQuery().processDefinitionKey(ProcessModels.PROCESS_KEY).singleResult().Id;

		// then
		DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
		BpmnModelInstance modelInstance = deploymentCache.BpmnModelInstanceCache.get(pi.ProcessDefinitionId);
		assertNotNull(modelInstance);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDescriptionIsNullWhenFetchProcessDefinitionDescriptionIsDisabled()
	  public virtual void testDescriptionIsNullWhenFetchProcessDefinitionDescriptionIsDisabled()
	  {

		// given
		deploy(ProcessModels.ONE_TASK_PROCESS_WITH_DOCUMENTATION);
		runtimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		// when
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(ProcessModels.PROCESS_KEY).singleResult();

		// then
		assertNull(processDefinition.Description);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDescriptionIsAvailableWhenFetchProcessDefinitionDescriptionIsEnabled()
	  public virtual void testDescriptionIsAvailableWhenFetchProcessDefinitionDescriptionIsEnabled()
	  {

		// given
		deploy(ProcessModels.ONE_TASK_PROCESS_WITH_DOCUMENTATION);
		processEngineConfiguration.EnableFetchProcessDefinitionDescription = true;
		runtimeService.startProcessInstanceByKey(ProcessModels.PROCESS_KEY);

		// when
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(ProcessModels.PROCESS_KEY).singleResult();

		// then
		assertNotNull(processDefinition.Description);
		assertEquals("This is a documentation!", processDefinition.Description);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLoadProcessDefinitionsFromDBWhenNotExistingInCacheAnymore()
	  public virtual void testLoadProcessDefinitionsFromDBWhenNotExistingInCacheAnymore()
	  {

		// given more processes to deploy than capacity in the cache
		int numberOfProcessesToDeploy = 10;
		IList<BpmnModelInstance> modelInstances = createProcesses(numberOfProcessesToDeploy);
		deploy(modelInstances);

		// when we start a process that was already removed from the cache
		assertNotNull(repositoryService.createProcessDefinitionQuery().processDefinitionKey("Process0").singleResult());
		runtimeService.startProcessInstanceByKey("Process0");

		// then we should be able to complete the process
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSequentialCallActivityCall()
	  public virtual void testSequentialCallActivityCall()
	  {

		// given a number process definitions which call each other by call activities (0->1->2->0->4),
		// which stops after the first repetition of 0 in 4
		IList<BpmnModelInstance> modelInstances = createSequentialCallActivityProcess();
		deploy(modelInstances);

		// when we start the first process 0
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["NextProcess"] = "Process1";
		runtimeService.startProcessInstanceByKey("Process0", variables);

		// then we should be able to complete the task in process 4
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSequentialCallActivityCallAsynchronously() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSequentialCallActivityCallAsynchronously()
	  {

		// given a number process definitions which call each other by call activities (0->1->2->0->4),
		// which stops after the first repetition of 0 in 4
		IList<BpmnModelInstance> modelInstances = createSequentialCallActivityProcessAsync();
		deploy(modelInstances);

		// when we start the first process 0
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["NextProcess"] = "Process1";
		runtimeService.startProcessInstanceByKey("Process0", variables);
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// when we reach process 0 a second time, we have to start that job as well
		job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// then we should be able to complete the task in process 4
		Task task = taskService.createTaskQuery().singleResult();
		taskService.complete(task.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSequentialCallActivityAsynchronousWithUnfinishedExecution() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSequentialCallActivityAsynchronousWithUnfinishedExecution()
	  {

		// given a number process definitions which call each other by call activities (0->1->2->0->4),
		// which stops after the first repetition of 0
		IList<BpmnModelInstance> modelInstances = createSequentialCallActivityProcessAsync();
		Deployment deployment = deploy(modelInstances);

		// when we start the first process 0
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["NextProcess"] = "Process1";
		runtimeService.startProcessInstanceByKey("Process0", variables);
		Job job = managementService.createJobQuery().singleResult();
		managementService.executeJob(job.Id);

		// then deleting the deployment should still be possible
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  protected internal virtual IList<BpmnModelInstance> createSequentialCallActivityProcess()
	  {
		IList<BpmnModelInstance> modelInstances = new LinkedList<BpmnModelInstance>();

		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessAsExpression(0));
		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(1, 2));
		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(2, 0));
		modelInstances.Add(ProcessModels.oneTaskProcess(3));

		return modelInstances;
	  }

	  protected internal virtual IList<BpmnModelInstance> createSequentialCallActivityProcessAsync()
	  {
		IList<BpmnModelInstance> modelInstances = new LinkedList<BpmnModelInstance>();

		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessAsExpressionAsync(0));
		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(1, 2));
		modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(2, 0));
		modelInstances.Add(ProcessModels.oneTaskProcess(3));

		return modelInstances;
	  }

	  protected internal virtual Deployment deploy(IList<BpmnModelInstance> modelInstances)
	  {
		DeploymentBuilder deploymentbuilder = processEngineConfiguration.RepositoryService.createDeployment();

		for (int i = 0; i < modelInstances.Count; i++)
		{
		  deploymentbuilder.addModelInstance("process" + i + ".bpmn", modelInstances[i]);
		}

		return testRule.deploy(deploymentbuilder);
	  }

	  protected internal virtual Deployment deploy(BpmnModelInstance modelInstance)
	  {
		DeploymentBuilder deploymentbuilder = processEngineConfiguration.RepositoryService.createDeployment();
		deploymentbuilder.addModelInstance("process0.bpmn", modelInstance);
		return testRule.deploy(deploymentbuilder);
	  }

	  protected internal virtual IList<BpmnModelInstance> createProcesses(int numberOfProcesses)
	  {

		IList<BpmnModelInstance> result = new List<BpmnModelInstance>(numberOfProcesses);
		for (int i = 0; i < numberOfProcesses; i++)
		{
		  result.Add(ProcessModels.oneTaskProcess(i));
		}
		return result;
	  }


	}
}