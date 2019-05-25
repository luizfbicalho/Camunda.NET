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


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using RestartProcessInstancesBatchCmd = org.camunda.bpm.engine.impl.batch.RestartProcessInstancesBatchCmd;
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using ActivityAfterInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityAfterInstantiationCmd;
	using ActivityBeforeInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityBeforeInstantiationCmd;
	using RestartProcessInstancesCmd = org.camunda.bpm.engine.impl.cmd.RestartProcessInstancesCmd;
	using TransitionInstantiationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstantiationCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using RestartProcessInstanceBuilder = org.camunda.bpm.engine.runtime.RestartProcessInstanceBuilder;

	/// <summary>
	/// @author Anna Pazola
	/// </summary>
	public class RestartProcessInstanceBuilderImpl : RestartProcessInstanceBuilder
	{

	  protected internal CommandExecutor commandExecutor;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> processInstanceIds_Conflict;
	  protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
	  protected internal string processDefinitionId;
	  protected internal HistoricProcessInstanceQuery query;
	  protected internal bool initialVariables;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipCustomListeners_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipIoMappings_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool withoutBusinessKey_Conflict;

	  public RestartProcessInstanceBuilderImpl(CommandExecutor commandExecutor, string processDefinitionId)
	  {
		ensureNotNull(typeof(NotValidException), "processDefinitionId", processDefinitionId);
		this.commandExecutor = commandExecutor;
		this.instructions = new List<AbstractProcessInstanceModificationCommand>();
		this.processDefinitionId = processDefinitionId;
		this.processInstanceIds_Conflict = new List<string>();
	  }

	  public override RestartProcessInstanceBuilder startBeforeActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		instructions.Add(new ActivityBeforeInstantiationCmd(null, activityId));
		return this;
	  }

	  public override RestartProcessInstanceBuilder startAfterActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		instructions.Add(new ActivityAfterInstantiationCmd(null, activityId));
		return this;
	  }

	  public override RestartProcessInstanceBuilder startTransition(string transitionId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", transitionId);
		instructions.Add(new TransitionInstantiationCmd(null, transitionId));
		return this;
	  }

	  public virtual void execute()
	  {
		execute(true);
	  }

	  public virtual void execute(bool writeUserOperationLog)
	  {
		commandExecutor.execute(new RestartProcessInstancesCmd(commandExecutor, this, writeUserOperationLog));
	  }

	  public virtual Batch executeAsync()
	  {
		return commandExecutor.execute(new RestartProcessInstancesBatchCmd(commandExecutor, this));
	  }

	  public virtual IList<AbstractProcessInstanceModificationCommand> Instructions
	  {
		  get
		  {
			return instructions;
		  }
		  set
		  {
			this.instructions = value;
		  }
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Conflict;
		  }
	  }

	  public virtual RestartProcessInstanceBuilder processInstanceIds(params string[] processInstanceIds)
	  {
		((IList<string>)this.processInstanceIds_Conflict).AddRange(Arrays.asList(processInstanceIds));
		return this;
	  }

	  public virtual RestartProcessInstanceBuilder historicProcessInstanceQuery(HistoricProcessInstanceQuery query)
	  {
		this.query = query;
		return this;
	  }

	  public virtual HistoricProcessInstanceQuery HistoricProcessInstanceQuery
	  {
		  get
		  {
			return query;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }



	  public virtual RestartProcessInstanceBuilder processInstanceIds(IList<string> processInstanceIds)
	  {
		((IList<string>)this.processInstanceIds_Conflict).AddRange(processInstanceIds);
		return this;
	  }

	  public virtual RestartProcessInstanceBuilder initialSetOfVariables()
	  {
		this.initialVariables = true;
		return this;
	  }

	  public virtual bool InitialVariables
	  {
		  get
		  {
			return initialVariables;
		  }
	  }

	  public virtual RestartProcessInstanceBuilder skipCustomListeners()
	  {
		this.skipCustomListeners_Conflict = true;
		return this;
	  }

	  public virtual RestartProcessInstanceBuilder skipIoMappings()
	  {
		this.skipIoMappings_Conflict = true;
		return this;
	  }

	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners_Conflict;
		  }
	  }

	  public virtual bool SkipIoMappings
	  {
		  get
		  {
			return skipIoMappings_Conflict;
		  }
	  }

	  public virtual RestartProcessInstanceBuilder withoutBusinessKey()
	  {
		withoutBusinessKey_Conflict = true;
		return this;
	  }

	  public virtual bool WithoutBusinessKey
	  {
		  get
		  {
			return withoutBusinessKey_Conflict;
		  }
	  }
	}

}