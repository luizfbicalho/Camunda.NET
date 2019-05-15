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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using DeploymentWithDefinitions = org.camunda.bpm.engine.repository.DeploymentWithDefinitions;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class RootProcessInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public RootProcessInstanceTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			CALLED_PROCESS = Bpmn.createExecutableProcess(CALLED_PROCESS_KEY).startEvent().userTask("userTask").endEvent().done();
			CALLED_AND_CALLING_PROCESS = Bpmn.createExecutableProcess(CALLED_AND_CALLING_PROCESS_KEY).startEvent().callActivity().calledElement(CALLED_PROCESS_KEY).endEvent().done();
			CALLING_PROCESS = Bpmn.createExecutableProcess(CALLING_PROCESS_KEY).startEvent().callActivity().calledElement(CALLED_AND_CALLING_PROCESS_KEY).endEvent().done();
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal readonly string CALLED_PROCESS_KEY = "calledProcess";
	  protected internal BpmnModelInstance CALLED_PROCESS;

	  protected internal readonly string CALLED_AND_CALLING_PROCESS_KEY = "calledAndCallingProcess";
	  protected internal BpmnModelInstance CALLED_AND_CALLING_PROCESS;

	  protected internal readonly string CALLING_PROCESS_KEY = "callingProcess";
	  protected internal BpmnModelInstance CALLING_PROCESS;

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal FormService formService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		formService = engineRule.FormService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPointToItself()
	  public virtual void shouldPointToItself()
	  {
		// given
		testRule.deploy(CALLED_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(CALLED_PROCESS_KEY);

		// assume
		assertThat(processInstance.RootProcessInstanceId, notNullValue());

		// then
		assertThat(processInstance.RootProcessInstanceId, @is(processInstance.ProcessInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPointToItselfBySubmittingStartForm()
	  public virtual void shouldPointToItselfBySubmittingStartForm()
	  {
		// given
		DeploymentWithDefinitions deployment = testRule.deploy(CALLED_PROCESS);

		string processDefinitionId = deployment.DeployedProcessDefinitions[0].Id;
		IDictionary<string, object> properties = new Dictionary<string, object>();

		// when
		ProcessInstance processInstance = formService.submitStartForm(processDefinitionId, properties);

		// assume
		assertThat(processInstance.RootProcessInstanceId, notNullValue());

		// then
		assertThat(processInstance.RootProcessInstanceId, @is(processInstance.ProcessInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPointToItselfByStartingAtActivity()
	  public virtual void shouldPointToItselfByStartingAtActivity()
	  {
		// given
		testRule.deploy(CALLED_PROCESS);

		// when
		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey(CALLED_PROCESS_KEY).startAfterActivity("userTask").execute();

		// assume
		assertThat(processInstance.RootProcessInstanceId, notNullValue());

		// then
		assertThat(processInstance.RootProcessInstanceId, @is(processInstance.ProcessInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPointToRoot()
	  public virtual void shouldPointToRoot()
	  {
		// given
		testRule.deploy(CALLED_PROCESS);
		testRule.deploy(CALLED_AND_CALLING_PROCESS);
		testRule.deploy(CALLING_PROCESS);

		// when
		runtimeService.startProcessInstanceByKey(CALLING_PROCESS_KEY);

		ProcessInstance calledProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey(CALLED_PROCESS_KEY).singleResult();

		ProcessInstance calledAndCallingProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey(CALLED_AND_CALLING_PROCESS_KEY).singleResult();

		ProcessInstance callingProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey(CALLING_PROCESS_KEY).singleResult();

		// assume
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(3L));
		assertThat(callingProcessInstance.RootProcessInstanceId, notNullValue());

		// then
		assertThat(callingProcessInstance.RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
		assertThat(calledProcessInstance.RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
		assertThat(calledAndCallingProcessInstance.RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPointToRootWithInitialCallAfterParallelGateway()
	  public virtual void shouldPointToRootWithInitialCallAfterParallelGateway()
	  {
		// given
		testRule.deploy(CALLED_PROCESS);

		testRule.deploy(CALLED_AND_CALLING_PROCESS);

		testRule.deploy(Bpmn.createExecutableProcess("callingProcessWithGateway").startEvent().parallelGateway("split").callActivity().calledElement(CALLED_AND_CALLING_PROCESS_KEY).moveToNode("split").callActivity().calledElement(CALLED_AND_CALLING_PROCESS_KEY).endEvent().done());

		// when
		runtimeService.startProcessInstanceByKey("callingProcessWithGateway");

		IList<ProcessInstance> calledProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey(CALLED_PROCESS_KEY).list();

		IList<ProcessInstance> calledAndCallingProcessInstances = runtimeService.createProcessInstanceQuery().processDefinitionKey(CALLED_AND_CALLING_PROCESS_KEY).list();

		ProcessInstance callingProcessInstance = runtimeService.createProcessInstanceQuery().processDefinitionKey("callingProcessWithGateway").singleResult();

		// assume
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(5L));
		assertThat(callingProcessInstance.ProcessInstanceId, notNullValue());

		assertThat(calledProcessInstances.Count, @is(2));
		assertThat(calledAndCallingProcessInstances.Count, @is(2));

		// then
		assertThat(callingProcessInstance.RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));

		assertThat(calledProcessInstances[0].RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
		assertThat(calledProcessInstances[1].RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));

		assertThat(calledAndCallingProcessInstances[0].RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
		assertThat(calledAndCallingProcessInstances[1].RootProcessInstanceId, @is(callingProcessInstance.ProcessInstanceId));
	  }

	}

}