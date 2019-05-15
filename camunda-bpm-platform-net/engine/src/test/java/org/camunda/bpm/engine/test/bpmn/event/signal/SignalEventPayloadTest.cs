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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
{
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class SignalEventPayloadTest
	{
		private bool InstanceFieldsInitialized = false;

		public SignalEventPayloadTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }


	  /// <summary>
	  /// Test case for CAM-8820 with a catching Start Signal event.
	  /// Using Source and Target Variable name mapping attributes.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml" }) public void testSignalPayloadStart()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml" })]
	  public virtual void testSignalPayloadStart()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["payloadVar1"] = "payloadVal1";
		variables["payloadVar2"] = "payloadVal2";

		// when
		runtimeService.startProcessInstanceByKey("throwPayloadSignal", variables);

		// then
		Task catchingPiUserTask = taskService.createTaskQuery().singleResult();

		IList<VariableInstance> catchingPiVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPiUserTask.ProcessInstanceId).list();
		assertEquals(2, catchingPiVariables.Count);

		foreach (VariableInstance variable in catchingPiVariables)
		{
		  if (variable.Name.Equals("payloadVar1Target"))
		  {
			assertEquals("payloadVal1", variable.Value);
		  }
		  else
		  {
			assertEquals("payloadVal2", variable.Value);
		  }
		}
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with a catching Intermediate Signal event.
	  /// Using Source and Target Variable name mapping attributes.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" }) public void testSignalPayloadIntermediate()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" })]
	  public virtual void testSignalPayloadIntermediate()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["payloadVar1"] = "payloadVal1";
		variables["payloadVar2"] = "payloadVal2";
		ProcessInstance catchingPI = runtimeService.startProcessInstanceByKey("catchIntermediatePayloadSignal");

		// when
		runtimeService.startProcessInstanceByKey("throwPayloadSignal", variables);

		// then
		IList<VariableInstance> catchingPiVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPI.Id).list();
		assertEquals(2, catchingPiVariables.Count);

		foreach (VariableInstance variable in catchingPiVariables)
		{
		  if (variable.Name.Equals("payloadVar1Target"))
		  {
			assertEquals("payloadVal1", variable.Value);
		  }
		  else
		  {
			assertEquals("payloadVal2", variable.Value);
		  }
		}
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with an expression as a source.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithExpressionPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" }) public void testSignalSourceExpressionPayload()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithExpressionPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" })]
	  public virtual void testSignalSourceExpressionPayload()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["payloadVar"] = "Val";
		ProcessInstance catchingPI = runtimeService.startProcessInstanceByKey("catchIntermediatePayloadSignal");

		// when
		runtimeService.startProcessInstanceByKey("throwExpressionPayloadSignal", variables);

		// then
		IList<VariableInstance> catchingPiVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPI.Id).list();
		assertEquals(1, catchingPiVariables.Count);

		assertEquals("srcExpressionResVal", catchingPiVariables[0].Name);
		assertEquals("sourceVal", catchingPiVariables[0].Value);
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with all the (global) source variables
	  /// as the signal payload.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithAllVariablesPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" }) public void testSignalAllSourceVariablesPayload()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithAllVariablesPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" })]
	  public virtual void testSignalAllSourceVariablesPayload()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["payloadVar1"] = "payloadVal1";
		variables["payloadVar2"] = "payloadVal2";
		ProcessInstance catchingPI = runtimeService.startProcessInstanceByKey("catchIntermediatePayloadSignal");

		// when
		runtimeService.startProcessInstanceByKey("throwPayloadSignal", variables);

		// then
		IList<VariableInstance> catchingPiVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPI.Id).list();
		assertEquals(2, catchingPiVariables.Count);

		foreach (VariableInstance variable in catchingPiVariables)
		{
		  if (variable.Name.Equals("payloadVar1"))
		  {
			assertEquals("payloadVal1", variable.Value);
		  }
		  else
		  {
			assertEquals("payloadVal2", variable.Value);
		  }
		}
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with all the (local) source variables
	  /// as the signal payload.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwEndSignalEventWithAllLocalVariablesPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" }) public void testSignalAllLocalSourceVariablesPayload()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwEndSignalEventWithAllLocalVariablesPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadIntermediate.bpmn20.xml" })]
	  public virtual void testSignalAllLocalSourceVariablesPayload()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["payloadVar1"] = "payloadVal1";
		string localVar1 = "localVar1";
		string localVal1 = "localVal1";
		string localVar2 = "localVar2";
		string localVal2 = "localVal2";
		ProcessInstance catchingPI = runtimeService.startProcessInstanceByKey("catchIntermediatePayloadSignal");

		// when
		runtimeService.startProcessInstanceByKey("throwPayloadSignal", variables);

		// then
		IList<VariableInstance> catchingPiVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPI.Id).list();
		assertEquals(2, catchingPiVariables.Count);

		foreach (VariableInstance variable in catchingPiVariables)
		{
		  if (variable.Name.Equals(localVar1))
		  {
			assertEquals(localVal1, variable.Value);
		  }
		  else
		  {
			assertEquals(localVal2, variable.Value);
		  }
		}
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with a Business Key
	  /// as signal payload.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithBusinessKeyPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml" }) public void testSignalBusinessKeyPayload()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithBusinessKeyPayload.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml" })]
	  public virtual void testSignalBusinessKeyPayload()
	  {
		// given
		string businessKey = "aBusinessKey";

		// when
		runtimeService.startProcessInstanceByKey("throwBusinessKeyPayloadSignal", businessKey);

		// then
		ProcessInstance catchingPI = runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(businessKey, catchingPI.BusinessKey);
	  }

	  /// <summary>
	  /// Test case for CAM-8820 with all possible options for a signal payload.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithAllOptions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml"}) public void testSignalPayloadWithAllOptions()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.throwSignalWithAllOptions.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/event/signal/SignalEventPayloadTests.catchSignalWithPayloadStart.bpmn20.xml"})]
	  public virtual void testSignalPayloadWithAllOptions()
	  {
		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		string globalVar1 = "payloadVar1";
		string globalVal1 = "payloadVar1";
		string globalVar2 = "payloadVar2";
		string globalVal2 = "payloadVal2";
		variables[globalVar1] = globalVal1;
		variables[globalVar2] = globalVal2;
		string localVar1 = "localVar1";
		string localVal1 = "localVal1";
		string localVar2 = "localVar2";
		string localVal2 = "localVal2";
		string businessKey = "aBusinessKey";

		// when
		runtimeService.startProcessInstanceByKey("throwCompletePayloadSignal", businessKey, variables);

		// then
		Task catchingPiUserTask = taskService.createTaskQuery().singleResult();
		ProcessInstance catchingPI = runtimeService.createProcessInstanceQuery().processInstanceId(catchingPiUserTask.ProcessInstanceId).singleResult();
		assertEquals(businessKey, catchingPI.BusinessKey);

		IList<VariableInstance> targetVariables = runtimeService.createVariableInstanceQuery().processInstanceIdIn(catchingPiUserTask.ProcessInstanceId).list();
		assertEquals(4, targetVariables.Count);

		foreach (VariableInstance variable in targetVariables)
		{
		  if (variable.Name.Equals(globalVar1 + "Target"))
		  {
			assertEquals(globalVal1, variable.Value);
		  }
		  else if (variable.Name.Equals(globalVar2 + "Target"))
		  {
			assertEquals(globalVal2 + "Source", variable.Value);
		  }
		  else if (variable.Name.Equals(localVar1))
		  {
			assertEquals(localVal1, variable.Value);
		  }
		  else if (variable.Name.Equals(localVar2))
		  {
			assertEquals(localVal2, variable.Value);
		  }
		}
	  }
	}

}