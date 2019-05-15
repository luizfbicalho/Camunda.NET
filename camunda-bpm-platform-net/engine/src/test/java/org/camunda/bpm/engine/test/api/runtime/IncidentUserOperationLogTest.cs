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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class IncidentUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public IncidentUserOperationLogTest()
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


	  protected internal ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogIncidentCreation()
	  public virtual void shouldLogIncidentCreation()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		assertEquals(0, historyService.createUserOperationLogQuery().count());

		// when
		identityService.AuthenticatedUserId = "userId";
		Incident incident = runtimeService.createIncident("foo", processInstance.Id, "aa", "bar");
		identityService.clearAuthentication();

		// then
		assertEquals(2, historyService.createUserOperationLogQuery().count());

		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().property("incidentType").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE_INCIDENT, entry.OperationType);
		assertEquals(EntityTypes.PROCESS_INSTANCE, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
		assertNull(entry.OrgValue);
		assertEquals("foo", entry.NewValue);
		assertNull(entry.ExecutionId);
		assertEquals(processInstance.Id, entry.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, entry.ProcessDefinitionId);
		assertEquals("Process", entry.ProcessDefinitionKey);

		entry = historyService.createUserOperationLogQuery().property("configuration").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE_INCIDENT, entry.OperationType);
		assertEquals(EntityTypes.PROCESS_INSTANCE, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
		assertNull(entry.OrgValue);
		assertEquals(incident.Configuration, entry.NewValue);
		assertNull(entry.ExecutionId);
		assertEquals(processInstance.Id, entry.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, entry.ProcessDefinitionId);
		assertEquals("Process", entry.ProcessDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogIncidentCreationFailure()
	  public virtual void shouldNotLogIncidentCreationFailure()
	  {
		// given
		assertEquals(0, historyService.createUserOperationLogQuery().count());

		// when
		thrown.expect(typeof(BadUserRequestException));
		runtimeService.createIncident("foo", null, "userTask1", "bar");

		// then
		assertEquals(0, historyService.createUserOperationLogQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogIncidentResolution()
	  public virtual void shouldLogIncidentResolution()
	  {
		// given
		testRule.deploy(ProcessModels.TWO_TASKS_PROCESS);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("Process");
		Incident incident = runtimeService.createIncident("foo", processInstance.Id, "userTask1", "bar");
		assertEquals(0, historyService.createUserOperationLogQuery().count());

		// when
		identityService.AuthenticatedUserId = "userId";
		runtimeService.resolveIncident(incident.Id);
		identityService.clearAuthentication();

		// then
		assertEquals(1, historyService.createUserOperationLogQuery().count());
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RESOLVE, entry.OperationType);
		assertEquals(EntityTypes.PROCESS_INSTANCE, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
		assertEquals("incidentId", entry.Property);
		assertNull(entry.OrgValue);
		assertEquals(incident.Id, entry.NewValue);
		assertNull(entry.ExecutionId);
		assertEquals(processInstance.Id, entry.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, entry.ProcessDefinitionId);
		assertEquals("Process", entry.ProcessDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogIncidentResolutionFailure()
	  public virtual void shouldNotLogIncidentResolutionFailure()
	  {
		// given
		assertEquals(0, historyService.createUserOperationLogQuery().count());

		// when
		thrown.expect(typeof(NotFoundException));
		runtimeService.resolveIncident("foo");

		// then
		assertEquals(0, historyService.createUserOperationLogQuery().count());
	  }
	}

}