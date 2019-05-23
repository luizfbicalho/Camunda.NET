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
namespace org.camunda.bpm.engine.rest.exception
{
	using ExceptionDto = org.camunda.bpm.engine.rest.dto.ExceptionDto;


	/// <summary>
	/// Translates any <seealso cref="System.Exception"/> to a HTTP 500 error and a JSON response.
	/// Response content format: <code>{"type" : "ExceptionType", "message" : "some exception message"}
	/// @author nico.rehwaldt
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Provider public class ExceptionHandler implements javax.ws.rs.ext.ExceptionMapper<Throwable>
	public class ExceptionHandler : ExceptionMapper<Exception>
	{

	  private static readonly Logger LOGGER = Logger.getLogger(typeof(ExceptionHandler).Name);

	  public override Response toResponse(Exception throwable)
	  {

		LOGGER.log(Level.WARNING, getStackTrace(throwable));

		ExceptionDto exceptionDto = ExceptionHandlerHelper.Instance.fromException(throwable);

		Response.Status responseStatus = ExceptionHandlerHelper.Instance.getStatus(throwable);

		return Response.status(responseStatus).entity(exceptionDto).type(MediaType.APPLICATION_JSON_TYPE).build();
	  }

	  protected internal virtual string getStackTrace(Exception aThrowable)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer result = new java.io.StringWriter();
		Writer result = new StringWriter();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PrintWriter printWriter = new java.io.PrintWriter(result);
		PrintWriter printWriter = new PrintWriter(result);
		aThrowable.printStackTrace(printWriter);
		return result.ToString();
	  }


	}

}