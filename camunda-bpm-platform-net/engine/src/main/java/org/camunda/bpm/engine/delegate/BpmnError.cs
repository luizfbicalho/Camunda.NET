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
namespace org.camunda.bpm.engine.@delegate
{
	using Error = org.camunda.bpm.engine.impl.bpmn.parser.Error;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;


	/// <summary>
	/// Special exception that can be used to throw a BPMN Error from
	/// <seealso cref="JavaDelegate"/>s and expressions.
	/// 
	/// This should only be used for business faults, which shall be handled by a
	/// Boundary Error Event or Error Event Sub-Process modeled in the process
	/// definition. Technical errors should be represented by other exception types.
	/// 
	/// This class represents an actual instance of a BPMN Error, whereas
	/// <seealso cref="Error"/> represents an Error definition.
	/// 
	/// @author Falko Menge
	/// </summary>
	public class BpmnError : ProcessEngineException
	{

	  private const long serialVersionUID = 1L;

	  private string errorCode;
	  private string errorMessage;

	  public BpmnError(string errorCode) : base("")
	  {
		ErrorCode = errorCode;
	  }

	  public BpmnError(string errorCode, string message) : base(message + " (errorCode='" + errorCode + "')")
	  {
		ErrorCode = errorCode;
		Message = message;
	  }

	  protected internal virtual string ErrorCode
	  {
		  set
		  {
			ensureNotEmpty("Error Code", value);
			this.errorCode = value;
		  }
		  get
		  {
			return errorCode;
		  }
	  }


	  public override string ToString()
	  {
		return base.ToString() + " (errorCode='" + errorCode + "')";
	  }

	  protected internal virtual string Message
	  {
		  set
		  {
			ensureNotEmpty("Error Message", value);
			this.errorMessage = value;
		  }
		  get
		  {
			return errorMessage;
		  }
	  }

	}

}