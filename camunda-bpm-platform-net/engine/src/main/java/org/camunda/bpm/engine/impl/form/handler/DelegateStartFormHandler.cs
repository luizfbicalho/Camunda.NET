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

	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DelegateStartFormHandler : DelegateFormHandler, StartFormHandler
	{

	  public DelegateStartFormHandler(StartFormHandler formHandler, DeploymentEntity deployment) : base(formHandler, deployment.Id)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.form.StartFormData createStartFormData(final org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
	  public virtual StartFormData createStartFormData(ProcessDefinitionEntity processDefinition)
	  {
		return performContextSwitch(new CallableAnonymousInnerClass(this, processDefinition));
	  }

	  private class CallableAnonymousInnerClass : Callable<StartFormData>
	  {
		  private readonly DelegateStartFormHandler outerInstance;

		  private ProcessDefinitionEntity processDefinition;

		  public CallableAnonymousInnerClass(DelegateStartFormHandler outerInstance, ProcessDefinitionEntity processDefinition)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinition = processDefinition;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.form.StartFormData call() throws Exception
		  public StartFormData call()
		  {
			CreateStartFormInvocation invocation = new CreateStartFormInvocation((StartFormHandler) outerInstance.formHandler, processDefinition);
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
			return (StartFormData) invocation.InvocationResult;
		  }
	  }

	  public override StartFormHandler FormHandler
	  {
		  get
		  {
			return (StartFormHandler) formHandler;
		  }
	  }

	}

}