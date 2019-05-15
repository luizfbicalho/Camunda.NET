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
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	public class MultiTenancySignalReceiveTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySignalReceiveTest()
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

	  protected internal static readonly BpmnModelInstance SIGNAL_START_PROCESS = Bpmn.createExecutableProcess("signalStart").startEvent().signal("signal").userTask().endEvent().done();

	  protected internal static readonly BpmnModelInstance SIGNAL_CATCH_PROCESS = Bpmn.createExecutableProcess("signalCatch").startEvent().intermediateCatchEvent().signal("signal").userTask().endEvent().done();

	  protected internal static readonly BpmnModelInstance SIGNAL_INTERMEDIATE_THROW_PROCESS = Bpmn.createExecutableProcess("signalThrow").startEvent().intermediateThrowEvent().signal("signal").endEvent().done();

	  protected internal static readonly BpmnModelInstance SIGNAL_END_THROW_PROCESS = Bpmn.createExecutableProcess("signalThrow").startEvent().endEvent().signal("signal").done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown= org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventForNonTenant()
	  public virtual void sendSignalToStartEventForNonTenant()
	  {
		testRule.deploy(SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createSignalEvent("signal").withoutTenantId().send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventForTenant()
	  public virtual void sendSignalToStartEventForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_ONE).send();
		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_TWO).send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventWithoutTenantIdForNonTenant()
	  public virtual void sendSignalToStartEventWithoutTenantIdForNonTenant()
	  {
		testRule.deploy(SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventWithoutTenantIdForTenant()
	  public virtual void sendSignalToStartEventWithoutTenantIdForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventForNonTenant()
	  public virtual void sendSignalToIntermediateCatchEventForNonTenant()
	  {
		testRule.deploy(SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.createSignalEvent("signal").withoutTenantId().send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
		assertThat(query.singleResult().TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventForTenant()
	  public virtual void sendSignalToIntermediateCatchEventForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_ONE).send();
		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_TWO).send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventWithoutTenantIdForNonTenant()
	  public virtual void sendSignalToIntermediateCatchEventWithoutTenantIdForNonTenant()
	  {
		testRule.deploy(SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").execute();

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventWithoutTenantIdForTenant()
	  public virtual void sendSignalToIntermediateCatchEventWithoutTenantIdForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").execute();

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventForNonTenant()
	  public virtual void sendSignalToStartAndIntermediateCatchEventForNonTenant()
	  {
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.createSignalEvent("signal").withoutTenantId().send();

		IList<Task> tasks = engineRule.TaskService.createTaskQuery().list();
		assertThat(tasks.Count, @is(2));
		assertThat(tasks[0].TenantId, @is(nullValue()));
		assertThat(tasks[1].TenantId, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventForTenant()
	  public virtual void sendSignalToStartAndIntermediateCatchEventForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_ONE).send();
		engineRule.RuntimeService.createSignalEvent("signal").tenantId(TENANT_TWO).send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(2L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartEventsForMultipleTenants()
	  public virtual void sendSignalToStartEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createSignalEvent("signal").send();

		ProcessInstanceQuery query = engineRule.RuntimeService.createProcessInstanceQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToIntermediateCatchEventsForMultipleTenants()
	  public virtual void sendSignalToIntermediateCatchEventsForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void sendSignalToStartAndIntermediateCatchEventForMultipleTenants()
	  public virtual void sendSignalToStartAndIntermediateCatchEventForMultipleTenants()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_CATCH_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").execute();

		engineRule.RuntimeService.createSignalEvent("signal").send();

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToSendSignalWithExecutionIdForTenant()
	  public virtual void failToSendSignalWithExecutionIdForTenant()
	  {

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot specify a tenant-id when deliver a signal to a single execution.");

		engineRule.RuntimeService.createSignalEvent("signal").executionId("id").tenantId(TENANT_ONE).send();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void throwIntermediateSignalForTenant()
	  public virtual void throwIntermediateSignalForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_INTERMEDIATE_THROW_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.startProcessInstanceByKey("signalThrow");

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void throwIntermediateSignalForNonTenant()
	  public virtual void throwIntermediateSignalForNonTenant()
	  {
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_INTERMEDIATE_THROW_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.startProcessInstanceByKey("signalThrow");

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void throwEndSignalForTenant()
	  public virtual void throwEndSignalForTenant()
	  {
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_END_THROW_PROCESS);
		testRule.deployForTenant(TENANT_TWO, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_TWO).execute();

		engineRule.RuntimeService.startProcessInstanceByKey("signalThrow");

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void throwEndSignalForNonTenant()
	  public virtual void throwEndSignalForNonTenant()
	  {
		testRule.deploy(SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS, SIGNAL_END_THROW_PROCESS);
		testRule.deployForTenant(TENANT_ONE, SIGNAL_START_PROCESS, SIGNAL_CATCH_PROCESS);

		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionWithoutTenantId().execute();
		engineRule.RuntimeService.createProcessInstanceByKey("signalCatch").processDefinitionTenantId(TENANT_ONE).execute();

		engineRule.RuntimeService.startProcessInstanceByKey("signalThrow");

		TaskQuery query = engineRule.TaskService.createTaskQuery();
		assertThat(query.withoutTenantId().count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(0L));
	  }
	}

}