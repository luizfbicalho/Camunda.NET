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
namespace org.camunda.bpm.engine.impl
{

	using TenantCheck = org.camunda.bpm.engine.impl.db.TenantCheck;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using TaskReport = org.camunda.bpm.engine.task.TaskReport;

	/// <summary>
	/// @author Stefan Hentschel
	/// 
	/// </summary>
	[Serializable]
	public class TaskReportImpl : TaskReport
	{

	  private const long serialVersionUID = 1L;

	  [NonSerialized]
	  protected internal CommandExecutor commandExecutor;

	  protected internal TenantCheck tenantCheck = new TenantCheck();

	  public TaskReportImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  protected internal virtual IList<TaskCountByCandidateGroupResult> createTaskCountByCandidateGroupReport(CommandContext commandContext)
	  {
		return commandContext.TaskReportManager.createTaskCountByCandidateGroupReport(this);
	  }

	  public virtual TenantCheck TenantCheck
	  {
		  get
		  {
			return tenantCheck;
		  }
	  }

	  public virtual IList<TaskCountByCandidateGroupResult> taskCountByCandidateGroup()
	  {
		return commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<IList<TaskCountByCandidateGroupResult>>
	  {
		  private readonly TaskReportImpl outerInstance;

		  public CommandAnonymousInnerClass(TaskReportImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public IList<TaskCountByCandidateGroupResult> execute(CommandContext commandContext)
		  {
			return outerInstance.createTaskCountByCandidateGroupReport(commandContext);
		  }
	  }

	}

}