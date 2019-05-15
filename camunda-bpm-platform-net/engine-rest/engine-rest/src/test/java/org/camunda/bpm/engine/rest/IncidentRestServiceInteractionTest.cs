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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using IncidentQueryImpl = org.camunda.bpm.engine.impl.IncidentQueryImpl;
	using ManagementServiceImpl = org.camunda.bpm.engine.impl.ManagementServiceImpl;
	using RepositoryServiceImpl = org.camunda.bpm.engine.impl.RepositoryServiceImpl;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;



	public class IncidentRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string INCIDENT_URL = TEST_RESOURCE_ROOT_PATH + "/incident";
	  protected internal static readonly string SINGLE_INCIDENT_URL = INCIDENT_URL + "/{id}";

	  private RuntimeServiceImpl mockRuntimeService;
	  private IncidentQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		IList<Incident> incidents = MockProvider.createMockIncidents();

		mockedQuery = setupMockIncidentQuery(incidents);
	  }

	  private IncidentQuery setupMockIncidentQuery(IList<Incident> incidents)
	  {
		IncidentQuery sampleQuery = mock(typeof(IncidentQuery));

		when(sampleQuery.incidentId(anyString())).thenReturn(sampleQuery);
		when(sampleQuery.singleResult()).thenReturn(mock(typeof(Incident)));

		mockRuntimeService = mock(typeof(RuntimeServiceImpl));
		when(processEngine.RuntimeService).thenReturn(mockRuntimeService);
		when(mockRuntimeService.createIncidentQuery()).thenReturn(sampleQuery);

		return sampleQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIncident()
	  public virtual void testGetIncident()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_INCIDENT_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_INCIDENT_URL);

		verify(mockRuntimeService).createIncidentQuery();
		verify(mockedQuery).incidentId(MockProvider.EXAMPLE_INCIDENT_ID);
		verify(mockedQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetUnexistingIncident()
	  public virtual void testGetUnexistingIncident()
	  {
		when(mockedQuery.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.EXAMPLE_INCIDENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(SINGLE_INCIDENT_URL);

		verify(mockRuntimeService).createIncidentQuery();
		verify(mockedQuery).incidentId(MockProvider.EXAMPLE_INCIDENT_ID);
		verify(mockedQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveIncident()
	  public virtual void testResolveIncident()
	  {

		given().pathParam("id", MockProvider.EXAMPLE_INCIDENT_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_INCIDENT_URL);

		verify(mockRuntimeService).resolveIncident(MockProvider.EXAMPLE_INCIDENT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveUnexistingIncident()
	  public virtual void testResolveUnexistingIncident()
	  {
		doThrow(new NotFoundException()).when(mockRuntimeService).resolveIncident(anyString());

		given().pathParam("id", MockProvider.EXAMPLE_INCIDENT_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().delete(SINGLE_INCIDENT_URL);

		verify(mockRuntimeService).resolveIncident(MockProvider.EXAMPLE_INCIDENT_ID);
	  }
	}

}