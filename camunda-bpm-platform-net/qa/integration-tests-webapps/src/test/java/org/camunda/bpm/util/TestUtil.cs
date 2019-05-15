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
namespace org.camunda.bpm.util
{

	using DefaultHttpClient = org.apache.http.impl.client.DefaultHttpClient;
	using UserCredentialsDto = org.camunda.bpm.engine.rest.dto.identity.UserCredentialsDto;
	using UserDto = org.camunda.bpm.engine.rest.dto.identity.UserDto;
	using UserProfileDto = org.camunda.bpm.engine.rest.dto.identity.UserProfileDto;

	using ClientResponse = com.sun.jersey.api.client.ClientResponse;
	using WebResource = com.sun.jersey.api.client.WebResource;
	using ClientConfig = com.sun.jersey.api.client.config.ClientConfig;
	using JSONConfiguration = com.sun.jersey.api.json.JSONConfiguration;
	using ApacheHttpClient4 = com.sun.jersey.client.apache4.ApacheHttpClient4;
	using DefaultApacheHttpClient4Config = com.sun.jersey.client.apache4.config.DefaultApacheHttpClient4Config;

	/// 
	/// <summary>
	/// @author nico.rehwaldt
	/// </summary>
	public class TestUtil
	{

	  private readonly ApacheHttpClient4 client;
	  private readonly DefaultHttpClient defaultHttpClient;

	  private readonly TestProperties testProperties;

	  public TestUtil(TestProperties testProperties)
	  {

		this.testProperties = testProperties;

		// create admin user:
		ClientConfig clientConfig = new DefaultApacheHttpClient4Config();
		clientConfig.Features.put(JSONConfiguration.FEATURE_POJO_MAPPING, true);
		client = ApacheHttpClient4.create(clientConfig);

		defaultHttpClient = (DefaultHttpClient) client.ClientHandler.HttpClient;
	  }

	  public virtual void destroy()
	  {
		client.destroy();
	  }

	  public virtual void createInitialUser(string id, string password, string firstName, string lastName)
	  {

		UserDto user = new UserDto();
		UserCredentialsDto credentials = new UserCredentialsDto();
		credentials.Password = password;
		user.Credentials = credentials;
		UserProfileDto profile = new UserProfileDto();
		profile.Id = id;
		profile.FirstName = firstName;
		profile.LastName = lastName;
		user.Profile = profile;

		WebResource webResource = client.resource(testProperties.getApplicationPath("/camunda/api/admin/setup/default/user/create"));
		ClientResponse clientResponse = webResource.accept(MediaType.APPLICATION_JSON).type(MediaType.APPLICATION_JSON).post(typeof(ClientResponse), user);
		try
		{
		  if (clientResponse.ResponseStatus != Response.Status.NO_CONTENT)
		  {
			throw new WebApplicationException(clientResponse.ResponseStatus);
		  }
		}
		finally
		{
		  clientResponse.close();
		}
	  }

	  public virtual void deleteUser(string id)
	  {
		// delete admin user
		WebResource webResource = client.resource(testProperties.getApplicationPath("/engine-rest/user/admin"));
		webResource.accept(MediaType.APPLICATION_JSON).type(MediaType.APPLICATION_JSON).delete();
	  }
	}

}