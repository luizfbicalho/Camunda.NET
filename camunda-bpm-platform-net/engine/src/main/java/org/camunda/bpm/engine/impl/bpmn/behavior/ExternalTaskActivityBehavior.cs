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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using BpmnExceptionHandler = org.camunda.bpm.engine.impl.bpmn.helper.BpmnExceptionHandler;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingExternalTaskInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingExternalTaskInstance;
	using MigratingInstanceParseContext = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;

	/// <summary>
	/// Implements behavior of external task activities, i.e. all service-task-like
	/// activities that have camunda:type="external".
	/// 
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class ExternalTaskActivityBehavior : AbstractBpmnActivityBehavior, MigrationObserverBehavior
	{

	  protected internal ParameterValueProvider topicNameValueProvider;
	  protected internal ParameterValueProvider priorityValueProvider;

	  public ExternalTaskActivityBehavior(ParameterValueProvider topicName, ParameterValueProvider paramValueProvider)
	  {
		this.topicNameValueProvider = topicName;
		this.priorityValueProvider = paramValueProvider;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		ExecutionEntity executionEntity = (ExecutionEntity) execution;
		PriorityProvider<ExternalTaskActivityBehavior> provider = Context.ProcessEngineConfiguration.ExternalTaskPriorityProvider;

		long priority = provider.determinePriority(executionEntity, this, null);
		string topic = (string) topicNameValueProvider.getValue(executionEntity);

		ExternalTaskEntity.createAndInsert(executionEntity, topic, priority);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		leave(execution);
	  }

	  public virtual ParameterValueProvider PriorityValueProvider
	  {
		  get
		  {
			return priorityValueProvider;
		  }
	  }

	  /// <summary>
	  /// It's used to propagate the bpmn error from an external task. </summary>
	  /// <param name="error"> the error which should be propagated </param>
	  /// <param name="execution"> the current activity execution </param>
	  /// <exception cref="Exception"> throws an exception if no handler was found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void propagateBpmnError(org.camunda.bpm.engine.delegate.BpmnError error, org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void propagateBpmnError(BpmnError error, ActivityExecution execution)
	  {
		BpmnExceptionHandler.propagateBpmnError(error, execution);
	  }

	  public virtual void migrateScope(ActivityExecution scopeExecution)
	  {
	  }

	  public virtual void onParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		ExecutionEntity execution = migratingInstance.resolveRepresentativeExecution();

		foreach (ExternalTaskEntity task in execution.ExternalTasks)
		{
		  MigratingExternalTaskInstance migratingTask = new MigratingExternalTaskInstance(task, migratingInstance);
		  migratingInstance.addMigratingDependentInstance(migratingTask);
		  parseContext.consume(task);
		  parseContext.submit(migratingTask);
		}
	  }
	}

}