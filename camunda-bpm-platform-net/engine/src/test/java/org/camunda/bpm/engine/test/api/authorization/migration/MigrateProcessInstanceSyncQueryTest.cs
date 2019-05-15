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
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrateProcessInstanceSyncQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrateProcessInstanceSyncQueryTest()
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

	  protected internal IList<Authorization> authorizations;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain chain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		authorizations = new List<Authorization>();
		authRule.createUserAndGroup("userId", "groupId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		foreach (Authorization authorization in authorizations)
		{
		  engineRule.AuthorizationService.deleteAuthorization(authorization.Id);
		}

		authRule.deleteUsersAndGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateWithQuery()
	  public virtual void testMigrateWithQuery()
	  {
		// given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		ProcessInstance instance1 = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);
		ProcessInstance instance2 = engineRule.RuntimeService.startProcessInstanceById(sourceDefinition.Id);

		grantAuthorization("user", Resources.PROCESS_INSTANCE, instance2.Id, Permissions.READ);
		grantAuthorization("user", Resources.PROCESS_DEFINITION, "*", Permissions.MIGRATE_INSTANCE);

		MigrationPlan migrationPlan = engineRule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();

		// when
		authRule.enableAuthorization("user");
		engineRule.RuntimeService.newMigration(migrationPlan).processInstanceQuery(query).execute();

		authRule.disableAuthorization();


		// then
		ProcessInstance instance1AfterMigration = engineRule.RuntimeService.createProcessInstanceQuery().processInstanceId(instance1.Id).singleResult();

		Assert.assertEquals(sourceDefinition.Id, instance1AfterMigration.ProcessDefinitionId);
	  }

	  protected internal virtual void grantAuthorization(string userId, Resource resource, string resourceId, Permission permission)
	  {
		Authorization authorization = engineRule.AuthorizationService.createNewAuthorization(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		authorization.Resource = resource;
		authorization.ResourceId = resourceId;
		authorization.addPermission(permission);
		authorization.UserId = userId;
		engineRule.AuthorizationService.saveAuthorization(authorization);
		authorizations.Add(authorization);
	  }
	}

}