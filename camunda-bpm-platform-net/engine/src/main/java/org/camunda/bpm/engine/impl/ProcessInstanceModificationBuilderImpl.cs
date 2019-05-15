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
	using AbstractInstantiationCmd = org.camunda.bpm.engine.impl.cmd.AbstractInstantiationCmd;
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using ActivityAfterInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityAfterInstantiationCmd;
	using ActivityBeforeInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityBeforeInstantiationCmd;
	using ActivityCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityCancellationCmd;
	using ActivityInstanceCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityInstanceCancellationCmd;
	using ModifyProcessInstanceAsyncCmd = org.camunda.bpm.engine.impl.cmd.ModifyProcessInstanceAsyncCmd;
	using ModifyProcessInstanceCmd = org.camunda.bpm.engine.impl.cmd.ModifyProcessInstanceCmd;
	using TransitionInstanceCancellationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstanceCancellationCmd;
	using TransitionInstantiationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstantiationCmd;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessInstanceModificationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationBuilder;
	using ProcessInstanceModificationInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstanceModificationInstantiationBuilder;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProcessInstanceModificationBuilderImpl : ProcessInstanceModificationInstantiationBuilder
	{

	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandContext commandContext;

	  protected internal string processInstanceId;
	  protected internal string modificationReason;

	  protected internal bool skipCustomListeners = false;
	  protected internal bool skipIoMappings = false;

	  protected internal IList<AbstractProcessInstanceModificationCommand> operations = new List<AbstractProcessInstanceModificationCommand>();

	  // variables not associated with an activity that are to be set on the instance itself
	  protected internal VariableMap processVariables = new VariableMapImpl();

	  public ProcessInstanceModificationBuilderImpl(CommandExecutor commandExecutor, string processInstanceId) : this(processInstanceId)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public ProcessInstanceModificationBuilderImpl(CommandContext commandContext, string processInstanceId) : this(processInstanceId)
	  {
		this.commandContext = commandContext;
	  }

	  public ProcessInstanceModificationBuilderImpl(CommandContext commandContext, string processInstanceId, string modificationReason) : this(processInstanceId)
	  {
		this.commandContext = commandContext;
		this.modificationReason = modificationReason;
	  }

	  public ProcessInstanceModificationBuilderImpl(string processInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "processInstanceId", processInstanceId);
		this.processInstanceId = processInstanceId;
	  }

	  public ProcessInstanceModificationBuilderImpl()
	  {
	  }

	  public virtual ProcessInstanceModificationBuilder cancelActivityInstance(string activityInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "activityInstanceId", activityInstanceId);
		operations.Add(new ActivityInstanceCancellationCmd(processInstanceId, activityInstanceId, this.modificationReason));
		return this;
	  }

	  public virtual ProcessInstanceModificationBuilder cancelTransitionInstance(string transitionInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "transitionInstanceId", transitionInstanceId);
		operations.Add(new TransitionInstanceCancellationCmd(processInstanceId, transitionInstanceId));
		return this;
	  }

	  public virtual ProcessInstanceModificationBuilder cancelAllForActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		operations.Add(new ActivityCancellationCmd(processInstanceId, activityId));
		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder startBeforeActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		AbstractInstantiationCmd currentInstantiation = new ActivityBeforeInstantiationCmd(processInstanceId, activityId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  public virtual ProcessInstanceModificationInstantiationBuilder startBeforeActivity(string activityId, string ancestorActivityInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		ensureNotNull(typeof(NotValidException), "ancestorActivityInstanceId", ancestorActivityInstanceId);
		AbstractInstantiationCmd currentInstantiation = new ActivityBeforeInstantiationCmd(processInstanceId, activityId, ancestorActivityInstanceId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder startAfterActivity(string activityId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		AbstractInstantiationCmd currentInstantiation = new ActivityAfterInstantiationCmd(processInstanceId, activityId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  public virtual ProcessInstanceModificationInstantiationBuilder startAfterActivity(string activityId, string ancestorActivityInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "activityId", activityId);
		ensureNotNull(typeof(NotValidException), "ancestorActivityInstanceId", ancestorActivityInstanceId);
		AbstractInstantiationCmd currentInstantiation = new ActivityAfterInstantiationCmd(processInstanceId, activityId, ancestorActivityInstanceId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder startTransition(string transitionId)
	  {
		ensureNotNull(typeof(NotValidException), "transitionId", transitionId);
		AbstractInstantiationCmd currentInstantiation = new TransitionInstantiationCmd(processInstanceId, transitionId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  public virtual ProcessInstanceModificationInstantiationBuilder startTransition(string transitionId, string ancestorActivityInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "transitionId", transitionId);
		ensureNotNull(typeof(NotValidException), "ancestorActivityInstanceId", ancestorActivityInstanceId);
		AbstractInstantiationCmd currentInstantiation = new TransitionInstantiationCmd(processInstanceId, transitionId, ancestorActivityInstanceId);
		operations.Add(currentInstantiation);
		return this;
	  }

	  protected internal virtual AbstractInstantiationCmd CurrentInstantiation
	  {
		  get
		  {
			if (operations.Count == 0)
			{
			  return null;
			}
    
			// casting should be safe
			AbstractProcessInstanceModificationCommand lastInstantiationCmd = operations[operations.Count - 1];
    
			if (!(lastInstantiationCmd is AbstractInstantiationCmd))
			{
			  throw new ProcessEngineException("last instruction is not an instantiation");
			}
    
			return (AbstractInstantiationCmd) lastInstantiationCmd;
		  }
	  }

	  public override ProcessInstanceModificationInstantiationBuilder setVariable(string name, object value)
	  {
		ensureNotNull(typeof(NotValidException), "Variable name must not be null", "name", name);

		AbstractInstantiationCmd currentInstantiation = CurrentInstantiation;
		if (currentInstantiation != null)
		{
		  currentInstantiation.addVariable(name, value);
		}
		else
		{
		  processVariables.put(name, value);
		}

		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder setVariableLocal(string name, object value)
	  {
		ensureNotNull(typeof(NotValidException), "Variable name must not be null", "name", name);

		AbstractInstantiationCmd currentInstantiation = CurrentInstantiation;
		if (currentInstantiation != null)
		{
		  currentInstantiation.addVariableLocal(name, value);
		}
		else
		{
		  processVariables.put(name, value);
		}

		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder setVariables(IDictionary<string, object> variables)
	  {
		ensureNotNull(typeof(NotValidException), "Variable map must not be null", "variables", variables);

		AbstractInstantiationCmd currentInstantiation = CurrentInstantiation;
		if (currentInstantiation != null)
		{
		  currentInstantiation.addVariables(variables);
		}
		else
		{
		  processVariables.putAll(variables);
		}
		return this;
	  }

	  public override ProcessInstanceModificationInstantiationBuilder setVariablesLocal(IDictionary<string, object> variables)
	  {
		ensureNotNull(typeof(NotValidException), "Variable map must not be null", "variablesLocal", variables);

		AbstractInstantiationCmd currentInstantiation = CurrentInstantiation;
		if (currentInstantiation != null)
		{
		  currentInstantiation.addVariablesLocal(variables);
		}
		else
		{
		  processVariables.putAll(variables);
		}
		return this;
	  }


	  public virtual void execute()
	  {
		execute(false, false);
	  }

	  public virtual void execute(bool skipCustomListeners, bool skipIoMappings)
	  {
		execute(true, skipCustomListeners, skipIoMappings);
	  }

	  public virtual void execute(bool writeUserOperationLog, bool skipCustomListeners, bool skipIoMappings)
	  {
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;

		ModifyProcessInstanceCmd cmd = new ModifyProcessInstanceCmd(this, writeUserOperationLog);
		if (commandExecutor != null)
		{
		  commandExecutor.execute(cmd);
		}
		else
		{
		  cmd.execute(commandContext);
		}
	  }

	  public virtual Batch executeAsync()
	  {
		return executeAsync(false, false);
	  }

	  public virtual Batch executeAsync(bool skipCustomListeners, bool skipIoMappings)
	  {
		this.skipCustomListeners = skipCustomListeners;
		this.skipIoMappings = skipIoMappings;

		return commandExecutor.execute(new ModifyProcessInstanceAsyncCmd(this));
	  }

	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
	  }

	  public virtual CommandContext CommandContext
	  {
		  get
		  {
			return commandContext;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual IList<AbstractProcessInstanceModificationCommand> ModificationOperations
	  {
		  get
		  {
			return operations;
		  }
		  set
		  {
			this.operations = value;
		  }
	  }


	  public virtual bool SkipCustomListeners
	  {
		  get
		  {
			return skipCustomListeners;
		  }
		  set
		  {
			this.skipCustomListeners = value;
		  }
	  }

	  public virtual bool SkipIoMappings
	  {
		  get
		  {
			return skipIoMappings;
		  }
		  set
		  {
			this.skipIoMappings = value;
		  }
	  }



	  public virtual VariableMap ProcessVariables
	  {
		  get
		  {
			return processVariables;
		  }
	  }

	  public virtual string ModificationReason
	  {
		  get
		  {
			return modificationReason;
		  }
		  set
		  {
			this.modificationReason = value;
		  }
	  }

	}

}