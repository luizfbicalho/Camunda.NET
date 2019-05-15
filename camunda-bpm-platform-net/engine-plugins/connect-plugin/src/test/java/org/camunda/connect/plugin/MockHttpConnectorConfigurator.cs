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
namespace org.camunda.connect.plugin
{

	using HttpVersion = org.apache.http.HttpVersion;
	using CloseableHttpResponse = org.apache.http.client.methods.CloseableHttpResponse;
	using ContentType = org.apache.http.entity.ContentType;
	using StringEntity = org.apache.http.entity.StringEntity;
	using BasicHttpResponse = org.apache.http.message.BasicHttpResponse;
	using HttpConnector = org.camunda.connect.httpclient.HttpConnector;
	using HttpResponseImpl = org.camunda.connect.httpclient.impl.HttpResponseImpl;
	using ConnectorConfigurator = org.camunda.connect.spi.ConnectorConfigurator;
	using ConnectorInvocation = org.camunda.connect.spi.ConnectorInvocation;
	using ConnectorRequestInterceptor = org.camunda.connect.spi.ConnectorRequestInterceptor;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MockHttpConnectorConfigurator : ConnectorConfigurator<HttpConnector>
	{

	  public virtual void configure(HttpConnector connecor)
	  {
		connecor.addRequestInterceptor(new ConnectorRequestInterceptorAnonymousInnerClass(this));
	  }

	  private class ConnectorRequestInterceptorAnonymousInnerClass : ConnectorRequestInterceptor
	  {
		  private readonly MockHttpConnectorConfigurator outerInstance;

		  public ConnectorRequestInterceptorAnonymousInnerClass(MockHttpConnectorConfigurator outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object handleInvocation(org.camunda.connect.spi.ConnectorInvocation invocation) throws Exception
		  public object handleInvocation(ConnectorInvocation invocation)
		  {

			// intercept the call. => do not call invocation.proceed()

			// Could do validation on the invocation here:
			// invocation.getRequest() ....

			// build response using http client api...
			TestHttpResonse testHttpResonse = new TestHttpResonse();
			testHttpResonse.Entity = new StringEntity("{...}", ContentType.APPLICATION_JSON);

			// return the response
			return new HttpResponseImpl(testHttpResonse);
		  }
	  }

	  public virtual Type<HttpConnector> ConnectorClass
	  {
		  get
		  {
			return typeof(HttpConnector);
		  }
	  }

	  internal class TestHttpResonse : BasicHttpResponse, CloseableHttpResponse
	  {

		public TestHttpResonse() : base(HttpVersion.HTTP_1_1, 200, "OK")
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public virtual void close()
		{
		  // no-op
		}
	  }

	}

}