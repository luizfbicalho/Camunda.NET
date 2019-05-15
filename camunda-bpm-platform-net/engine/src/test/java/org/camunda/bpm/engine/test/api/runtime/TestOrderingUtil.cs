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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using TestCase = junit.framework.TestCase;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using Query = org.camunda.bpm.engine.query.Query;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// This class provides utils to verify the sorting of queries of engine entities.
	/// Assuming we sort over a property x, there are two valid orderings when some entities
	/// have values where x = null: Either, these values precede the overall list, or they trail it.
	/// Thus, this class does not use regular comparators but a <seealso cref="NullTolerantComparator"/>
	/// that can be used to assert a list of entites in both ways.
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TestOrderingUtil
	{

	  // EXECUTION

	  public static NullTolerantComparator<Execution> executionByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass());
	  }

	  private class PropertyAccessorAnonymousInnerClass : PropertyAccessor<Execution, string>
	  {
		  public string getProperty(Execution obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<Execution> executionByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass2());
	  }

	  private class PropertyAccessorAnonymousInnerClass2 : PropertyAccessor<Execution, string>
	  {
		  public string getProperty(Execution obj)
		  {
			return ((ExecutionEntity) obj).ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<Execution> executionByProcessDefinitionKey(ProcessEngine processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RuntimeService runtimeService = processEngine.getRuntimeService();
		RuntimeService runtimeService = processEngine.RuntimeService;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RepositoryService repositoryService = processEngine.getRepositoryService();
		RepositoryService repositoryService = processEngine.RepositoryService;

		return propertyComparator(new PropertyAccessorAnonymousInnerClass3(runtimeService, repositoryService));
	  }

	  private class PropertyAccessorAnonymousInnerClass3 : PropertyAccessor<Execution, string>
	  {
		  private RuntimeService runtimeService;
		  private RepositoryService repositoryService;

		  public PropertyAccessorAnonymousInnerClass3(RuntimeService runtimeService, RepositoryService repositoryService)
		  {
			  this.runtimeService = runtimeService;
			  this.repositoryService = repositoryService;
		  }

		  public string getProperty(Execution obj)
		  {
			ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(obj.ProcessInstanceId).singleResult();
			ProcessDefinition processDefinition = repositoryService.getProcessDefinition(processInstance.ProcessDefinitionId);
			return processDefinition.Key;
		  }
	  }

	  //PROCESS INSTANCE

	  public static NullTolerantComparator<ProcessInstance> processInstanceByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass4());
	  }

	  private class PropertyAccessorAnonymousInnerClass4 : PropertyAccessor<ProcessInstance, string>
	  {
		  public string getProperty(ProcessInstance obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<ProcessInstance> processInstanceByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass5());
	  }

	  private class PropertyAccessorAnonymousInnerClass5 : PropertyAccessor<ProcessInstance, string>
	  {
		  public string getProperty(ProcessInstance obj)
		  {
			return obj.ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<ProcessInstance> processInstanceByBusinessKey()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass6());
	  }

	  private class PropertyAccessorAnonymousInnerClass6 : PropertyAccessor<ProcessInstance, string>
	  {
		  public string getProperty(ProcessInstance obj)
		  {
			return obj.BusinessKey;
		  }
	  }

	  //HISTORIC PROCESS INSTANCE

	  public static NullTolerantComparator<HistoricProcessInstance> historicProcessInstanceByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass7());
	  }

	  private class PropertyAccessorAnonymousInnerClass7 : PropertyAccessor<HistoricProcessInstance, string>
	  {
		  public string getProperty(HistoricProcessInstance obj)
		  {
			return obj.ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricProcessInstance> historicProcessInstanceByProcessDefinitionKey()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass8());
	  }

	  private class PropertyAccessorAnonymousInnerClass8 : PropertyAccessor<HistoricProcessInstance, string>
	  {
		  public string getProperty(HistoricProcessInstance obj)
		  {
			return obj.ProcessDefinitionKey;
		  }
	  }

	  public static NullTolerantComparator<HistoricProcessInstance> historicProcessInstanceByProcessDefinitionName()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass9());
	  }

	  private class PropertyAccessorAnonymousInnerClass9 : PropertyAccessor<HistoricProcessInstance, string>
	  {
		  public string getProperty(HistoricProcessInstance obj)
		  {
			return obj.ProcessDefinitionName;
		  }
	  }

	  public static NullTolerantComparator<HistoricProcessInstance> historicProcessInstanceByProcessDefinitionVersion()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass10());
	  }

	  private class PropertyAccessorAnonymousInnerClass10 : PropertyAccessor<HistoricProcessInstance, int>
	  {
		  public int? getProperty(HistoricProcessInstance obj)
		  {
			return obj.ProcessDefinitionVersion;
		  }
	  }

	  public static NullTolerantComparator<HistoricProcessInstance> historicProcessInstanceByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass11());
	  }

	  private class PropertyAccessorAnonymousInnerClass11 : PropertyAccessor<HistoricProcessInstance, string>
	  {
		  public string getProperty(HistoricProcessInstance obj)
		  {
			return obj.Id;
		  }
	  }

	  // CASE EXECUTION

	  public static NullTolerantComparator<CaseExecution> caseExecutionByDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass12());
	  }

	  private class PropertyAccessorAnonymousInnerClass12 : PropertyAccessor<CaseExecution, string>
	  {
		  public string getProperty(CaseExecution obj)
		  {
			return obj.CaseDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<CaseExecution> caseExecutionByDefinitionKey(ProcessEngine processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RepositoryService repositoryService = processEngine.getRepositoryService();
		RepositoryService repositoryService = processEngine.RepositoryService;
		return propertyComparator(new PropertyAccessorAnonymousInnerClass13(repositoryService));
	  }

	  private class PropertyAccessorAnonymousInnerClass13 : PropertyAccessor<CaseExecution, string>
	  {
		  private RepositoryService repositoryService;

		  public PropertyAccessorAnonymousInnerClass13(RepositoryService repositoryService)
		  {
			  this.repositoryService = repositoryService;
		  }

		  public string getProperty(CaseExecution obj)
		  {
			CaseDefinition caseDefinition = repositoryService.getCaseDefinition(obj.CaseDefinitionId);
			return caseDefinition.Key;
		  }
	  }

	  public static NullTolerantComparator<CaseExecution> caseExecutionById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass14());
	  }

	  private class PropertyAccessorAnonymousInnerClass14 : PropertyAccessor<CaseExecution, string>
	  {
		  public string getProperty(CaseExecution obj)
		  {
			return obj.Id;
		  }
	  }

	  // TASK

	  public static NullTolerantComparator<Task> taskById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass15());
	  }

	  private class PropertyAccessorAnonymousInnerClass15 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.Id;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByName()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass16());
	  }

	  private class PropertyAccessorAnonymousInnerClass16 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.Name;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByPriority()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass17());
	  }

	  private class PropertyAccessorAnonymousInnerClass17 : PropertyAccessor<Task, int>
	  {
		  public int? getProperty(Task obj)
		  {
			return obj.Priority;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByAssignee()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass18());
	  }

	  private class PropertyAccessorAnonymousInnerClass18 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.Assignee;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByDescription()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass19());
	  }

	  private class PropertyAccessorAnonymousInnerClass19 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.Description;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass20());
	  }

	  private class PropertyAccessorAnonymousInnerClass20 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByExecutionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass21());
	  }

	  private class PropertyAccessorAnonymousInnerClass21 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.ExecutionId;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByCreateTime()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass22());
	  }

	  private class PropertyAccessorAnonymousInnerClass22 : PropertyAccessor<Task, DateTime>
	  {
		  public DateTime getProperty(Task obj)
		  {
			return obj.CreateTime;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByDueDate()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass23());
	  }

	  private class PropertyAccessorAnonymousInnerClass23 : PropertyAccessor<Task, DateTime>
	  {
		  public DateTime getProperty(Task obj)
		  {
			return obj.DueDate;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByFollowUpDate()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass24());
	  }

	  private class PropertyAccessorAnonymousInnerClass24 : PropertyAccessor<Task, DateTime>
	  {
		  public DateTime getProperty(Task obj)
		  {
			return obj.FollowUpDate;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByCaseInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass25());
	  }

	  private class PropertyAccessorAnonymousInnerClass25 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.CaseInstanceId;
		  }
	  }

	  public static NullTolerantComparator<Task> taskByCaseExecutionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass26());
	  }

	  private class PropertyAccessorAnonymousInnerClass26 : PropertyAccessor<Task, string>
	  {
		  public string getProperty(Task obj)
		  {
			return obj.CaseExecutionId;
		  }
	  }

	  // HISTORIC JOB LOG

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByTimestamp()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass27());
	  }

	  private class PropertyAccessorAnonymousInnerClass27 : PropertyAccessor<HistoricJobLog, DateTime>
	  {
		  public DateTime getProperty(HistoricJobLog obj)
		  {
			return obj.Timestamp;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByJobId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass28());
	  }

	  private class PropertyAccessorAnonymousInnerClass28 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.JobId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByJobDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass29());
	  }

	  private class PropertyAccessorAnonymousInnerClass29 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.JobDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByJobDueDate()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass30());
	  }

	  private class PropertyAccessorAnonymousInnerClass30 : PropertyAccessor<HistoricJobLog, DateTime>
	  {
		  public DateTime getProperty(HistoricJobLog obj)
		  {
			return obj.JobDueDate;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByJobRetries()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass31());
	  }

	  private class PropertyAccessorAnonymousInnerClass31 : PropertyAccessor<HistoricJobLog, int>
	  {
		  public int? getProperty(HistoricJobLog obj)
		  {
			return obj.JobRetries;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByActivityId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass32());
	  }

	  private class PropertyAccessorAnonymousInnerClass32 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.ActivityId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByExecutionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass33());
	  }

	  private class PropertyAccessorAnonymousInnerClass33 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.ExecutionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass34());
	  }

	  private class PropertyAccessorAnonymousInnerClass34 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass35());
	  }

	  private class PropertyAccessorAnonymousInnerClass35 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByProcessDefinitionKey(ProcessEngine processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RepositoryService repositoryService = processEngine.getRepositoryService();
		RepositoryService repositoryService = processEngine.RepositoryService;

		return propertyComparator(new PropertyAccessorAnonymousInnerClass36(repositoryService));
	  }

	  private class PropertyAccessorAnonymousInnerClass36 : PropertyAccessor<HistoricJobLog, string>
	  {
		  private RepositoryService repositoryService;

		  public PropertyAccessorAnonymousInnerClass36(RepositoryService repositoryService)
		  {
			  this.repositoryService = repositoryService;
		  }

		  public string getProperty(HistoricJobLog obj)
		  {
			ProcessDefinition processDefinition = repositoryService.getProcessDefinition(obj.ProcessDefinitionId);
			return processDefinition.Key;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByDeploymentId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass37());
	  }

	  private class PropertyAccessorAnonymousInnerClass37 : PropertyAccessor<HistoricJobLog, string>
	  {
		  public string getProperty(HistoricJobLog obj)
		  {
			return obj.DeploymentId;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogByJobPriority()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass38());
	  }

	  private class PropertyAccessorAnonymousInnerClass38 : PropertyAccessor<HistoricJobLog, long>
	  {
		  public long? getProperty(HistoricJobLog obj)
		  {
			return obj.JobPriority;
		  }
	  }

	  public static NullTolerantComparator<HistoricJobLog> historicJobLogPartiallyByOccurence()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass39());
	  }

	  private class PropertyAccessorAnonymousInnerClass39 : PropertyAccessor<HistoricJobLog, long>
	  {
		  public long? getProperty(HistoricJobLog obj)
		  {
			return ((HistoricJobLogEventEntity) obj).SequenceCounter;
		  }
	  }

	  // jobs

	  public static NullTolerantComparator<Job> jobByPriority()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass40());
	  }

	  private class PropertyAccessorAnonymousInnerClass40 : PropertyAccessor<Job, long>
	  {
		  public long? getProperty(Job obj)
		  {
			return obj.Priority;
		  }
	  }

	  // external task

	  public static NullTolerantComparator<ExternalTask> externalTaskById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass41());
	  }

	  private class PropertyAccessorAnonymousInnerClass41 : PropertyAccessor<ExternalTask, string>
	  {
		  public string getProperty(ExternalTask obj)
		  {
			return obj.Id;
		  }
	  }

	  public static NullTolerantComparator<ExternalTask> externalTaskByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass42());
	  }

	  private class PropertyAccessorAnonymousInnerClass42 : PropertyAccessor<ExternalTask, string>
	  {
		  public string getProperty(ExternalTask obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<ExternalTask> externalTaskByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass43());
	  }

	  private class PropertyAccessorAnonymousInnerClass43 : PropertyAccessor<ExternalTask, string>
	  {
		  public string getProperty(ExternalTask obj)
		  {
			return obj.ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<ExternalTask> externalTaskByProcessDefinitionKey()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass44());
	  }

	  private class PropertyAccessorAnonymousInnerClass44 : PropertyAccessor<ExternalTask, string>
	  {
		  public string getProperty(ExternalTask obj)
		  {
			return obj.ProcessDefinitionKey;
		  }
	  }

	  public static NullTolerantComparator<ExternalTask> externalTaskByLockExpirationTime()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass45());
	  }

	  private class PropertyAccessorAnonymousInnerClass45 : PropertyAccessor<ExternalTask, DateTime>
	  {
		  public DateTime getProperty(ExternalTask obj)
		  {
			return obj.LockExpirationTime;
		  }
	  }

	  public static NullTolerantComparator<ExternalTask> externalTaskByPriority()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass46());
	  }

	  private class PropertyAccessorAnonymousInnerClass46 : PropertyAccessor<ExternalTask, long>
	  {
		  public long? getProperty(ExternalTask obj)
		  {
			return obj.Priority;
		  }
	  }

	  // batch

	  public static NullTolerantComparator<Batch> batchById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass47());
	  }

	  private class PropertyAccessorAnonymousInnerClass47 : PropertyAccessor<Batch, string>
	  {
		  public string getProperty(Batch obj)
		  {
			return obj.Id;
		  }
	  }

	  public static NullTolerantComparator<Batch> batchByTenantId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass48());
	  }

	  private class PropertyAccessorAnonymousInnerClass48 : PropertyAccessor<Batch, string>
	  {
		  public string getProperty(Batch obj)
		  {
			return obj.TenantId;
		  }
	  }

	  public static NullTolerantComparator<HistoricBatch> historicBatchById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass49());
	  }

	  private class PropertyAccessorAnonymousInnerClass49 : PropertyAccessor<HistoricBatch, string>
	  {
		  public string getProperty(HistoricBatch obj)
		  {
			return obj.Id;
		  }
	  }

	  public static NullTolerantComparator<HistoricBatch> historicBatchByTenantId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass50());
	  }

	  private class PropertyAccessorAnonymousInnerClass50 : PropertyAccessor<HistoricBatch, string>
	  {
		  public string getProperty(HistoricBatch obj)
		  {
			return obj.TenantId;
		  }
	  }

	  public static NullTolerantComparator<HistoricBatch> historicBatchByStartTime()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass51());
	  }

	  private class PropertyAccessorAnonymousInnerClass51 : PropertyAccessor<HistoricBatch, DateTime>
	  {
		  public DateTime getProperty(HistoricBatch obj)
		  {
			return obj.StartTime;
		  }
	  }

	  public static NullTolerantComparator<HistoricBatch> historicBatchByEndTime()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass52());
	  }

	  private class PropertyAccessorAnonymousInnerClass52 : PropertyAccessor<HistoricBatch, DateTime>
	  {
		  public DateTime getProperty(HistoricBatch obj)
		  {
			return obj.EndTime;
		  }
	  }

	  public static NullTolerantComparator<BatchStatistics> batchStatisticsById()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass53());
	  }

	  private class PropertyAccessorAnonymousInnerClass53 : PropertyAccessor<BatchStatistics, string>
	  {
		  public string getProperty(BatchStatistics obj)
		  {
			return obj.Id;
		  }
	  }

	  public static NullTolerantComparator<BatchStatistics> batchStatisticsByTenantId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass54());
	  }

	  private class PropertyAccessorAnonymousInnerClass54 : PropertyAccessor<BatchStatistics, string>
	  {
		  public string getProperty(BatchStatistics obj)
		  {
			return obj.TenantId;
		  }
	  }

	  // HISTORIC EXTERNAL TASK LOG

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskByTimestamp()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass55());
	  }

	  private class PropertyAccessorAnonymousInnerClass55 : PropertyAccessor<HistoricExternalTaskLog, DateTime>
	  {
		  public DateTime getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.Timestamp;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByExternalTaskId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass56());
	  }

	  private class PropertyAccessorAnonymousInnerClass56 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ExternalTaskId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByRetries()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass57());
	  }

	  private class PropertyAccessorAnonymousInnerClass57 : PropertyAccessor<HistoricExternalTaskLog, int>
	  {
		  public int? getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.Retries;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByPriority()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass58());
	  }

	  private class PropertyAccessorAnonymousInnerClass58 : PropertyAccessor<HistoricExternalTaskLog, long>
	  {
		  public long? getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.Priority;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByTopicName()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass59());
	  }

	  private class PropertyAccessorAnonymousInnerClass59 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.TopicName;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByWorkerId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass60());
	  }

	  private class PropertyAccessorAnonymousInnerClass60 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.WorkerId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByActivityId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass61());
	  }

	  private class PropertyAccessorAnonymousInnerClass61 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ActivityId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByActivityInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass62());
	  }

	  private class PropertyAccessorAnonymousInnerClass62 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ActivityInstanceId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByExecutionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass63());
	  }

	  private class PropertyAccessorAnonymousInnerClass63 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ExecutionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByProcessInstanceId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass64());
	  }

	  private class PropertyAccessorAnonymousInnerClass64 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ProcessInstanceId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByProcessDefinitionId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass65());
	  }

	  private class PropertyAccessorAnonymousInnerClass65 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.ProcessDefinitionId;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByProcessDefinitionKey(ProcessEngine processEngine)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.RepositoryService repositoryService = processEngine.getRepositoryService();
		RepositoryService repositoryService = processEngine.RepositoryService;

		return propertyComparator(new PropertyAccessorAnonymousInnerClass66(repositoryService));
	  }

	  private class PropertyAccessorAnonymousInnerClass66 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  private RepositoryService repositoryService;

		  public PropertyAccessorAnonymousInnerClass66(RepositoryService repositoryService)
		  {
			  this.repositoryService = repositoryService;
		  }

		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			ProcessDefinition processDefinition = repositoryService.getProcessDefinition(obj.ProcessDefinitionId);
			return processDefinition.Key;
		  }
	  }

	  public static NullTolerantComparator<HistoricExternalTaskLog> historicExternalTaskLogByTenantId()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass67());
	  }

	  private class PropertyAccessorAnonymousInnerClass67 : PropertyAccessor<HistoricExternalTaskLog, string>
	  {
		  public string getProperty(HistoricExternalTaskLog obj)
		  {
			return obj.TenantId;
		  }
	  }

	  // SCHEMA LOG
	  public static NullTolerantComparator<SchemaLogEntry> schemaLogEntryByTimestamp()
	  {
		return propertyComparator(new PropertyAccessorAnonymousInnerClass68());
	  }

	  private class PropertyAccessorAnonymousInnerClass68 : PropertyAccessor<SchemaLogEntry, DateTime>
	  {
		  public DateTime getProperty(SchemaLogEntry obj)
		  {
			return obj.Timestamp;
		  }
	  }

	  // general

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T, P extends Comparable<P>> NullTolerantComparator<T> propertyComparator(final PropertyAccessor<T, P> accessor)
	  public static NullTolerantComparator<T> propertyComparator<T, P>(PropertyAccessor<T, P> accessor) where P : IComparable<P>
	  {
		return new NullTolerantComparatorAnonymousInnerClass(accessor);
	  }

	  private class NullTolerantComparatorAnonymousInnerClass : NullTolerantComparator<T>
	  {
		  private org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.PropertyAccessor<T, P> accessor;

		  public NullTolerantComparatorAnonymousInnerClass(org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.PropertyAccessor<T, P> accessor)
		  {
			  this.accessor = accessor;
		  }


		  public override int compare(T o1, T o2)
		  {
			P prop1 = accessor.getProperty(o1);
			P prop2 = accessor.getProperty(o2);

			return prop1.compareTo(prop2);
		  }

		  public override bool hasNullProperty(T @object)
		  {
			return accessor.getProperty(@object) == null;
		  }
	  }

	  protected internal interface PropertyAccessor<T, P> where P : IComparable<P>
	  {
		P getProperty(T obj);
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T> NullTolerantComparator<T> inverted(final NullTolerantComparator<T> comparator)
	  public static NullTolerantComparator<T> inverted<T>(NullTolerantComparator<T> comparator)
	  {
		return new NullTolerantComparatorAnonymousInnerClass2(comparator);
	  }

	  private class NullTolerantComparatorAnonymousInnerClass2 : NullTolerantComparator<T>
	  {
		  private org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T> comparator;

		  public NullTolerantComparatorAnonymousInnerClass2(org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T> comparator)
		  {
			  this.comparator = comparator;
		  }

		  public int compare(T o1, T o2)
		  {
			return - comparator.Compare(o1, o2);
		  }

		  public override bool hasNullProperty(T @object)
		  {
			return comparator.hasNullProperty(@object);
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static <T> NullTolerantComparator<T> hierarchical(final NullTolerantComparator<T> baseComparator, final NullTolerantComparator<T>... minorOrderings)
	  public static NullTolerantComparator<T> hierarchical<T>(NullTolerantComparator<T> baseComparator, params NullTolerantComparator<T>[] minorOrderings)
	  {
		return new NullTolerantComparatorAnonymousInnerClass3(baseComparator, minorOrderings);
	  }

	  private class NullTolerantComparatorAnonymousInnerClass3 : NullTolerantComparator<T>
	  {
		  private org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T> baseComparator;
		  private org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T>[] minorOrderings;

		  public NullTolerantComparatorAnonymousInnerClass3(org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T> baseComparator, org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator<T>[] minorOrderings)
		  {
			  this.baseComparator = baseComparator;
			  this.minorOrderings = minorOrderings;
		  }

		  public override int compare(T o1, T o2, bool nullPrecedes)
		  {
			int comparison = baseComparator.compare(o1, o2, nullPrecedes);

			int i = 0;
			while (comparison == 0 && i < minorOrderings.Length)
			{
			  NullTolerantComparator<T> comparator = minorOrderings[i];
			  comparison = comparator.compare(o1, o2, nullPrecedes);
			  i++;
			}

			return comparison;
		  }

		  public int compare(T o1, T o2)
		  {
			throw new System.NotSupportedException();
		  }

		  public override bool hasNullProperty(T @object)
		  {
			throw new System.NotSupportedException();
		  }
	  }

	  public abstract class NullTolerantComparator<T> : IComparer<T>
	  {

		public virtual int Compare(T o1, T o2, bool nullPrecedes)
		{
		  bool o1Null = hasNullProperty(o1);
		  bool o2Null = hasNullProperty(o2);

		  if (o1Null)
		  {
			if (o2Null)
			{
			  return 0;
			}
			else
			{
			  if (nullPrecedes)
			  {
				return -1;
			  }
			  else
			  {
				return 1;
			  }
			}
		  }
		  else
		  {

			if (o2Null)
			{
			  if (nullPrecedes)
			  {
				return 1;
			  }
			  else
			  {
				return -1;
			  }
			}
		  }

		  return compare(o1, o2);
		}

		public abstract bool hasNullProperty(T @object);
	  }

	  public static void verifySorting<T>(IList<T> actualElements, NullTolerantComparator<T> expectedOrdering)
	  {
		// check two orderings: one in which values with null properties are at the front of the list
		bool leadingNullOrdering = orderingConsistent(actualElements, expectedOrdering, true);

		if (leadingNullOrdering)
		{
		  return;
		}

		// and one where the values with null properties are at the end of the list
		bool trailingNullOrdering = orderingConsistent(actualElements, expectedOrdering, false);
		TestCase.assertTrue("Ordering not consistent with comparator", trailingNullOrdering);
	  }

	  public static bool orderingConsistent<T>(IList<T> actualElements, NullTolerantComparator<T> expectedOrdering, bool nullPrecedes)
	  {
		for (int i = 0; i < actualElements.Count - 1; i++)
		{
		  T currentExecution = actualElements[i];
		  T nextExecution = actualElements[i + 1];

		  int comparison = expectedOrdering.compare(currentExecution, nextExecution, nullPrecedes);
		  if (comparison > 0)
		  {
			return false;
		  }
		}

		return true;
	  }

	  public static void verifySortingAndCount<T, T1>(Query<T1> query, int expectedCount, NullTolerantComparator<T> expectedOrdering)
	  {
		IList<T> elements = query.list();
		TestCase.assertEquals(expectedCount, elements.Count);

		verifySorting(elements, expectedOrdering);
	  }

	}

}