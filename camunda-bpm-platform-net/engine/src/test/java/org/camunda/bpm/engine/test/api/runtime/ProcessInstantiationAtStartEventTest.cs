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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	public class ProcessInstantiationAtStartEventTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deployment(Bpmn.createExecutableProcess(PROCESS_DEFINITION_KEY).startEvent().userTask().endEvent().done());
	  }

	  public virtual void testStartProcessInstanceById()
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();

		runtimeService.createProcessInstanceById(processDefinition.Id).execute();

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceByKey()
	  {

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).execute();

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));
	  }

	  public virtual void testStartProcessInstanceAndSetBusinessKey()
	  {

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).businessKey("businessKey").execute();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance, @is(notNullValue()));
		assertThat(processInstance.BusinessKey, @is("businessKey"));
	  }

	  public virtual void testStartProcessInstanceAndSetCaseInstanceId()
	  {

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).caseInstanceId("caseInstanceId").execute();

		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		assertThat(processInstance, @is(notNullValue()));
		assertThat(processInstance.CaseInstanceId, @is("caseInstanceId"));
	  }

	  public virtual void testStartProcessInstanceAndSetVariable()
	  {

		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).setVariable("var", "value").execute();

		object variable = runtimeService.getVariable(processInstance.Id, "var");
		assertThat(variable, @is(notNullValue()));
		assertThat(variable, @is((object) "value"));
	  }

	  public virtual void testStartProcessInstanceAndSetVariables()
	  {
		IDictionary<string, object> variables = Variables.createVariables().putValue("var1", "v1").putValue("var2", "v2");

		ProcessInstance processInstance = runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).setVariables(variables).execute();

		assertThat(runtimeService.getVariables(processInstance.Id), @is(variables));
	  }

	  public virtual void testStartProcessInstanceNoSkipping()
	  {

		runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).execute(false, false);

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));
	  }

	  public virtual void testFailToStartProcessInstanceSkipListeners()
	  {
		try
		{
		  runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).execute(true, false);

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot skip"));
		}
	  }

	  public virtual void testFailToStartProcessInstanceSkipInputOutputMapping()
	  {
		try
		{
		  runtimeService.createProcessInstanceByKey(PROCESS_DEFINITION_KEY).execute(false, true);

		  fail("expected exception");
		}
		catch (BadUserRequestException e)
		{
		  assertThat(e.Message, containsString("Cannot skip"));
		}
	  }

	}

}