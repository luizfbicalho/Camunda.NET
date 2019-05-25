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
//	import static org.camunda.bpm.engine.impl.util.CompareUtil.areNotInAscendingOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricProcessInstanceReport = org.camunda.bpm.engine.history.HistoricProcessInstanceReport;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using TenantCheck = org.camunda.bpm.engine.impl.db.TenantCheck;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricProcessInstanceReportImpl : HistoricProcessInstanceReport
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedAfter_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime startedBefore_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] processDefinitionIdIn_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string[] processDefinitionKeyIn_Conflict;

	  protected internal PeriodUnit durationPeriodUnit;

	  protected internal CommandExecutor commandExecutor;

	  protected internal TenantCheck tenantCheck = new TenantCheck();

	  public HistoricProcessInstanceReportImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  // query parameter ///////////////////////////////////////////////

	  public virtual HistoricProcessInstanceReport startedAfter(DateTime startedAfter)
	  {
		ensureNotNull(typeof(NotValidException), "startedAfter", startedAfter);
		this.startedAfter_Conflict = startedAfter;
		return this;
	  }

	  public virtual HistoricProcessInstanceReport startedBefore(DateTime startedBefore)
	  {
		ensureNotNull(typeof(NotValidException), "startedBefore", startedBefore);
		this.startedBefore_Conflict = startedBefore;
		return this;
	  }

	  public virtual HistoricProcessInstanceReport processDefinitionIdIn(params string[] processDefinitionIds)
	  {
		ensureNotNull(typeof(NotValidException), "", "processDefinitionIdIn", (object[]) processDefinitionIds);
		this.processDefinitionIdIn_Conflict = processDefinitionIds;
		return this;
	  }

	  public virtual HistoricProcessInstanceReport processDefinitionKeyIn(params string[] processDefinitionKeys)
	  {
		ensureNotNull(typeof(NotValidException), "", "processDefinitionKeyIn", (object[]) processDefinitionKeys);
		this.processDefinitionKeyIn_Conflict = processDefinitionKeys;
		return this;
	  }

	  // report execution /////////////////////////////////////////////

	  public virtual IList<DurationReportResult> duration(PeriodUnit periodUnit)
	  {
		ensureNotNull(typeof(NotValidException), "periodUnit", periodUnit);
		this.durationPeriodUnit = periodUnit;

		CommandContext commandContext = Context.CommandContext;

		if (commandContext == null)
		{
		  return commandExecutor.execute(new CommandAnonymousInnerClass(this, commandContext));
		}
		else
		{
		  return executeDurationReport(commandContext);
		}

	  }

	  private class CommandAnonymousInnerClass : Command<IList<DurationReportResult>>
	  {
		  private readonly HistoricProcessInstanceReportImpl outerInstance;

		  private CommandContext commandContext;

		  public CommandAnonymousInnerClass(HistoricProcessInstanceReportImpl outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }


		  public IList<DurationReportResult> execute(CommandContext commandContext)
		  {
			return outerInstance.executeDurationReport(commandContext);
		  }

	  }

	  public virtual IList<DurationReportResult> executeDurationReport(CommandContext commandContext)
	  {

		doAuthCheck(commandContext);

		if (areNotInAscendingOrder(startedAfter_Conflict, startedBefore_Conflict))
		{
		  return Collections.emptyList();
		}

		return commandContext.HistoricReportManager.selectHistoricProcessInstanceDurationReport(this);

	  }

	  protected internal virtual void doAuthCheck(CommandContext commandContext)
	  {
		// since a report does only make sense in context of historic
		// data, the authorization check will be performed here
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  if (processDefinitionIdIn_Conflict == null && processDefinitionKeyIn_Conflict == null)
		  {
			checker.checkReadHistoryAnyProcessDefinition();
		  }
		  else
		  {
			IList<string> processDefinitionKeys = new List<string>();
			if (processDefinitionKeyIn_Conflict != null)
			{
			  ((IList<string>)processDefinitionKeys).AddRange(Arrays.asList(processDefinitionKeyIn_Conflict));
			}

			if (processDefinitionIdIn_Conflict != null)
			{
			  foreach (string processDefinitionId in processDefinitionIdIn_Conflict)
			  {
				ProcessDefinition processDefinition = commandContext.ProcessDefinitionManager.findLatestProcessDefinitionById(processDefinitionId);

				if (processDefinition != null && !string.ReferenceEquals(processDefinition.Key, null))
				{
				  processDefinitionKeys.Add(processDefinition.Key);
				}
			  }
			}

			if (processDefinitionKeys.Count > 0)
			{
			  foreach (string processDefinitionKey in processDefinitionKeys)
			  {
				checker.checkReadHistoryProcessDefinition(processDefinitionKey);
			  }
			}
		  }
		}
	  }

	  // getter //////////////////////////////////////////////////////

	  public virtual DateTime StartedAfter
	  {
		  get
		  {
			return startedAfter_Conflict;
		  }
	  }

	  public virtual DateTime StartedBefore
	  {
		  get
		  {
			return startedBefore_Conflict;
		  }
	  }

	  public virtual string[] ProcessDefinitionIdIn
	  {
		  get
		  {
			return processDefinitionIdIn_Conflict;
		  }
	  }

	  public virtual string[] ProcessDefinitionKeyIn
	  {
		  get
		  {
			return processDefinitionKeyIn_Conflict;
		  }
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