using System;
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
namespace org.camunda.bpm.engine.test.bpmn.scripttask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// 
	/// <summary>
	/// @author Daniel Meyer (Javascript)
	/// @author Sebastian Menski (Python)
	/// @author Nico Rehwaldt (Ruby)
	/// @author Christian Lipphardt (Groovy)
	/// 
	/// </summary>
	public class ScriptTaskTest : PluggableProcessEngineTestCase
	{

	  private const string JAVASCRIPT = "javascript";
	  private const string PYTHON = "python";
	  private const string RUBY = "ruby";
	  private const string GROOVY = "groovy";
	  private const string JUEL = "juel";

	  private new IList<string> deploymentIds = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		foreach (string deploymentId in deploymentIds)
		{
		  repositoryService.deleteDeployment(deploymentId, true);
		}
	  }

	  public virtual void testJavascriptProcessVarVisibility()
	  {

		deployProcess(JAVASCRIPT, "execution.setVariable('foo', 'a');" + "if (typeof foo !== 'undefined') { " + "  throw 'Variable foo should be defined as script variable.';" + "}" + "var foo = 'b';" + "if(execution.getVariable('foo') != 'a') {" + "  throw 'Execution should contain variable foo';" + "}" + "if(foo != 'b') {" + "  throw 'Script variable must override the visibiltity of the execution variable.';" + "}");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertEquals("a", variableValue);

	  }

	  public virtual void testPythonProcessVarAssignment()
	  {

		deployProcess(PYTHON, "execution.setVariable('foo', 'a')\n" + "if not foo:\n" + "    raise Exception('Variable foo should be defined as script variable.')\n" + "foo = 'b'\n" + "if execution.getVariable('foo') != 'a':\n" + "    raise Exception('Execution should contain variable foo')\n" + "if foo != 'b':\n" + "    raise Exception('Script variable must override the visibiltity of the execution variable.')\n");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertEquals("a", variableValue);

	  }

	  public virtual void testRubyProcessVarVisibility()
	  {

		deployProcess(RUBY, "$execution.setVariable('foo', 'a')\n" + "raise 'Variable foo should be defined as script variable.' if !$foo.nil?\n" + "$foo = 'b'\n" + "if $execution.getVariable('foo') != 'a'\n" + "  raise 'Execution should contain variable foo'\n" + "end\n" + "if $foo != 'b'\n" + "  raise 'Script variable must override the visibiltity of the execution variable.'\n" + "end");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertEquals("a", variableValue);

	  }

	  public virtual void testGroovyProcessVarVisibility()
	  {

		deployProcess(GROOVY, "execution.setVariable('foo', 'a')\n" + "if ( !foo ) {\n" + "  throw new Exception('Variable foo should be defined as script variable.')\n" + "}\n" + "foo = 'b'\n" + "if (execution.getVariable('foo') != 'a') {\n" + "  throw new Exception('Execution should contain variable foo')\n" + "}\n" + "if (foo != 'b') {\n" + "  throw new Exception('Script variable must override the visibiltity of the execution variable.')\n" + "}");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the script task can be executed without exceptions
		// the execution variable is stored and has the correct value
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertEquals("a", variableValue);

	  }

	  public virtual void testJavascriptFunctionInvocation()
	  {

		deployProcess(JAVASCRIPT, "function sum(a,b){" + "  return a+b;" + "};" + "var result = sum(1,2);" + "execution.setVariable('foo', result);");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.getVariable(pi.Id, "foo");

		if (variable is double?)
		{
		  // jdk 6/7 - rhino returns Double 3.0 for 1+2
		  assertEquals(3.0, variable);
		}
		else if (variable is int?)
		{
		  // jdk8 - nashorn returns Integer 3 for 1+2
		  assertEquals(3, variable);
		}

	  }

	  public virtual void testPythonFunctionInvocation()
	  {

		deployProcess(PYTHON, "def sum(a, b):\n" + "    return a + b\n" + "result = sum(1,2)\n" + "execution.setVariable('foo', result)");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.getVariable(pi.Id, "foo");
		assertEquals(3, variable);

	  }

	  public virtual void testRubyFunctionInvocation()
	  {

		deployProcess(RUBY, "def sum(a, b)\n" + "    return a + b\n" + "end\n" + "result = sum(1,2)\n" + "$execution.setVariable('foo', result)\n");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.getVariable(pi.Id, "foo");
		assertEquals(3l, variable);

	  }

	  public virtual void testGroovyFunctionInvocation()
	  {

		deployProcess(GROOVY, "def sum(a, b) {\n" + "    return a + b\n" + "}\n" + "result = sum(1,2)\n" + "execution.setVariable('foo', result)\n");

		// GIVEN
		// that we start an instance of this process
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		// THEN
		// the variable is defined
		object variable = runtimeService.getVariable(pi.Id, "foo");
		assertEquals(3, variable);

	  }

	  public virtual void testJsVariable()
	  {

		string scriptText = "var foo = 1;";

		deployProcess(JAVASCRIPT, scriptText);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertNull(variableValue);

	  }

	  public virtual void testPythonVariable()
	  {

		string scriptText = "foo = 1";

		deployProcess(PYTHON, scriptText);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertNull(variableValue);

	  }

	  public virtual void testRubyVariable()
	  {

		string scriptText = "foo = 1";

		deployProcess(RUBY, scriptText);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertNull(variableValue);

	  }

	  public virtual void testGroovyVariable()
	  {

		string scriptText = "def foo = 1";

		deployProcess(GROOVY, scriptText);

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");
		object variableValue = runtimeService.getVariable(pi.Id, "foo");
		assertNull(variableValue);

	  }

	  public virtual void testJuelExpression()
	  {
		deployProcess(JUEL, "${execution.setVariable('foo', 'bar')}");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string variableValue = (string) runtimeService.getVariable(pi.Id, "foo");
		assertEquals("bar", variableValue);
	  }

	  public virtual void testJuelCapitalizedExpression()
	  {
		deployProcess(JUEL.ToUpper(), "${execution.setVariable('foo', 'bar')}");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		string variableValue = (string) runtimeService.getVariable(pi.Id, "foo");
		assertEquals("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionAsVariable()
	  {
		deployProcess(PYTHON, "${scriptSource}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "execution.setVariable('foo', 'bar')";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.getVariable(pi.Id, "foo");
		assertEquals("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionAsNonExistingVariable()
	  {
		deployProcess(PYTHON, "${scriptSource}");

		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Process variable 'scriptSource' not defined");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresentIgnoreCase("Cannot resolve identifier 'scriptSource'", e.Message);
		}
	  }

	  public virtual void testSourceAsExpressionAsBean()
	  {
		deployProcess(PYTHON, "#{scriptResourceBean.getSource()}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptResourceBean"] = new ScriptResourceBean();
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.getVariable(pi.Id, "foo");
		assertEquals("bar", variableValue);
	  }

	  public virtual void testSourceAsExpressionWithWhitespace()
	  {
		deployProcess(PYTHON, "\t\n  \t \n  ${scriptSource}");

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["scriptSource"] = "execution.setVariable('foo', 'bar')";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", variables);

		string variableValue = (string) runtimeService.getVariable(pi.Id, "foo");
		assertEquals("bar", variableValue);
	  }

	  public virtual void testJavascriptVariableSerialization()
	  {
		deployProcess(JAVASCRIPT, "execution.setVariable('date', new java.util.Date(0));");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.getVariable(pi.Id, "date");
		assertEquals(0, date.Ticks);

		deployProcess(JAVASCRIPT, "execution.setVariable('myVar', new org.camunda.bpm.engine.test.bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.startProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.getVariable(pi.Id, "myVar");
		assertEquals("test", myVar.Name);
	  }

	  public virtual void testPythonVariableSerialization()
	  {
		deployProcess(PYTHON, "import java.util.Date\nexecution.setVariable('date', java.util.Date(0))");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.getVariable(pi.Id, "date");
		assertEquals(0, date.Ticks);

		deployProcess(PYTHON, "import org.camunda.bpm.engine.test.bpmn.scripttask.MySerializable\n" + "execution.setVariable('myVar', org.camunda.bpm.engine.test.bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.startProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.getVariable(pi.Id, "myVar");
		assertEquals("test", myVar.Name);
	  }

	  public virtual void testRubyVariableSerialization()
	  {
		deployProcess(RUBY, "require 'java'\n$execution.setVariable('date', java.util.Date.new(0))");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.getVariable(pi.Id, "date");
		assertEquals(0, date.Ticks);

		deployProcess(RUBY, "$execution.setVariable('myVar', org.camunda.bpm.engine.test.bpmn.scripttask.MySerializable.new('test'));");

		pi = runtimeService.startProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.getVariable(pi.Id, "myVar");
		assertEquals("test", myVar.Name);
	  }

	  public virtual void testGroovyVariableSerialization()
	  {
		deployProcess(GROOVY, "execution.setVariable('date', new java.util.Date(0))");

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		DateTime date = (DateTime) runtimeService.getVariable(pi.Id, "date");
		assertEquals(0, date.Ticks);

		deployProcess(GROOVY, "execution.setVariable('myVar', new org.camunda.bpm.engine.test.bpmn.scripttask.MySerializable('test'));");

		pi = runtimeService.startProcessInstanceByKey("testProcess");

		MySerializable myVar = (MySerializable) runtimeService.getVariable(pi.Id, "myVar");
		assertEquals("test", myVar.Name);
	  }

	  public virtual void testGroovyNotExistingImport()
	  {
		deployProcess(GROOVY, "import unknown");

		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Should fail during script compilation");
		}
		catch (ScriptCompilationException e)
		{
		  assertTextPresentIgnoreCase("import unknown", e.Message);
		}
	  }

	  public virtual void testGroovyNotExistingImportWithoutCompilation()
	  {
		// disable script compilation
		processEngineConfiguration.EnableScriptCompilation = false;

		deployProcess(GROOVY, "import unknown");

		try
		{
		  runtimeService.startProcessInstanceByKey("testProcess");
		  fail("Should fail during script evaluation");
		}
		catch (ScriptEvaluationException e)
		{
		  assertTextPresentIgnoreCase("import unknown", e.Message);
		}
		finally
		{
		  // re-enable script compilation
		  processEngineConfiguration.EnableScriptCompilation = true;
		}
	  }

	  public virtual void testShouldNotDeployProcessWithMissingScriptElementAndResource()
	  {
		try
		{
		  deployProcess(Bpmn.createExecutableProcess("testProcess").startEvent().scriptTask().scriptFormat(RUBY).userTask().endEvent().done());

		  fail("this process should not be deployable");
		}
		catch (ProcessEngineException)
		{
		  // happy path
		}
	  }

	  public virtual void testShouldUseJuelAsDefaultScriptLanguage()
	  {
		deployProcess(Bpmn.createExecutableProcess("testProcess").startEvent().scriptTask().scriptText("${true}").userTask().endEvent().done());

		runtimeService.startProcessInstanceByKey("testProcess");

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);
	  }

	  protected internal virtual void deployProcess(BpmnModelInstance process)
	  {
		Deployment deployment = repositoryService.createDeployment().addModelInstance("testProcess.bpmn", process).deploy();
		  deploymentIds.Add(deployment.Id);
	  }

	  protected internal virtual void deployProcess(string scriptFormat, string scriptText)
	  {
		BpmnModelInstance process = createProcess(scriptFormat, scriptText);
		deployProcess(process);
	  }

	  protected internal virtual BpmnModelInstance createProcess(string scriptFormat, string scriptText)
	  {

		return Bpmn.createExecutableProcess("testProcess").startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).userTask().endEvent().done();

	  }

	  public virtual void testAutoStoreScriptVarsOff()
	  {
		assertFalse(processEngineConfiguration.AutoStoreScriptVariables);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment public void testPreviousTaskShouldNotHandleException()
	  public virtual void testPreviousTaskShouldNotHandleException()
	  {
		try
		{
		  runtimeService.startProcessInstanceByKey("process");
		  fail();
		}
		// since the NVE extends the ProcessEngineException we have to handle it
		// separately
		catch (NullValueException)
		{
		  fail("Shouldn't have received NullValueException");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Invalid format"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment public void testSetScriptResultToProcessVariable()
	  public virtual void testSetScriptResultToProcessVariable()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["echo"] = "hello";
		variables["existingProcessVariableName"] = "one";

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("setScriptResultToProcessVariable", variables);

		assertEquals("hello", runtimeService.getVariable(pi.Id, "existingProcessVariableName"));
		assertEquals(pi.Id, runtimeService.getVariable(pi.Id, "newProcessVariableName"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment public void testGroovyScriptExecution()
	  public virtual void testGroovyScriptExecution()
	  {
		try
		{

		  processEngineConfiguration.AutoStoreScriptVariables = true;
		  int[] inputArray = new int[] {1, 2, 3, 4, 5};
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("scriptExecution", CollectionUtil.singletonMap("inputArray", inputArray));

		  int? result = (int?) runtimeService.getVariable(pi.Id, "sum");
		  assertEquals(15, result.Value);

		}
		finally
		{
		  processEngineConfiguration.AutoStoreScriptVariables = false;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment public void testGroovySetVariableThroughExecutionInScript()
	  public virtual void testGroovySetVariableThroughExecutionInScript()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("setScriptVariableThroughExecution");

		// Since 'def' is used, the 'scriptVar' will be script local
		// and not automatically stored as a process variable.
		assertNull(runtimeService.getVariable(pi.Id, "scriptVar"));
		assertEquals("test123", runtimeService.getVariable(pi.Id, "myVar"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @org.camunda.bpm.engine.test.Deployment public void testScriptEvaluationException()
	  public virtual void testScriptEvaluationException()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("Process_1").singleResult();
		try
		{
		  runtimeService.startProcessInstanceByKey("Process_1");
		}
		catch (ScriptEvaluationException e)
		{
		  assertTextPresent("Unable to evaluate script while executing activity 'Failing' in the process definition with id '" + processDefinition.Id + "'", e.Message);
		}
	  }
	}

}