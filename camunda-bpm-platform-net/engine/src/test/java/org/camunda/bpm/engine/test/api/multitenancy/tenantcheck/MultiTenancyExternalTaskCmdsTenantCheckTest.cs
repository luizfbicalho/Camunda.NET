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
namespace org.camunda.bpm.engine.test.api.multitenancy.tenantcheck
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancyExternalTaskCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyExternalTaskCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string PROCESS_DEFINITION_KEY = "twoExternalTaskProcess";
	  protected internal const string PROCESS_DEFINITION_KEY_ONE = "oneExternalTaskProcess";
	  private const string ERROR_DETAILS = "anErrorDetail";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal const string WORKER_ID = "aWorkerId";

	  protected internal const long LOCK_TIME = 10000L;

	  protected internal const string TOPIC_NAME = "externalTaskTopic";

	  protected internal const string ERROR_MESSAGE = "errorMessage";

	  protected internal ExternalTaskService externalTaskService;

	  protected internal TaskService taskService;

	  protected internal string processInstanceId;

	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		externalTaskService = engineRule.ExternalTaskService;

		taskService = engineRule.TaskService;

		identityService = engineRule.IdentityService;

		testRule.deployForTenant(TENANT_ONE, "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml");

		processInstanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

	  }

	  // fetch and lock test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithAuthenticatedTenant()
	  public virtual void testFetchAndLockWithAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// then
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithNoAuthenticatedTenant()
	  public virtual void testFetchAndLockWithNoAuthenticatedTenant()
	  {

		identityService.setAuthentication("aUserId", null);

		// then external task cannot be fetched due to the absence of tenant Id authentication
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithDifferentTenant()
	  public virtual void testFetchAndLockWithDifferentTenant()
	  {

		identityService.setAuthentication("aUserId", null, Arrays.asList("tenantTwo"));

		// then external task cannot be fetched due to the absence of 'tenant1' authentication
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(0, externalTasks.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithDisabledTenantCheck()
	  public virtual void testFetchAndLockWithDisabledTenantCheck()
	  {

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;
		// then
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();
		assertEquals(1, externalTasks.Count);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithoutTenantId()
	  public virtual void testFetchAndLockWithoutTenantId()
	  {
		// given
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).withoutTenantId().execute();

		// then
		assertEquals(0, externalTasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithTenantId()
	  public virtual void testFetchAndLockWithTenantId()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml");
		engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_ONE).Id;
		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).tenantIdIn(TENANT_ONE).execute();

		// then
		assertEquals(1, externalTasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithTenantIdIn()
	  public virtual void testFetchAndLockWithTenantIdIn()
	  {

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).tenantIdIn(TENANT_ONE, TENANT_TWO).execute();

		// then
		assertEquals(1, externalTasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFetchAndLockWithTenantIdInTwoTenants()
	  public virtual void testFetchAndLockWithTenantIdInTwoTenants()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskWithPriorityProcess.bpmn20.xml");
		engineRule.RuntimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess").Id;
		testRule.deployForTenant(TENANT_TWO, "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml");
		string instanceId = engineRule.RuntimeService.startProcessInstanceByKey(PROCESS_DEFINITION_KEY_ONE).Id;

		// when
		IList<LockedExternalTask> externalTasks = externalTaskService.fetchAndLock(2, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).tenantIdIn(TENANT_ONE, TENANT_TWO).execute();

		// then
		assertEquals(2, externalTasks.Count);

		foreach (LockedExternalTask externalTask in externalTasks)
		{
		  if (externalTask.ProcessInstanceId.Equals(processInstanceId))
		  {
			assertEquals(TENANT_ONE, externalTask.TenantId);
		  }
		  else if (externalTask.ProcessInstanceId.Equals(instanceId))
		  {
			assertEquals(TENANT_TWO, externalTask.TenantId);
		  }
		  else
		  {
			fail("No other external tasks should be available!");
		  }
		}
	  }

	  // complete external task test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithAuthenticatedTenant()
	  public virtual void testCompleteWithAuthenticatedTenant()
	  {

		string externalTaskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		assertEquals(1, externalTaskService.createExternalTaskQuery().active().count());

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		externalTaskService.complete(externalTaskId, WORKER_ID);

		assertThat(externalTaskService.createExternalTaskQuery().active().count(), @is(0L));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithNoAuthenticatedTenant()
	  public virtual void testCompleteWithNoAuthenticatedTenant()
	  {

		string externalTaskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		assertEquals(1, externalTaskService.createExternalTaskQuery().active().count());

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		externalTaskService.complete(externalTaskId, WORKER_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteWithDisableTenantCheck()
	  public virtual void testCompleteWithDisableTenantCheck()
	  {

		string externalTaskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		assertEquals(1, externalTaskService.createExternalTaskQuery().active().count());

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		externalTaskService.complete(externalTaskId, WORKER_ID);
		// then
		assertThat(externalTaskService.createExternalTaskQuery().active().count(), @is(0L));
	  }

	  // handle failure test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithAuthenticatedTenant()
	  public virtual void testHandleFailureWithAuthenticatedTenant()
	  {

		LockedExternalTask task = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0];

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		externalTaskService.handleFailure(task.Id, WORKER_ID, ERROR_MESSAGE, 1, 0);

		// then
		assertEquals(ERROR_MESSAGE, externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].ErrorMessage);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithNoAuthenticatedTenant()
	  public virtual void testHandleFailureWithNoAuthenticatedTenant()
	  {

		LockedExternalTask task = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0];

		identityService.setAuthentication("aUserId", null);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		externalTaskService.handleFailure(task.Id, WORKER_ID, ERROR_MESSAGE, 1, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleFailureWithDisabledTenantCheck()
	  public virtual void testHandleFailureWithDisabledTenantCheck()
	  {

		string taskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		externalTaskService.handleFailure(taskId, WORKER_ID, ERROR_MESSAGE, 1, 0);
		// then
		assertEquals(ERROR_MESSAGE, externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].ErrorMessage);
	  }

	  // handle BPMN error
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithAuthenticatedTenant()
	  public virtual void testHandleBPMNErrorWithAuthenticatedTenant()
	  {

		string taskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		externalTaskService.handleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

		// then
		assertEquals(taskService.createTaskQuery().singleResult().TaskDefinitionKey, "afterBpmnError");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithNoAuthenticatedTenant()
	  public virtual void testHandleBPMNErrorWithNoAuthenticatedTenant()
	  {

		string taskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.handleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHandleBPMNErrorWithDisabledTenantCheck()
	  public virtual void testHandleBPMNErrorWithDisabledTenantCheck()
	  {

		string taskId = externalTaskService.fetchAndLock(1, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		externalTaskService.handleBpmnError(taskId, WORKER_ID, "ERROR-OCCURED");

		// then
		assertEquals(taskService.createTaskQuery().singleResult().TaskDefinitionKey, "afterBpmnError");

	  }

	  // setRetries test
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithAuthenticatedTenant()
	  public virtual void testSetRetriesWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		externalTaskService.setRetries(externalTaskId, 5);

		// then
		assertEquals(5, (int) externalTaskService.createExternalTaskQuery().singleResult().Retries);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithNoAuthenticatedTenant()
	  public virtual void testSetRetriesWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.setRetries(externalTaskId, 5);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithDisabledTenantCheck()
	  public virtual void testSetRetriesWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		externalTaskService.setRetries(externalTaskId, 5);

		// then
		assertEquals(5, (int) externalTaskService.createExternalTaskQuery().singleResult().Retries);

	  }

	  // set priority test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithAuthenticatedTenant()
	  public virtual void testSetPriorityWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		externalTaskService.setPriority(externalTaskId, 1);

		// then
		assertEquals(1, (int) externalTaskService.createExternalTaskQuery().singleResult().Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithNoAuthenticatedTenant()
	  public virtual void testSetPriorityWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.setPriority(externalTaskId, 1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityWithDisabledTenantCheck()
	  public virtual void testSetPriorityWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// when
		externalTaskService.setPriority(externalTaskId, 1);

		// then
		assertEquals(1, (int) externalTaskService.createExternalTaskQuery().singleResult().Priority);
	  }

	  // unlock test cases
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithAuthenticatedTenant()
	  public virtual void testUnlockWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		assertThat(externalTaskService.createExternalTaskQuery().locked().count(), @is(1L));

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when
		externalTaskService.unlock(externalTaskId);

		// then
		assertThat(externalTaskService.createExternalTaskQuery().locked().count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithNoAuthenticatedTenant()
	  public virtual void testUnlockWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot update the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.unlock(externalTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnlockWithDisabledTenantCheck()
	  public virtual void testUnlockWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		externalTaskService.unlock(externalTaskId);
		// then
		assertThat(externalTaskService.createExternalTaskQuery().locked().count(), @is(0L));
	  }

	  // get error details tests
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithAuthenticatedTenant()
	  public virtual void testGetErrorDetailsWithAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		externalTaskService.handleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.setAuthentication("aUserId", null, Arrays.asList(TENANT_ONE));

		// when then
		assertThat(externalTaskService.getExternalTaskErrorDetails(externalTaskId), @is(ERROR_DETAILS));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithNoAuthenticatedTenant()
	  public virtual void testGetErrorDetailsWithNoAuthenticatedTenant()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		externalTaskService.handleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.setAuthentication("aUserId", null);

		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("Cannot read the process instance '" + processInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		externalTaskService.getExternalTaskErrorDetails(externalTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsWithDisabledTenantCheck()
	  public virtual void testGetErrorDetailsWithDisabledTenantCheck()
	  {
		// given
		string externalTaskId = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute()[0].Id;

		externalTaskService.handleFailure(externalTaskId,WORKER_ID,ERROR_MESSAGE,ERROR_DETAILS,1,1000L);

		identityService.setAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.TenantCheckEnabled = false;

		// then
		assertThat(externalTaskService.getExternalTaskErrorDetails(externalTaskId), @is(ERROR_DETAILS));
	  }
	}

}