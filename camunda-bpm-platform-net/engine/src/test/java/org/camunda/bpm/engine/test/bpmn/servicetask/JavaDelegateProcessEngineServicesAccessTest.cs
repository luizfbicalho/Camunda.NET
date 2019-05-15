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
namespace org.camunda.bpm.engine.test.bpmn.servicetask
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using AbstractProcessEngineServicesAccessTest = org.camunda.bpm.engine.test.bpmn.common.AbstractProcessEngineServicesAccessTest;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ServiceTask = org.camunda.bpm.model.bpmn.instance.ServiceTask;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JavaDelegateProcessEngineServicesAccessTest : AbstractProcessEngineServicesAccessTest
	{

	  protected internal override Type TestServiceAccessibleClass
	  {
		  get
		  {
			return typeof(AccessServicesJavaDelegate);
		  }
	  }

	  protected internal override Type QueryClass
	  {
		  get
		  {
			return typeof(PerformQueryJavaDelegate);
		  }
	  }

	  protected internal override Type StartProcessInstanceClass
	  {
		  get
		  {
			return typeof(StartProcessJavaDelegate);
		  }
	  }

	  protected internal override Type ProcessEngineStartProcessClass
	  {
		  get
		  {
			return typeof(ProcessEngineStartProcessJavaDelegate);
		  }
	  }

	  protected internal override Task createModelAccessTask(BpmnModelInstance modelInstance, Type delegateClass)
	  {
		ServiceTask serviceTask = modelInstance.newInstance(typeof(ServiceTask));
		serviceTask.Id = "serviceTask";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		serviceTask.CamundaClass = delegateClass.FullName;
		return serviceTask;
	  }

	  public class AccessServicesJavaDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  assertCanAccessServices(execution.ProcessEngineServices);
		}
	  }

	  public class PerformQueryJavaDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  assertCanPerformQuery(execution.ProcessEngineServices);
		}
	  }

	  public class StartProcessJavaDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  assertCanStartProcessInstance(execution.ProcessEngineServices);
		}
	  }

	  public class ProcessEngineStartProcessJavaDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  assertCanStartProcessInstance(execution.ProcessEngine);
		}
	  }
	}

}