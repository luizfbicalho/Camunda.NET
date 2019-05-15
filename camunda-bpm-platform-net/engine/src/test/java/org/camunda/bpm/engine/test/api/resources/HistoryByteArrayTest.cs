using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.resources
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.repository.ResourceTypes.HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils;
	using RuntimeSqlException = org.apache.ibatis.jdbc.RuntimeSqlException;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoricDecisionInputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInputInstanceEntity;
	using HistoricDecisionOutputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionOutputInstanceEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using JavaSerializable = org.camunda.bpm.engine.test.api.variables.JavaSerializable;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoryByteArrayTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryByteArrayTest()
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

	  protected internal const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";
	  protected internal const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";
	  protected internal const string WORKER_ID = "aWorkerId";
	  protected internal const long LOCK_TIME = 10000L;
	  protected internal const string TOPIC_NAME = "externalTaskTopic";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ExternalTaskService externalTaskService;

	  protected internal string taskId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		configuration = engineRule.ProcessEngineConfiguration;
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		externalTaskService = engineRule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (!string.ReferenceEquals(taskId, null))
		{
		  // delete task
		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableBinaryForFileValues()
	  public virtual void testHistoricVariableBinaryForFileValues()
	  {
		// given
		BpmnModelInstance instance = createProcess();

		testRule.deploy(instance);
		FileValue fileValue = createFile();

		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValueTyped("fileVar", fileValue));

		string byteArrayValueId = ((HistoricVariableInstanceEntity)historyService.createHistoricVariableInstanceQuery().singleResult()).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableBinary()
	  public virtual void testHistoricVariableBinary()
	  {
		sbyte[] binaryContent = "some binary content".GetBytes();

		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["binaryVariable"] = binaryContent;
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskId = task.Id;
		taskService.setVariablesLocal(taskId, variables);

		string byteArrayValueId = ((HistoricVariableInstanceEntity)historyService.createHistoricVariableInstanceQuery().singleResult()).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDetailBinaryForFileValues()
	  public virtual void testHistoricDetailBinaryForFileValues()
	  {
		// given
		BpmnModelInstance instance = createProcess();

		testRule.deploy(instance);
		FileValue fileValue = createFile();

		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValueTyped("fileVar", fileValue));

		string byteArrayValueId = ((HistoricDetailVariableInstanceUpdateEntity) historyService.createHistoricDetailQuery().singleResult()).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDecisionInputInstanceBinary()
	  public virtual void testHistoricDecisionInputInstanceBinary()
	  {
		testRule.deploy(DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN);

		startProcessInstanceAndEvaluateDecision(new JavaSerializable("foo"));

		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().includeInputs().singleResult();
		IList<HistoricDecisionInputInstance> inputInstances = historicDecisionInstance.Inputs;
		assertEquals(1, inputInstances.Count);

		string byteArrayValueId = ((HistoricDecisionInputInstanceEntity) inputInstances[0]).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricDecisionOutputInstanceBinary()
	  public virtual void testHistoricDecisionOutputInstanceBinary()
	  {
		testRule.deploy(DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN);

		startProcessInstanceAndEvaluateDecision(new JavaSerializable("foo"));

		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().includeOutputs().singleResult();
		IList<HistoricDecisionOutputInstance> outputInstances = historicDecisionInstance.Outputs;
		assertEquals(1, outputInstances.Count);


		string byteArrayValueId = ((HistoricDecisionOutputInstanceEntity) outputInstances[0]).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAttachmentContentBinaries()
	  public virtual void testAttachmentContentBinaries()
	  {
		  // create and save task
		  Task task = taskService.newTask();
		  taskService.saveTask(task);
		  taskId = task.Id;

		  // when
		  AttachmentEntity attachment = (AttachmentEntity) taskService.createAttachment("web page", taskId, "someprocessinstanceid", "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));

		  ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(attachment.ContentId));

		  checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExceptionStacktraceBinary()
	  public virtual void testHistoricExceptionStacktraceBinary()
	  {
		// given
		BpmnModelInstance instance = createFailingProcess();
		testRule.deploy(instance);
		runtimeService.startProcessInstanceByKey("Process");
		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		try
		{
		  managementService.executeJob(jobId);
		  fail();
		}
		catch (Exception)
		{
		  // expected
		}

		HistoricJobLogEventEntity entity = (HistoricJobLogEventEntity) historyService.createHistoricJobLogQuery().failureLog().singleResult();
		assertNotNull(entity);

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(entity.ExceptionByteArrayId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskJobLogStacktraceBinary()
	  public virtual void testHistoricExternalTaskJobLogStacktraceBinary()
	  {
		// given
		testRule.deploy("org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml");
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic(TOPIC_NAME, LOCK_TIME).execute();

		LockedExternalTask task = tasks[0];

		// submitting a failure (after a simulated processing time of three seconds)
		ClockUtil.CurrentTime = nowPlus(3000L);

		string errorMessage;
		string exceptionStackTrace;
		try
		{
		  throw new RuntimeSqlException("test cause");
		}
		catch (Exception e)
		{
		  exceptionStackTrace = ExceptionUtils.getStackTrace(e);
		  errorMessage = e.Message;
		}
		assertNotNull(exceptionStackTrace);

		externalTaskService.handleFailure(task.Id, WORKER_ID, errorMessage, exceptionStackTrace, 5, 3000L);

		HistoricExternalTaskLogEntity entity = (HistoricExternalTaskLogEntity) historyService.createHistoricExternalTaskLogQuery().errorMessage(errorMessage).singleResult();
		assertNotNull(entity);

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(entity.ErrorDetailsByteArrayId));

		// then
		checkBinary(byteArrayEntity);
	  }

	  protected internal virtual void checkBinary(ByteArrayEntity byteArrayEntity)
	  {
		assertNotNull(byteArrayEntity);
		assertNotNull(byteArrayEntity.CreateTime);
		assertEquals(HISTORY.Value, byteArrayEntity.Type);
	  }

	  protected internal virtual FileValue createFile()
	  {
		string fileName = "text.txt";
		string encoding = "crazy-encoding";
		string mimeType = "martini/dry";

		FileValue fileValue = Variables.fileValue(fileName).file("ABC".GetBytes()).encoding(encoding).mimeType(mimeType).create();
		return fileValue;
	  }

	  protected internal virtual BpmnModelInstance createProcess()
	  {
		return Bpmn.createExecutableProcess("Process").startEvent().userTask("user").endEvent().done();
	  }

	  protected internal virtual BpmnModelInstance createFailingProcess()
	  {
		return Bpmn.createExecutableProcess("Process").startEvent().serviceTask("failing").camundaAsyncAfter().camundaAsyncBefore().camundaClass(typeof(FailingDelegate)).endEvent().done();
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecision(object input)
	  {
		return engineRule.RuntimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("input1", input));
	  }


	  protected internal virtual DateTime nowPlus(long millis)
	  {
		return new DateTime(ClockUtil.CurrentTime.Ticks + millis);
	  }
	}

}