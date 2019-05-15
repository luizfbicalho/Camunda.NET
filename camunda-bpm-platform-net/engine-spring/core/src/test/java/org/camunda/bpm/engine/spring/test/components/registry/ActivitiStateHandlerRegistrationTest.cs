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
namespace org.camunda.bpm.engine.spring.test.components.registry
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using ActivitiStateHandlerRegistration = org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistration;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public class ActivitiStateHandlerRegistrationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveDetailledStringRepresentation() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldHaveDetailledStringRepresentation()
	  {
		IDictionary<int, string> processVariablesExpected = Collections.singletonMap(34, "testValue");
		System.Reflection.MethodInfo handlerMethod = this.GetType().GetMethod("shouldHaveDetailledStringRepresentation");
		object handler = new ObjectAnonymousInnerClass(this);
		string stateName = "running";
		string beanName = "testBean";
		int processVariablesIndex = 4;
		int processIdIndex = 2;
		string processName = "testProcess";
		ActivitiStateHandlerRegistration registration = new ActivitiStateHandlerRegistration(processVariablesExpected, handlerMethod, handler, stateName, beanName, processVariablesIndex, processIdIndex, processName);
		assertEquals("org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistration@" + registration.GetHashCode().ToString("x") + "[" + "processVariablesExpected={34=testValue}, " + "handlerMethod=public void org.camunda.bpm.engine.spring.test.components.registry.ActivitiStateHandlerRegistrationTest.shouldHaveDetailledStringRepresentation() throws java.lang.Exception, " + "handler=org.camunda.bpm.engine.spring.test.components.registry.ActivitiStateHandlerRegistrationTest$1@" + handler.GetHashCode().ToString("x") + ", " + "stateName=running, " + "beanName=testBean, " + "processVariablesIndex=4, " + "processIdIndex=2, " + "processName=testProcess]", registration.ToString());
	  }

	  private class ObjectAnonymousInnerClass : object
	  {
		  private readonly ActivitiStateHandlerRegistrationTest outerInstance;

		  public ObjectAnonymousInnerClass(ActivitiStateHandlerRegistrationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;

			  testValue = 76;
			  testValue++;
		  }

		  public int? testValue;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveDetailledStringRepresentationWithNullValues() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldHaveDetailledStringRepresentationWithNullValues()
	  {
		IDictionary<int, string> processVariablesExpected = Collections.singletonMap(34, "testValue");
		System.Reflection.MethodInfo handlerMethod = null;
		object handler = null;
		string stateName = "running";
		string beanName = "testBean";
		int processVariablesIndex = 4;
		int processIdIndex = 2;
		string processName = "testProcess";
		ActivitiStateHandlerRegistration registration = new ActivitiStateHandlerRegistration(processVariablesExpected, handlerMethod, handler, stateName, beanName, processVariablesIndex, processIdIndex, processName);
		assertEquals("org.camunda.bpm.engine.spring.components.registry.ActivitiStateHandlerRegistration@" + registration.GetHashCode().ToString("x") + "[" + "processVariablesExpected={34=testValue}, " + "handlerMethod=null, " + "handler=null, " + "stateName=running, " + "beanName=testBean, " + "processVariablesIndex=4, " + "processIdIndex=2, " + "processName=testProcess]", registration.ToString());
	  }
	}

}