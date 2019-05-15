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
namespace org.camunda.bpm.engine.impl.form.validator
{

	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

	/// <summary>
	/// <seealso cref="FormFieldValidator"/> delegating to a custom, user-provided validator implementation.
	/// The implementation is resolved either using a fully qualified classname of a Java Class
	/// or using a java delegate implementation.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class DelegateFormFieldValidator : FormFieldValidator
	{

	  protected internal string clazz;
	  protected internal Expression delegateExpression;

	  public DelegateFormFieldValidator(Expression expression)
	  {
		delegateExpression = expression;
	  }

	  public DelegateFormFieldValidator(string clazz)
	  {
		this.clazz = clazz;
	  }

	  public DelegateFormFieldValidator()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public boolean validate(final Object submittedValue, final FormFieldValidatorContext validatorContext)
	  public virtual bool validate(object submittedValue, FormFieldValidatorContext validatorContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.DelegateExecution execution = validatorContext.getExecution();
		DelegateExecution execution = validatorContext.Execution;

		if (shouldPerformPaContextSwitch(validatorContext.Execution))
		{
		  ProcessApplicationReference processApplicationReference = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);

		  return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, submittedValue, validatorContext)
		 , processApplicationReference, new InvocationContext(execution));

		}
		else
		{
		  return doValidate(submittedValue, validatorContext);

		}

	  }

	  private class CallableAnonymousInnerClass : Callable<bool>
	  {
		  private readonly DelegateFormFieldValidator outerInstance;

		  private object submittedValue;
		  private org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext validatorContext;

		  public CallableAnonymousInnerClass(DelegateFormFieldValidator outerInstance, object submittedValue, org.camunda.bpm.engine.impl.form.validator.FormFieldValidatorContext validatorContext)
		  {
			  this.outerInstance = outerInstance;
			  this.submittedValue = submittedValue;
			  this.validatorContext = validatorContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public System.Nullable<bool> call() throws Exception
		  public bool? call()
		  {
			return outerInstance.doValidate(submittedValue, validatorContext);
		  }
	  }

	  protected internal virtual bool shouldPerformPaContextSwitch(DelegateExecution execution)
	  {
		if (execution == null)
		{
		  return false;
		}
		else
		{
		  ProcessApplicationReference targetPa = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
		  return targetPa != null && !targetPa.Equals(Context.CurrentProcessApplication);
		}
	  }

	  protected internal virtual bool doValidate(object submittedValue, FormFieldValidatorContext validatorContext)
	  {
		FormFieldValidator validator;

		if (!string.ReferenceEquals(clazz, null))
		{
		  // resolve validator using Fully Qualified Classname
		  object validatorObject = ReflectUtil.instantiate(clazz);
		  if (validatorObject is FormFieldValidator)
		  {
			validator = (FormFieldValidator) validatorObject;

		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ProcessEngineException("Validator class '" + clazz + "' is not an instance of " + typeof(FormFieldValidator).FullName);

		  }
		}
		else
		{
		  //resolve validator using expression
		  object validatorObject = delegateExpression.getValue(validatorContext.Execution);
		  if (validatorObject is FormFieldValidator)
		  {
			validator = (FormFieldValidator) validatorObject;

		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ProcessEngineException("Validator expression '" + delegateExpression + "' does not resolve to instance of " + typeof(FormFieldValidator).FullName);

		  }
		}

		FormFieldValidatorInvocation invocation = new FormFieldValidatorInvocation(validator, submittedValue, validatorContext);
		try
		{
		  Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException(e);
		}

		return invocation.InvocationResult.Value;
	  }

	}

}