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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using BasicUserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.BasicUserCredentialsDto;
	using GroupDto = org.camunda.bpm.engine.rest.dto.identity.GroupDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

	public class IdentityRestServiceQueryTest : AbstractRestServiceTest
	{

	  protected internal const string TEST_USERNAME = "testUsername";
	  protected internal const string TEST_PASSWORD = "testPassword";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string IDENTITY_URL = TEST_RESOURCE_ROOT_PATH + "/identity";
	  protected internal static readonly string TASK_GROUPS_URL = IDENTITY_URL + "/groups";
	  protected internal static readonly string VERIFY_USER_URL = IDENTITY_URL + "/verify";

	  private User mockUser;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		createMockIdentityQueries();
	  }

	  private void createMockIdentityQueries()
	  {
		UserQuery sampleUserQuery = mock(typeof(UserQuery));

		IList<User> mockUsers = new List<User>();

		mockUser = MockProvider.createMockUser();
		mockUsers.Add(mockUser);

		when(sampleUserQuery.list()).thenReturn(mockUsers);
		when(sampleUserQuery.memberOfGroup(anyString())).thenReturn(sampleUserQuery);
		when(sampleUserQuery.count()).thenReturn((long) mockUsers.Count);

		GroupQuery sampleGroupQuery = mock(typeof(GroupQuery));

		IList<Group> mockGroups = MockProvider.createMockGroups();
		when(sampleGroupQuery.list()).thenReturn(mockGroups);
		when(sampleGroupQuery.groupMember(anyString())).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.orderByGroupName()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.orderByGroupId()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.orderByGroupType()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.asc()).thenReturn(sampleGroupQuery);
		when(sampleGroupQuery.desc()).thenReturn(sampleGroupQuery);

		when(processEngine.IdentityService.createGroupQuery()).thenReturn(sampleGroupQuery);
		when(processEngine.IdentityService.createUserQuery()).thenReturn(sampleUserQuery);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupInfoQuery()
	  public virtual void testGroupInfoQuery()
	  {
		given().queryParam("userId", "name").then().expect().statusCode(Status.OK.StatusCode).body("groups.size()", @is(1)).body("groups[0].id", equalTo(MockProvider.EXAMPLE_GROUP_ID)).body("groups[0].name", equalTo(MockProvider.EXAMPLE_GROUP_NAME)).body("groupUsers.size()", @is(1)).body("groupUsers[0].id", equalTo(mockUser.Id)).when().get(TASK_GROUPS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGroupInfoQueryWithMissingUserParameter()
	  public virtual void testGroupInfoQueryWithMissingUserParameter()
	  {
		expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("No user id was supplied")).when().get(TASK_GROUPS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void verifyUserWithMissingParameter()
	  public virtual void verifyUserWithMissingParameter()
	  {
		given().body(new BasicUserCredentialsDto()).contentType(ContentType.JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Username and password are required")).when().post(VERIFY_USER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void verifyUser()
	  public virtual void verifyUser()
	  {
		when(processEngine.IdentityService.checkPassword(TEST_USERNAME, TEST_PASSWORD)).thenReturn(true);

		BasicUserCredentialsDto userCredentialsDto = new BasicUserCredentialsDto();
		userCredentialsDto.Username = TEST_USERNAME;
		userCredentialsDto.Username = TEST_PASSWORD;
		given().body(userCredentialsDto).contentType(ContentType.JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Username and password are required")).when().post(VERIFY_USER_URL);
	  }
	}

}