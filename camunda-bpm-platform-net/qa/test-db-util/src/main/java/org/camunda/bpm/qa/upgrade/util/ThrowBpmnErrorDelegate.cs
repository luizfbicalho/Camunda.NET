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
namespace org.camunda.bpm.qa.upgrade.util
{
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ThrowBpmnErrorDelegate : JavaDelegate, ExecutionListener
	{

	  public const string ERROR_INDICATOR_VARIABLE = "throwError";
	  public const string ERROR_NAME_VARIABLE = "errorName";

	  public const string EXCEPTION_INDICATOR_VARIABLE = "throwException";
	  public const string EXCEPTION_MESSAGE_VARIABLE = "exceptionMessage";

	  public static readonly string DEFAULT_ERROR_NAME = typeof(ThrowBpmnErrorDelegate).Name;
	  public static readonly string DEFAULT_EXCEPTION_MESSAGE = DEFAULT_ERROR_NAME;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		throwErrorIfRequested(execution);
		throwExceptionIfRequested(execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		execute(execution);
	  }

	  protected internal virtual void throwErrorIfRequested(DelegateExecution execution)
	  {
		bool? shouldThrowError = (bool?) execution.getVariable(ERROR_INDICATOR_VARIABLE);

		if (true.Equals(shouldThrowError))
		{
		  string errorName = (string) execution.getVariable(ERROR_NAME_VARIABLE);
		  if (string.ReferenceEquals(errorName, null))
		  {
			errorName = DEFAULT_ERROR_NAME;
		  }

		  throw new BpmnError(errorName);
		}
	  }

	  protected internal virtual void throwExceptionIfRequested(DelegateExecution execution)
	  {
		bool? shouldThrowException = (bool?) execution.getVariable(EXCEPTION_INDICATOR_VARIABLE);

		if (true.Equals(shouldThrowException))
		{
		  string exceptionMessage = (string) execution.getVariable(EXCEPTION_MESSAGE_VARIABLE);
		  if (string.ReferenceEquals(exceptionMessage, null))
		  {
			exceptionMessage = DEFAULT_EXCEPTION_MESSAGE;
		  }

		  throw new ThrowBpmnErrorDelegateException(exceptionMessage);
		}
	  }

	  public class ThrowBpmnErrorDelegateException : Exception
	  {

		internal const long serialVersionUID = 1L;

		public ThrowBpmnErrorDelegateException(string message) : base(message)
		{
		}

	  }

	}

}