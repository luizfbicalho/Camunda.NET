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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using DelegateVariableMapping = org.camunda.bpm.engine.@delegate.DelegateVariableMapping;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior.NUMBER_OF_ACTIVE_INSTANCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior.NUMBER_OF_COMPLETED_INSTANCES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior.NUMBER_OF_INSTANCES;

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;
	using CallableElement = org.camunda.bpm.engine.impl.core.model.CallableElement;
	using DelegateInvocation = org.camunda.bpm.engine.impl.@delegate.DelegateInvocation;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ClassDelegateUtil = org.camunda.bpm.engine.impl.util.ClassDelegateUtil;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class CallableElementActivityBehavior : AbstractBpmnActivityBehavior, SubProcessActivityBehavior
	{

	  protected internal string[] variablesFilter = new string[] {NUMBER_OF_INSTANCES, NUMBER_OF_ACTIVE_INSTANCES, NUMBER_OF_COMPLETED_INSTANCES};

	  protected internal CallableElement callableElement;

	  /// <summary>
	  /// The expression which identifies the delegation for the variable mapping.
	  /// </summary>
	  protected internal Expression expression;

	  /// <summary>
	  /// The class name of the delegated variable mapping, which should be used.
	  /// </summary>
	  protected internal string className;

	  public CallableElementActivityBehavior()
	  {
	  }

	  public CallableElementActivityBehavior(string className)
	  {
		this.className = className;
	  }

	  public CallableElementActivityBehavior(Expression expression)
	  {
		this.expression = expression;
	  }

	  protected internal virtual DelegateVariableMapping getDelegateVariableMapping(object instance)
	  {
		if (instance is DelegateVariableMapping)
		{
		  return (DelegateVariableMapping) instance;
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.missingDelegateVariableMappingParentClassException(instance.GetType().FullName, typeof(DelegateVariableMapping).FullName);
		}
	  }

	  protected internal virtual DelegateVariableMapping resolveDelegation(ActivityExecution execution)
	  {
		object @delegate = resolveDelegateClass(execution);
		return @delegate != null ? getDelegateVariableMapping(@delegate) : null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Object resolveDelegateClass(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution)
	  public virtual object resolveDelegateClass(ActivityExecution execution)
	  {
		ProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
		if (ProcessApplicationContextUtil.requiresContextSwitch(targetProcessApplication))
		{
		  return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, execution)
		 , targetProcessApplication, new InvocationContext(execution));
		}
		else
		{
		  return instantiateDelegateClass(execution);
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<object>
	  {
		  private readonly CallableElementActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(CallableElementActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object call() throws Exception
		  public override object call()
		  {
			return outerInstance.resolveDelegateClass(execution);
		  }
	  }

	  protected internal virtual object instantiateDelegateClass(ActivityExecution execution)
	  {
		object @delegate = null;
		if (expression != null)
		{
		  @delegate = expression.getValue(execution);
		}
		else if (!string.ReferenceEquals(className, null))
		{
		  @delegate = ClassDelegateUtil.instantiateDelegate(className, null);
		}
		return @delegate;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void execute(ActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.variable.VariableMap variables = getInputVariables(execution);
		VariableMap variables = getInputVariables(execution);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.DelegateVariableMapping varMapping = resolveDelegation(execution);
		DelegateVariableMapping varMapping = resolveDelegation(execution);
		if (varMapping != null)
		{
		  invokeVarMappingDelegation(new DelegateInvocationAnonymousInnerClass(this, execution, variables, varMapping));
		}

		string businessKey = getBusinessKey(execution);
		startInstance(execution, variables, businessKey);
	  }

	  private class DelegateInvocationAnonymousInnerClass : DelegateInvocation
	  {
		  private readonly CallableElementActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private VariableMap variables;
		  private DelegateVariableMapping varMapping;

		  public DelegateInvocationAnonymousInnerClass(CallableElementActivityBehavior outerInstance, ActivityExecution execution, VariableMap variables, DelegateVariableMapping varMapping) : base(execution, null)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.variables = variables;
			  this.varMapping = varMapping;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void invoke() throws Exception
		  protected internal override void invoke()
		  {
			varMapping.mapInputVariables(execution, variables);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void passOutputVariables(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final org.camunda.bpm.engine.delegate.VariableScope subInstance)
	  public virtual void passOutputVariables(ActivityExecution execution, VariableScope subInstance)
	  {
		// only data. no control flow available on this execution.
		VariableMap variables = filterVariables(getOutputVariables(subInstance));
		VariableMap localVariables = getOutputVariablesLocal(subInstance);

		execution.Variables = variables;
		execution.VariablesLocal = localVariables;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.DelegateVariableMapping varMapping = resolveDelegation(execution);
		DelegateVariableMapping varMapping = resolveDelegation(execution);
		if (varMapping != null)
		{
		  invokeVarMappingDelegation(new DelegateInvocationAnonymousInnerClass2(this, execution, subInstance, varMapping));
		}
	  }

	  private class DelegateInvocationAnonymousInnerClass2 : DelegateInvocation
	  {
		  private readonly CallableElementActivityBehavior outerInstance;

		  private ActivityExecution execution;
		  private VariableScope subInstance;
		  private DelegateVariableMapping varMapping;

		  public DelegateInvocationAnonymousInnerClass2(CallableElementActivityBehavior outerInstance, ActivityExecution execution, VariableScope subInstance, DelegateVariableMapping varMapping) : base(execution, null)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
			  this.subInstance = subInstance;
			  this.varMapping = varMapping;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void invoke() throws Exception
		  protected internal override void invoke()
		  {
			varMapping.mapOutputVariables(execution, subInstance);
		  }
	  }

	  protected internal virtual void invokeVarMappingDelegation(DelegateInvocation delegation)
	  {
		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(delegation);
		}
		catch (Exception ex)
		{
		  throw new ProcessEngineException(ex);
		}
	  }

	  protected internal virtual VariableMap filterVariables(VariableMap variables)
	  {
		if (variables != null)
		{
		  foreach (string key in variablesFilter)
		  {
			variables.remove(key);
		  }
		}
		return variables;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void completed(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void completed(ActivityExecution execution)
	  {
		// only control flow. no sub instance data available
		leave(execution);
	  }

	  public virtual CallableElement CallableElement
	  {
		  get
		  {
			return callableElement;
		  }
		  set
		  {
			this.callableElement = value;
		  }
	  }


	  protected internal virtual string getBusinessKey(ActivityExecution execution)
	  {
		return CallableElement.getBusinessKey(execution);
	  }

	  protected internal virtual VariableMap getInputVariables(ActivityExecution callingExecution)
	  {
		return CallableElement.getInputVariables(callingExecution);
	  }

	  protected internal virtual VariableMap getOutputVariables(VariableScope calledElementScope)
	  {
		return CallableElement.getOutputVariables(calledElementScope);
	  }

	  protected internal virtual VariableMap getOutputVariablesLocal(VariableScope calledElementScope)
	  {
		return CallableElement.getOutputVariablesLocal(calledElementScope);
	  }

	  protected internal virtual int? getVersion(ActivityExecution execution)
	  {
		return CallableElement.getVersion(execution);
	  }

	  protected internal virtual string getDeploymentId(ActivityExecution execution)
	  {
		return CallableElement.DeploymentId;
	  }

	  protected internal virtual CallableElementBinding Binding
	  {
		  get
		  {
			return CallableElement.Binding;
		  }
	  }

	  protected internal virtual bool LatestBinding
	  {
		  get
		  {
			return CallableElement.LatestBinding;
		  }
	  }

	  protected internal virtual bool DeploymentBinding
	  {
		  get
		  {
			return CallableElement.DeploymentBinding;
		  }
	  }

	  protected internal virtual bool VersionBinding
	  {
		  get
		  {
			return CallableElement.VersionBinding;
		  }
	  }

	  protected internal abstract void startInstance(ActivityExecution execution, VariableMap variables, string businessKey);

	}

}