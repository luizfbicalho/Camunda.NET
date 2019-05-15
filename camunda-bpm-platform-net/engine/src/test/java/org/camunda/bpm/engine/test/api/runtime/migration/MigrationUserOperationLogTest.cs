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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MigrationUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  public const string USER_ID = "userId";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLogCreation()
	  public virtual void testLogCreation()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		rule.IdentityService.AuthenticatedUserId = USER_ID;
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();
		rule.IdentityService.clearAuthentication();

		// then
		IList<UserOperationLogEntry> opLogEntries = rule.HistoryService.createUserOperationLogQuery().list();
		Assert.assertEquals(3, opLogEntries.Count);

		IDictionary<string, UserOperationLogEntry> entries = asMap(opLogEntries);

		UserOperationLogEntry procDefEntry = entries["processDefinitionId"];
		Assert.assertNotNull(procDefEntry);
		Assert.assertEquals("ProcessInstance", procDefEntry.EntityType);
		Assert.assertEquals("Migrate", procDefEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, procDefEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, procDefEntry.ProcessDefinitionKey);
		Assert.assertNull(procDefEntry.ProcessInstanceId);
		Assert.assertEquals(sourceProcessDefinition.Id, procDefEntry.OrgValue);
		Assert.assertEquals(targetProcessDefinition.Id, procDefEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, procDefEntry.Category);

		UserOperationLogEntry asyncEntry = entries["async"];
		Assert.assertNotNull(asyncEntry);
		Assert.assertEquals("ProcessInstance", asyncEntry.EntityType);
		Assert.assertEquals("Migrate", asyncEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, asyncEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, asyncEntry.ProcessDefinitionKey);
		Assert.assertNull(asyncEntry.ProcessInstanceId);
		Assert.assertNull(asyncEntry.OrgValue);
		Assert.assertEquals("false", asyncEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, asyncEntry.Category);

		UserOperationLogEntry numInstanceEntry = entries["nrOfInstances"];
		Assert.assertNotNull(numInstanceEntry);
		Assert.assertEquals("ProcessInstance", numInstanceEntry.EntityType);
		Assert.assertEquals("Migrate", numInstanceEntry.OperationType);
		Assert.assertEquals(sourceProcessDefinition.Id, numInstanceEntry.ProcessDefinitionId);
		Assert.assertEquals(sourceProcessDefinition.Key, numInstanceEntry.ProcessDefinitionKey);
		Assert.assertNull(numInstanceEntry.ProcessInstanceId);
		Assert.assertNull(numInstanceEntry.OrgValue);
		Assert.assertEquals("1", numInstanceEntry.NewValue);
		Assert.assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, numInstanceEntry.Category);

		Assert.assertEquals(procDefEntry.OperationId, asyncEntry.OperationId);
		Assert.assertEquals(asyncEntry.OperationId, numInstanceEntry.OperationId);
	  }

	  protected internal virtual IDictionary<string, UserOperationLogEntry> asMap(IList<UserOperationLogEntry> logEntries)
	  {
		IDictionary<string, UserOperationLogEntry> map = new Dictionary<string, UserOperationLogEntry>();

		foreach (UserOperationLogEntry entry in logEntries)
		{

		  UserOperationLogEntry previousValue = map[entry.Property] = entry;
		  if (previousValue != null)
		  {
			Assert.fail("expected only entry for every property");
		  }
		}

		return map;
	  }
	}

}