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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	public class ConditionStartEventAuthorizationTest : AuthorizationTest
	{

	  private const string SINGLE_CONDITIONAL_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testSingleConditionalStartEvent1.bpmn20.xml";
	  private const string TRUE_CONDITIONAL_XML = "org/camunda/bpm/engine/test/bpmn/event/conditional/ConditionalStartEventTest.testStartInstanceWithTrueConditionalStartEvent.bpmn20.xml";
	  protected internal const string PROCESS_KEY = "conditionalEventProcess";
	  protected internal const string PROCESS_KEY_TWO = "trueConditionProcess";

	  [Deployment(resources : { SINGLE_CONDITIONAL_XML, TRUE_CONDITIONAL_XML })]
	  public virtual void testWithAllPermissions()
	  {
		// given two deployed processes with conditional start event

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY_TWO, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 42).evaluateStartConditions();

		// then
		assertEquals(1, instances.Count);
	  }

	  [Deployment(resources : { SINGLE_CONDITIONAL_XML })]
	  public virtual void testWithoutProcessDefinitionPermissions()
	  {
		// given deployed process with conditional start event

		// user does not have process definition permissions
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		try
		{
		  runtimeService.createConditionEvaluation().setVariable("foo", 42).evaluateStartConditions();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("No subscriptions were found during evaluation of the conditional start events."));
		}

	  }

	  [Deployment(resources : { SINGLE_CONDITIONAL_XML })]
	  public virtual void testWithoutCreateInstancePermissions()
	  {
		// given deployed process with conditional start event

		// user does not have process definition CREATE_INSTANCE permissions
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		try
		{
		  runtimeService.createConditionEvaluation().setVariable("foo", 42).evaluateStartConditions();
		  fail("expected exception");
		}
		catch (AuthorizationException e)
		{
		  assertTrue(e.Message.contains("The user with id 'test' does not have 'CREATE_INSTANCE' permission on resource 'conditionalEventProcess' of type 'ProcessDefinition'."));
		}

	  }

	  [Deployment(resources : { SINGLE_CONDITIONAL_XML })]
	  public virtual void testWithoutProcessInstanccePermission()
	  {
		// given deployed process with conditional start event

		// the user doesn't have CREATE permission for process instances
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ, CREATE_INSTANCE);

		// when
		try
		{
		  runtimeService.createConditionEvaluation().setVariable("foo", 42).evaluateStartConditions();
		  fail("expected exception");
		}
		catch (AuthorizationException e)
		{
		  assertTrue(e.Message.contains("The user with id 'test' does not have 'CREATE' permission on resource 'ProcessInstance'."));
		}

	  }

	  [Deployment(resources : { SINGLE_CONDITIONAL_XML })]
	  public virtual void testWithRevokeAuthorizations()
	  {
		// given deployed process with conditional start event

		createRevokeAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		try
		{
		  runtimeService.createConditionEvaluation().setVariable("foo", 42).evaluateStartConditions();
		  fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  assertTrue(e.Message.contains("No subscriptions were found during evaluation of the conditional start events."));
		}
	  }
	}

}