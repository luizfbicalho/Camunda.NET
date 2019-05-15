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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;
	using Sets = org.python.google.common.collect.Sets;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	public class UpdateProcessInstancesSuspendStateAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public UpdateProcessInstancesSuspendStateAsyncTest()
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
			helper = new BatchSuspensionHelper(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal BatchSuspensionHelper helper;

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
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchSuspensionById()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchSuspensionById()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);


		// then
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchActivationById()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchActivationById()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);
		Batch activateprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id)).activateAsync();
		helper.executeSeedJob(activateprocess);
		helper.executeJobs(activateprocess);


		// then
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

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().active()).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);


		// then
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testBatchActivationByProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testBatchActivationByProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().active()).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);
		Batch activateprocess = runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceQuery(runtimeService.createProcessInstanceQuery().suspended()).activateAsync();
		helper.executeSeedJob(activateprocess);
		helper.executeJobs(activateprocess);


		// then
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

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);


		// then
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertTrue(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertTrue(p2c.Suspended);

	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testBatchActivationByHistoricProcessInstanceQuery()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testBatchActivationByHistoricProcessInstanceQuery()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		// when
		Batch suspendprocess = runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).suspendAsync();
		helper.executeSeedJob(suspendprocess);
		helper.executeJobs(suspendprocess);
		Batch activateprocess = runtimeService.updateProcessInstanceSuspensionState().byHistoricProcessInstanceQuery(historyService.createHistoricProcessInstanceQuery().processInstanceIds(Sets.newHashSet(processInstance1.Id, processInstance2.Id))).activateAsync();
		helper.executeSeedJob(activateprocess);
		helper.executeJobs(activateprocess);


		// then
		ProcessInstance p1c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance1.Id).singleResult();
		assertFalse(p1c.Suspended);
		ProcessInstance p2c = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance2.Id).singleResult();
		assertFalse(p2c.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceListSuspendAsync()
	  public virtual void testEmptyProcessInstanceListSuspendAsync()
	  {
		// given
		// nothing

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("No process instance ids given");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds().suspendAsync();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyProcessInstanceListActivateAsync()
	  public virtual void testEmptyProcessInstanceListActivateAsync()
	  {
		// given
		// nothing

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("No process instance ids given");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds().activateAsync();


	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testNullProcessInstanceListActivateAsync()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testNullProcessInstanceListActivateAsync()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot be null");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id, null)).activateAsync();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"}) public void testNullProcessInstanceListSuspendAsync()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml"})]
	  public virtual void testNullProcessInstanceListSuspendAsync()
	  {
		// given
		ProcessInstance processInstance1 = runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");
		ProcessInstance processInstance2 = runtimeService.startProcessInstanceByKey("twoExternalTaskProcess");

		thrown.expect(typeof(BadUserRequestException));
		thrown.expectMessage("Cannot be null");

		// when
		runtimeService.updateProcessInstanceSuspensionState().byProcessInstanceIds(Arrays.asList(processInstance1.Id, processInstance2.Id, null)).suspendAsync();

	  }
	}

}