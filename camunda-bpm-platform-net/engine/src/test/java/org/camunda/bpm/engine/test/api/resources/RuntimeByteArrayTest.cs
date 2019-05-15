using System;
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
namespace org.camunda.bpm.engine.test.api.resources
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.repository.ResourceTypes.RUNTIME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils;
	using RuntimeSqlException = org.apache.ibatis.jdbc.RuntimeSqlException;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Task = org.camunda.bpm.engine.task.Task;
	using FailingDelegate = org.camunda.bpm.engine.test.api.runtime.FailingDelegate;
	using MigrationTestRule = org.camunda.bpm.engine.test.api.runtime.migration.MigrationTestRule;
	using BatchMigrationHelper = org.camunda.bpm.engine.test.api.runtime.migration.batch.BatchMigrationHelper;
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

	public class RuntimeByteArrayTest
	{
		private bool InstanceFieldsInitialized = false;

		public RuntimeByteArrayTest()
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
			migrationRule = new MigrationTestRule(engineRule);
			helper = new BatchMigrationHelper(engineRule, migrationRule);
			ruleChain = RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
		}

	  protected internal const string WORKER_ID = "aWorkerId";
	  protected internal const long LOCK_TIME = 10000L;
	  protected internal const string TOPIC_NAME = "externalTaskTopic";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;
	  protected internal MigrationTestRule migrationRule;
	  protected internal BatchMigrationHelper helper;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl configuration;
	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal TaskService taskService;
	  protected internal RepositoryService repositoryService;
	  protected internal ExternalTaskService externalTaskService;

	  protected internal string id;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		configuration = engineRule.ProcessEngineConfiguration;
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		externalTaskService = engineRule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeBatches()
	  public virtual void removeBatches()
	  {
		helper.removeAllRunningAndHistoricBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (!string.ReferenceEquals(id, null))
		{
		  // delete task
		  taskService.deleteTask(id, true);
		}
		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableBinaryForFileValues()
	  public virtual void testVariableBinaryForFileValues()
	  {
		// given
		BpmnModelInstance instance = createProcess();

		testRule.deploy(instance);
		FileValue fileValue = createFile();

		runtimeService.startProcessInstanceByKey("Process", Variables.createVariables().putValueTyped("fileVar", fileValue));

		string byteArrayValueId = ((VariableInstanceEntity)runtimeService.createVariableInstanceQuery().singleResult()).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableBinary()
	  public virtual void testVariableBinary()
	  {
		sbyte[] binaryContent = "some binary content".GetBytes();

		// given
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["binaryVariable"] = binaryContent;
		Task task = taskService.newTask();
		taskService.saveTask(task);
		id = task.Id;
		taskService.setVariablesLocal(id, variables);

		string byteArrayValueId = ((VariableInstanceEntity)runtimeService.createVariableInstanceQuery().singleResult()).ByteArrayValueId;

		// when
		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBatchBinary()
	  public virtual void testBatchBinary()
	  {
		// when
		helper.migrateProcessInstancesAsync(15);

		string byteArrayValueId = ((BatchEntity) managementService.createBatchQuery().singleResult()).Configuration;

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(byteArrayValueId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExceptionStacktraceBinary()
	  public virtual void testExceptionStacktraceBinary()
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

		JobEntity job = (JobEntity) managementService.createJobQuery().singleResult();
		assertNotNull(job);

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(job.ExceptionByteArrayId));

		checkBinary(byteArrayEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExternalTaskStacktraceBinary()
	  public virtual void testExternalTaskStacktraceBinary()
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

		ExternalTaskEntity externalTask = (ExternalTaskEntity) externalTaskService.createExternalTaskQuery().singleResult();

		ByteArrayEntity byteArrayEntity = configuration.CommandExecutorTxRequired.execute(new GetByteArrayCommand(externalTask.ErrorDetailsByteArrayId));

		// then
		checkBinary(byteArrayEntity);
	  }

	  protected internal virtual void checkBinary(ByteArrayEntity byteArrayEntity)
	  {
		assertNotNull(byteArrayEntity);
		assertNotNull(byteArrayEntity.CreateTime);
		assertEquals(RUNTIME.Value, byteArrayEntity.Type);
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

	  protected internal virtual DateTime nowPlus(long millis)
	  {
		return new DateTime(ClockUtil.CurrentTime.Ticks + millis);
	  }

	}

}