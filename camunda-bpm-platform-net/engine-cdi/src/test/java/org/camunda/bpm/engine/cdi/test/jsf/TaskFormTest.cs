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
namespace org.camunda.bpm.engine.cdi.test.jsf
{


	using CamundaTaskForm = org.camunda.bpm.engine.cdi.compat.CamundaTaskForm;
	using FoxTaskForm = org.camunda.bpm.engine.cdi.compat.FoxTaskForm;
	using TaskForm = org.camunda.bpm.engine.cdi.jsf.TaskForm;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class TaskFormTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskFormInjectable() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTaskFormInjectable()
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> taskForm = beanManager.getBeans(org.camunda.bpm.engine.cdi.jsf.TaskForm.class);
		ISet<Bean<object>> taskForm = beanManager.getBeans(typeof(TaskForm));
		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.enterprise.inject.spi.Bean<? extends Object> bean = beanManager.resolve(taskForm);
		  Bean<object> bean = beanManager.resolve(taskForm);
		  Assert.assertNotNull(bean);
		}
		catch (AmbiguousResolutionException)
		{
		  Assert.fail("Injection of TaskForm is ambiguous.");
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> foxTaskForm = beanManager.getBeans(org.camunda.bpm.engine.cdi.compat.FoxTaskForm.class);
		ISet<Bean<object>> foxTaskForm = beanManager.getBeans(typeof(FoxTaskForm));
		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.enterprise.inject.spi.Bean<? extends Object> bean = beanManager.resolve(foxTaskForm);
		  Bean<object> bean = beanManager.resolve(foxTaskForm);
		  Assert.assertNotNull(bean);
		}
		catch (AmbiguousResolutionException)
		{
		  Assert.fail("Injection of FoxTaskForm is ambiguous.");
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> camundaTaskForm = beanManager.getBeans(org.camunda.bpm.engine.cdi.compat.CamundaTaskForm.class);
		ISet<Bean<object>> camundaTaskForm = beanManager.getBeans(typeof(CamundaTaskForm));
		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: javax.enterprise.inject.spi.Bean<? extends Object> bean = beanManager.resolve(camundaTaskForm);
		  Bean<object> bean = beanManager.resolve(camundaTaskForm);
		  Assert.assertNotNull(bean);
		}
		catch (AmbiguousResolutionException)
		{
		  Assert.fail("Injection of CamundaTaskForm is ambiguous.");
		}

	  }

	}

}