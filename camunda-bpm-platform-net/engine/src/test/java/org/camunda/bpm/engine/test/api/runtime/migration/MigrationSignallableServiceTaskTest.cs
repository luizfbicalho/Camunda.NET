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
//	import static org.camunda.bpm.engine.test.util.MigratingProcessInstanceValidationReportAssert.assertThat;

	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationSignallableServiceTaskTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationSignallableServiceTaskTest()
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateActivityInstance()
	  public virtual void testCannotMigrateActivityInstance()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = ProcessModels.newModel().startEvent().serviceTask("serviceTask").camundaClass(typeof(SignallableServiceTaskDelegate).FullName).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("serviceTask", "serviceTask").build();

		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasActivityInstanceFailures("serviceTask", "The type of the source activity is not supported for activity instance migration");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateAsyncActivityInstance()
	  public virtual void testCannotMigrateAsyncActivityInstance()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = ProcessModels.newModel().startEvent().serviceTask("serviceTask").camundaAsyncBefore().camundaClass(typeof(SignallableServiceTaskDelegate).FullName).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("serviceTask", "serviceTask").build();

		string processInstanceId = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id).Id;
		testHelper.executeAvailableJobs();

		// when
		try
		{
		  rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstanceId).execute();

		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasActivityInstanceFailures("serviceTask", "The type of the source activity is not supported for activity instance migration");
		}
	  }

	  public class SignallableServiceTaskDelegate : SignallableActivityBehavior
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
		public virtual void signal(ActivityExecution execution, string signalEvent, object signalData)
		{
		  PvmTransition transition = execution.Activity.OutgoingTransitions[0];
		  execution.leaveActivityViaTransition(transition);
		}

	  }
	}

}