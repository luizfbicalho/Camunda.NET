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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	public class MultiTenancyExecutionPropagationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string CMMN_FILE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
	  protected internal const string SET_VARIABLE_CMMN_FILE = "org/camunda/bpm/engine/test/api/multitenancy/HumanTaskSetVariableExecutionListener.cmmn";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";
	  protected internal const string TENANT_ID = "tenant1";

	  public virtual void testPropagateTenantIdToProcessDefinition()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).done());

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		assertNotNull(processDefinition);
		// inherit the tenant id from deployment
		assertEquals(TENANT_ID, processDefinition.TenantId);
	  }

	  public virtual void testPropagateTenantIdToProcessInstance()
	  {
		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance, @is(notNullValue()));
		// inherit the tenant id from process definition
		assertThat(processInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToConcurrentExecution()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().parallelGateway("fork").userTask().parallelGateway("join").endEvent().moveToNode("fork").userTask().connectTo("join").done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IList<Execution> executions = runtimeService.createExecutionQuery().list();
		assertThat(executions.Count, @is(3));
		assertThat(executions[0].TenantId, @is(TENANT_ID));
		// inherit the tenant id from process instance
		assertThat(executions[1].TenantId, @is(TENANT_ID));
		assertThat(executions[2].TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToEmbeddedSubprocess()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().subProcess().embeddedSubProcess().startEvent().userTask().endEvent().subProcessDone().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		IList<Execution> executions = runtimeService.createExecutionQuery().list();
		assertThat(executions.Count, @is(2));
		assertThat(executions[0].TenantId, @is(TENANT_ID));
		// inherit the tenant id from parent execution (e.g. process instance)
		assertThat(executions[1].TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToTask()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		Task task = taskService.createTaskQuery().singleResult();
		assertThat(task, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(task.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceOnStartProcessInstance()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());

		VariableMap variables = Variables.putValue("var", "test");

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		runtimeService.startProcessInstanceById(processDefinition.Id, variables);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from process instance
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceFromExecution()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaClass(typeof(SetVariableTask).FullName).camundaAsyncAfter().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceFromTask()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().camundaAsyncAfter().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		VariableMap variables = Variables.createVariables().putValue("var", "test");
		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariablesLocal(task.Id, variables);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from task
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToStartMessageEventSubscription()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().message("start").endEvent().done());

		// the event subscription of the message start is created on deployment
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertThat(eventSubscription, @is(notNullValue()));
		// inherit the tenant id from process definition
		assertThat(eventSubscription.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToStartSignalEventSubscription()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().signal("start").endEvent().done());

		// the event subscription of the signal start event is created on deployment
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertThat(eventSubscription, @is(notNullValue()));
		// inherit the tenant id from process definition
		assertThat(eventSubscription.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToIntermediateMessageEventSubscription()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().intermediateCatchEvent().message("start").endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertThat(eventSubscription, @is(notNullValue()));
		// inherit the tenant id from process instance
		assertThat(eventSubscription.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToIntermediateSignalEventSubscription()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().intermediateCatchEvent().signal("start").endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertThat(eventSubscription, @is(notNullValue()));
		// inherit the tenant id from process instance
		assertThat(eventSubscription.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToCompensationEventSubscription()
	  {

		deploymentForTenant(TENANT_ID, "org/camunda/bpm/engine/test/api/multitenancy/compensationBoundaryEvent.bpmn");

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the event subscription is created after execute the activity with the attached compensation boundary event
		EventSubscription eventSubscription = runtimeService.createEventSubscriptionQuery().singleResult();
		assertThat(eventSubscription, @is(notNullValue()));
		// inherit the tenant id from process instance
		assertThat(eventSubscription.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToStartTimerJobDefinition()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().timerWithDuration("PT1M").endEvent().done());

		// the job definition is created on deployment
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		assertThat(jobDefinition, @is(notNullValue()));
		// inherit the tenant id from process definition
		assertThat(jobDefinition.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToIntermediateTimerJob()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().intermediateCatchEvent().timerWithDuration("PT1M").endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the job is created when the timer event is reached
		Job job = managementService.createJobQuery().singleResult();
		assertThat(job, @is(notNullValue()));
		// inherit the tenant id from job definition
		assertThat(job.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToAsyncJob()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().camundaAsyncBefore().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// the job is created when the asynchronous activity is reached
		Job job = managementService.createJobQuery().singleResult();
		assertThat(job, @is(notNullValue()));
		// inherit the tenant id from job definition
		assertThat(job.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToFailedJobIncident()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaExpression("${failing}").camundaAsyncBefore().endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertThat(incident, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(incident.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToFailedStartTimerIncident()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().timerWithDuration("PT1M").serviceTask().camundaExpression("${failing}").endEvent().done());

		executeAvailableJobs();

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertThat(incident, @is(notNullValue()));
		// inherit the tenant id from job
		assertThat(incident.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToFailedExternalTaskIncident()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaType("external").camundaTopic("test").endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		// fetch the external task and mark it as failed which create an incident
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, "test-worker").topic("test", 1000).execute();
		externalTaskService.handleFailure(tasks[0].Id, "test-worker", "expected", 0, 0);

		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertThat(incident, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(incident.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToExternalTask()
	  {

		deploymentForTenant(TENANT_ID, Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().serviceTask().camundaType("external").camundaTopic("test").endEvent().done());

		startProcessInstance(PROCESS_DEFINITION_KEY);

		ExternalTask externalTask = externalTaskService.createExternalTaskQuery().singleResult();
		assertThat(externalTask, @is(notNullValue()));
		// inherit the tenant id from execution
		assertThat(externalTask.TenantId, @is(TENANT_ID));

		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, "test").topic("test", 1000).execute();
		assertThat(externalTasks.Count, @is(1));
		assertThat(externalTasks[0].TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceOnCreateCaseInstance()
	  {

		deploymentForTenant(TENANT_ID, CMMN_FILE);

		VariableMap variables = Variables.putValue("var", "test");

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		caseService.createCaseInstanceById(caseDefinition.Id, variables);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from case instance
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceFromCaseExecution()
	  {

		deploymentForTenant(TENANT_ID, SET_VARIABLE_CMMN_FILE);

		createCaseInstance();

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from case execution
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToVariableInstanceFromHumanTask()
	  {

		deploymentForTenant(TENANT_ID, CMMN_FILE);

		createCaseInstance();

		VariableMap variables = Variables.createVariables().putValue("var", "test");
		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		caseService.setVariables(caseExecution.Id, variables);

		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().singleResult();
		assertThat(variableInstance, @is(notNullValue()));
		// inherit the tenant id from human task
		assertThat(variableInstance.TenantId, @is(TENANT_ID));
	  }

	  public virtual void testPropagateTenantIdToTaskOnCreateCaseInstance()
	  {
		deploymentForTenant(TENANT_ID, CMMN_FILE);

		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		caseService.createCaseInstanceById(caseDefinition.Id);

		Task task = taskService.createTaskQuery().taskName("A HumanTask").singleResult();
		assertThat(task, @is(notNullValue()));
		// inherit the tenant id from case instance
		assertThat(task.TenantId, @is(TENANT_ID));
	  }

	  public class SetVariableTask : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  execution.setVariable("var", "test");
		}
	  }

	  protected internal virtual void startProcessInstance(string processDefinitionKey)
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).latestVersion().singleResult();

		runtimeService.startProcessInstanceById(processDefinition.Id);
	  }

	  protected internal virtual void createCaseInstance()
	  {
		CaseDefinition caseDefinition = repositoryService.createCaseDefinitionQuery().singleResult();
		caseService.createCaseInstanceById(caseDefinition.Id);
	  }

	}

}