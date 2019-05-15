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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DeleteUserOperationLogEntryCmd : Command<Void>
	{

	  protected internal string entryId;

	  public DeleteUserOperationLogEntryCmd(string entryId)
	  {
		this.entryId = entryId;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull(typeof(NotValidException), "entryId", entryId);

		UserOperationLogEntry entry = commandContext.OperationLogManager.findOperationLogById(entryId);

		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkDeleteUserOperationLog(entry);
		}

		commandContext.OperationLogManager.deleteOperationLogEntryById(entryId);
		return null;
	  }

	}

}