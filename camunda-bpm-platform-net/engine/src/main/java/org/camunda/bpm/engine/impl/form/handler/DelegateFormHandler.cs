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
namespace org.camunda.bpm.engine.impl.form.handler
{

	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class DelegateFormHandler
	{

	  protected internal string deploymentId;
	  protected internal FormHandler formHandler;

	  public DelegateFormHandler(FormHandler formHandler, string deploymentId)
	  {
		this.formHandler = formHandler;
		this.deploymentId = deploymentId;
	  }

	  public virtual void parseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
	  {
		// should not be called!
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected <T> T performContextSwitch(final java.util.concurrent.Callable<T> callable)
	  protected internal virtual T performContextSwitch<T>(Callable<T> callable)
	  {

		ProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.getTargetProcessApplication(deploymentId);

		if (targetProcessApplication != null)
		{

		  return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, callable)
		 , targetProcessApplication);

		}
		else
		{
		  return doCall(callable);
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<T>
	  {
		  private readonly DelegateFormHandler outerInstance;

		  private Callable<T> callable;

		  public CallableAnonymousInnerClass(DelegateFormHandler outerInstance, Callable<T> callable)
		  {
			  this.outerInstance = outerInstance;
			  this.callable = callable;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T call() throws Exception
		  public T call()
		  {
			return outerInstance.doCall(callable);
		  }

	  }

	  protected internal virtual T doCall<T>(Callable<T> callable)
	  {
		try
		{
		  return callable.call();
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException(e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void submitFormVariables(final org.camunda.bpm.engine.variable.VariableMap properties, final org.camunda.bpm.engine.delegate.VariableScope variableScope)
	  public virtual void submitFormVariables(VariableMap properties, VariableScope variableScope)
	  {
		performContextSwitch(new CallableAnonymousInnerClass2(this, properties, variableScope));
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly DelegateFormHandler outerInstance;

		  private VariableMap properties;
		  private VariableScope variableScope;

		  public CallableAnonymousInnerClass2(DelegateFormHandler outerInstance, VariableMap properties, VariableScope variableScope)
		  {
			  this.outerInstance = outerInstance;
			  this.properties = properties;
			  this.variableScope = variableScope;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(new SubmitFormVariablesInvocation(outerInstance.formHandler, properties, variableScope));

			return null;
		  }
	  }

	  public abstract FormHandler FormHandler {get;}

	}

}