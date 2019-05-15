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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.hasItems;

	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public class ExceptionHandlerTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  private static readonly string EXCEPTION_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/unannotated";

	  private static readonly string GENERAL_EXCEPTION_URL = EXCEPTION_RESOURCE_URL + "/exception";
	  private static readonly string PROCESS_ENGINE_EXCEPTION_URL = EXCEPTION_RESOURCE_URL + "/processEngineException";
	  private static readonly string REST_EXCEPTION_URL = EXCEPTION_RESOURCE_URL + "/restException";
	  private static readonly string AUTH_EXCEPTION_URL = EXCEPTION_RESOURCE_URL + "/authorizationException";
	  private static readonly string AUTH_EXCEPTION_MULTIPLE_URL = EXCEPTION_RESOURCE_URL + "/authorizationExceptionMultiple";
	  private static readonly string STACK_OVERFLOW_ERROR_URL = EXCEPTION_RESOURCE_URL + "/stackOverflowError";


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGeneralExceptionHandler()
	  public virtual void testGeneralExceptionHandler()
	  {
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(Exception).Name)).body("message", equalTo("expected exception")).when().get(GENERAL_EXCEPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestExceptionHandler()
	  public virtual void testRestExceptionHandler()
	  {
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("expected exception")).when().get(REST_EXCEPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineExceptionHandler()
	  public virtual void testProcessEngineExceptionHandler()
	  {
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("expected exception")).when().get(PROCESS_ENGINE_EXCEPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationExceptionHandler()
	  public virtual void testAuthorizationExceptionHandler()
	  {
	  //    TODO remove "resourceName", "resourceId", "permissionName" once the deprecated methods from AuthorizationException are removed
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("The user with id 'someUser' does not have 'somePermission' permission on resource 'someResourceId' of type 'someResourceName'.")).body("userId", equalTo("someUser")).body("resourceName", equalTo("someResourceName")).body("resourceId", equalTo("someResourceId")).body("permissionName", equalTo("somePermission")).body("missingAuthorizations.resourceName", hasItems("someResourceName")).body("missingAuthorizations.resourceId", hasItems("someResourceId")).body("missingAuthorizations.permissionName", hasItems("somePermission")).when().get(AUTH_EXCEPTION_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAuthorizationExceptionWithMultipleMissingAuthorizationsHandler()
	  public virtual void testAuthorizationExceptionWithMultipleMissingAuthorizationsHandler()
	  {
	  //    TODO remove "resourceName", "resourceId", "permissionName" once the deprecated methods from AuthorizationException are removed
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo("The user with id 'someUser' does not have one of the following permissions: 'somePermission1' permission on resource " + "'someResourceId1' of type 'someResourceName1' or 'somePermission2' permission on resource " + "'someResourceId2' of type 'someResourceName2'")).body("userId", equalTo("someUser")).body("resourceName", equalTo(null)).body("resourceId", equalTo(null)).body("permissionName", equalTo(null)).body("missingAuthorizations.resourceName", hasItems("someResourceName1", "someResourceName2")).body("missingAuthorizations.resourceId", hasItems("someResourceId1", "someResourceId2")).body("missingAuthorizations.permissionName", hasItems("somePermission1", "somePermission2")).when().get(AUTH_EXCEPTION_MULTIPLE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testThrowableHandler()
	  public virtual void testThrowableHandler()
	  {
		given().header(ACCEPT_WILDCARD_HEADER).expect().contentType(ContentType.JSON).statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(StackOverflowError).Name)).body("message", equalTo("Stack overflow")).when().get(STACK_OVERFLOW_ERROR_URL);
	  }

	}

}