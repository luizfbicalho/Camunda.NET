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
namespace org.camunda.bpm.engine.spring.test.configuration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration(classes = { InMemProcessEngineConfiguration.class, SpringProcessEngineServicesConfiguration.class }) public class JavaBasedProcessEngineConfigTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class JavaBasedProcessEngineConfigTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private Counter couter;
		private Counter couter;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired protected org.camunda.bpm.engine.RuntimeService runtimeService;
	  protected internal new RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateExpression()
	  public virtual void testDelegateExpression()
	  {
		runtimeService.startProcessInstanceByKey("SpringProcess");

		assertThat(couter.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExpression()
	  public virtual void testExpression()
	  {
		runtimeService.startProcessInstanceByKey("SpringProcess");

		assertThat(couter.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDelegateExpressionWithProcessServices()
	  public virtual void testDelegateExpressionWithProcessServices()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("SpringProcess").Id;

		assertThat(couter.Count, @is(1));
		assertThat((int?) runtimeService.getVariable(processInstanceId, "count"), @is(1));
	  }

	}

}