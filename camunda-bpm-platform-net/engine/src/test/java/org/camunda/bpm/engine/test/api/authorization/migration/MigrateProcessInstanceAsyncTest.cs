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
namespace org.camunda.bpm.engine.test.api.authorization.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrateProcessInstanceAsyncTest
	public class MigrateProcessInstanceAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrateProcessInstanceAsyncTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public AuthorizationTestRule authRule;
	  public ProcessEngineTestRule testHelper;

	  protected internal Batch batch;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario scenario;
	  public AuthorizationScenario scenario;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "sourceDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE), grant(Resources.PROCESS_DEFINITION, "targetDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "sourceDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)).failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "sourceDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE), grant(Resources.PROCESS_DEFINITION, "targetDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "targetDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)).failsDueToRequired(grant(Resources.PROCESS_DEFINITION, "sourceDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE), grant(Resources.PROCESS_DEFINITION, "targetDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "sourceDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE), grant(Resources.PROCESS_DEFINITION, "targetDefinitionKey", "userId", Permissions.MIGRATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.MIGRATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES), grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ), grant(Resources.PROCESS_DEFINITION, "*", "userId", Permissions.MIGRATE_INSTANCE)).succeeds(), scenario().withAuthorizations(grant(Resources.PROCESS_INSTANCE, "processInstance", "userId", Permissions.READ)).failsDueToRequired(grant(Resources.BATCH, "*", "userId", Permissions.CREATE), grant(Resources.BATCH, "*", "userId", BatchPermissions.CREATE_BATCH_MIGRATE_PROCESS_INSTANCES)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		batch = null;
		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (batch != null)
		{
		  engineRule.ManagementService.deleteBatch(batch.Id, true);
		}
		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml") public void testMigrate()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml")]
	  public virtual void testMigrate()
	  {

		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		// when
		authRule.init(scenario).withUser("userId").bindResource("sourceDefinitionKey", sourceDefinition.Key).bindResource("targetDefinitionKey", targetDefinition.Key).bindResource("processInstance", processInstance.Id).start();

		batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).executeAsync();

		// then
		if (authRule.assertScenario(scenario))
		{
		  Assert.assertEquals("userId", batch.CreateUserId);

		  Assert.assertEquals(1, engineRule.ManagementService.createBatchQuery().count());
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml") public void testMigrateWithQuery()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml")]
	  public virtual void testMigrateWithQuery()
	  {

		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		ProcessInstance processInstance = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();

		// when
		authRule.init(scenario).withUser("userId").bindResource("sourceDefinitionKey", sourceDefinition.Key).bindResource("targetDefinitionKey", targetDefinition.Key).bindResource("processInstance", processInstance.Id).start();

		batch = engineRule.RuntimeService.newMigration(migrationPlan).processInstanceQuery(query).executeAsync();

		// then
		if (authRule.assertScenario(scenario))
		{
		  Assert.assertEquals(1, engineRule.ManagementService.createBatchQuery().count());
		}

	  }
	}

}