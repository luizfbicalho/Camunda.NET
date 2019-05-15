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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{


	/// <summary>
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// </summary>
	[Serializable]
	public class ErrorEventDefinition
	{

	  public static IComparer<ErrorEventDefinition> comparator = new ComparatorAnonymousInnerClass();

	  private class ComparatorAnonymousInnerClass : IComparer<ErrorEventDefinition>
	  {
		  public int compare(ErrorEventDefinition o1, ErrorEventDefinition o2)
		  {
			return o2.Precedence.compareTo(o1.Precedence);
		  }
	  }

	  private const long serialVersionUID = 1L;

	  protected internal readonly string handlerActivityId;
	  protected internal string errorCode;
	  protected internal int? precedence = 0;
	  protected internal string errorCodeVariable;
	  protected internal string errorMessageVariable;

	  public ErrorEventDefinition(string handlerActivityId)
	  {
		this.handlerActivityId = handlerActivityId;
	  }

	  public virtual string ErrorCode
	  {
		  get
		  {
			return errorCode;
		  }
		  set
		  {
			this.errorCode = value;
		  }
	  }


	  public virtual string HandlerActivityId
	  {
		  get
		  {
			return handlerActivityId;
		  }
	  }

	  public virtual int? Precedence
	  {
		  get
		  {
			// handlers with error code take precedence over catchall-handlers
			return precedence + (!string.ReferenceEquals(errorCode, null) ? 1 : 0);
		  }
		  set
		  {
			this.precedence = value;
		  }
	  }


	  public virtual bool catchesError(string errorCode)
	  {
		return string.ReferenceEquals(this.errorCode, null) || this.errorCode.Equals(errorCode);
	  }

	  public virtual bool catchesException(Exception ex)
	  {

		if (string.ReferenceEquals(this.errorCode, null))
		{
		  return false;

		}
		else
		{

		  // unbox exception
		  while ((ex is ProcessEngineException || ex is ScriptException) && ex.InnerException != null)
		  {
			ex = (Exception) ex.InnerException;
		  }

		  // check exception hierarchy
		  Type exceptionClass = ex.GetType();
		  do
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			if (this.errorCode.Equals(exceptionClass.FullName))
			{
			  return true;
			}
			exceptionClass = exceptionClass.BaseType;
		  } while (exceptionClass != null);

		  return false;
		}
	  }

	  public virtual string ErrorCodeVariable
	  {
		  set
		  {
			this.errorCodeVariable = value;
		  }
		  get
		  {
			return errorCodeVariable;
		  }
	  }


	  public virtual string ErrorMessageVariable
	  {
		  set
		  {
			this.errorMessageVariable = value;
		  }
		  get
		  {
			return errorMessageVariable;
		  }
	  }

	}

}