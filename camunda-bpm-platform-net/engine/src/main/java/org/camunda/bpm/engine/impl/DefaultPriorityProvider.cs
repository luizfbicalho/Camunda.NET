using System;

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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;

	/// <summary>
	/// Represents a default priority provider, which contains some functionality to evaluate the priority.
	/// Can be used as base class for other priority providers. *  
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com> </summary>
	/// @param <T> the type of the param to determine the priority </param>
	public abstract class DefaultPriorityProvider<T> : PriorityProvider<T>
	{

	  /// <summary>
	  /// The default priority.
	  /// </summary>
	  public static long DEFAULT_PRIORITY = 0;

	  /// <summary>
	  /// The default priority in case of resolution failure.
	  /// </summary>
	  public static long DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = 0;

	  /// <summary>
	  /// Returns the default priority.
	  /// </summary>
	  /// <returns> the default priority </returns>
	  public virtual long DefaultPriority
	  {
		  get
		  {
			return DEFAULT_PRIORITY;
		  }
	  }

	  /// <summary>
	  /// Returns the default priority in case of resolution failure.
	  /// </summary>
	  /// <returns> the default priority </returns>
	  public virtual long DefaultPriorityOnResolutionFailure
	  {
		  get
		  {
			return DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE;
		  }
	  }

	  /// <summary>
	  /// Evaluates a given value provider with the given execution entity to determine
	  /// the correct value. The error message heading is used for the error message 
	  /// if the validation fails because the value is no valid priority.
	  /// </summary>
	  /// <param name="valueProvider"> the provider which contains the value </param>
	  /// <param name="execution"> the execution entity </param>
	  /// <param name="errorMessageHeading"> the heading which is used for the error message </param>
	  /// <returns> the valid priority value </returns>
	  protected internal virtual long? evaluateValueProvider(ParameterValueProvider valueProvider, ExecutionEntity execution, string errorMessageHeading)
	  {
		object value;
		try
		{
		  value = valueProvider.getValue(execution);

		}
		catch (ProcessEngineException e)
		{

		  if (Context.ProcessEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure && isSymptomOfContextSwitchFailure(e, execution))
		  {

			value = DefaultPriorityOnResolutionFailure;
			logNotDeterminingPriority(execution, value, e);
		  }
		  else
		  {
			throw e;
		  }
		}

		if (!(value is Number))
		{
		  throw new ProcessEngineException(errorMessageHeading + ": Priority value is not an Integer");
		}
		else
		{
		  Number numberValue = (Number) value;
		  if (isValidLongValue(numberValue))
		  {
			return numberValue.longValue();
		  }
		  else
		  {
			throw new ProcessEngineException(errorMessageHeading + ": Priority value must be either Short, Integer, or Long");
		  }
		}
	  }

	  public virtual long determinePriority(ExecutionEntity execution, T param, string jobDefinitionId)
	  {
		if (param != default(T) || execution != null)
		{
		  long? specificPriority = getSpecificPriority(execution, param, jobDefinitionId);
		  if (specificPriority != null)
		  {
			return specificPriority.Value;
		  }

		  long? processDefinitionPriority = getProcessDefinitionPriority(execution, param);
		  if (processDefinitionPriority != null)
		  {
			return processDefinitionPriority.Value;
		  }
		}
		return DefaultPriority;
	  }

	  /// <summary>
	  /// Returns the priority defined in the specific entity. Like a job definition priority or
	  /// an activity priority. The result can also be null in that case the process 
	  /// priority will be used.
	  /// </summary>
	  /// <param name="execution"> the current execution </param>
	  /// <param name="param"> the generic param </param>
	  /// <param name="jobDefinitionId"> the job definition id if related to a job </param>
	  /// <returns> the specific priority </returns>
	  protected internal abstract long? getSpecificPriority(ExecutionEntity execution, T param, string jobDefinitionId);

	  /// <summary>
	  /// Returns the priority defined in the process definition. Can also be null
	  /// in that case the fallback is the default priority.
	  /// </summary>
	  /// <param name="execution"> the current execution </param>
	  /// <param name="param"> the generic param </param>
	  /// <returns> the priority defined in the process definition </returns>
	  protected internal abstract long? getProcessDefinitionPriority(ExecutionEntity execution, T param);

	  /// <summary>
	  /// Returns the priority which is defined in the given process definition.
	  /// The priority value is identified with the given propertyKey. 
	  /// Returns null if the process definition is null or no priority was defined.
	  /// </summary>
	  /// <param name="processDefinition"> the process definition that should contains the priority </param>
	  /// <param name="propertyKey"> the key which identifies the property </param>
	  /// <param name="execution"> the current execution </param>
	  /// <param name="errorMsgHead"> the error message header which is used if the evaluation fails </param>
	  /// <returns> the priority defined in the given process </returns>
	  protected internal virtual long? getProcessDefinedPriority(ProcessDefinitionImpl processDefinition, string propertyKey, ExecutionEntity execution, string errorMsgHead)
	  {
		if (processDefinition != null)
		{
		  ParameterValueProvider priorityProvider = (ParameterValueProvider) processDefinition.getProperty(propertyKey);
		  if (priorityProvider != null)
		  {
			return evaluateValueProvider(priorityProvider, execution, errorMsgHead);
		  }
		}
		return null;
	  }
	  /// <summary>
	  /// Logs the exception which was thrown if the priority can not be determined.
	  /// </summary>
	  /// <param name="execution"> the current execution entity </param>
	  /// <param name="value"> the current value </param>
	  /// <param name="e"> the exception which was catched </param>
	  protected internal abstract void logNotDeterminingPriority(ExecutionEntity execution, object value, ProcessEngineException e);


	  protected internal virtual bool isSymptomOfContextSwitchFailure(Exception t, ExecutionEntity contextExecution)
	  {
		// a context switch failure can occur, if the current engine has no PA registration for the deployment
		// subclasses may assert the actual throwable to narrow down the diagnose
		return ProcessApplicationContextUtil.getTargetProcessApplication(contextExecution) == null;
	  }

	  /// <summary>
	  /// Checks if the given number is a valid long value. </summary>
	  /// <param name="value"> the number which should be checked </param>
	  /// <returns> true if is a valid long value, false otherwise </returns>
	  protected internal virtual bool isValidLongValue(Number value)
	  {
		return value is short? || value is int? || value is long?;
	  }
	}

}