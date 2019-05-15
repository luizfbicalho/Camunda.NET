using System.Text;

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
namespace org.camunda.bpm.engine.rest.standalone
{
	using HttpEntity = org.apache.http.HttpEntity;
	using RequestConfig = org.apache.http.client.config.RequestConfig;
	using CloseableHttpResponse = org.apache.http.client.methods.CloseableHttpResponse;
	using HttpPost = org.apache.http.client.methods.HttpPost;
	using ByteArrayEntity = org.apache.http.entity.ByteArrayEntity;
	using ContentType = org.apache.http.entity.ContentType;
	using CloseableHttpClient = org.apache.http.impl.client.CloseableHttpClient;
	using HttpClients = org.apache.http.impl.client.HttpClients;
	using EntityUtils = org.apache.http.util.EntityUtils;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using ProcessInstantiationBuilder = org.camunda.bpm.engine.runtime.ProcessInstantiationBuilder;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public abstract class AbstractEmptyBodyFilterTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal new const string TEST_RESOURCE_ROOT_PATH = "/rest-test/rest";
	  protected internal static readonly string PROCESS_DEFINITION_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition";
	  protected internal static readonly string SINGLE_PROCESS_DEFINITION_BY_KEY_URL = PROCESS_DEFINITION_URL + "/key/" + MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
	  protected internal static readonly string START_PROCESS_INSTANCE_BY_KEY_URL = SINGLE_PROCESS_DEFINITION_BY_KEY_URL + "/start";

	  protected internal ProcessInstantiationBuilder mockInstantiationBuilder;
	  protected internal RuntimeService runtimeServiceMock;

	  protected internal CloseableHttpClient client;
	  protected internal RequestConfig reqConfig;
	  protected internal readonly string BASE_URL = "http://localhost:38080";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpHttpClientAndRuntimeData()
	  public virtual void setUpHttpClientAndRuntimeData()
	  {
		client = HttpClients.createDefault();
		reqConfig = RequestConfig.custom().setConnectTimeout(3 * 60 * 1000).setSocketTimeout(10 * 60 * 1000).build();

		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();

		runtimeServiceMock = mock(typeof(RuntimeService));
		when(processEngine.RuntimeService).thenReturn(runtimeServiceMock);

		mockInstantiationBuilder = mock(typeof(ProcessInstantiationBuilder));
		when(mockInstantiationBuilder.setVariables(any(typeof(System.Collections.IDictionary)))).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.businessKey(anyString())).thenReturn(mockInstantiationBuilder);
		when(mockInstantiationBuilder.caseInstanceId(anyString())).thenReturn(mockInstantiationBuilder);
		when(runtimeServiceMock.createProcessInstanceById(anyString())).thenReturn(mockInstantiationBuilder);

		ProcessInstanceWithVariables resultInstanceWithVariables = MockProvider.createMockInstanceWithVariables();
		when(mockInstantiationBuilder.executeWithVariablesInReturn(anyBoolean(), anyBoolean())).thenReturn(resultInstanceWithVariables);

		ProcessDefinitionQuery processDefinitionQueryMock = mock(typeof(ProcessDefinitionQuery));
		when(processDefinitionQueryMock.processDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.withoutTenantId()).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.latestVersion()).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.singleResult()).thenReturn(mockDefinition);

		RepositoryService repositoryServiceMock = mock(typeof(RepositoryService));
		when(processEngine.RepositoryService).thenReturn(repositoryServiceMock);
		when(repositoryServiceMock.createProcessDefinitionQuery()).thenReturn(processDefinitionQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		client.close();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBodyIsEmpty() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBodyIsEmpty()
	  {
		evaluatePostRequest(new ByteArrayEntity("".GetBytes(Encoding.UTF8)), ContentType.create(MediaType.APPLICATION_JSON).ToString(), 200, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBodyIsNull() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBodyIsNull()
	  {
		evaluatePostRequest(null, ContentType.create(MediaType.APPLICATION_JSON).ToString(), 200, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBodyIsNullAndContentTypeIsNull() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBodyIsNullAndContentTypeIsNull()
	  {
		evaluatePostRequest(null, null, 415, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBodyIsNullAndContentTypeHasISOCharset() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBodyIsNullAndContentTypeHasISOCharset()
	  {
		evaluatePostRequest(null, ContentType.create(MediaType.APPLICATION_JSON, "iso-8859-1").ToString(), 200, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBodyIsEmptyJSONObject() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBodyIsEmptyJSONObject()
	  {
		evaluatePostRequest(new ByteArrayEntity(EMPTY_JSON_OBJECT.GetBytes(Encoding.UTF8)), ContentType.create(MediaType.APPLICATION_JSON).ToString(), 200, true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void evaluatePostRequest(org.apache.http.HttpEntity reqBody, String reqContentType, int expectedStatusCode, boolean assertResponseBody) throws java.io.IOException
	  private void evaluatePostRequest(HttpEntity reqBody, string reqContentType, int expectedStatusCode, bool assertResponseBody)
	  {
		HttpPost post = new HttpPost(BASE_URL + START_PROCESS_INSTANCE_BY_KEY_URL);
		post.Config = reqConfig;

		if (!string.ReferenceEquals(reqContentType, null))
		{
		  post.setHeader(HttpHeaders.CONTENT_TYPE, reqContentType);
		}

		post.Entity = reqBody;

		CloseableHttpResponse response = client.execute(post);

		assertEquals(expectedStatusCode, response.StatusLine.StatusCode);

		if (assertResponseBody)
		{
		  assertThat(EntityUtils.ToString(response.Entity, "UTF-8"), containsString(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID));
		}

		response.close();
	  }

	}

}