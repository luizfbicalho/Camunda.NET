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
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using ActivityAfterInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityAfterInstantiationCmd;
	using ActivityBeforeInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityBeforeInstantiationCmd;
	using ActivityCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityCancellationCmd;
	using ProcessInstanceModificationBatchCmd = org.camunda.bpm.engine.impl.cmd.ProcessInstanceModificationBatchCmd;
	using ProcessInstanceModificationCmd = org.camunda.bpm.engine.impl.cmd.ProcessInstanceModificationCmd;
	using TransitionInstantiationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstantiationCmd;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ModificationBuilder = org.camunda.bpm.engine.runtime.ModificationBuilder;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	public class ModificationBuilderImpl : ModificationBuilder
	{

	  protected internal CommandExecutor commandExecutor;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ProcessInstanceQuery processInstanceQuery_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IList<string> processInstanceIds_Conflict;
	  protected internal IList<AbstractProcessInstanceModificationCommand> instructions;
	  protected internal string processDefinitionId;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipCustomListeners_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool skipIoMappings_Conflict;

	  public ModificationBuilderImpl(CommandExecutor commandExecutor, string processDefinitionId)
	  {
		this.commandExecutor = commandExecutor;
		ensureNotNull(typeof(NotValidException),"processDefinitionId", processDefinitionId);
		this.processDefinitionId = processDefinitionId;
		processInstanceIds_Conflict = new List<string>();
		instructions = new List<AbstractProcessInstanceModificationCommand>();
	  }

	  public override ModificationBuilder startBeforeActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		instructions.Add(new ActivityBeforeInstantiationCmd(activityId));
		return this;
	  }

	  public override ModificationBuilder startAfterActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		instructions.Add(new ActivityAfterInstantiationCmd(activityId));
		return this;
	  }

	  public override ModificationBuilder startTransition(string transitionId)
	  {
		ensureNotNull(typeof(NotValidException), "transitionId", transitionId);
		instructions.Add(new TransitionInstantiationCmd(transitionId));
		return this;
	  }

	  public virtual ModificationBuilder cancelAllForActivity(string activityId)
	  {
		return cancelAllForActivity(activityId, false);
	  }

	  public virtual ModificationBuilder cancelAllForActivity(string activityId, bool cancelCurrentActiveActivityInstances)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		ActivityCancellationCmd activityCancellationCmd = new ActivityCancellationCmd(activityId);
		activityCancellationCmd.CancelCurrentActiveActivityInstances = cancelCurrentActiveActivityInstances;
		instructions.Add(activityCancellationCmd);
		return this;
	  }

	  public virtual ModificationBuilder processInstanceIds(IList<string> processInstanceIds)
	  {
		this.processInstanceIds_Conflict = processInstanceIds;
		return this;
	  }

	  public virtual ModificationBuilder processInstanceIds(params string[] processInstanceIds)
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

	  public virtual ModificationBuilder processInstanceQuery(ProcessInstanceQuery processInstanceQuery)
	  {
		this.processInstanceQuery_Conflict = processInstanceQuery;
		return this;
	  }

	  public virtual ModificationBuilder skipCustomListeners()
	  {
		this.skipCustomListeners_Conflict = true;
		return this;
	  }

	  public virtual ModificationBuilder skipIoMappings()
	  {
		this.skipIoMappings_Conflict = true;
		return this;
	  }

	  public virtual void execute(bool writeUserOperationLog)
	  {
		commandExecutor.execute(new ProcessInstanceModificationCmd(this, writeUserOperationLog));
	  }

	  public virtual void execute()
	  {
		execute(true);
	  }

	  public virtual Batch executeAsync()
	  {
		return commandExecutor.execute(new ProcessInstanceModificationBatchCmd(this));
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual ProcessInstanceQuery ProcessInstanceQuery
	  {
		  get
		  {
			return processInstanceQuery_Conflict;
		  }
	  }

	  public virtual IList<string> ProcessInstanceIds
	  {
		  get
		  {
			return processInstanceIds_Conflict;
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

	}

}