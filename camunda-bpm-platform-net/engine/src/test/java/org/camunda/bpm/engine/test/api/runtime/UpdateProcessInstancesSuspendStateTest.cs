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
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
	using Sets = org.python.google.common.collect.Sets;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	public class UpdateProcessInstancesSuspendStateTest
	{
		private bool InstanceFieldsInitialized = false;

		public UpdateProcessInstancesSuspendStateTest()
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


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();


	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchSuspensionById()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchSuspensionById()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(processInstance1.Id, processInstance2.Id).suspend();

		// Update the process instances and they are suspended
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchActivatationById()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchActivatationById()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(processInstance1.Id, processInstance2.Id).suspend();

		// when they are activated again
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(processInstance1.Id, processInstance2.Id).activate();

		// Update the process instances and they are active again
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(p2c.Suspended);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchSuspensionByIdArray()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchSuspensionByIdArray()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspend();

		// Update the process instances and they are suspended
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchActivatationByIdArray()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchActivatationByIdArray()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspend();

		// when they are activated again
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).activate();


		// Update the process instances and they are active again
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchSuspensionByProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchSuspensionByProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().active()).suspend();

		// Update the process instances and they are suspended
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchActivatationByProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchActivatationByProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().active()).suspend();


		// when they are activated again
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().suspended()).activate();


		// Update the process instances and they are active again
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(p2c.Suspended);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testBatchSuspensionByHistoricProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testBatchSuspensionByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");


		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).suspend();

		// Update the process instances and they are suspended
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testBatchActivatationByHistoricProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testBatchActivatationByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");


		// when the process instances are suspended
		runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).suspend();

		// when they are activated again
		runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).activate();


		// Update the process instances and they are active again
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceListSuspend()
	  public virtual void testEmptyProcessInstanceListSuspend()
	  {
		// given
		// nothing

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("No process instance ids given");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds().suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceListActivateUpdateProcessInstancesSuspendStateAsyncTest()
	  public virtual void testEmptyProcessInstanceListActivateUpdateProcessInstancesSuspendStateAsyncTest()
	  {
		// given
		// nothing

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("No process instance ids given");

	  // when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds().activate();

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testNullProcessInstanceListActivate()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testNullProcessInstanceListActivate()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot be null");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id, null)).activate();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testNullProcessInstanceListSuspend()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testNullProcessInstanceListSuspend()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot be null");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id, null)).suspend();

	  }

	}

}