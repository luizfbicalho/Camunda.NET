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

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
	using UpdateExternalTaskRetriesBuilder = org.camunda.bpm.engine.externaltask.UpdateExternalTaskRetriesBuilder;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	/// <summary>
	/// @author smirnov
	/// 
	/// </summary>
	public class UpdateExternalTaskRetriesBuilderImpl : UpdateExternalTaskRetriesBuilder
	{

	  protected internal CommandExecutor commandExecutor;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> externalTaskIds_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> processInstanceIds_Conflict;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ExternalTaskQuery externalTaskQuery_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ProcessInstanceQuery processInstanceQuery_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal HistoricProcessInstanceQuery historicProcessInstanceQuery_Conflict;

	  protected internal int retries;

	  public UpdateExternalTaskRetriesBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder externalTaskIds(IList<string> externalTaskIds)
	  {
		this.externalTaskIds_Conflict = externalTaskIds;
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder externalTaskIds(params string[] externalTaskIds)
	  {
		if (externalTaskIds == null)
		{
		  this.externalTaskIds_Conflict = Collections.emptyList();
		}
		else
		{
		  this.externalTaskIds_Conflict = Arrays.asList(externalTaskIds);
		}
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder processInstanceIds(IList<string> processInstanceIds)
	  {
		this.processInstanceIds_Conflict = processInstanceIds;
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder processInstanceIds(params string[] processInstanceIds)
	  {
		if (processInstanceIds == null)
		{
		  this.processInstanceIds_Conflict = Collections.emptyList();
		}
		else
		{
		  this.processInstanceIds_Conflict = Arrays.asList(processInstanceIds);
		}
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder externalTaskQuery(ExternalTaskQuery externalTaskQuery)
	  {
		this.externalTaskQuery_Conflict = externalTaskQuery;
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery)
	  {
		this.processInstanceQuery_Conflict = processInstanceQuery;
		return this;
	  }

	  public virtual UpdateExternalTaskRetriesBuilder historicProcessInstanceQuery(HistoricProcessInstanceQuery historicProcessInstanceQuery)
	  {
		this.historicProcessInstanceQuery_Conflict = historicProcessInstanceQuery;
		return this;
	  }

	  public virtual void set(int retries)
	  {
		this.retries = retries;
		commandExecutor.execute(new SetExternalTasksRetriesCmd(this));
	  }

	  public virtual Batch setAsync(int retries)
	  {
		this.retries = retries;
		return commandExecutor.execute(new SetExternalTasksRetriesBatchCmd(this));
	  }

	  public virtual int Retries
	  {
		  get
		  {
			return retries;
		  }
	  }

	  public virtual IList<string> ExternalTaskIds
	  {
		  get
		  {
			return externalTaskIds_Conflict;
		  }
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Conflict;
		  }
	  }

	  public virtual ExternalTaskQuery ExternalTaskQuery
	  {
		  get
		  {
			return externalTaskQuery_Conflict;
		  }
	  }

	  public virtual ProcessInstanceQuery ProcessInstanceQuery
	  {
		  get
		  {
			return processInstanceQuery_Conflict;
		  }
	  }

	  public virtual HistoricProcessInstanceQuery HistoricProcessInstanceQuery
	  {
		  get
		  {
			return historicProcessInstanceQuery_Conflict;
		  }
	  }

	}

}