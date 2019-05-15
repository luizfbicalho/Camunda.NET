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
namespace org.camunda.spin.plugin.el
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using TestVariableScope = org.camunda.spin.plugin.script.TestVariableScope;
	using SpinXmlElement = org.camunda.spin.xml.SpinXmlElement;

	/// <summary>
	/// <para>Testcase ensuring integration of camunda Spin into Process Engine expression language.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpinFunctionMapperTest : PluggableProcessEngineTestCase
	{

	  internal string xmlString = "<elementName attrName=\"attrValue\" />";
	  internal string jsonString = "{\"foo\": \"bar\"}";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T> T executeExpression(String expression)
	  protected internal virtual T executeExpression<T>(string expression)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.spin.plugin.script.TestVariableScope varScope = new org.camunda.spin.plugin.script.TestVariableScope();
		TestVariableScope varScope = new TestVariableScope();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.Expression compiledExpression = processEngineConfiguration.getExpressionManager().createExpression(expression);
		Expression compiledExpression = processEngineConfiguration.ExpressionManager.createExpression(expression);

		return (T) processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, varScope, compiledExpression));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly SpinFunctionMapperTest outerInstance;

		  private TestVariableScope varScope;
		  private Expression compiledExpression;

		  public CommandAnonymousInnerClass(SpinFunctionMapperTest outerInstance, TestVariableScope varScope, Expression compiledExpression)
		  {
			  this.outerInstance = outerInstance;
			  this.varScope = varScope;
			  this.compiledExpression = compiledExpression;
		  }

		  public object execute(CommandContext commandContext)
		  {
			return compiledExpression.getValue(varScope);
		  }
	  }

	  public virtual void testSpin_S_Available()
	  {

		SpinXmlElement spinXmlEl = executeExpression("${ S('" + xmlString + "') }");
		assertNotNull(spinXmlEl);
		assertEquals("elementName", spinXmlEl.name());
	  }

	  public virtual void testSpin_XML_Available()
	  {

		SpinXmlElement spinXmlEl = executeExpression("${ XML('" + xmlString + "') }");
		assertNotNull(spinXmlEl);
		assertEquals("elementName", spinXmlEl.name());
	  }

	  public virtual void testSpin_JSON_Available()
	  {

		SpinJsonNode spinJsonEl = executeExpression("${ JSON('" + jsonString + "') }");
		assertNotNull(spinJsonEl);
		assertEquals("bar", spinJsonEl.prop("foo").stringValue());
	  }

	  public virtual void testSpin_XPath_Available()
	  {

		string elName = executeExpression("${ S('" + xmlString + "').xPath('/elementName').element().name() }");
		assertNotNull(elName);
		assertEquals("elementName", elName);
	  }

	  public virtual void testSpin_JsonPath_Available()
	  {

		string property = executeExpression("${ S('" + jsonString + "').jsonPath('$.foo').stringValue() }");
		assertNotNull(property);
		assertEquals("bar", property);
	  }

	  public virtual void testSpinAvailableInBpmn()
	  {

		BpmnModelInstance bpmnModelInstance = Bpmn.createExecutableProcess("testProcess").startEvent().serviceTask().camundaExpression("${ execution.setVariable('customer', " + "S(xmlVar).xPath('/customers/customer').element().toString()" + ")}").receiveTask("wait").endEvent().done();

		Deployment deployment = repositoryService.createDeployment().addModelInstance("process.bpmn", bpmnModelInstance).deploy();

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["xmlVar"] = "<customers><customer /></customers>";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		string customerXml = (string) runtimeService.getVariable(pi.Id, "customer");
		assertNotNull(customerXml);
		assertTrue(customerXml.Contains("customer"));
		assertFalse(customerXml.Contains("customers"));

		runtimeService.signal(pi.Id);

		repositoryService.deleteDeployment(deployment.Id, true);

	  }
	}

}