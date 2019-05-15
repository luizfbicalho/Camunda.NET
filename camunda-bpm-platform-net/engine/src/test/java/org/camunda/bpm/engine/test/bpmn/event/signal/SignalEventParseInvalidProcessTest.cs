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
namespace org.camunda.bpm.engine.test.bpmn.@event.signal
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
//ORIGINAL LINE: @RunWith(Parameterized.class) public class SignalEventParseInvalidProcessTest
	public class SignalEventParseInvalidProcessTest
	{

	  private const string PROCESS_DEFINITION_DIRECTORY = "org/camunda/bpm/engine/test/bpmn/event/signal/";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}, expected error message = {1}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"InvalidProcessWithDuplicateSignalNames.bpmn20.xml", "duplicate signal name"},
			new object[] {"InvalidProcessWithNoSignalName.bpmn20.xml", "signal with id 'alertSignal' has no name"},
			new object[] {"InvalidProcessWithSignalNoId.bpmn20.xml", "signal must have an id"},
			new object[] {"InvalidProcessWithSignalNoRef.bpmn20.xml", "signalEventDefinition does not have required property 'signalRef'"},
			new object[] {"InvalidProcessWithMultipleSignalStartEvents.bpmn20.xml", "Cannot have more than one signal event subscription with name 'signal'"},
			new object[] {"InvalidProcessWithMultipleInterruptingSignalEventSubProcesses.bpmn20.xml", "Cannot have more than one signal event subscription with name 'alert'"}
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
		  repositoryService.createDeployment().addClasspathResource(PROCESS_DEFINITION_DIRECTORY + processDefinitionResource).deploy();

		  fail("exception expected: " + expectedErrorMessage);
		}
		catch (Exception e)
		{
		  assertTextPresent(expectedErrorMessage, e.Message);
		}
	  }

	  public virtual void assertTextPresent(string expected, string actual)
	  {
		if (string.ReferenceEquals(actual, null) || !actual.Contains(expected))
		{
		  throw new AssertionFailedError("expected presence of [" + expected + "], but was [" + actual + "]");
		}
	  }
	}

}