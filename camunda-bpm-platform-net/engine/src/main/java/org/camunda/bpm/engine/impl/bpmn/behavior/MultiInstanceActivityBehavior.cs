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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using ModificationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ModificationObserverBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using IntegerValue = org.camunda.bpm.engine.variable.value.IntegerValue;


	/// <summary>
	/// Abstract Multi Instance Behavior: used for both parallel and sequential
	/// multi instance implementation.
	/// 
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public abstract class MultiInstanceActivityBehavior : AbstractBpmnActivityBehavior, CompositeActivityBehavior, ModificationObserverBehavior
	{
		public abstract void destroyInnerInstance(ActivityExecution concurrentExecution);
		public abstract ActivityExecution createInnerInstance(ActivityExecution scopeExecution);
		public abstract IList<ActivityExecution> initializeScope(ActivityExecution scopeExecution, int nrOfInnerInstances);
		public abstract void complete(ActivityExecution scopeExecution);
		public abstract void concurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution);

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  // Variable names for mi-body scoped variables (as described in spec)
	  public const string NUMBER_OF_INSTANCES = "nrOfInstances";
	  public const string NUMBER_OF_ACTIVE_INSTANCES = "nrOfActiveInstances";
	  public const string NUMBER_OF_COMPLETED_INSTANCES = "nrOfCompletedInstances";

	  // Variable names for mi-instance scoped variables (as described in the spec)
	  public const string LOOP_COUNTER = "loopCounter";

	  protected internal Expression loopCardinalityExpression;
	  protected internal Expression completionConditionExpression;
	  protected internal Expression collectionExpression;
	  protected internal string collectionVariable;
	  protected internal string collectionElementVariable;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		int nrOfInstances = resolveNrOfInstances(execution);
		if (nrOfInstances == 0)
		{
		  leave(execution);
		}
		else if (nrOfInstances < 0)
		{
		  throw LOG.invalidAmountException("instances", nrOfInstances);
		}
		else
		{
		  createInstances(execution, nrOfInstances);
		}
	  }

	  protected internal virtual void performInstance(ActivityExecution execution, PvmActivity activity, int loopCounter)
	  {
		setLoopVariable(execution, LOOP_COUNTER, loopCounter);
		evaluateCollectionVariable(execution, loopCounter);
		execution.Ended = false;
		execution.Active = true;
		execution.executeActivity(activity);
	  }

	  protected internal virtual void evaluateCollectionVariable(ActivityExecution execution, int loopCounter)
	  {
		if (usesCollection() && !string.ReferenceEquals(collectionElementVariable, null))
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Collection<?> collection = null;
		  ICollection<object> collection = null;
		  if (collectionExpression != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: collection = (java.util.Collection<?>) collectionExpression.getValue(execution);
			collection = (ICollection<object>) collectionExpression.getValue(execution);
		  }
		  else if (!string.ReferenceEquals(collectionVariable, null))
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: collection = (java.util.Collection<?>) execution.getVariable(collectionVariable);
			collection = (ICollection<object>) execution.getVariable(collectionVariable);
		  }

		  object value = getElementAtIndex(loopCounter, collection);
		  setLoopVariable(execution, collectionElementVariable, value);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void createInstances(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, int nrOfInstances) throws Exception;
	  protected internal abstract void createInstances(ActivityExecution execution, int nrOfInstances);

	  // Helpers //////////////////////////////////////////////////////////////////////

	  protected internal virtual int resolveNrOfInstances(ActivityExecution execution)
	  {
		int nrOfInstances = -1;
		if (loopCardinalityExpression != null)
		{
		  nrOfInstances = resolveLoopCardinality(execution);
		}
		else if (collectionExpression != null)
		{
		  object obj = collectionExpression.getValue(execution);
		  if (!(obj is System.Collections.ICollection))
		  {
			throw LOG.unresolvableExpressionException(collectionExpression.ExpressionText, "Collection");
		  }
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: nrOfInstances = ((java.util.Collection<?>) obj).size();
		  nrOfInstances = ((ICollection<object>) obj).Count;
		}
		else if (!string.ReferenceEquals(collectionVariable, null))
		{
		  object obj = execution.getVariable(collectionVariable);
		  if (!(obj is System.Collections.ICollection))
		  {
			throw LOG.invalidVariableTypeException(collectionVariable, "Collection");
		  }
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: nrOfInstances = ((java.util.Collection<?>) obj).size();
		  nrOfInstances = ((ICollection<object>) obj).Count;
		}
		else
		{
		  throw LOG.resolveCollectionExpressionOrVariableReferenceException();
		}
		return nrOfInstances;
	  }

	  protected internal virtual object getElementAtIndex<T1>(int i, ICollection<T1> collection)
	  {
		object value = null;
		int index = 0;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Iterator<?> it = collection.iterator();
		IEnumerator<object> it = collection.GetEnumerator();
		while (index <= i)
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  value = it.next();
		  index++;
		}
		return value;
	  }

	  protected internal virtual bool usesCollection()
	  {
		return collectionExpression != null || !string.ReferenceEquals(collectionVariable, null);
	  }

	  protected internal virtual int resolveLoopCardinality(ActivityExecution execution)
	  {
		// Using Number since expr can evaluate to eg. Long (which is also the default for Juel)
		object value = loopCardinalityExpression.getValue(execution);
		if (value is Number)
		{
		  return ((Number) value).intValue();
		}
		else if (value is string)
		{
		  return Convert.ToInt32((string) value);
		}
		else
		{
		  throw LOG.expressionNotANumberException("loopCardinality", loopCardinalityExpression.ExpressionText);
		}
	  }

	  protected internal virtual bool completionConditionSatisfied(ActivityExecution execution)
	  {
		if (completionConditionExpression != null)
		{
		  object value = completionConditionExpression.getValue(execution);
		  if (!(value is bool?))
		  {
			throw LOG.expressionNotBooleanException("completionCondition", completionConditionExpression.ExpressionText);
		  }
		  bool? booleanValue = (bool?) value;

		  LOG.multiInstanceCompletionConditionState(booleanValue);
		  return booleanValue.Value;
		}
		return false;
	  }

	  public override void doLeave(ActivityExecution execution)
	  {
		CompensationUtil.createEventScopeExecution((ExecutionEntity) execution);

		base.doLeave(execution);
	  }

	  /// <summary>
	  /// Get the inner activity of the multi instance execution.
	  /// </summary>
	  /// <param name="execution">
	  ///          of multi instance activity </param>
	  /// <returns> inner activity </returns>
	  public virtual ActivityImpl getInnerActivity(PvmActivity miBodyActivity)
	  {
		foreach (PvmActivity activity in miBodyActivity.Activities)
		{
		  ActivityImpl innerActivity = (ActivityImpl) activity;
		  // note that miBody can contains also a compensation handler
		  if (!innerActivity.CompensationHandler)
		  {
			return innerActivity;
		  }
		}
		throw new ProcessEngineException("inner activity of multi instance body activity '" + miBodyActivity.Id + "' not found");
	  }

	  protected internal virtual void setLoopVariable(ActivityExecution execution, string variableName, object value)
	  {
		execution.setVariableLocal(variableName, value);
	  }

	  protected internal virtual int? getLoopVariable(ActivityExecution execution, string variableName)
	  {
		IntegerValue value = execution.getVariableLocalTyped(variableName);
		ensureNotNull("The variable \"" + variableName + "\" could not be found in execution with id " + execution.Id, "value", value);
		return value.Value;
	  }


	  protected internal virtual int? getLocalLoopVariable(ActivityExecution execution, string variableName)
	  {
		return (int?) execution.getVariableLocal(variableName);
	  }

	  public virtual bool hasLoopVariable(ActivityExecution execution, string variableName)
	  {
		return execution.hasVariableLocal(variableName);
	  }

	  public virtual void removeLoopVariable(ActivityExecution execution, string variableName)
	  {
		execution.removeVariableLocal(variableName);
	  }

	  // Getters and Setters ///////////////////////////////////////////////////////////

	  public virtual Expression LoopCardinalityExpression
	  {
		  get
		  {
			return loopCardinalityExpression;
		  }
		  set
		  {
			this.loopCardinalityExpression = value;
		  }
	  }


	  public virtual Expression CompletionConditionExpression
	  {
		  get
		  {
			return completionConditionExpression;
		  }
		  set
		  {
			this.completionConditionExpression = value;
		  }
	  }


	  public virtual Expression CollectionExpression
	  {
		  get
		  {
			return collectionExpression;
		  }
		  set
		  {
			this.collectionExpression = value;
		  }
	  }


	  public virtual string CollectionVariable
	  {
		  get
		  {
			return collectionVariable;
		  }
		  set
		  {
			this.collectionVariable = value;
		  }
	  }


	  public virtual string CollectionElementVariable
	  {
		  get
		  {
			return collectionElementVariable;
		  }
		  set
		  {
			this.collectionElementVariable = value;
		  }
	  }


	}

}