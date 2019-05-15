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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class FindActiveActivityIdsCmd : Command<IList<string>>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string executionId;

	  public FindActiveActivityIdsCmd(string executionId)
	  {
		this.executionId = executionId;
	  }

	  public virtual IList<string> execute(CommandContext commandContext)
	  {
		ensureNotNull("executionId", executionId);

		// fetch execution
		ExecutionManager executionManager = commandContext.ExecutionManager;
		ExecutionEntity execution = executionManager.findExecutionById(executionId);
		ensureNotNull("execution " + executionId + " doesn't exist", "execution", execution);

		checkGetActivityIds(execution, commandContext);

		// fetch active activities
		return execution.findActiveActivityIds();
	  }

	  protected internal virtual void checkGetActivityIds(ExecutionEntity execution, CommandContext commandContext)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadProcessInstance(execution);
		}
	  }
	}

}