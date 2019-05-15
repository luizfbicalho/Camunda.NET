using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.rest
{
	using ClientResponse = com.sun.jersey.api.client.ClientResponse;
	using HttpEntity = org.apache.http.HttpEntity;
	using CloseableHttpResponse = org.apache.http.client.methods.CloseableHttpResponse;
	using HttpPost = org.apache.http.client.methods.HttpPost;
	using HttpClientContext = org.apache.http.client.protocol.HttpClientContext;
	using StringEntity = org.apache.http.entity.StringEntity;
	using CloseableHttpClient = org.apache.http.impl.client.CloseableHttpClient;
	using HttpClients = org.apache.http.impl.client.HttpClients;
	using PoolingHttpClientConnectionManager = org.apache.http.impl.conn.PoolingHttpClientConnectionManager;
	using EntityUtils = org.apache.http.util.EntityUtils;
	using JSONException = org.codehaus.jettison.json.JSONException;
	using JSONObject = org.codehaus.jettison.json.JSONObject;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	public class RestJaxRs2IT : AbstractWebappIntegrationTest
	{

	  private const string ENGINE_DEFAULT_PATH = "engine/default";
	  private static readonly string FETCH_AND_LOCK_PATH = ENGINE_DEFAULT_PATH + "/external-task/fetchAndLock";

	  protected internal override string ApplicationContextPath
	  {
		  get
		  {
			return "engine-rest/";
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setup() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static void setup()
	  {
		// just wait some seconds before starting because of Wildfly / Cargo race conditions
		Thread.Sleep(5 * 1000);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(timeout=10000) public void shouldUseJaxRs2Artifact() throws org.codehaus.jettison.json.JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldUseJaxRs2Artifact()
	  {
		IDictionary<string, object> payload = new Dictionary<string, object>();
		payload["workerId"] = "aWorkerId";
		payload["asyncResponseTimeout"] = 1000 * 60 * 30 + 1;

		ClientResponse response = client.resource(APP_BASE_PATH + FETCH_AND_LOCK_PATH).accept(MediaType.APPLICATION_JSON).entity(payload, MediaType.APPLICATION_JSON_TYPE).post(typeof(ClientResponse));

		assertEquals(400, response.Status);
		string responseMessage = response.getEntity(typeof(JSONObject)).get("message").ToString();
		assertTrue(responseMessage.Equals("The asynchronous response timeout cannot be set to a value greater than 1800000 milliseconds"));
		response.close();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPerform500ConcurrentRequests() throws InterruptedException, java.util.concurrent.ExecutionException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldPerform500ConcurrentRequests()
	  {
		PoolingHttpClientConnectionManager cm = new PoolingHttpClientConnectionManager();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.http.impl.client.CloseableHttpClient httpClient = org.apache.http.impl.client.HttpClients.custom().setConnectionManager(cm).build();
		CloseableHttpClient httpClient = HttpClients.custom().setConnectionManager(cm).build();

		Callable<string> performRequest = new CallableAnonymousInnerClass(this, httpClient);

		int requestsCount = 500;
		ExecutorService service = Executors.newFixedThreadPool(requestsCount);

		IList<Callable<string>> requests = new List<Callable<string>>();
		for (int i = 0; i < requestsCount; i++)
		{
		  requests.Add(performRequest);
		}

		IList<Future<string>> futures = service.invokeAll(requests);
		service.shutdown();
		service.awaitTermination(1, TimeUnit.HOURS);

		foreach (Future<string> future in futures)
		{
		  assertEquals(future.get(), "[]");
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<string>
	  {
		  private readonly RestJaxRs2IT outerInstance;

		  private CloseableHttpClient httpClient;

		  public CallableAnonymousInnerClass(RestJaxRs2IT outerInstance, CloseableHttpClient httpClient)
		  {
			  this.outerInstance = outerInstance;
			  this.httpClient = httpClient;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public String call() throws java.io.IOException
		  public override string call()
		  {
			HttpPost request = new HttpPost(outerInstance.APP_BASE_PATH + FETCH_AND_LOCK_PATH);
			request.setHeader(HttpHeaders.CONTENT_TYPE, "application/json");
			StringEntity stringEntity = new StringEntity("{ \"workerId\": \"aWorkerId\", \"asyncResponseTimeout\": 1000 }");
			request.Entity = stringEntity;

			CloseableHttpResponse response = httpClient.execute(request, HttpClientContext.create());
			string responseBody = null;
			try
			{
			  HttpEntity entity = response.Entity;
			  responseBody = EntityUtils.ToString(entity);
			  request.releaseConnection();
			}
			finally
			{
			  response.close();
			}

			return responseBody;
		  }

	  }

	}

}