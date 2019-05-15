using System;

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
namespace org.camunda.bpm.engine.rest.helper
{
	using InvocationOnMock = org.mockito.invocation.InvocationOnMock;
	using Answer = org.mockito.stubbing.Answer;

	/// <summary>
	/// Default answer for Mockito mocks to always return themselves if the method return type matches.
	/// Can be used to mock fluent APIs more easily.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class FluentAnswer : Answer<object>
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object answer(org.mockito.invocation.InvocationOnMock invocation) throws Throwable
	  public override object answer(InvocationOnMock invocation)
	  {
		Type returnType = invocation.Method.ReturnType;

		object mock = invocation.Mock;
		if (returnType.IsAssignableFrom(mock.GetType()))
		{
		  return mock;
		}
		else
		{
		  return null;
		}
	  }
	}
}