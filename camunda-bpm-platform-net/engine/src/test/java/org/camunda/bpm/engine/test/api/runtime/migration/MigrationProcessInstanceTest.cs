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
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	public class MigrationProcessInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationProcessInstanceTest()
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


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateWithIdVarargsArray()
	  public virtual void testMigrateWithIdVarargsArray()
	  {
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceById(sourceDefinition.Id);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance1.Id, processInstance2.Id).execute();

		// then
		Assert.assertEquals(2, runtimeService.createProcessInstanceQuery().processDefinitionId(targetDefinition.Id).count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullMigrationPlan()
	  public virtual void testNullMigrationPlan()
	  {
		try
		{
		  runtimeService.newMigration(null).processInstanceIds(System.Linq.Enumerable.Empty<string>()).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("migration plan is null"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceIdsList()
	  public virtual void testNullProcessInstanceIdsList()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds((IList<string>) null).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceIdsListWithNullValue()
	  public virtual void testProcessInstanceIdsListWithNullValue()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList("foo", null, "bar")).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceIdsArray()
	  public virtual void testNullProcessInstanceIdsArray()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds((string[]) null).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceIdsArrayWithNullValue()
	  public virtual void testProcessInstanceIdsArrayWithNullValue()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds("foo", null, "bar").execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceIdsList()
	  public virtual void testEmptyProcessInstanceIdsList()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(System.Linq.Enumerable.Empty<string>()).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceIdsArray()
	  public virtual void testEmptyProcessInstanceIdsArray()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(new string[]{}).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateProcessInstanceOfWrongProcessDefinition()
	  public virtual void testNotMigrateProcessInstanceOfWrongProcessDefinition()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition wrongProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(wrongProcessDefinition.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList(processInstance.Id)).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.StartsWith("ENGINE-23002"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateUnknownProcessInstance()
	  public virtual void testNotMigrateUnknownProcessInstance()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList("unknown")).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.StartsWith("ENGINE-23003"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNotMigrateNullProcessInstance()
	  public virtual void testNotMigrateNullProcessInstance()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList<string>(null)).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids contains null value"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateProcessInstanceQuery()
	  public virtual void testMigrateProcessInstanceQuery()
	  {
		int processInstanceCount = 10;

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		for (int i = 0; i < processInstanceCount; i++)
		{
		  runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		}

		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		ProcessInstanceQuery targetProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(targetProcessDefinition.Id);

		assertEquals(processInstanceCount, sourceProcessInstanceQuery.count());
		assertEquals(0, targetProcessInstanceQuery.count());


		runtimeService.newMigration(migrationPlan).processInstanceQuery(sourceProcessInstanceQuery).execute();

		assertEquals(0, sourceProcessInstanceQuery.count());
		assertEquals(processInstanceCount, targetProcessInstanceQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNullProcessInstanceQuery()
	  public virtual void testNullProcessInstanceQuery()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceQuery(null).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceQuery()
	  public virtual void testEmptyProcessInstanceQuery()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery emptyProcessInstanceQuery = runtimeService.createProcessInstanceQuery();
		assertEquals(0, emptyProcessInstanceQuery.count());

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceQuery(emptyProcessInstanceQuery).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.containsString("process instance ids is empty"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceQueryOfWrongProcessDefinition()
	  public virtual void testProcessInstanceQueryOfWrongProcessDefinition()
	  {
		ProcessDefinition testProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition wrongProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		runtimeService.startProcessInstanceById(wrongProcessDefinition.Id);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstanceQuery wrongProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(wrongProcessDefinition.Id);
		assertEquals(1, wrongProcessInstanceQuery.count());

		try
		{
		  runtimeService.newMigration(migrationPlan).processInstanceQuery(wrongProcessInstanceQuery).execute();
		  fail("Should not be able to migrate");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, CoreMatchers.StartsWith("ENGINE-23002"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessInstanceIdsAndQuery()
	  public virtual void testProcessInstanceIdsAndQuery()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id);
		ProcessInstanceQuery targetProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(targetProcessDefinition.Id);

		assertEquals(0, targetProcessInstanceQuery.count());

		runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList(processInstance1.Id)).processInstanceQuery(sourceProcessInstanceQuery).execute();

		assertEquals(2, targetProcessInstanceQuery.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOverlappingProcessInstanceIdsAndQuery()
	  public virtual void testOverlappingProcessInstanceIdsAndQuery()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance1 = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		ProcessInstanceQuery targetProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(targetProcessDefinition.Id);

		assertEquals(0, targetProcessInstanceQuery.count());

		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).processInstanceQuery(sourceProcessInstanceQuery).execute();

		assertEquals(2, targetProcessInstanceQuery.count());
	  }

	}

}