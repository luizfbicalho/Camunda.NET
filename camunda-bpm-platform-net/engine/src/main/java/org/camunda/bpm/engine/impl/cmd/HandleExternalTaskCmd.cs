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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;

	/// <summary>
	/// Represents an abstract class for the handle of external task commands.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public abstract class HandleExternalTaskCmd : ExternalTaskCmd
	{

	  /// <summary>
	  /// The reported worker id.
	  /// </summary>
	  protected internal string workerId;

	  public HandleExternalTaskCmd(string externalTaskId, string workerId) : base(externalTaskId)
	  {
		this.workerId = workerId;
	  }

	  public override Void execute(CommandContext commandContext)
	  {
		validateInput();

		ExternalTaskEntity externalTask = commandContext.ExternalTaskManager.findExternalTaskById(externalTaskId);
		EnsureUtil.ensureNotNull(typeof(NotFoundException), "Cannot find external task with id " + externalTaskId, "externalTask", externalTask);

		if (!workerId.Equals(externalTask.WorkerId))
		{
		  throw new BadUserRequestException(ErrorMessageOnWrongWorkerAccess + "'. It is locked by worker '" + externalTask.WorkerId + "'.");
		}

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkUpdateProcessInstanceById(externalTask.ProcessInstanceId);
		}

		execute(externalTask);

		return null;
	  }

	  /// <summary>
	  /// Returns the error message. Which is used to create an specific message
	  ///  for the BadUserRequestException if an worker has no rights to execute commands of the external task.
	  /// </summary>
	  /// <returns> the specific error message </returns>
	  public abstract string ErrorMessageOnWrongWorkerAccess {get;}

	  /// <summary>
	  /// Validates the current input of the command.
	  /// </summary>
	  protected internal override void validateInput()
	  {
		EnsureUtil.ensureNotNull("workerId", workerId);
	  }
	}

}