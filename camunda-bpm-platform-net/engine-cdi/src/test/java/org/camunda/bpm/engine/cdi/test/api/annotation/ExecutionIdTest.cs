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
namespace org.camunda.bpm.engine.cdi.test.api.annotation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using ExecutionIdLiteral = org.camunda.bpm.engine.cdi.annotation.ExecutionIdLiteral;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ExecutionIdTest extends org.camunda.bpm.engine.cdi.test.CdiProcessEngineTestCase
	public class ExecutionIdTest : CdiProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testExecutionIdInjectableByName()
		public virtual void testExecutionIdInjectableByName()
		{
		getBeanInstance(typeof(BusinessProcess)).startProcessByKey("keyOfTheProcess");
		string processInstanceId = (string) getBeanInstance("processInstanceId");
		Assert.assertNotNull(processInstanceId);
		string executionId = (string) getBeanInstance("executionId");
		Assert.assertNotNull(executionId);

		assertEquals(processInstanceId, executionId);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testExecutionIdInjectableByQualifier()
	  public virtual void testExecutionIdInjectableByQualifier()
	  {
		getBeanInstance(typeof(BusinessProcess)).startProcessByKey("keyOfTheProcess");

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<javax.enterprise.inject.spi.Bean<?>> beans = beanManager.getBeans(String.class, new org.camunda.bpm.engine.cdi.annotation.ExecutionIdLiteral());
		ISet<Bean<object>> beans = beanManager.getBeans(typeof(string), new ExecutionIdLiteral());
		Bean<string> bean = (Bean<string>) beanManager.resolve(beans);

		CreationalContext<string> ctx = beanManager.createCreationalContext(bean);
		string executionId = (string) beanManager.getReference(bean, typeof(string), ctx);
		Assert.assertNotNull(executionId);

		string processInstanceId = (string) getBeanInstance("processInstanceId");
		Assert.assertNotNull(processInstanceId);

		assertEquals(processInstanceId, executionId);
	  }

	}

}