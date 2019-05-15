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
namespace org.camunda.bpm.engine.cdi.impl.annotation
{


	using CompleteTask = org.camunda.bpm.engine.cdi.annotation.CompleteTask;

	/// <summary>
	/// <seealso cref="Interceptor"/> for handling the <seealso cref="CompleteTask"/>-Annotation
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Interceptor @CompleteTask public class CompleteTaskInterceptor implements java.io.Serializable
	[Serializable]
	public class CompleteTaskInterceptor
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject BusinessProcess businessProcess;
	  internal BusinessProcess businessProcess;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AroundInvoke public Object invoke(javax.interceptor.InvocationContext ctx) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual object invoke(InvocationContext ctx)
	  {
		try
		{
		  object result = ctx.proceed();

		  CompleteTask completeTaskAnnotation = ctx.Method.getAnnotation(typeof(CompleteTask));
		  bool endConversation = completeTaskAnnotation.endConversation();
		  businessProcess.completeTask(endConversation);

		  return result;
		}
		catch (InvocationTargetException e)
		{
		  throw new ProcessEngineCdiException("Error while completing task: " + e.InnerException.Message, e.InnerException);
		}
	  }

	}

}