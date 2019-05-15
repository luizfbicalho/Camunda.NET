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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricTaskInstanceReport = org.camunda.bpm.engine.history.HistoricTaskInstanceReport;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using TenantCheck = org.camunda.bpm.engine.impl.db.TenantCheck;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class HistoricTaskInstanceReportImpl : HistoricTaskInstanceReport
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime completedAfter_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime completedBefore_Renamed;

	  protected internal PeriodUnit durationPeriodUnit;

	  protected internal CommandExecutor commandExecutor;

	  protected internal TenantCheck tenantCheck = new TenantCheck();

	  public HistoricTaskInstanceReportImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public virtual IList<HistoricTaskInstanceReportResult> countByProcessDefinitionKey()
	  {
		CommandContext commandContext = Context.CommandContext;

		if (commandContext == null)
		{
		  return commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext));
		}
		else
		{
		  return executeCountByProcessDefinitionKey(commandContext);
		}
	  }

	  private class CommandAnonymousInnerClass : Command<IList<HistoricTaskInstanceReportResult>>
	  {
		  private readonly HistoricTaskInstanceReportImpl outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass(HistoricTaskInstanceReportImpl outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }


		  public IList<HistoricTaskInstanceReportResult> execute(CommandContext commandContext)
		  {
			return outerInstance.executeCountByProcessDefinitionKey(commandContext);
		  }

	  }

	  protected internal virtual IList<HistoricTaskInstanceReportResult> executeCountByProcessDefinitionKey(CommandContext commandContext)
	  {
		doAuthCheck(commandContext);
		return commandContext.TaskReportManager.selectHistoricTaskInstanceCountByProcDefKeyReport(this);
	  }

	  public virtual IList<HistoricTaskInstanceReportResult> countByTaskName()
	  {
		CommandContext commandContext = Context.CommandContext;

		if (commandContext == null)
		{
		  return commandExecutor.execute(new CommandAnonymousInnerClass2(this, commandContext));
		}
		else
		{
		  return executeCountByTaskName(commandContext);
		}
	  }

	  private class CommandAnonymousInnerClass2 : Command<IList<HistoricTaskInstanceReportResult>>
	  {
		  private readonly HistoricTaskInstanceReportImpl outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass2(HistoricTaskInstanceReportImpl outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }


		  public IList<HistoricTaskInstanceReportResult> execute(CommandContext commandContext)
		  {
			return outerInstance.executeCountByTaskName(commandContext);
		  }

	  }

	  protected internal virtual IList<HistoricTaskInstanceReportResult> executeCountByTaskName(CommandContext commandContext)
	  {
		doAuthCheck(commandContext);
		return commandContext.TaskReportManager.selectHistoricTaskInstanceCountByTaskNameReport(this);
	  }

	  public virtual IList<DurationReportResult> duration(PeriodUnit periodUnit)
	  {
		ensureNotNull(typeof(NotValidException), "periodUnit", periodUnit);
		this.durationPeriodUnit = periodUnit;

		CommandContext commandContext = Context.CommandContext;

		if (commandContext == null)
		{
		  return commandExecutor.execute(new CommandAnonymousInnerClass3(this, commandContext));
		}
		else
		{
		  return executeDuration(commandContext);
		}
	  }

	  private class CommandAnonymousInnerClass3 : Command<IList<DurationReportResult>>
	  {
		  private readonly HistoricTaskInstanceReportImpl outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass3(HistoricTaskInstanceReportImpl outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }


		  public IList<DurationReportResult> execute(CommandContext commandContext)
		  {
			return outerInstance.executeDuration(commandContext);
		  }

	  }


	  protected internal virtual IList<DurationReportResult> executeDuration(CommandContext commandContext)
	  {
		doAuthCheck(commandContext);
		return commandContext.TaskReportManager.createHistoricTaskDurationReport(this);
	  }

	  protected internal virtual void doAuthCheck(CommandContext commandContext)
	  {
		// since a report does only make sense in context of historic
		// data, the authorization check will be performed here
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkReadHistoryAnyProcessDefinition();
		}
	  }

	  public virtual DateTime CompletedAfter
	  {
		  get
		  {
			return completedAfter_Renamed;
		  }
	  }

	  public virtual DateTime CompletedBefore
	  {
		  get
		  {
			return completedBefore_Renamed;
		  }
	  }

	  public virtual HistoricTaskInstanceReport completedAfter(DateTime completedAfter)
	  {
		ensureNotNull(typeof(NotValidException), "completedAfter", completedAfter);
		this.completedAfter_Renamed = completedAfter;
		return this;
	  }

	  public virtual HistoricTaskInstanceReport completedBefore(DateTime completedBefore)
	  {
		ensureNotNull(typeof(NotValidException), "completedBefore", completedBefore);
		this.completedBefore_Renamed = completedBefore;
		return this;
	  }

	  public virtual TenantCheck TenantCheck
	  {
		  get
		  {
			return tenantCheck;
		  }
	  }

	  public virtual string ReportPeriodUnitName
	  {
		  get
		  {
			return durationPeriodUnit.name();
		  }
	  }

	}

}