using System.IO;

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
namespace org.camunda.bpm.engine.spring.test.components.scope
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ProcessScope = org.camunda.bpm.engine.spring.components.scope.ProcessScope;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	public class ProcessScopeTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogExceptionStacktrace() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldLogExceptionStacktrace()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Logger logger = Logger.getLogger(typeof(ProcessScope).FullName);
		using (MemoryStream @out = new MemoryStream(),)
		{
		  Handler handler = new StreamHandler(@out, new SimpleFormatter());
		  logger.addHandler(handler);
		  try
		  {
			ProcessScope scope = new ProcessScope();
			object variable = scope.get("testObject", null);
			assertNull(variable);
		  }
		  finally
		  {
			handler.flush();
			handler.close();
			logger.removeHandler(handler);
		  }
		  // test for logged exception
		  string message = StringHelper.NewString(@out.toByteArray(), StandardCharsets.UTF_8);
		  assertTrue(message.Length > 0);
		  assertTrue(message.Contains("org.camunda.bpm.engine.spring.components.scope.ProcessScope get"));
		  assertTrue(message.Contains("couldn't return value from process scope! java.lang.NullPointerException"));
		  assertTrue(message.Contains("at org.camunda.bpm.engine.spring.components.scope.ProcessScope.getExecutionId(ProcessScope.java:"));
		  assertTrue(message.Contains("at org.camunda.bpm.engine.spring.components.scope.ProcessScope.getConversationId(ProcessScope.java:"));
		  assertTrue(message.Contains("at org.camunda.bpm.engine.spring.components.scope.ProcessScope.get(ProcessScope.java:"));
		  assertTrue(message.Contains("at org.camunda.bpm.engine.spring.test.components.scope.ProcessScopeTest.shouldLogExceptionStacktrace(ProcessScopeTest.java:"));
		}
	  }
	}

}