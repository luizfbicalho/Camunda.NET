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
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;
	using PasswordPolicyDigitRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyDigitRuleImpl;
	using PasswordPolicyLengthRuleImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyLengthRuleImpl;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// </summary>
	public class PasswordPolicyServiceQueryTest : AbstractRestServiceTest
	{

	  protected internal static readonly string QUERY_URL = TEST_RESOURCE_ROOT_PATH + IdentityRestService_Fields.PATH + "/password-policy";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal ProcessEngineConfiguration processEngineConfigurationMock = mock(typeof(ProcessEngineConfiguration));

	  protected internal IdentityService identityServiceMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpMocks()
	  public virtual void setUpMocks()
	  {
		identityServiceMock = mock(typeof(IdentityService));

		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationMock);

		when(processEngine.IdentityService).thenReturn(identityServiceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldGetPolicy()
	  public virtual void shouldGetPolicy()
	  {
		when(processEngineConfigurationMock.EnablePasswordPolicy).thenReturn(true);

		PasswordPolicy passwordPolicyMock = mock(typeof(PasswordPolicy));

		when(identityServiceMock.PasswordPolicy).thenReturn(passwordPolicyMock);

		when(passwordPolicyMock.Rules).thenReturn(Collections.singletonList<PasswordPolicyRule>(new PasswordPolicyDigitRuleImpl(1)));

		given().then().expect().statusCode(Status.OK.StatusCode).body("rules[0].placeholder", equalTo("PASSWORD_POLICY_DIGIT")).body("rules[0].parameter.minDigit", equalTo("1")).when().get(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnNotFound_PasswordPolicyDisabled()
	  public virtual void shouldReturnNotFound_PasswordPolicyDisabled()
	  {
		when(processEngineConfigurationMock.EnablePasswordPolicy).thenReturn(false);

		given().then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckInvalidPassword()
	  public virtual void shouldCheckInvalidPassword()
	  {
		when(processEngineConfigurationMock.EnablePasswordPolicy).thenReturn(true);

		PasswordPolicyResult passwordPolicyResultMock = mock(typeof(PasswordPolicyResult));

		when(identityServiceMock.checkPasswordAgainstPolicy(anyString())).thenReturn(passwordPolicyResultMock);

		when(passwordPolicyResultMock.FulfilledRules).thenReturn(Collections.singletonList<PasswordPolicyRule>(new PasswordPolicyDigitRuleImpl(1)));

		when(passwordPolicyResultMock.ViolatedRules).thenReturn(Collections.singletonList<PasswordPolicyRule>(new PasswordPolicyLengthRuleImpl(1)));

		given().header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(Collections.singletonMap("password", "password")).then().expect().statusCode(Status.OK.StatusCode).body("rules[0].placeholder", equalTo("PASSWORD_POLICY_DIGIT")).body("rules[0].parameter.minDigit", equalTo("1")).body("rules[0].valid", equalTo(true)).body("rules[1].placeholder", equalTo("PASSWORD_POLICY_LENGTH")).body("rules[1].parameter.minLength", equalTo("1")).body("rules[1].valid", equalTo(false)).body("valid", equalTo(false)).when().post(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckValidPassword()
	  public virtual void shouldCheckValidPassword()
	  {
		when(processEngineConfigurationMock.EnablePasswordPolicy).thenReturn(true);

		PasswordPolicyResult passwordPolicyResultMock = mock(typeof(PasswordPolicyResult));

		when(identityServiceMock.checkPasswordAgainstPolicy(anyString())).thenReturn(passwordPolicyResultMock);

		when(passwordPolicyResultMock.FulfilledRules).thenReturn(Collections.singletonList<PasswordPolicyRule>(new PasswordPolicyDigitRuleImpl(1)));

		when(passwordPolicyResultMock.ViolatedRules).thenReturn(System.Linq.Enumerable.Empty<PasswordPolicyRule>());

		given().header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(Collections.singletonMap("password", "password")).then().expect().statusCode(Status.OK.StatusCode).body("valid", equalTo(true)).when().post(QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCheckPasswordAgainstNoPolicy()
	  public virtual void shouldCheckPasswordAgainstNoPolicy()
	  {
		when(processEngineConfigurationMock.EnablePasswordPolicy).thenReturn(false);

		given().header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(Collections.singletonMap("password", "password")).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().post(QUERY_URL);
	  }

	}
}