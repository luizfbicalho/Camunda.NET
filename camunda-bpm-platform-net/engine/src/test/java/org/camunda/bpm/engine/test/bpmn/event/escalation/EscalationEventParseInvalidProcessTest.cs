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
namespace org.camunda.bpm.engine.test.bpmn.@event.escalation
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using AssertionFailedError = junit.framework.AssertionFailedError;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// Parse an invalid process definition and assert the error message.
	/// 
	/// @author Philipp Ossler
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class EscalationEventParseInvalidProcessTest
	public class EscalationEventParseInvalidProcessTest
	{

	  private const string PROCESS_DEFINITION_DIRECTORY = "org/camunda/bpm/engine/test/bpmn/event/escalation/";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}, expected error message = {1}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"EscalationEventParseInvalidProcessTest.missingIdOnEscalation.bpmn20.xml", "escalation must have an id"},
			new object[] {"EscalationEventParseInvalidProcessTest.invalidAttachement.bpmn20.xml", "An escalation boundary event should only be attached to a subprocess or a call activity"},
			new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnBoundaryEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithSameEscalationCode.bpmn20.xml", "multiple escalation boundary events with the same escalationCode 'escalationCode' are not supported on same scope"},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithAndWithoutEscalationCode.bpmn20.xml", "The same scope can not contains an escalation boundary event without escalation code and another one with escalation code."},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithoutEscalationCode.bpmn20.xml", "The same scope can not contains more than one escalation boundary event without escalation code."},
			new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationCodeOnIntermediateThrowingEscalationEvent.bpmn20.xml", "throwing escalation event must have an 'escalationCode'"},
			new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationRefOnIntermediateThrowingEvent.bpmn20.xml", "escalationEventDefinition does not have required attribute 'escalationRef'"},
			new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnIntermediateThrowingEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
			new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationCodeOnEscalationEndEvent.bpmn20.xml", "escalation end event must have an 'escalationCode'"},
			new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationRefOnEndEvent.bpmn20.xml", "escalationEventDefinition does not have required attribute 'escalationRef'"},
			new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnEndEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
			new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnEscalationEventSubprocess.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleInterruptingEscalationEventSubprocesses.bpmn20.xml", "multiple escalation event subprocesses with the same escalationCode 'escalationCode' are not supported on same scope"},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithSameEscalationCode.bpmn20.xml", "multiple escalation event subprocesses with the same escalationCode 'escalationCode' are not supported on same scope"},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithAndWithoutEscalationCode.bpmn20.xml", "The same scope can not contains an escalation event subprocess without escalation code and another one with escalation code."},
			new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithoutEscalationCode.bpmn20.xml", "The same scope can not contains more than one escalation event subprocess without escalation code."}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String processDefinitionResource;
	  public string processDefinitionResource;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public String expectedErrorMessage;
	  public string expectedErrorMessage;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		repositoryService = rule.RepositoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseInvalidProcessDefinition()
	  public virtual void testParseInvalidProcessDefinition()
	  {
		try
		{
		  string deploymentId = repositoryService.createDeployment().addClasspathResource(PROCESS_DEFINITION_DIRECTORY + processDefinitionResource).deploy().Id;

		  // in case that the deployment do not fail
		  repositoryService.deleteDeployment(deploymentId, true);

		  fail("exception expected: " + expectedErrorMessage);
		}
		catch (Exception e)
		{
		  assertExceptionMessageContainsText(e, expectedErrorMessage);
		}
	  }

	  public virtual void assertExceptionMessageContainsText(Exception e, string expectedMessage)
	  {
		string actualMessage = e.Message;
		if (string.ReferenceEquals(actualMessage, null) || !actualMessage.Contains(expectedMessage))
		{
		  throw new AssertionFailedError("expected presence of [" + expectedMessage + "], but was [" + actualMessage + "]");
		}
	  }
	}

}