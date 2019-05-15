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
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using AuthorizationExceptionDto = org.camunda.bpm.engine.rest.dto.AuthorizationExceptionDto;
	using ExceptionDto = org.camunda.bpm.engine.rest.dto.ExceptionDto;
	using MigratingProcessInstanceValidationExceptionDto = org.camunda.bpm.engine.rest.dto.migration.MigratingProcessInstanceValidationExceptionDto;
	using MigrationPlanValidationExceptionDto = org.camunda.bpm.engine.rest.dto.migration.MigrationPlanValidationExceptionDto;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class ExceptionHandlerHelper
	{

	  private static ExceptionHandlerHelper INSTANCE = new ExceptionHandlerHelper();

	  private ExceptionHandlerHelper()
	  {
	  }

	  public static ExceptionHandlerHelper Instance
	  {
		  get
		  {
			return INSTANCE;
		  }
	  }

	  public virtual ExceptionDto fromException(Exception e)
	  {
		if (e is MigratingProcessInstanceValidationException)
		{
		  return MigratingProcessInstanceValidationExceptionDto.from((MigratingProcessInstanceValidationException)e);
		}
		else if (e is MigrationPlanValidationException)
		{
		  return MigrationPlanValidationExceptionDto.from((MigrationPlanValidationException)e);
		}
		else if (e is AuthorizationException)
		{
		  return AuthorizationExceptionDto.fromException((AuthorizationException)e);
		}
		else
		{
		  return ExceptionDto.fromException(e);
		}
	  }

	  public virtual Response.Status getStatus(Exception exception)
	  {
		Response.Status responseStatus = Response.Status.INTERNAL_SERVER_ERROR;

		if (exception is ProcessEngineException)
		{
		  responseStatus = getStatus((ProcessEngineException)exception);
		}
		else if (exception is RestException)
		{
		  responseStatus = getStatus((RestException) exception);
		}
		else if (exception is WebApplicationException)
		{
		  //we need to check this, as otherwise the logic for processing WebApplicationException will be overridden
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int statusCode = ((javax.ws.rs.WebApplicationException) exception).getResponse().getStatus();
		  int statusCode = ((WebApplicationException) exception).Response.Status;
		  responseStatus = Response.Status.fromStatusCode(statusCode);
		}
		return responseStatus;
	  }

	  public virtual Response.Status getStatus(ProcessEngineException exception)
	  {
		Response.Status responseStatus = Response.Status.INTERNAL_SERVER_ERROR;

		// provide custom handling of authorization exception
		if (exception is AuthorizationException)
		{
		  responseStatus = Response.Status.FORBIDDEN;
		}
		else if (exception is MigrationPlanValidationException || exception is MigratingProcessInstanceValidationException || exception is BadUserRequestException)
		{
		  responseStatus = Response.Status.BAD_REQUEST;
		}
		return responseStatus;
	  }

	  public virtual Response.Status getStatus(RestException exception)
	  {
		if (exception.Status != null)
		{
		  return exception.Status;
		}
		return Response.Status.INTERNAL_SERVER_ERROR;
	  }
	}

}