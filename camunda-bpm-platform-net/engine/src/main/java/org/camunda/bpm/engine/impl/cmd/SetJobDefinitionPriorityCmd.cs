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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using UserOperationLogContext = org.camunda.bpm.engine.impl.oplog.UserOperationLogContext;
	using UserOperationLogContextEntry = org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntry;
	using UserOperationLogContextEntryBuilder = org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntryBuilder;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SetJobDefinitionPriorityCmd : Command<Void>
	{

	  public const string JOB_DEFINITION_OVERRIDING_PRIORITY = "overridingPriority";

	  protected internal string jobDefinitionId;
	  protected internal long? priority;
	  protected internal bool cascade = false;

	  public SetJobDefinitionPriorityCmd(string jobDefinitionId, long? priority, bool cascade)
	  {
		this.jobDefinitionId = jobDefinitionId;
		this.priority = priority;
		this.cascade = cascade;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull(typeof(NotValidException), "jobDefinitionId", jobDefinitionId);

		JobDefinitionEntity jobDefinition = commandContext.JobDefinitionManager.findById(jobDefinitionId);

		ensureNotNull(typeof(NotFoundException), "Job definition with id '" + jobDefinitionId + "' does not exist", "jobDefinition", jobDefinition);

		checkUpdateProcess(commandContext, jobDefinition);

		long? currentPriority = jobDefinition.OverridingJobPriority;
		jobDefinition.JobPriority = priority;

		UserOperationLogContext opLogContext = new UserOperationLogContext();
		createJobDefinitionOperationLogEntry(opLogContext, currentPriority, jobDefinition);

		if (cascade && priority != null)
		{
		  commandContext.JobManager.updateJobPriorityByDefinitionId(jobDefinitionId, priority.Value);
		  createCascadeJobsOperationLogEntry(opLogContext, jobDefinition);
		}

		commandContext.OperationLogManager.logUserOperations(opLogContext);

		return null;
	  }

	  protected internal virtual void checkUpdateProcess(CommandContext commandContext, JobDefinitionEntity jobDefinition)
	  {

		string processDefinitionId = jobDefinition.ProcessDefinitionId;

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessDefinitionById(processDefinitionId);

		  if (cascade)
		  {
			checker.checkUpdateProcessInstanceByProcessDefinitionId(processDefinitionId);
		  }
		}
	  }

	  protected internal virtual void createJobDefinitionOperationLogEntry(UserOperationLogContext opLogContext, long? previousPriority, JobDefinitionEntity jobDefinition)
	  {

		PropertyChange propertyChange = new PropertyChange(JOB_DEFINITION_OVERRIDING_PRIORITY, previousPriority, jobDefinition.OverridingJobPriority);

		UserOperationLogContextEntry entry = UserOperationLogContextEntryBuilder.entry(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, EntityTypes.JOB_DEFINITION).inContextOf(jobDefinition).propertyChanges(propertyChange).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).create();

		opLogContext.addEntry(entry);
	  }

	  protected internal virtual void createCascadeJobsOperationLogEntry(UserOperationLogContext opLogContext, JobDefinitionEntity jobDefinition)
	  {
		// old value is unknown
		PropertyChange propertyChange = new PropertyChange(SetJobPriorityCmd.JOB_PRIORITY_PROPERTY, null, jobDefinition.OverridingJobPriority);

		UserOperationLogContextEntry entry = UserOperationLogContextEntryBuilder.entry(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, EntityTypes.JOB).inContextOf(jobDefinition).propertyChanges(propertyChange).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).create();

		opLogContext.addEntry(entry);
	  }

	}

}