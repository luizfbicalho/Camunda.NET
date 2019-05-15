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
namespace org.camunda.bpm.integrationtest.functional.el
{
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ResolveFormDataBean = org.camunda.bpm.integrationtest.functional.el.beans.ResolveFormDataBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ElResolveTaskFormDataTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class ElResolveTaskFormDataTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(ResolveFormDataBean)).addAsResource("org/camunda/bpm/integrationtest/functional/el/elTaskFormProcessWithFormData.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskFormDataWithDefaultValueExpression()
	  public virtual void testTaskFormDataWithDefaultValueExpression()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("elTaskFormProcess");
		Task task = taskService.createTaskQuery().processInstanceId(instance.Id).singleResult();

		TaskFormData formData = formService.getTaskFormData(task.Id);
		object defaultValue = formData.FormFields[0].Value.Value;

		Assert.assertNotNull(defaultValue);
		Assert.assertEquals("testString123", defaultValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskFormDataWithLabelExpression()
	  public virtual void testTaskFormDataWithLabelExpression()
	  {
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("elTaskFormProcess");
		Task task = taskService.createTaskQuery().processInstanceId(instance.Id).singleResult();

		TaskFormData formData = formService.getTaskFormData(task.Id);

		string label = formData.FormFields[0].Label;
		Assert.assertNotNull(label);
		Assert.assertEquals("testString123", label);
	  }

	}

}