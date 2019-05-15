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
namespace org.camunda.bpm.engine.rest.application
{
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;


	/// <summary>
	/// Does not declare produced media types.
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Path("/unannotated") public class UnannotatedResource
	public class UnannotatedResource
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/exception") public String throwAnException() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual string throwAnException()
		{
		throw new Exception("expected exception");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/processEngineException") public String throwProcessEngineException() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual string throwProcessEngineException()
	  {
		throw new ProcessEngineException("expected exception");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/restException") public String throwRestException() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual string throwRestException()
	  {
		throw new RestException(Status.BAD_REQUEST, "expected exception");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/authorizationException") public String throwAuthorizationException() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual string throwAuthorizationException()
	  {
		throw new AuthorizationException("someUser", "somePermission", "someResourceName", "someResourceId");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/stackOverflowError") public String throwStackOverflowError() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual string throwStackOverflowError()
	  {
		throw new StackOverflowError("Stack overflow");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/authorizationExceptionMultiple") public String throwAuthorizationExceptionMultiple() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual string throwAuthorizationExceptionMultiple()
	  {
		IList<MissingAuthorization> missingAuthorizations = new List<MissingAuthorization>();

		missingAuthorizations.Add(new MissingAuthorization("somePermission1", "someResourceName1", "someResourceId1"));
		missingAuthorizations.Add(new MissingAuthorization("somePermission2", "someResourceName2", "someResourceId2"));
		throw new AuthorizationException("someUser", missingAuthorizations);
	  }
	}

}