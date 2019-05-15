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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using UpdateProcessInstancesSuspendStateBatchCmd = org.camunda.bpm.engine.impl.cmd.UpdateProcessInstancesSuspendStateBatchCmd;
	using UpdateProcessInstancesSuspendStateCmd = org.camunda.bpm.engine.impl.cmd.UpdateProcessInstancesSuspendStateCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using UpdateProcessInstancesSuspensionStateBuilder = org.camunda.bpm.engine.runtime.UpdateProcessInstancesSuspensionStateBuilder;

	public class UpdateProcessInstancesSuspensionStateBuilderImpl : UpdateProcessInstancesSuspensionStateBuilder
	{

	  protected internal IList<string> processInstanceIds;
	  protected internal ProcessInstanceQuery processInstanceQuery;
	  protected internal HistoricProcessInstanceQuery historicProcessInstanceQuery;
	  protected internal CommandExecutor commandExecutor;
	  protected internal string processDefinitionId;

	  public UpdateProcessInstancesSuspensionStateBuilderImpl(CommandExecutor commandExecutor)
	  {
		this.processInstanceIds = new List<string>();
		this.commandExecutor = commandExecutor;
	  }

	  public virtual UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceIds(IList<string> processInstanceIds)
	  {
		((IList<string>)this.processInstanceIds).AddRange(processInstanceIds);
		return this;
	  }

	  public virtual UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceIds(params string[] processInstanceIds)
	  {
		((IList<string>)this.processInstanceIds).AddRange(Arrays.asList(processInstanceIds));
		return this;
	  }

	  public virtual UpdateProcessInstancesSuspensionStateBuilder byProcessInstanceQuery(ProcessInstanceQuery processInstanceQuery)
	  {
		this.processInstanceQuery = processInstanceQuery;
		return this;
	  }

	  public virtual UpdateProcessInstancesSuspensionStateBuilder byHistoricProcessInstanceQuery(HistoricProcessInstanceQuery historicProcessInstanceQuery)
	  {
		this.historicProcessInstanceQuery = historicProcessInstanceQuery;
		return this;
	  }

	  public virtual void suspend()
	  {
		commandExecutor.execute(new UpdateProcessInstancesSuspendStateCmd(commandExecutor, this, true));
	  }

	  public virtual void activate()
	  {
		commandExecutor.execute(new UpdateProcessInstancesSuspendStateCmd(commandExecutor, this, false));
	  }

	  public virtual Batch suspendAsync()
	  {
		return commandExecutor.execute(new UpdateProcessInstancesSuspendStateBatchCmd(commandExecutor, this, true));
	  }

	  public virtual Batch activateAsync()
	  {
		return commandExecutor.execute(new UpdateProcessInstancesSuspendStateBatchCmd(commandExecutor, this, false));
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds;
		  }
	  }

	  public virtual ProcessInstanceQuery ProcessInstanceQuery
	  {
		  get
		  {
			return processInstanceQuery;
		  }
	  }

	  public virtual HistoricProcessInstanceQuery HistoricProcessInstanceQuery
	  {
		  get
		  {
			return historicProcessInstanceQuery;
		  }
	  }

	}

}