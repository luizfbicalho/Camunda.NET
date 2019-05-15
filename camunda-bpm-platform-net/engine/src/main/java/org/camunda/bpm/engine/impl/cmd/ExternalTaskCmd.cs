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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// Represents a base class for the external task commands.
	/// Contains functionality to get the external task by id and check
	/// the authorization for the execution of a command on the requested external task.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public abstract class ExternalTaskCmd : Command<Void>
	{

	  /// <summary>
	  /// The corresponding external task id.
	  /// </summary>
	  protected internal string externalTaskId;

	  public ExternalTaskCmd(string externalTaskId)
	  {
		this.externalTaskId = externalTaskId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		EnsureUtil.ensureNotNull("externalTaskId", externalTaskId);
		validateInput();

		ExternalTaskEntity externalTask = commandContext.ExternalTaskManager.findExternalTaskById(externalTaskId);
		ensureNotNull(typeof(NotFoundException), "Cannot find external task with id " + externalTaskId, "externalTask", externalTask);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstanceById(externalTask.ProcessInstanceId);
		}

		writeUserOperationLog(commandContext, externalTask, UserOperationLogOperationType, getUserOperationLogPropertyChanges(externalTask));

		execute(externalTask);

		return null;
	  }

	  protected internal virtual void writeUserOperationLog(CommandContext commandContext, ExternalTaskEntity externalTask, string operationType, IList<PropertyChange> propertyChanges)
	  {
		if (!string.ReferenceEquals(operationType, null))
		{
		  commandContext.OperationLogManager.logExternalTaskOperation(operationType, externalTask, propertyChanges == null || propertyChanges.Count == 0 ? Collections.singletonList(PropertyChange.EMPTY_CHANGE) : propertyChanges);
		}
	  }

	  protected internal virtual string UserOperationLogOperationType
	  {
		  get
		  {
			return null;
		  }
	  }

	  protected internal virtual IList<PropertyChange> getUserOperationLogPropertyChanges(ExternalTaskEntity externalTask)
	  {
		return Collections.emptyList();
	  }

	  /// <summary>
	  /// Executes the specific external task commands, which belongs to the current sub class.
	  /// </summary>
	  /// <param name="externalTask"> the external task which is used for the command execution </param>
	  protected internal abstract void execute(ExternalTaskEntity externalTask);

	  /// <summary>
	  /// Validates the current input of the command.
	  /// </summary>
	  protected internal abstract void validateInput();

	}

}