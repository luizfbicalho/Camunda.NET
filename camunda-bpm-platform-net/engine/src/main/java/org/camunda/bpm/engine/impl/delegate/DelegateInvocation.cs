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
namespace org.camunda.bpm.engine.impl.@delegate
{
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateInterceptor = org.camunda.bpm.engine.impl.interceptor.DelegateInterceptor;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;

	/// <summary>
	/// Provides context about the invocation of usercode and handles the actual
	/// invocation
	/// 
	/// @author Daniel Meyer </summary>
	/// <seealso cref= DelegateInterceptor </seealso>
	public abstract class DelegateInvocation
	{

	  protected internal object invocationResult;
	  protected internal BaseDelegateExecution contextExecution;
	  protected internal ResourceDefinitionEntity contextResource;

	  /// <summary>
	  /// Provide a context execution or resource definition in which context the invocation
	  ///   should be performed. If both parameters are null, the invocation is performed in the
	  ///   current context.
	  /// </summary>
	  /// <param name="contextExecution"> set to an execution </param>
	  public DelegateInvocation(BaseDelegateExecution contextExecution, ResourceDefinitionEntity contextResource)
	  {
		// This constructor forces sub classes to call it, thereby making it more visible
		// whether a context switch is going to be performed for them.
		this.contextExecution = contextExecution;
		this.contextResource = contextResource;
	  }

	  /// <summary>
	  /// make the invocation proceed, performing the actual invocation of the user
	  /// code.
	  /// </summary>
	  /// <exception cref="Exception">
	  ///           the exception thrown by the user code </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void proceed() throws Exception
	  public virtual void proceed()
	  {
		invoke();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void invoke() throws Exception;
	  protected internal abstract void invoke();

	  /// <returns> the result of the invocation (can be null if the invocation does
	  ///         not return a result) </returns>
	  public virtual object InvocationResult
	  {
		  get
		  {
			return invocationResult;
		  }
	  }

	  /// <summary>
	  /// returns the execution in which context this delegate is invoked. may be null
	  /// </summary>
	  public virtual BaseDelegateExecution ContextExecution
	  {
		  get
		  {
			return contextExecution;
		  }
	  }

	  public virtual ResourceDefinitionEntity ContextResource
	  {
		  get
		  {
			return contextResource;
		  }
	  }
	}

}