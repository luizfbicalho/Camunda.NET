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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class MigrationIncidentTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationIncidentTest()
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
			chain = RuleChain.outerRule(engineRule).around(testHelper);
		}



	  public class NewDelegate : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		}
	  }

	  public const string FAIL_CALLED_PROC_KEY = "calledProc";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance FAIL_CALLED_PROC = Bpmn.createExecutableProcess(FAIL_CALLED_PROC_KEY).startEvent("start").serviceTask("task").camundaAsyncBefore().camundaClass(typeof(FailingDelegate).FullName).endEvent("end").done();

	  public const string FAIL_CALL_PROC_KEY = "oneFailingServiceTaskProcess";
	  public static readonly BpmnModelInstance FAIL_CALL_ACT_JOB_PROC = Bpmn.createExecutableProcess(FAIL_CALL_PROC_KEY).startEvent("start").callActivity("calling").calledElement(FAIL_CALLED_PROC_KEY).endEvent("end").done();



	  public const string NEW_CALLED_PROC_KEY = "newCalledProc";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance NEW_CALLED_PROC = Bpmn.createExecutableProcess(NEW_CALLED_PROC_KEY).startEvent("start").serviceTask("taskV2").camundaAsyncBefore().camundaClass(typeof(NewDelegate).FullName).endEvent("end").done();

	  public const string NEW_CALL_PROC_KEY = "newServiceTaskProcess";
	  public static readonly BpmnModelInstance NEW_CALL_ACT_PROC = Bpmn.createExecutableProcess(NEW_CALL_PROC_KEY).startEvent("start").callActivity("callingV2").calledElement(NEW_CALLED_PROC_KEY).endEvent("end").done();


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/callingProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/callingProcess_v2.bpmn"}) public void testCallActivityExternalTaskIncidentMigration() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/callingProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/callingProcess_v2.bpmn"})]
	  public virtual void testCallActivityExternalTaskIncidentMigration()
	  {
		// Given we create a new process instance
		ProcessDefinition callingProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("callingProcess").singleResult();
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(callingProcess.Id);

		LockedExternalTask task = engineRule.ExternalTaskService.fetchAndLock(1, "foo").topic("foo", 1000L).execute()[0];
		// creating an incident in the called and calling process
		engineRule.ExternalTaskService.handleFailure(task.Id, "foo", "error", 0, 1000L);

		Incident incidentInCallingProcess = engineRule.RuntimeService.createIncidentQuery().processDefinitionId(callingProcess.Id).singleResult();

		// when
		ProcessDefinition callingProcessV2 = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("callingProcessV2").singleResult();

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(callingProcess.Id, callingProcessV2.Id).mapEqualActivities().mapActivities("CallActivity", "CallActivityV2").build();

		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		Incident incidentAfterMigration = engineRule.RuntimeService.createIncidentQuery().incidentId(incidentInCallingProcess.Id).singleResult();
		Assert.assertEquals(callingProcessV2.Id, incidentAfterMigration.ProcessDefinitionId);
		Assert.assertEquals("CallActivityV2", incidentAfterMigration.ActivityId);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/calledProcess_v2.bpmn"}) public void testExternalTaskIncidentMigration() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/calledProcess_v2.bpmn"})]
	  public virtual void testExternalTaskIncidentMigration()
	  {

		// Given we create a new process instance
		ProcessDefinition callingProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("calledProcess").singleResult();
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(callingProcess.Id);

		LockedExternalTask task = engineRule.ExternalTaskService.fetchAndLock(1, "foo").topic("foo", 1000L).execute()[0];
		// creating an incident in the called and calling process
		engineRule.ExternalTaskService.handleFailure(task.Id, "foo", "error", 0, 1000L);

		Incident incidentInCallingProcess = engineRule.RuntimeService.createIncidentQuery().processDefinitionId(callingProcess.Id).singleResult();

		// when
		ProcessDefinition callingProcessV2 = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("calledProcessV2").singleResult();

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(callingProcess.Id, callingProcessV2.Id).mapEqualActivities().mapActivities("ServiceTask_1p58ywb", "ServiceTask_V2").build();

		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		Incident incidentAfterMigration = engineRule.RuntimeService.createIncidentQuery().incidentId(incidentInCallingProcess.Id).singleResult();
		Assert.assertEquals(callingProcessV2.Id, incidentAfterMigration.ProcessDefinitionId);
		Assert.assertEquals("ServiceTask_V2", incidentAfterMigration.ActivityId);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCallActivityJobIncidentMigration()
	  public virtual void testCallActivityJobIncidentMigration()
	  {
		// Given we deploy process definitions
		testHelper.deploy(FAIL_CALLED_PROC, FAIL_CALL_ACT_JOB_PROC, NEW_CALLED_PROC, NEW_CALL_ACT_PROC);

		ProcessDefinition failingProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(FAIL_CALL_PROC_KEY).singleResult();

		ProcessDefinition newProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(NEW_CALL_PROC_KEY).singleResult();

		//create process instance and execute job which fails
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey(FAIL_CALL_PROC_KEY);
		testHelper.executeAvailableJobs();

		Incident incidentInCallingProcess = engineRule.RuntimeService.createIncidentQuery().processDefinitionId(failingProcess.Id).singleResult();

		// when
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(failingProcess.Id, newProcess.Id).mapEqualActivities().mapActivities("calling", "callingV2").build();

		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		Incident incidentAfterMigration = engineRule.RuntimeService.createIncidentQuery().incidentId(incidentInCallingProcess.Id).singleResult();
		Assert.assertEquals(newProcess.Id, incidentAfterMigration.ProcessDefinitionId);
		Assert.assertEquals("callingV2", incidentAfterMigration.ActivityId);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobIncidentMigration()
	  public virtual void testJobIncidentMigration()
	  {
		// Given we deploy process definitions
		testHelper.deploy(FAIL_CALLED_PROC, NEW_CALLED_PROC);

		ProcessDefinition failingProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(FAIL_CALLED_PROC_KEY).singleResult();

		ProcessDefinition newProcess = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey(NEW_CALLED_PROC_KEY).singleResult();

		//create process instance and execute job which fails
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceByKey(FAIL_CALLED_PROC_KEY);
		testHelper.executeAvailableJobs();

		Incident incidentInCallingProcess = engineRule.RuntimeService.createIncidentQuery().processDefinitionId(failingProcess.Id).singleResult();

		// when
		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(failingProcess.Id, newProcess.Id).mapEqualActivities().mapActivities("task", "taskV2").build();

		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		Incident incidentAfterMigration = engineRule.RuntimeService.createIncidentQuery().incidentId(incidentInCallingProcess.Id).singleResult();
		Assert.assertEquals(newProcess.Id, incidentAfterMigration.ProcessDefinitionId);
		Assert.assertEquals("taskV2", incidentAfterMigration.ActivityId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomIncidentMigration()
	  public virtual void testCustomIncidentMigration()
	  {
		// given
		RuntimeService runtimeService = engineRule.RuntimeService;
		BpmnModelInstance instance1 = Bpmn.createExecutableProcess("process1").startEvent().userTask("u1").endEvent().done();
		BpmnModelInstance instance2 = Bpmn.createExecutableProcess("process2").startEvent().userTask("u2").endEvent().done();

		testHelper.deploy(instance1, instance2);

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process1");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process2");

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(processInstance1.ProcessDefinitionId, processInstance2.ProcessDefinitionId).mapActivities("u1", "u2").build();

		runtimeService.createIncident("custom", processInstance1.Id, "foo");

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance1.Id).execute();

		// then
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertEquals(processInstance2.ProcessDefinitionId, incident.ProcessDefinitionId);
		assertEquals("custom", incident.IncidentType);
		assertEquals(processInstance1.Id, incident.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCustomIncidentMigrationWithoutConfiguration()
	  public virtual void testCustomIncidentMigrationWithoutConfiguration()
	  {
		// given
		RuntimeService runtimeService = engineRule.RuntimeService;
		BpmnModelInstance instance1 = Bpmn.createExecutableProcess("process1").startEvent().userTask("u1").endEvent().done();
		BpmnModelInstance instance2 = Bpmn.createExecutableProcess("process2").startEvent().userTask("u2").endEvent().done();

		testHelper.deploy(instance1, instance2);

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("process1");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("process2");

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(processInstance1.ProcessDefinitionId, processInstance2.ProcessDefinitionId).mapActivities("u1", "u2").build();

		runtimeService.createIncident("custom", processInstance1.Id, null);

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance1.Id).execute();

		// then
		Incident incident = runtimeService.createIncidentQuery().singleResult();
		assertEquals(processInstance2.ProcessDefinitionId, incident.ProcessDefinitionId);
		assertEquals("custom", incident.IncidentType);
		assertEquals(processInstance1.Id, incident.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) @Deployment(resources = {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/calledProcess_v2.bpmn"}) public void historicIncidentRemainsOpenAfterMigration()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL), Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/migration/calledProcess.bpmn", "org/camunda/bpm/engine/test/api/runtime/migration/calledProcess_v2.bpmn"})]
	  public virtual void historicIncidentRemainsOpenAfterMigration()
	  {

		// Given we create a new process instance
		ProcessDefinition process1 = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("calledProcess").singleResult();
		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(process1.Id);

		LockedExternalTask task = engineRule.ExternalTaskService.fetchAndLock(1, "foo").topic("foo", 1000L).execute()[0];

		engineRule.ExternalTaskService.handleFailure(task.Id, "foo", "error", 0, 1000L);

		Incident incidentInProcess = engineRule.RuntimeService.createIncidentQuery().processDefinitionId(process1.Id).singleResult();

		// when
		ProcessDefinition process2 = engineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("calledProcessV2").singleResult();

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(process1.Id, process2.Id).mapEqualActivities().mapActivities("ServiceTask_1p58ywb", "ServiceTask_V2").build();

		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		HistoricIncident historicIncidentAfterMigration = engineRule.HistoryService.createHistoricIncidentQuery().singleResult();
		assertNotNull(historicIncidentAfterMigration);
		assertNull(historicIncidentAfterMigration.EndTime);
		assertTrue(historicIncidentAfterMigration.Open);

		HistoricProcessInstance historicProcessInstanceAfterMigration = engineRule.HistoryService.createHistoricProcessInstanceQuery().withIncidents().incidentStatus("open").singleResult();
		assertNotNull(historicProcessInstanceAfterMigration);

		Incident incidentAfterMigration = engineRule.RuntimeService.createIncidentQuery().incidentId(incidentInProcess.Id).singleResult();
		assertEquals(process2.Id, incidentAfterMigration.ProcessDefinitionId);
		assertEquals("ServiceTask_V2", incidentAfterMigration.ActivityId);
	  }
	}

}